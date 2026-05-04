
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="use register"/> command.<para/>
    /// </summary>
    internal class UserRegister : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal UserRegister() : base("use", "register", "command_use_register") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            bool isRegisterd = await PermissionManager.HasUserAcceptTos(command.User.Id);
            if (isRegisterd)
            {
                string errorMessage = await LanguageManager.GetTranslation("alreadyRegistered", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildData == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string label = await LanguageManager.GetTranslation("buttonAcceptUserTOS", 0, guildData.Language);
            var buttonBuilder = new ComponentBuilder().WithButton(label, $"respond_userregister_{guildData.Language}", ButtonStyle.Success);

            string message = await LanguageManager.GetTranslation("registerUserInfoText", 0, guildData.Language, command.User.GlobalName);

            await command.ModifyOriginalResponseAsync(func => { func.Content = message; func.Components = buttonBuilder.Build(); });
        }
    }



    /// <summary>
    /// Building a button for <seealso cref="userregister"/>.
    /// </summary>
    internal class UserRegisterButton : ButtonPressed
    {
        /// <summary>
        /// Builder for the button with custom id <paramref name="userregister"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="UserRegister"/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal UserRegisterButton(string customId) : base(customId)
        {
            WithCustomId("userregister");
        }

        /// <summary>
        /// Handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            bool isRegisterd = await PermissionManager.HasUserAcceptTos(button.User.Id);
            if (isRegisterd)
            {
                string errorMessage = await LanguageManager.GetTranslation("alreadyRegistered", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string[] splitedCustomId = button.Data.CustomId.Split('_'); // "respond_userregister_language"

            string message;

            bool isRegistered = await UserManager.RegisterUser(button.User.Id, splitedCustomId[2]);

            await LanguageManager.SetUserLanguage(splitedCustomId[2], button.User.Id);

            if (isRegistered)
                message = await LanguageManager.GetTranslation("registerMessage", button.User.Id);
            else
                message = await LanguageManager.GetTranslation("registerCanceled", button.User.Id);

            await button.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
