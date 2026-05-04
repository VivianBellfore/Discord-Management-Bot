
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="admin help"/> command.<para/>
    /// </summary>
    internal class GetAdminHelp : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal GetAdminHelp() : base("admin", "help", "command_admin_help") { }

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
                if (obj.GroupName == "admin")
                    message += $"\n`/admin {obj.Name}` - {await LanguageManager.GetTranslation(obj.TranslationId, userId)}";
            }

            message += $"\n\n" + await LanguageManager.GetTranslation("imprintGDPR", userId) + "\n" + await LanguageManager.GetTranslation("installationLink", command.User.Id);

            string title = await LanguageManager.GetTranslation("helpTitleAdmin", userId);

            var embedBuiler = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(message)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuiler.Build(); });
        }
    }
}
