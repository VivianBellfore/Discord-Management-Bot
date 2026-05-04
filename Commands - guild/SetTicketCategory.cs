
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="tickets"/> command.
    /// </summary>
    internal class SetTicketCategory : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetTicketCategory() : base("guild", "tickets", "command_guild_tickets") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            GuildObject guild = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            IGuildChannel channel = command.Data.Options.First().Options.First().Value as IGuildChannel;

            if (channel == null)
            {
                await Utilities.SendDevLogMessage(1, $"Channel was null. Guild id was {(ulong)command.GuildId}");
                string errorMessage = await LanguageManager.GetTranslation("channelReadError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (channel.GetChannelType() != ChannelType.Category)
            {
                string errorMessage = await LanguageManager.GetTranslation("channelNotCategory", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `guild_channel` SET `ticket` = @category_ticket WHERE `guild_id` = @guild_id",
                    new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "category_ticket", channel.Id } });

            if (updateCount <= 0)
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    $"INSERT INTO `guild_channel` (`guild_id`, `ticket`) VALUES (@guild_id, @category_ticket)",
                    new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "category_ticket", channel.Id } });

                if (insertCount <= 0)
                {
                    await Utilities.SendDevLogMessage(1, $"Data could not be updated. Guild id was {(ulong)command.GuildId} and channel id {channel.Id}");
                    string errorMessage = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }
            }

            string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
