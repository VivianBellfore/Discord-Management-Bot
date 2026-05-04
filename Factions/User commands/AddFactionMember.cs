
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="addmember"/> command.
    /// </summary>
    internal class AddFactionMember : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal AddFactionMember() : base("fact", "addmember", "command_fact_addmember") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            (bool isUserFactionLeader, string factionOwnerString) = await FactionManager.IsUserFactionOwner(command.User.Id, (ulong)command.GuildId);
            if (!isUserFactionLeader)
            {
                string errorMessage = await LanguageManager.GetTranslation("notFactionLeader", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            FactionObject faction = await FactionManager.GetFactionData(Convert.ToInt32(factionOwnerString.Split(' ')[0]));
            if (faction == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch faction data after checking fction owner? Owner is ||{command.User.Id}|| and faction id is {factionOwnerString.Split(' ')[0]}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content= errorMessage);
                return;
            }

            if (faction.Member != null && faction.MaxMember <= faction.Member.Count)
            {
                string errorMessage = await LanguageManager.GetTranslation("factionMemberMaxCount", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            SocketGuildUser user = (SocketGuildUser)command.Data.Options.First().Options.ElementAt(0).Value;
            if ( user == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild user. Id was ||{command.Data.Options.First().Options.ElementAt(0).Value.ToString()}||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if ( faction.Member != null && faction.Member.ContainsKey(user.Id))
            {
                string errorMessage = await LanguageManager.GetTranslation("userIsAlreadyFactionUser", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await FactionManager.AddFactionUser(command, factionOwnerString, user);
        }
    }
}
