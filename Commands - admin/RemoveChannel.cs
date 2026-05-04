
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <paramref name="admin remchannel"/> command.<para/>
    /// Channel just need to be setup in AdminCommands.cs, name needs to be db column name.
    /// </summary>
    internal class RemoveChannel : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal RemoveChannel() : base("admin", "remchannel", "command_admin_remchannel") { }



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

            string name = command.Data.Options.First().Options.ElementAt(0).Name;

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                $"UPDATE `guild_channel` SET `{name}` = @value WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "value", 0 } });

            if ( updateCount <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Could not update channel! Channel name is {name} guild was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
