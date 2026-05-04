
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="wordremove"/> command.
    /// </summary>
    internal class RemoveWordfilter : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal RemoveWordfilter() : base("admin", "wordremove", "command_admin_wordremove") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            GuildObject guildObject = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildObject == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string text = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            if (text == null || text.Length == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("commandValueInvalid", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `blocked_text` WHERE `guild_id` = @guild_id AND `text` = @text",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "text", text } });

            if ( updateCount <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("wordfilterNotContains", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (BlockedTextManager.GuildBlacklists.ContainsKey(guildObject.GuildId))
                BlockedTextManager.GuildBlacklists[guildObject.GuildId].Remove(text);

            string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
