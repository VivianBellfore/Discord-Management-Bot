
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="mod help"/> command.<para/>
    /// </summary>
    internal class GetModHelp : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal GetModHelp() : base("mod", "help", "command_mod_help") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            ulong userId = command.User.Id;
            string message = "";

            foreach (CommandObject obj in commandObjectList)
            {
                if (obj.GroupName == "mod")
                    message += $"\n`/mod {obj.Name}` - {await LanguageManager.GetTranslation(obj.TranslationId, userId)}";
            }

            message += $"\n\n" + await LanguageManager.GetTranslation("imprintGDPR", userId) + "\n" + await LanguageManager.GetTranslation("installationLink", command.User.Id);

            string title = await LanguageManager.GetTranslation("helpTitleMod", userId);

            var embedBuiler = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(message)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuiler.Build(); });
        }
    }
}
