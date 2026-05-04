
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="roles"/> command.
    /// </summary>
    internal class SendRolesMessage : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SendRolesMessage() : base("admin", "roles", "command_admin_roles") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            string roleType = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            if (roleType == "role")
                await PostRoleMessage(command);

            else if (roleType == "roleget")
                await GetCurrentRoleList(command);

            else if (roleType == "roleclear")
                await ClearCurrentRoleList(command);

            else
            {
                string message = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync( func => func.Content = message );
                await Utilities.SendDevLogMessage(1, $"Role type was invalid: {roleType}");
            }
        }



        #region Methodes
        /// <summary>
        /// Sends a message with role buttons in the command channel.
        /// </summary>
        private static async Task PostRoleMessage(SocketSlashCommand command)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildData == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            List<dynamic> roles = await MySqlWrapper.SQLExecuteReader(
               "SELECT `role_id` FROM `guild_user_roles` WHERE `guild_id` = @guild_id",
               new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId } });

            if (roles == null || roles.Count <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("notFoundAnyUserRoles", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
            if (guild == null)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("fetchGuildError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                await Utilities.SendDevLogMessage(1, $"Guild data could not be fetched: {(ulong)command.GuildId}.");
                return;
            }

            int componentLimit = 0; // max 40 components possible.

            var builder = new ComponentBuilder();
            foreach (dynamic id in roles)
            {
                var role = guild.GetRole(Convert.ToUInt64(id.role_id));
                builder.WithButton(role.Name, "respond_role_" + id.role_id.ToString(), ButtonStyle.Primary);

                componentLimit++;
                if (componentLimit >= 40) break;
            }

            string message = await LanguageManager.GetTranslation("sendUserRolesMessage",0, guildData.Language);
            await command.DeleteOriginalResponseAsync();
            await command.Channel.SendMessageAsync(message, components: builder.Build());
        }

        /// <summary>
        /// Sends a list with the current registered user roles for this guild back.
        /// </summary>
        private static async Task GetCurrentRoleList(SocketSlashCommand command)
        {
            List<dynamic> roles = await MySqlWrapper.SQLExecuteReader(
               "SELECT `role_id` FROM `guild_user_roles` WHERE `guild_id` = @guild_id",
               new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId } });

            if (roles == null || roles.Count <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("notFoundAnyUserRoles", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("userRolesTitel", command.User.Id);

            foreach (dynamic row in roles)
                message = message + $"- <@&{row.role_id}>\n";

            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }

        /// <summary>
        /// Removes all user roles for this guild.
        /// </summary>
        private static async Task ClearCurrentRoleList(SocketSlashCommand command)
        {
            int delteCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `guild_user_roles` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId } });

            if (delteCount > 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("allUserRolesForGuildRemoved", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("notFoundAnyUserRoles", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
        #endregion
    }



    /// <summary>
    /// This class is building a button for <seealso cref="Roles"/>.
    /// </summary>
    internal class RoleButton : ButtonPressed
    {
        /// <summary>
        /// This constructor is a builder for the button with custom id <paramref name="role"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="Roles"/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal RoleButton(string customId) : base(customId)
        {
            WithCustomId("role");
        }

        /// <summary>
        /// This function is handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)button.GuildId);
            if (guildData == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)button.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (guildData.IsGatedCommunity && await PermissionManager.HasUserBotPermissionRole("member", (ulong)button.GuildId, (SocketGuildUser)button.User) == false)
            {
                string errorMessage = await LanguageManager.GetTranslation("notMember", button.User.Id);
                await button.ModifyOriginalResponseAsync( func => func.Content = errorMessage );
                return;
            }

            string[] splitedCustomId = button.Data.CustomId.Split('_'); // respond_role_roleId
            ulong roleId = Convert.ToUInt64(splitedCustomId[2]);

            var guild = StartBotInstance._client.GetGuild((ulong)button.GuildId);
            if (guild == null)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync( func => func.Content = errorMessage2 );
                await Utilities.SendDevLogMessage(1, $"Could not fetch guild: {(ulong)button.GuildId}");
                return;
            }

            var role = guild.GetRole(roleId);
            if (role == null)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                await Utilities.SendDevLogMessage(1, $"Could not fetch role. Guild {(ulong)button.GuildId} role {roleId}");
                return;
            }

            SocketGuildUser user = (SocketGuildUser)button.User;
            if ( user == null)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket user. Guild {(ulong)button.GuildId} role {roleId}");
                return;
            }

            var roleList = user.Roles.Where(x => !x.IsEveryone).Select(x => x.Id).ToList();

            if (roleList.Count() > 0 && roleList.Contains(roleId))
            {
                await user.RemoveRoleAsync(roleId);

                string message = await LanguageManager.GetTranslation("youTossedTheRole", button.User.Id, "", role.Name);
                await button.ModifyOriginalResponseAsync(func => func.Content = message);
            }
            else
            {
                await user.AddRoleAsync(roleId);

                string message = await LanguageManager.GetTranslation("youGotTheRole", button.User.Id, "", role.Name);
                await button.ModifyOriginalResponseAsync(func => func.Content = message);
            }
        }
    }
}
