
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="permissions"/> command.
    /// </summary>
    internal class SetGuildPermissions : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetGuildPermissions() : base("guild", "permissions", "command_guild_permissions") { }

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

            IRole admin = command.Data.Options.First().Options.ElementAt(0).Value as IRole;
            IRole mod = command.Data.Options.First().Options.ElementAt(1).Value as IRole;

            if (admin == null || mod == null)
            {
                await Utilities.SendDevLogMessage(1, $"One of the roles was null!");
                string errorMessage = await LanguageManager.GetTranslation("roleReadError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_data` SET `role_mod` = @role_mod, `role_admin` = @role_admin WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_mod", mod.Id }, { "role_admin", admin.Id } });

            if (updateCount > 0)
            {
                string message = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = message);
                return;
            }

            await Utilities.SendDevLogMessage(1, $"Error while saving guild permission roles to database.\nMod id {mod.Id} and Admin id {admin.Id} on guild id {(ulong)command.GuildId}.");

            string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
        }
    }
}
