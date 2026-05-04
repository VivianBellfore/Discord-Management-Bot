
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <paramref name="admin channel"/> command.<para/>
    /// Channel just need to be setup in AdminCommands.cs, name needs to be db column name.
    /// </summary>
    internal class SetChannel : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal SetChannel() : base("admin", "channel", "command_admin_channel") { }



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

            foreach (var option in command.Data.Options.First().Options)
            {
                int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                    $"UPDATE `guild_channel` SET `{option.Name}` = @value WHERE `guild_id` = @guild_id",
                    new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "value", (option.Value as IChannel).Id } });

                if (updateCount <= 0)
                {
                    int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                        $"INSERT INTO `guild_channel` (`guild_id`, `{option.Name}`) VALUES (@guild_id, @value)",
                        new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "value", (option.Value as IChannel).Id } });

                    if (insertCount <= 0)
                    {
                        await Utilities.SendDevLogMessage(1, $"Could not update or insert guild channel `{option.Name}` for guild {(ulong)command.GuildId}.");
                        string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                        await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                        return;
                    }
                }
            }

            string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
