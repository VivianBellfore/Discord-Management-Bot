
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="member"/> command.
    /// </summary>
    internal class SetGuildMember : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetGuildMember() : base("guild", "member", "command_guild_member") { }

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

            IRole member = command.Data.Options.First().Options.ElementAt(0).Value as IRole;
            if (member == null)
            {
                await Utilities.SendDevLogMessage(1, $"The roles was null!");
                string errorMessage2 = await LanguageManager.GetTranslation("roleReadError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_data` SET `role_member` = @role_member WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_member", member.Id } });

            if (updateCount > 0)
            {
                string errorMessage3 = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
                return;
            }         

            await Utilities.SendDevLogMessage(1, $"Data could not be saved. Guild is existing.\nGuild id is {(ulong)command.GuildId} and role was {member.Id}.");
            string message = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
