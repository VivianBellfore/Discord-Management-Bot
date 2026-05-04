
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Managing permission functions.<para/>
    /// </summary>
    internal class PermissionManager
    {
        #region Discord related permissions
        /// <summary>
        /// Returns true, if user has given permission.<para/>
        /// Returns false, if user dont has permission or if one parameter was null.
        /// </summary>
        internal static async Task<bool> HasUserDiscordChannelPermission(SocketSlashCommand command, ulong channelId, ChannelPermission permission)
        {
            SocketGuild socketGuild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
            if (socketGuild == null)
            {
                await Utilities.SendDevLogMessage(1, $"SocketGuild was null! Guild id was {(ulong)command.GuildId}.");
                return false;
            }

            IGuildUser guildUser = command.User as IGuildUser;
            if (guildUser == null)
            {
                await Utilities.SendDevLogMessage(1, $"IGuildUser was null! User id was ||{command.User.Id}||.");
                return false;
            }

            IGuildChannel guildChannel = socketGuild.GetChannel(channelId);
            if (guildChannel == null)
            {
                await Utilities.SendDevLogMessage(1, $"IGuildChannel was null! Channel was null! Channel id was {channelId}.");
                return false;
            }

            ChannelPermissions permissionList = guildUser.GetPermissions(guildChannel);

            if (permissionList.Has(permission))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checking if a user has a general guild permission for a guild.
        /// </summary>
        internal static async Task<bool> HasUserDiscordGuildPermission(ulong userId, ulong guildId, GuildPermission permission)
        {
            IGuild guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild was null for guildId {guildId}.");
                return false;
            }

            IGuildUser guildUser = await guild.GetUserAsync(userId);
            if (guildUser == null)
            {
                await Utilities.SendDevLogMessage(1, $"GuildUser was null for user {userId}.");
                return false;
            }

            if (guildUser.GuildPermissions.Has(permission))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a user has a given role in the given guild.
        /// </summary>
        public static async Task<bool> HasUserDiscordGuildRole(ulong userId, ulong guildId, ulong roleId)
        {
            IGuild guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild was null for guildId {guildId}.");
                return false;
            }

            IGuildUser guildUser = await guild.GetUserAsync(userId);
            if (guildUser == null)
            {
                await Utilities.SendDevLogMessage(1, $"GuildUser was null for user {userId}.");
                return false;
            }

            if (guildUser.RoleIds.Contains(roleId))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checking if a user is the guild owner.
        /// </summary>
        internal static async Task<bool> IsUserGuildOwner(ulong guildId, ulong userId)
        {
            IGuild guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild was null for guildId {guildId}.");
                return false;
            }

            if (guild.OwnerId == userId)
                return true;
            else
                return false;
        }
        #endregion



        #region Internal data base related permissions
        /// <summary>
        /// Reading the roles of a user and is checking if he has the member role for this guild.<para/>
        /// Permission level:<br/>
        /// member  = Member<br/>
        /// mod     = Moderator<br/>
        /// admin   = Admin<para/>
        /// </summary>
        /// <returns>fales - If somthing got null.<br/>true - If user has the permission role.</returns>
        internal static async Task<bool> HasUserBotPermissionRole(string permission, ulong guildId, SocketGuildUser user)
        {
            object roleId = null;
            try
            {
                roleId = await MySqlWrapper.SQLExecuteScalar(
                    $"SELECT `role_{permission}` FROM `guild_data` WHERE `guild_id` = @guild_id",
                    new Dictionary<string, object>() { { "guild_id", guildId } });
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"The given permission type was invalid: {permission}.\n\n{ex}");
                return false;
            }        

            if (roleId == null || (ulong)roleId == 0)
                return false;     

            var guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild fetched from guild id {guildId} was null!");
                return false;
            }

            var role = guild.GetRole((ulong)roleId);
            if (role == null)
            {
                await Utilities.SendDevLogMessage(1, $"Role fetched from guild was null! Id was {roleId}.");
                return false;
            }

            var roleList = user.Roles.Where(x => !x.IsEveryone).Select(x => x.Id).ToList();
            if (roleList.Contains((ulong)roleId))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checking if a guild is gated.<para/>
        /// True - if guild is gated.<br/>
        /// False - if guild was null or guild is not gated.
        /// </summary>
        internal static async Task<bool> IsGuildAGatedCommunity(ulong guildId)
        {
            GuildObject guildObject = await GuildManager.GetGuildData(guildId);

            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {guildId}.");
                await Utilities.SendDevLogMessage(1, $"GuildObject was null! Id is {guildId}.");
                return false;
            }

            return guildObject.IsGatedCommunity;
        }

        /// <summary>
        /// Cheking if user has accepted the bot terms of service at any point, on any guild.
        /// </summary>
        internal static async Task<bool> HasUserAcceptTos(ulong userId)
        {
            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `user_id` FROM `user_profile` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (result == null)
                return false;

            return true;
        }

        /// <summary>
        /// Checking if a user is blocked from using the bot by e specific type, guild or global.
        /// </summary>
        /// <returns>
        /// False and empty string when nothing is blocked.<br/>
        /// True and a string with a message of the blocked type.
        /// </returns>
        internal static async Task<(bool, string)> IsUserBlockedFromBotUsage(ulong userId)
        {
            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `bot_user_bans` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (results == null || results.Count <= 0)
                return (false, "");

            return (true, await LanguageManager.GetTranslation("youAreBlockedGlobal", userId));
        }

        /// <summary>
        /// Checking data base table "user_profile" for user setting "block_bot_dm".
        /// </summary>
        internal static async Task<bool> IsUserBlockingBotDM(ulong userId)
        {
            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `block_bot_dm` FROM `user_profile` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (result == null || Convert.ToInt32(result) == 0 )
                return false;

            return true;
        }
        #endregion
    }
}
