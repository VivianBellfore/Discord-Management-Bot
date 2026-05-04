
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="rolechange"/> command.
    /// </summary>
    internal class ChangeUserRoles : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal ChangeUserRoles() : base("admin", "rolechange", "command_admin_rolechange") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            IRole member = command.Data.Options.First().Options.ElementAt(0).Value as IRole;
            if (member == null)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("roleReadError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            if (command.Data.Options.First().Options.ElementAt(1).Value.ToString() == "add")
            {
                await AddUserRole(command, member);
                return;
            }
                
            if (command.Data.Options.First().Options.ElementAt(1).Value.ToString() == "remove")
            {
                await RemoveUserRole(command, member);
                return;
            }

            string message = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
            await Utilities.SendDevLogMessage(1, $"User role âdd or remove value was invalid.");
        }



        /// <summary>
        /// Adds a user role to the database.
        /// </summary>
        private async Task AddUserRole(SocketSlashCommand command, IRole role)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildData == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (guildData.MemberRole == role.Id || guildData.AdminRole == role.Id || guildData.ModeratorRole == role.Id)
            {
                string errorMessage = await LanguageManager.GetTranslation("roleIsSystemRole", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            object roleExist = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `role_id` FROM `guild_user_roles` WHERE `guild_id` = @guild_id AND `role_id` = @role_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_id", role.Id } });

            if (roleExist != null)
            {
                string errorMessage = await LanguageManager.GetTranslation( "userRoleAlreadyAdded", command.User.Id, "", role.Name );
                await command.ModifyOriginalResponseAsync( func => func.Content = errorMessage );
                return;
            }

            int countInsert = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `guild_user_roles` (`guild_id`, `role_id`) VALUES (@guild_id, @role_id)",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_id", role.Id } });

            if (countInsert > 0)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("userRoleAdded", command.User.Id, "", role.Name );
                await command.ModifyOriginalResponseAsync( func => func.Content = errorMessage2);
                return;
            }

            string message = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
            await Utilities.SendDevLogMessage(1, $"User role could not be saved: Guild {(ulong)command.GuildId} and Role {role.Id}");
        }

        /// <summary>
        /// Removes a user role from database.
        /// </summary>
        private async Task RemoveUserRole(SocketSlashCommand command, IRole role)
        {
            object roleExist = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `role_id` FROM `guild_user_roles` WHERE `guild_id` = @guild_id AND `role_id` = @role_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_id", role.Id } });

            if (roleExist == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("roleNotSavedAsUserRole", command.User.Id, "", role.Name);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int removeCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `guild_user_roles` WHERE `guild_id` = @guild_id AND `role_id` = @role_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_id", role.Id } });

            if (removeCount > 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("userRoleRemoved", command.User.Id, "", role.Name);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
            await Utilities.SendDevLogMessage(1, $"User role could not be removed: Guild {(ulong)command.GuildId} and Role {role.Id}");
        }
    }
}
