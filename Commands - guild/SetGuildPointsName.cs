
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="pointname"/> command.
    /// </summary>
    internal class SetGuildPointsName : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetGuildPointsName() : base("guild", "pointname", "command_guild_pointname") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildData == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string name = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            if (name == null || name.Length == 0 || name.Length > 30)
            {
                string errorMessage = await LanguageManager.GetTranslation("commandValueInvalid", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_data` SET `points_name` = @points_name WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "points_name", name } });

            if (updateCount > 0)
            {
                string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = message);
                return;
            }

            await Utilities.SendDevLogMessage(1, $"Error while saving name.\nGuild id was {(ulong)command.GuildId} and name was `{name}`.");
            string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
        }
    }
}
