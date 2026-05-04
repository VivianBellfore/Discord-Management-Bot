
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="use botdm"/> command.
    /// </summary>
    internal class BlockBotDM : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal BlockBotDM() : base("use", "botdm", "command_use_botdm") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            SocketGuildUser user = command.User as SocketGuildUser;
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"User was null! Id was || {command.User.Id} ||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            bool isMember = await PermissionManager.HasUserBotPermissionRole("member", (ulong)command.GuildId, user);

            if (!isMember)
            {
                string errorMessage = await LanguageManager.GetTranslation("notMember", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }  

            string setting = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            int settingInteger = setting == "no" ? 1 : 0;

            bool isUpdated = await MySqlWrapper.SetIntegerForIdentifier("user_profile", "block_bot_dm", new Dictionary<string, object>() { { "user_id", command.User.Id } }, settingInteger, 0, false);

            if (!isUpdated)
            {
                await Utilities.SendDevLogMessage(1, $"Could not save data. User was || {command.User.Id} || and integer was {settingInteger}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
