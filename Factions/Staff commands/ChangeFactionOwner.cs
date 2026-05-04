
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="owner"/> command.
    /// </summary>
    internal class ChangeFactionOwner : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal ChangeFactionOwner() : base("fact", "owner", "command_fact_owner") { }



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
                    await command.ModifyOriginalResponseAsync(func => { func.Content = errorMessage; });
                    return;
                }
            }

            SocketGuildUser owner = (SocketGuildUser)command.Data.Options.First().Options.ElementAt(0).Value;
            if (owner == null)
            {
                await Utilities.SendDevLogMessage(1, $"User could not be fetched. Guild {(ulong)command.GuildId}.");
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

            int factionId = Convert.ToInt32(command.Data.Options.First().Options.ElementAt(1).Value);

            FactionObject faction = await FactionManager.GetFactionData(factionId);
            if ( faction == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch faction data from database. Id was {factionId}");
                string message2 = await LanguageManager.GetTranslation("generalError", command.User.Id, "", factionOwned);
                await command.ModifyOriginalResponseAsync(func => func.Content = message2);
                return;
            }

            if ( faction.Member.ContainsKey(owner.Id))
            {
                int updateMemberCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `factions_user` SET `rank_id` = @rank_id WHERE `faction_id` = @faction_id AND `user_id` = @user_id",
                    new Dictionary<string, object> { { "user_id", owner.Id }, { "faction_id", factionId }, { "rank_id", -1 } });
            }
            else
            {
                int insertMemberCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `factions_user` (`faction_id`, `user_id`, `rank_id`) VALUES (@faction_id, @user_id, @rank_id)",
                    new Dictionary<string, object> { { "user_id", owner.Id }, { "faction_id", factionId }, { "rank_id", -1 } });
            }

                string removeOldOwner = await FactionManager.RemoveOwnerPermissionFromAllFactionChannel(factionId, faction.OwnerId, command.User.Id, (ulong)command.GuildId);
            if ( removeOldOwner != "")
            {
                await Utilities.SendDevLogMessage(1, $"Old owner permissions could not be removed: {removeOldOwner}");
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `factions` SET `owner_id` = @owner_id WHERE `id` = @id AND `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "owner_id", owner.Id }, { "guild_id", (ulong)command.GuildId }, { "id", factionId } });

            if (updateCount <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Owner was not updated. Guild {(ulong)command.GuildId} user was ||{owner.Id}||.");
                string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            string addNewOwner = await FactionManager.AddOwnerPermissionForAllFactionChannel(factionId, owner.Id, command.User.Id, (ulong)command.GuildId);
            if ( addNewOwner != "")
            {
                await Utilities.SendDevLogMessage(1, $"New owner permissions could not be added: {addNewOwner}");
            }

            string message = await LanguageManager.GetTranslation("factionOwnerChanged", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);

            if ( await PermissionManager.HasUserAcceptTos(owner.Id) && await PermissionManager.IsUserBlockingBotDM(owner.Id) == false)
            {
                try
                {
                    string messageOwner = await LanguageManager.GetTranslation("factionOwnerTransfered", owner.Id, "", faction.Name);
                    await owner.SendMessageAsync(messageOwner);
                }
                catch
                {
                    await Utilities.SendDevLogMessage(2, $"Could not send new faction owner ||{owner.Id}|| the transfere message for getting ownership of the faction {faction.Name}.");
                }  
            }
        }
    }
}
