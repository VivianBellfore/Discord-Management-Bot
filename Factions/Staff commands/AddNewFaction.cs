
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="new"/> command.
    /// </summary>
    internal class AddNewFaction : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal AddNewFaction() : base("fact", "new", "command_fact_new") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            if (await PermissionManager.HasUserBotPermissionRole("admin", (ulong)command.GuildId, (SocketGuildUser)command.User) == false)
            {
                if (await PermissionManager.IsUserGuildOwner((ulong)command.GuildId, command.User.Id) == false)
                {
                    string errorMessage = await LanguageManager.GetTranslation("missingPermisson", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }
            }

            SocketGuildUser owner = (SocketGuildUser)command.Data.Options.First().Options.ElementAt(0).Value;
            if (owner == null)
            {
                await Utilities.SendDevLogMessage(1, $"User could not be fetched. Guild {(ulong)command.GuildId} user was {command.Data.Options.First().Options.ElementAt(0).Value}");
                string errorMessage2 = await LanguageManager.GetTranslation("userDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            (bool isUserAlreadyFactionOwner, string factionOwned) = await FactionManager.IsUserFactionOwner(owner.Id, (ulong)command.GuildId);
            if (isUserAlreadyFactionOwner)
            {
                string message2 = await LanguageManager.GetTranslation("userIsAlreadyOwner", command.User.Id, "", factionOwned);
                await command.ModifyOriginalResponseAsync(func => func.Content = message2);
                return;
            }

            await FactionManager.CreateNewGuildFaction(command, owner);
        }
    }
}
