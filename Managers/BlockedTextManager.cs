
using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Managing blocked text content.<para/>
    /// </summary>
    internal class BlockedTextManager
    {
        /// <summary>
        /// The global text blacklist for all connected guilds.
        /// </summary>
        internal static List<string> GlobalBlacklist = new List<string>();

        /// <summary>
        /// The local guild text blacklists for all connected guilds.<br/>
        /// Sorted by key - guild ids.
        /// </summary>
        internal static Dictionary<ulong, List<string>> GuildBlacklists = new Dictionary<ulong, List<string>>();



        /// <summary>
        /// Reading all connected guilds from bot client and setup blacklists for them.<para/>
        /// Is executing:<br/> <seealso cref="FetchBlockedTextFromDB(ulong)"/>
        /// </summary>
        internal async Task LoadSettingsAndContent()
        {
            IReadOnlyCollection<SocketGuild> connectedGuilds = StartBotInstance._client.Guilds;

            if (connectedGuilds == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch connected guilds for blacklist settings.");
                return;
            }

            foreach (SocketGuild guild in connectedGuilds)
            {
                await FetchBlockedTextFromDB(guild.Id);
            }

            await FetchBlockedTextFromDB(0);
        }

        /// <summary>
        /// Writing all global blacklist texts from data base into <seealso cref="GlobalBlacklist"/>.<br/>
        /// Writing all guilded blacklist texts from data base into <seealso cref="GuildBlacklists"/>.<para/>
        /// </summary>
        private async Task FetchBlockedTextFromDB(ulong guildId)
        {
            List<dynamic> textBlacklist = new List<dynamic>();

            try
            {
                textBlacklist = await MySqlWrapper.SQLExecuteReader(
                "SELECT `text`, `guild_id` FROM `blocked_text` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", guildId } });
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch \"text\" from database table \"blocked_text\". No entry was found.\nException: {ex}");
                return;
            }

            if (textBlacklist.Count <= 0) return;

            List<string> guildList = new List<string>();

            if (guildId == 0)
            {
                GlobalBlacklist.Clear();

                foreach (dynamic item in textBlacklist)
                {
                    GlobalBlacklist.Add(item.text.ToString());
                }
            }
            else
            {
                GuildBlacklists.Remove(guildId);

                foreach (dynamic item in textBlacklist)
                {
                    guildList.Add(item.text.ToString());
                }

                GuildBlacklists.Add(guildId, guildList);
            }
        }

        /// <summary>
        /// Checking if a text contains blocket content from a guild or global.
        /// </summary>
        internal async Task<bool> DeletProhibitedMessage(SocketMessage message, GuildObject guildObject, ITextChannel channel)
        {
            // its the bot, dont make a loop...
            if (message.Author.Id == Configurations.BotClientId) return false;

            if (GlobalBlacklist.Count <= 0 && !GuildBlacklists.ContainsKey(guildObject.GuildId)) return false;

            bool isContentBlocked = false;

            if (GlobalBlacklist.Count > 0)
            {
                foreach (string text in GlobalBlacklist)
                {
                    if (message.Content.ToLower().Contains(text))
                    {
                        isContentBlocked = true;
                    }
                }
            }

            if (GuildBlacklists.ContainsKey(guildObject.GuildId))
            {
                foreach ( string word in GuildBlacklists[guildObject.GuildId])
                {
                    if (message.Content.ToLower().Contains(word))
                    {
                        isContentBlocked = true;
                    }
                }
            }

            if (isContentBlocked == false) return false;

            try
            {
                await GuildManager.SendSystemMessageToGuild((ulong)channel.GuildId, 0, await LanguageManager.GetTranslation("blockedTextTitle", message.Author.Id),
                await LanguageManager.GetTranslation("blockedTextWarning", message.Author.Id, "", message.Author.Id, message.Content));

                await message.DeleteAsync();
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"Could not delete message or send warning message to server.\nServer: {guildObject.GuildId}, " +
                    $"Channel: {channel.Id}, Author: {message.Author.Id}, Message: {message}\nException: {ex}");
            }

            if (await PermissionManager.IsUserBlockingBotDM(message.Author.Id) == false)
                await message.Author.SendMessageAsync(await LanguageManager.GetTranslation("blacklistMessageDeleted", message.Author.Id, "", channel.Guild.Name));

            // TODO:
            //IGuildUser user = (IGuildUser)message.Author;

            //int timeout = await GetBlacklistStrikesFromUser(user.Id, (ulong)channel.GuildId);
            //await user.SetTimeOutAsync(span: new TimeSpan(0, timeout, 0));

            return isContentBlocked;
        }
    }
}
