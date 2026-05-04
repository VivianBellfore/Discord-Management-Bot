
using Discord.WebSocket;

using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="wordlist"/> command.
    /// </summary>
    internal class GetWordfilterList : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal GetWordfilterList() : base("mod", "wordlist", "command_mod_wordlist") { }

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

            List<string> blocketTexts = new List<string>();
            string message = await LanguageManager.GetTranslation("wordListTitle", command.User.Id);

            if (BlockedTextManager.GuildBlacklists.ContainsKey(guildObject.GuildId) && BlockedTextManager.GuildBlacklists[guildObject.GuildId].Count > 0)
                blocketTexts = BlockedTextManager.GuildBlacklists[guildObject.GuildId];
            else
            {
                string emptyMessage = await LanguageManager.GetTranslation("wordListEmpty", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = message + emptyMessage);
                return;
            }

            foreach (string text in blocketTexts)
            {
                message += $"- {text}\n";
            }

            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
