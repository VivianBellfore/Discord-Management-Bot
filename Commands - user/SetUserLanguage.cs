
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="use language"/> command.
    /// </summary>
    internal class SetUserLanguage : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetUserLanguage() : base("use", "language", "command_use_language") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            bool isRegisterd = await PermissionManager.HasUserAcceptTos(command.User.Id);
            if (!isRegisterd)
            {
                string errorMessage = await LanguageManager.GetTranslation("noUserDataFound", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string language = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            await LanguageManager.SetUserLanguage(language, command.User.Id);

            await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("dataSaved", command.User.Id));
        }
    }
}
