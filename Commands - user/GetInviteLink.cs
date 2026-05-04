
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <paramref name="use invite"/> command.<para/>
    /// </summary>
    internal class GetInviteLink : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GetInviteLink() : base("use", "invite", "command_use_invite") { }

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
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (guildData.InviteLink == "")
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("missingInvite", command.User.Id));
            else
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("getInviteLink", command.User.Id, "", guildData.InviteLink));
        }
    }
}
