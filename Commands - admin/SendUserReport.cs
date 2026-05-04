
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="report"/> command.
    /// </summary>
    internal class SendUserReport : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal SendUserReport() : base("admin", "report", "command_admin_report") { }



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

            string reason = command.Data.Options.First().Options.ElementAt(1).Value.ToString();
            string report = command.Data.Options.First().Options.ElementAt(2).Value.ToString();

            if ( report == "" || report.Length < 4)
            {
                string errorMessage = await LanguageManager.GetTranslation("reportEmpty", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `reports` (`user_id`, `comment`, `date`, `reporter_name`) VALUES (@user_id, @comment, @date, @reporter_name)",
                new Dictionary<string, object>() { { "user_id", userId }, { "reason", reason }, { "comment", report }, { "date", DateTime.Today.ToString("dd/MM/yyyy") }, 
                    { "reporter_id", command.User.Id }, { "guild_id", (ulong)command.GuildId} });

            string message = "";

            if (insertCount > 0)
                message = await LanguageManager.GetTranslation("reportInserted", command.User.Id);
            else
                message = await LanguageManager.GetTranslation("ReportInsertError", command.User.Id);

            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
