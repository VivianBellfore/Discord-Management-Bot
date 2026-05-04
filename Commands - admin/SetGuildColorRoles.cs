
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="color"/> command.
    /// </summary>
    internal class SetGuildColorRoles : CommandObject
    {
        /// <summary>
        /// Struct for the help command informations.
        /// </summary>
        internal SetGuildColorRoles() : base("admin", "color", "command_admin_color") { }



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

            IRole role = command.Data.Options.First().Options.ElementAt(0).Value as IRole;
            ulong roleId = role.Id;
            bool isAddedRole = command.Data.Options.First().Options.ElementAt(1).Value.ToString() == "add" ? true : false;
             
            if ( isAddedRole)
            {
                object existingEntry = await MySqlWrapper.SQLExecuteScalar(
                    "SELECT `role_type` FROM `guild_special_roles` WHERE `guild_id` = @guild_id AND `role_id` = @role_id AND `role_type` = @role_type",
                    new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_id", roleId }, { "role_type", "color" } });

                if (existingEntry != null)
                {
                    string errorMessage = await LanguageManager.GetTranslation("roleAlreadyAdded", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `guild_special_roles` (`guild_id`, `role_id`, `role_type`) VALUES (@guild_id, @role_id, @role_type)",
                    new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_id", roleId }, { "role_type", "color" } });

                if (insertCount > 0)
                {
                    string errorMessage = await LanguageManager.GetTranslation("colorRoleAdded", command.User.Id, "", roleId);
                    await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                await Utilities.SendDevLogMessage(1, $"Could not save color role! Guild id was: {(ulong)command.GuildId} and role id was: {roleId}.");
                string errorMessage2 = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            int removeCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `guild_special_roles` WHERE `guild_id` = @guild_id AND `role_id` = @role_id AND `role_type` = @role_type",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_id", roleId }, { "role_type", "color" } });

            if (removeCount > 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("colorRoleRemoved", command.User.Id, "", roleId);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await Utilities.SendDevLogMessage(1, $"Could not remove color role! Guild id was: {(ulong)command.GuildId} and role id was: {roleId}.");
            string errorMessage3 = await LanguageManager.GetTranslation("generalError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
        }
    }
}
