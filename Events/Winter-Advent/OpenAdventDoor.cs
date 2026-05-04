
using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the "/winter <paramref name="advent"/>" command.<para/>
    /// </summary>
    internal class OpenAdventDoor : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal OpenAdventDoor() : base("winter", "advent", "command_winter_advent") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            if (DateTime.Now.Month != 12 || DateTime.Now.Month == 12 && DateTime.Now.Day > 24)
            {
                string errorMessage = await LanguageManager.GetTranslation("itsNotAdventTime", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            object lastActivatedDate = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `date` FROM `user_event_date` WHERE `user_id` = @user_id AND `event_type` = @event_type",
                new Dictionary<string, object>() { { "user_id", command.User.Id }, { "event_type", "discord_advent" } });

            DateTime lastAction;

            if (lastActivatedDate == null)
            {
                await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `user_event_date` (`user_id`, `date`, `event_type`) VALUES (@user_id, @date, @event_type)",
                    new Dictionary<string, object>() { { "user_id", command.User.Id }, { "date", DateTime.Now.ToString("o") }, { "event_type", "discord_advent" } });

                lastAction = DateTime.Now.AddDays(-1);
            }
            else
            {
                await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `user_event_date` SET `date` = @date WHERE `user_id` = @user_id AND `event_type` = @event_type",
                    new Dictionary<string, object>() { { "user_id", command.User.Id }, { "date", DateTime.Now.ToString("o") }, { "event_type", "discord_advent" } });

                lastAction = DateTime.Parse(Convert.ToString(lastActivatedDate));
            }
            
            if ( lastAction.Day == DateTime.Now.Day)
            {
                string errorMessage = await LanguageManager.GetTranslation("adventAlreadyOpend", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await OpenAdventCalenderDoor(command);
        }



        private async Task OpenAdventCalenderDoor(SocketSlashCommand command)
        {
            int date = DateTime.Now.Day;

            if (date == 6)
            {
                await GetNikolausPresent(command);
                return;
            }

            if (date == 24)
            {
                await GetChristmasPresent(command);
                return;
            }

            AdventObject adventItem = WinterManager.AdventItems[date];

            int chance = Utilities.random.Next(0, 14);
            ItemObject present = null;

            if (chance < 10)
            {
                List<int> itemIds = InventoryManager.GetItemsByType("advent");
                present = InventoryManager.ExistingItems[itemIds[Utilities.random.Next(0, itemIds.Count)]];

                bool itemIsGiven = await InventoryManager.GiveItemToUser(command.User.Id, present.Id, 1);

                if ( !itemIsGiven )
                {
                    await Utilities.SendDevLogMessage(1, $"Give advent item was invalid. User is ||{command.User.Id}|| and item is {present.Id}.");
                    string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }
            }
            else
            {
                present = InventoryManager.ExistingItems[1];
                bool itemIsGiven = await InventoryManager.GiveItemToUser(command.User.Id, present.Id, 1);

                if (!itemIsGiven)
                {
                    await Utilities.SendDevLogMessage(1, $"Give advent item was invalid. User is ||{command.User.Id}|| and item is {present.Id}.");
                    string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }
            }

            await MySqlWrapper.SetIntegerForIdentifier("user_profile", "winter_points", new Dictionary<string, object> { { "user_id", command.User.Id } }, 200, 1, false);

            string description = await LanguageManager.GetTranslation("adventDoorDescription", command.User.Id, "", adventItem.Poem, present.Name, present.CardURL);

            var embedBuilder = new EmbedBuilder()
                    .WithTitle($"Advent {DateTime.Now.Year} :christmas_tree:")
                    .WithDescription(description)
                    .WithColor(Color.Gold)
                    .WithImageUrl(adventItem.Gif);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuilder.Build(); });
        }

        private async Task GetNikolausPresent(SocketSlashCommand command)
        {
            ItemObject present = InventoryManager.ExistingItems[2];

            bool itemIsGiven = await InventoryManager.GiveItemToUser(command.User.Id, present.Id, 1);

            if (!itemIsGiven)
            {
                await Utilities.SendDevLogMessage(1, $"Give advent item was invalid. User is ||{command.User.Id}|| and item is {present.Id}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await MySqlWrapper.SetIntegerForIdentifier("user_profile", "winter_points", new Dictionary<string, object> { { "user_id", command.User.Id } }, 600, 1, false);

            AdventObject adventItem = WinterManager.AdventItems[DateTime.Now.Day]; 

            string description = await LanguageManager.GetTranslation("adventDoorDescription", command.User.Id, "", adventItem.Poem, present.Name, present.CardURL);

            var embedBuilder = new EmbedBuilder()
                    .WithTitle($"Advent {DateTime.Now.Year} :christmas_tree:")
                    .WithDescription(description)
                    .WithColor(Color.Gold)
                    .WithImageUrl(adventItem.Gif);

            await command.ModifyOriginalResponseAsync(func => func.Embed = embedBuilder.Build());
        }

        private async Task GetChristmasPresent(SocketSlashCommand command)
        {
            ItemObject present = InventoryManager.ExistingItems[3];

            bool itemIsGiven = await InventoryManager.GiveItemToUser(command.User.Id, present.Id, 1);

            if (!itemIsGiven)
            {
                await Utilities.SendDevLogMessage(1, $"Give advent item was invalid. User is ||{command.User.Id}|| and item is {present.Id}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await MySqlWrapper.SetIntegerForIdentifier("user_profile", "winter_points", new Dictionary<string, object> { { "user_id", command.User.Id } }, 1000, 1, false);

            AdventObject adventItem = WinterManager.AdventItems[DateTime.Now.Day];

            string description = await LanguageManager.GetTranslation("adventDoorDescription", command.User.Id, "", adventItem.Poem, present.Name, present.CardURL);

            var embedBuilder = new EmbedBuilder()
                    .WithTitle($"Advent {DateTime.Now.Year} :christmas_tree:")
                    .WithDescription(description)
                    .WithColor(Color.Gold)
                    .WithImageUrl(adventItem.Gif);

            await command.ModifyOriginalResponseAsync(func => func.Embed = embedBuilder.Build());
        }
    }
}
