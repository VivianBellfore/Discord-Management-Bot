
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="color"/> command.
    /// </summary>
    internal class GetColorRoles : CommandObject
    {
        /// <summary>
        /// Struct for the help command informations.
        /// </summary>
        internal GetColorRoles() : base("use", "color", "command_use_color") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT `role_id` FROM `guild_special_roles` WHERE `guild_id` = @guild_id AND `role_type` = @role_type",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "role_type", "color" } });

            if (results.Count <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("noColorRolesForGuild", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string roleList = "";
            foreach (dynamic result in results)
            {
                roleList += $"- <@&{Convert.ToUInt64(result.role_id)}>\n";
            }

            string message = await LanguageManager.GetTranslation("getGuildColorList", command.User.Id, "", roleList);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
