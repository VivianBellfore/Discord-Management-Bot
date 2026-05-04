
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="colorrole"/> command.
    /// </summary>
    internal class BuyColorRole : CommandObject
    {
        /// <summary>
        /// Struct for the help command informations.
        /// </summary>
        internal BuyColorRole() : base("use", "colorrole", "command_use_colorrole") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            if ( await PermissionManager.HasUserAcceptTos(command.User.Id) == false)
            {
                string errorMessage = await LanguageManager.GetTranslation("needToBeRegistered", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT `role_id` FROM `guild_special_roles` WHERE `guild_id` = @guild_id AND `role_type` = @role_type",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_type", "color" } });

            if (results.Count <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("noColorRolesForGuild", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            IRole role = command.Data.Options.First().Options.ElementAt(0).Value as IRole;
            ulong roleId = role.Id;

            bool isColorRole = false;
            foreach ( dynamic result in results )
            {
                if ( Convert.ToUInt64(result.role_id) == roleId)
                    isColorRole = true;
            }

            if (!isColorRole)
            {
                string errorMessage = await LanguageManager.GetTranslation("roleIsNotColorRole", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            SocketGuildUser user = (SocketGuildUser)command.User;
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket user. Guild {(ulong)command.GuildId} role {roleId}");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var roleList = user.Roles.Where(x => !x.IsEveryone).Select(x => x.Id).ToList();

            if (roleList.Count() > 0 && roleList.Contains(roleId))
            {
                string errorMessage = await LanguageManager.GetTranslation("alreadyOwnedColorRole", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            UserObject userData = await UserManager.GetUserData(command.User.Id);
            if (userData == null)
            {
                await Utilities.SendDevLogMessage(1, $"User should be existing, has checked the TOS before, but is not fetching user data. User id is ||{command.User.Id}||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if ( !userData.GuildPoints.ContainsKey((ulong)command.GuildId) || userData.GuildPoints[(ulong)command.GuildId] < 200)
            {
                string errorMessage = await LanguageManager.GetTranslation("notEnoughGuildPoints", command.User.Id, "", 200);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            bool pointsAreRemoved = await UserManager.SetUserGuildPoints(command.User.Id, (ulong)command.GuildId, 200, false);

            if ( !pointsAreRemoved)
            {
                await Utilities.SendDevLogMessage(1, $"Could ot remove guild points even when they should exist from user data. User id is ||{command.User.Id}|| and guild is {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            foreach (dynamic result in results)
            {
                if ( roleList.Contains(Convert.ToUInt64(result.role_id)) )
                {
                    await user.RemoveRoleAsync(Convert.ToUInt64(result.role_id));
                    await Task.Delay(1500);
                }
            }

            await user.AddRoleAsync(roleId);
            await Task.Delay(1500);

            string message = await LanguageManager.GetTranslation("youGotTheRole", command.User.Id, "", role.Name);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
