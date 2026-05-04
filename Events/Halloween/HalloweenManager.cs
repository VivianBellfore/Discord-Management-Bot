
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    internal class HalloweenManager
    {
        internal static async Task<bool> IsHallowwenActiveOnGuild( ulong guildId)
        {
            GuildObject guildData = await GuildManager.GetGuildData(guildId);
            if ( guildData == null ) return false;
            else if ( guildData.IsHalloweenActive )
                return true;
            else return false;
        }

        internal static async Task StartHalloweenOnAllGuilds()
        {
            IReadOnlyCollection<SocketGuild> connectedGuilds = StartBotInstance._client.Guilds;

            if (connectedGuilds == null || connectedGuilds.Count == 0)
            {
                Console.WriteLine("No connected guilds found!");
                return;
            }

            foreach (SocketGuild guild in connectedGuilds)
            {
                GuildObject guildData = await GuildManager.GetGuildData(guild.Id);
                if (guildData == null) continue;

                if ( guildData.IsHalloweenActive)
                    await SendIntroductionMessage(guildData);
            }
        }

        internal static async Task SendIntroductionMessage(GuildObject guildData)
        {
            SocketGuild guild = StartBotInstance._client.GetGuild(guildData.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild, id was {guildData.GuildId}.");
                return;
            }

            SocketTextChannel eventChannel = guild.GetTextChannel(guildData.EventChannel);
            if (eventChannel == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find bot event channel, id was {guildData.EventChannel}.");
                return;
            }

            string language = guildData.Language;

            var buttonBuilder = new ComponentBuilder();
            buttonBuilder.WithButton(await LanguageManager.GetTranslation("halloweenSearchButton", 0, language), $"respond_halloween_search_{language}", ButtonStyle.Success);
            buttonBuilder.WithButton(await LanguageManager.GetTranslation("halloweenDoTrickhButton", 0, language), $"respond_halloween_dotrick_{language}", ButtonStyle.Danger);
            buttonBuilder.WithButton(await LanguageManager.GetTranslation("halloweenDefendTrickButton", 0, language), $"respond_halloween_deftrick_{language}", ButtonStyle.Primary);
            buttonBuilder.WithButton(await LanguageManager.GetTranslation("halloweenCandyButton", 0, language), $"respond_halloween_candy_{language}", ButtonStyle.Secondary);

            var embedBuilder = new EmbedBuilder();
            embedBuilder.ImageUrl = Configurations.HalloweenEventPictureUrl;
            embedBuilder.Description = await LanguageManager.GetTranslation("halloweenIntroduction", 0, language);

            await eventChannel.SendMessageAsync(embed: embedBuilder.Build(), components: buttonBuilder.Build());
        }

        /// <summary>
        /// Gives the user a random candy with random amount and adds it to the database.
        /// </summary>
        internal static async Task SearchForCandy(SocketMessageComponent button, string language)
        {
            UserObject user = await UserManager.GetUserData(button.User.Id);
            if (user.UserId == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("needToBeRegistered", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (user.HalloweenDate.AddMinutes(30) > DateTime.Now)
            {
                string errorMessage = await LanguageManager.GetTranslation("halloweenCooldown", button.User.Id, "_non_", user.HalloweenDate.AddMinutes(30).ToShortTimeString());
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int candyId = Utilities.GetRandomByUserSeed(button.User.Id).Next(1, Candy.candys.Count() +1);
            int amount = Utilities.GetRandomByUserSeed(button.User.Id).Next(1, 10);

            bool isUpdated = await MySqlWrapper.SetIntegerForIdentifier("user_halloween_candy", "amount", new Dictionary<string, object>() { { "user_id", button.User.Id }, { "candy_id", candyId } }, amount, 1, false);

            if ( !isUpdated )
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `user_halloween_candy` (`user_id`, `candy_id`, `amount`) VALUES (@user_id, @candy_id, @amount)",
                    new Dictionary<string, object>() { { "user_id", button.User.Id }, { "candy_id", candyId }, { "amount", amount } });

                if ( insertCount <= 0)
                {
                    await Utilities.SendDevLogMessage(1, $"Could not update or insert candy id {candyId} with amount {amount} for user ||{button.User.Id}||.");
                    string errorMessage = await LanguageManager.GetTranslation("saveDataError", button.User.Id);
                    await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }
            }

            string message = await LanguageManager.GetTranslation("halloweenFoundCandy", button.User.Id, "", Candy.candys[candyId].Name, amount);
            await button.ModifyOriginalResponseAsync(func => func.Content = message);

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `user_profile` SET `halloween_date` = @datetime WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "datetime", DateTime.Now.ToString("o") }, { "user_id", button.User.Id } });

            if (updateCount <= 0)
                await Utilities.SendDevLogMessage(1, $"Searching candy time was not updated for user ||{button.User.Id}|| at time {DateTime.Now}.");
        }


        internal static async Task DoATrickOnSomeone(SocketMessageComponent button, string language)
        {
            UserObject user = await UserManager.GetUserData(button.User.Id);
            if (user.UserId == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("needToBeRegistered", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (user.HalloweenAction.Year == DateTime.Now.Year && user.HalloweenAction.Month == DateTime.Now.Month && user.HalloweenAction.Day == DateTime.Now.Day)
            {
                string errorMessage = await LanguageManager.GetTranslation("halloweenAlreadyUsed", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `user_halloween_candy`",
                new Dictionary<string, object>() { });

            if (results.Count <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("halloweenNoCandyStolen", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
                random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
                random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
                random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
                random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
                random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
                random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
                random = Utilities.random.Next(0, results.Count);
            if (results[random].user_id == button.User.Id)
            {
                await button.ModifyOriginalResponseAsync(func => func.Content = "You constantly try to steal from yourself... stop it! xD");
                return;
            }

            UserObject targetUser = await UserManager.GetUserData(results[random].user_id);
            if (targetUser.UserId == 0)
            {
                await Utilities.SendDevLogMessage(1, $"Target user was null in user data. Target id was ||{results[random].user_id}|| and active user was ||{button.User.Id}||.");
                string errorMessage = await LanguageManager.GetTranslation("userDataError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (targetUser.HalloweenProtection.Year == DateTime.Now.Year && targetUser.HalloweenProtection.Month == DateTime.Now.Month && targetUser.HalloweenProtection.Day == DateTime.Now.Day)
            {
                string errorMessage = await LanguageManager.GetTranslation("halloweenPrankNotSuccess", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);

                int updateAction = await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `user_profile` SET `halloween_action` = @datetime WHERE `user_id` = @user_id",
                    new Dictionary<string, object>() { { "datetime", DateTime.Now.ToString("o") }, { "user_id", button.User.Id } });

                int updateProtection = await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `user_profile` SET `halloween_protection` = @datetime WHERE `user_id` = @user_id",
                    new Dictionary<string, object>() { { "datetime", "2025-09-25T14:15:00" }, { "user_id", targetUser.UserId } });

                return;
            }

            int stolenAmount = Utilities.random.Next(1, Convert.ToInt32(results[random].amount));
            if (stolenAmount > 20) stolenAmount = 20; 

            bool isTargetSaved = await MySqlWrapper.SetIntegerForIdentifier("user_halloween_candy", "amount", new Dictionary<string, object>() { { "user_id", targetUser.UserId }, 
                { "candy_id", results[random].candy_id } }, stolenAmount, 2, false);
            if ( !isTargetSaved)
            {
                await Utilities.SendDevLogMessage(1, $"Could not remove candy amount from target. Target id was ||{results[random].user_id}|| and active user was ||{button.User.Id}||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            bool isUserSaved = await MySqlWrapper.SetIntegerForIdentifier("user_halloween_candy", "amount", new Dictionary<string, object>() { { "user_id", button.User.Id }, 
                { "candy_id", results[random].candy_id } }, stolenAmount, 1, false);
            if (!isUserSaved)
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `user_halloween_candy` (`user_id`, `candy_id`, `amount`) VALUES (@user_id, @candy_id, @amount)",
                    new Dictionary<string, object>() { { "user_id", button.User.Id }, { "candy_id", results[random].candy_id }, { "amount", stolenAmount } });

                if (insertCount <= 0)
                {
                    await Utilities.SendDevLogMessage(1, $"Could not update or insert candy id {results[random].candy_id} with amount {stolenAmount} for user ||{button.User.Id}||.");
                    string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", button.User.Id);
                    await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                    return;
                }
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `user_profile` SET `halloween_action` = @datetime WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "datetime", DateTime.Now.ToString("o") }, { "user_id", button.User.Id } });

            string message = await LanguageManager.GetTranslation("halloweenStolenCandyMessage", button.User.Id, "", stolenAmount, Candy.candys[results[random].candy_id].Name);
            await button.ModifyOriginalResponseAsync(func => func.Content = message);
        }


        internal static async Task DefendForATrick(SocketMessageComponent button, string language)
        {
            UserObject user = await UserManager.GetUserData(button.User.Id);
            if (user.UserId == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("needToBeRegistered", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (user.HalloweenAction.Year == DateTime.Now.Year && user.HalloweenAction.Month == DateTime.Now.Month && user.HalloweenAction.Day == DateTime.Now.Day)
            {
                string errorMessage = await LanguageManager.GetTranslation("halloweenAlreadyUsed", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `user_profile` SET `halloween_action` = @datetime WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "datetime", DateTime.Now.ToString("o") }, { "user_id", button.User.Id } });

            int updateProtection = await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `user_profile` SET `halloween_protection` = @datetime WHERE `user_id` = @user_id",
                    new Dictionary<string, object>() { { "datetime", DateTime.Now.ToString("o") }, { "user_id", button.User.Id } });

            if (updateCount <= 0 || updateProtection <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Could not save updated actions. User was ||{button.User.Id}||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("halloweenProtection", button.User.Id);
            await button.ModifyOriginalResponseAsync(func => func.Content = message);
        }

        /// <summary>
        /// Shows the user a list of collected event items as button response.
        /// </summary>
        internal static async Task ShowCandyStatistics(SocketMessageComponent button, string language)
        {
            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `user_halloween_candy` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", button.User.Id } });

            if ( results == null || results.Count <= 0)
            {
                string emptyMessage = await LanguageManager.GetTranslation("halloweenNoCandy", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = emptyMessage);
                return;
            }

            string contentText = "";
            int points = 0;

            foreach (dynamic item in results)
            {
                contentText += $"- **{Candy.candys[item.candy_id].Name}** x **{item.amount}**\n";
                points += Convert.ToInt32(item.amount) * Candy.candys[item.candy_id].Points;
            }
            
            string message = await LanguageManager.GetTranslation("halloweenUserCandyList", button.User.Id, "", contentText, points);
            await button.ModifyOriginalResponseAsync(func => func.Content = message);
        }



        /// <summary>
        /// Was used to fetch the collected candy points for all user.
        /// </summary>
        /// <returns></returns>
        private async Task CandyMath()
        {
            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `user_halloween_candy`",
                new Dictionary<string, object> { });

            if (results.Count <= 0) return;

            Dictionary<ulong, int> pointList = new Dictionary<ulong, int>();

            foreach (dynamic entry in results)
            {
                if (pointList.ContainsKey(Convert.ToUInt64(entry.user_id)))
                    pointList[Convert.ToUInt64(entry.user_id)] += Candy.candys[Convert.ToInt32(entry.candy_id)].Points * Convert.ToInt32(entry.amount);
                else
                    pointList.Add(Convert.ToUInt64(entry.user_id), Candy.candys[Convert.ToInt32(entry.candy_id)].Points * Convert.ToInt32(entry.amount));
            }

            foreach (var entry in pointList)
            {
                Console.WriteLine($"[ Halloween ] <@{entry.Key}> mit {entry.Value} Punkten!");
            }
        }
    }



    /// <summary>
    /// This class is building a button for <seealso cref="halloween"/>.
    /// </summary>
    internal class HalloweenButton : ButtonPressed
    {
        /// <summary>
        /// This constructor is a builder for the button with custom id <paramref name="halloween"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="SetGatedCommunity"/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal HalloweenButton(string customId) : base(customId)
        {
            WithCustomId("halloween");
        }

        /// <summary>
        /// This function is handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            // respond_halloween_TYPE_language
            string[] splitedCustomId = button.Data.CustomId.Split('_');

            string buttonType = splitedCustomId[2];
            string language = splitedCustomId[3];

            switch (buttonType)
            {
                case "search":
                    await HalloweenManager.SearchForCandy(button, language);
                    break;
                case "dotrick":
                    await HalloweenManager.DoATrickOnSomeone(button, language);
                    break;
                case "deftrick":
                    await HalloweenManager.DefendForATrick(button, language);
                    break;
                case "candy":
                    await HalloweenManager.ShowCandyStatistics(button, language);
                    break;
                default:
                    {
                        await Utilities.SendDevLogMessage(1, $"Member type was not valid! Id was {buttonType}.");
                        string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                        await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    }
                    return;
            }
        }
    }
}
