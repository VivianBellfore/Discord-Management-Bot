
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is handeling all functions for the <paramref name="channel"/> command.
    /// </summary>
    internal class SetGuildChannel : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal SetGuildChannel() : base("guild", "channel", "command_guild_channel") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
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

            IGuildChannel logs = command.Data.Options.First().Options.ElementAt(0).Value as IGuildChannel;
            IGuildChannel news = command.Data.Options.First().Options.ElementAt(1).Value as IGuildChannel;
            IGuildChannel events = command.Data.Options.First().Options.ElementAt(2).Value as IGuildChannel;

            if (logs == null || news == null || events == null)
            {
                await Utilities.SendDevLogMessage(1, $"One or more of the channel was null!\nGuild id is {(ulong)command.GuildId} and user was || {command.User.Id} ||.");
                string errorMessage = await LanguageManager.GetTranslation("channelReadError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_channel` SET `system` = @channel_logs, `news` = @channel_news, `events` = @channel_events WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "channel_logs", logs.Id }, { "channel_news", news.Id }, { "channel_events", events.Id } });

            if (updateCount <= 0)
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    $"INSERT INTO `guild_channel` (`guild_id`, `news`, `system`, `events`) VALUES (@guild_id, @news, @system, @events)",
                    new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "system", logs.Id }, { "news", news.Id }, { "events", events.Id } });

                if (insertCount <= 0)
                {
                    await Utilities.SendDevLogMessage(1, $"Data could not be updated. Guild id was {(ulong)command.GuildId}.");
                    string errorMessage = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }
            }

            string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
        }
    }
}
