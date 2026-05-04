
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Command for admins to set the invite link into database.
    /// </summary>
    internal class SetInviteLink : CommandObject
    {
        /// <summary>
        /// Strukt for the invite link command informations.
        /// </summary>
        internal SetInviteLink() : base("admin", "invite", "command_admin_invite") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            string invite = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            if ( !Utilities.ValidateUrlWithUri(invite) )
            {
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("urlInvalid", command.User.Id));
                return;
            }

            GuildObject guild = await GuildManager.GetGuildData((ulong)command.GuildId);

            if ( guild == null )
            {
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id));
                return;
            }

            int update = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_data` SET `invite_link` = @url WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "url", invite }, { "guild_id", (ulong)command.GuildId } });

            if (update <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Invite link for server {(ulong)command.GuildId} could not be saved. The link was: \n<{invite}>");
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("saveDataError", command.User.Id));
                return;
            }

            await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("dataSaved", command.User.Id));
        }
    }
}
