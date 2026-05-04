
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="name"/> command.
    /// </summary>
    internal class SetFactionName : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetFactionName() : base("fact", "name", "command_fact_name") { }



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

            string name = command.Data.Options.First().Options.ElementAt(0).Value.ToString();
            string description = command.Data.Options.First().Options.ElementAt(1).Value.ToString();

            int minLength = 6;

            if ( name.Length < minLength || description.Length < minLength)
            {
                string errorMessage2 = await LanguageManager.GetTranslation("inputTextToShort", command.User.Id, "", minLength);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            int factionId = Convert.ToInt32(factionOwnerString.Split(' ')[0]);

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `factions` SET `name` = @name, `description` = @description WHERE `id` = @id",
                new Dictionary<string, object>() { { "id", factionId }, { "name", name }, { "description", description } });

            if (updateCount <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Could not save name and description. User id was ||{command.User.Id}|| and text was name: {name} and description: {description}");
                string errorMessage3 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
                return;
            }

            FactionObject faction = await FactionManager.GetFactionData(factionId);
            if (faction == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch faction data. User id was ||{command.User.Id}|| and faction id is {factionId}.");
                string errorMessage3 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
                return;
            }

            SocketGuild guild = Utilities.GetGuildSocket((ulong)command.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild from id {(ulong)command.GuildId}.");
                string errorMessage3 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
                return;
            }

            SocketCategoryChannel category = guild.GetCategoryChannel(faction.CategoryId);
            if (category == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild channel from id {faction.CategoryId}.");
                string errorMessage3 = await LanguageManager.GetTranslation("saveDataError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
                return;
            }

            await category.ModifyAsync(func => func.Name = faction.Name);

            string emessage = await LanguageManager.GetTranslation("dataSaved", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = emessage);
        }
    }
}
