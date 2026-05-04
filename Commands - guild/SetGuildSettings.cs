
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="settings"/> command.
    /// </summary>
    internal class SetGuildSettings : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetGuildSettings() : base("guild", "settings", "command_guild_settings") { }

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

            int updateError = 0;

            foreach (var option in command.Data.Options.First().Options)
            {
                int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                    $"UPDATE `guild_data` SET `{option.Name}` = @value WHERE `guild_id` = @guild_id",
                    new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "value", Convert.ToInt32(option.Value) } });

                if (updateCount <= 0)
                {
                    await Utilities.SendDevLogMessage(1, $"Setting {option.Name} for guild {(ulong)command.GuildId} could not be updated! Value was {Convert.ToInt32(option.Value)}.");
                    updateError++;
                }
                    
            }

            string errorMessage2 = "";
            if ( updateError > 0 )
                errorMessage2 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            else
                errorMessage2 = await LanguageManager.GetTranslation("dataSaved", command.User.Id);

            await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
        }
    }
}
