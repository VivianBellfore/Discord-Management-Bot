
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the "/winter <paramref name="work"/>" command.<para/>
    /// </summary>
    internal class DoWinterWork : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal DoWinterWork() : base("winter", "work", "command_winter_work") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            if ( DateTime.Now.Month != 12 )
            {
                string errorMessage = await LanguageManager.GetTranslation("itsNotWinterTime", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            (bool canWork, string workTime) = await WinterManager.CanUserDoWinterWorkNow(command.User.Id);

            if (!canWork)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("cantDoWinterWorkNow", command.User.Id, "", workTime);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            await MySqlWrapper.SetIntegerForIdentifier("user_profile", "winter_points", new Dictionary<string, object> { { "user_id", command.User.Id } }, 10, 1, false);

            string message = await LanguageManager.GetTranslation("doWinterWork", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
