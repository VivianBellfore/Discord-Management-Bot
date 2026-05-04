
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="seereport"/> command.
    /// </summary>
    internal class GetUserReports : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GetUserReports() : base("mod", "seereport", "command_mod_seereport") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            ulong userId = 0;
            try
            {
                userId = Convert.ToUInt64(command.Data.Options.First().Options.ElementAt(0).Value);
            }
            catch
            {
                string errorMessage = await LanguageManager.GetTranslation("userDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            List<dynamic> userReports = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `reports` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (userReports == null || userReports.Count == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("noUserReports", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("userReportTitle", command.User.Id, "", userId);
            foreach (dynamic report in userReports)
            {
                message += $"- **{report.date}** {report.reason} - {report.comment}\n";
            }

            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
