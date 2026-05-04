
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="wordadd"/> command.
    /// </summary>
    internal class AddWordfilter : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal AddWordfilter() : base("admin", "wordadd", "command_admin_wordadd") { }

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

            if (text == null || text.Length == 0 || text.Length > 50)
            {
                string errorMessage = await LanguageManager.GetTranslation("commandValueInvalid", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `blocked_text` (`guild_id`, `text`) VALUES (@guild_id, @text)",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "text", text } });

            if (updateCount > 0)
            {
                if (BlockedTextManager.GuildBlacklists.ContainsKey(guildObject.GuildId))
                    BlockedTextManager.GuildBlacklists[(ulong)command.GuildId].Add(text);
                else
                    BlockedTextManager.GuildBlacklists.Add(guildObject.GuildId, new List<string>() { text });

                string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = message);
                return;
            }

            await Utilities.SendDevLogMessage(1, $"Error while saving name.\nGuild id was {(ulong)command.GuildId} and name was `{text}`.");
            string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
        }
    }
}
