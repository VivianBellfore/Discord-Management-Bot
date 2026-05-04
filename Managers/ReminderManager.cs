
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    internal class ReminderManager
    {
        /// <summary>
        /// Need to block loading on API resume connection for API on runtime.
        /// </summary>
        private static bool blockedTextLoaded;

        /// <summary>
        /// Holding all reminder from the database.
        /// </summary>
        internal static List<PublicReminderObject> publicReminderObjects = new List<PublicReminderObject>();

        /// <summary>
        /// Holding all private reminder from the database.
        /// </summary>
        internal static List<PrivateReminderObject> privateReminderObjects = new List<PrivateReminderObject>();

        /// <summary>
        /// This list is holding temp timer objects to save information for the modal execute event.
        /// </summary>
        internal static Dictionary<ulong, PublicReminderObject> publicReminderObjectsDictionary = new Dictionary<ulong, PublicReminderObject>();

        /// <summary>
        /// This list is holding temp timer objects to save information for the modal execute event.
        /// </summary>
        internal static Dictionary<ulong, PrivateReminderObject> privateReminderObjectsDictionary = new Dictionary<ulong, PrivateReminderObject>();




        #region Public reminder functions
        /// <summary>
        /// Loading all reminder from database. Triggerd on bot start once in <seealso cref="StartBotInstance.BotSetup"/>.
        /// </summary>
        internal static async Task LoadPublicReminder()
        {
            if (blockedTextLoaded) return;

            List<dynamic> reminder = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `reminder_public`",
                new Dictionary<string, object>() { });

            foreach (dynamic entry in reminder)
            {
                publicReminderObjects.Add(new PublicReminderObject(entry.id, entry.guild_id, entry.channel_id, entry.user_id, entry.time, entry.title, entry.description, entry.picture, entry.color, entry.daily,
                    entry.weekday, entry.duration, entry.date, entry.role_ids));
            }

            blockedTextLoaded = true;
        }

        /// <summary>
        /// Checking for existing public timer and starting there handling.
        /// </summary>
        internal static async Task DoPublicReminder(string timeNow)
        {
            int rateLimitCounter = 0;
            List<PublicReminderObject> removeFromPublicList = new List<PublicReminderObject>();

            foreach (PublicReminderObject timer in publicReminderObjects)
            {
                if (timer.Time != timeNow)
                    continue;

                if (++rateLimitCounter >= 45)
                {
                    rateLimitCounter = 0;
                    await Task.Delay(3000);
                }

                if (timer.Daily == 1)
                {
                    await SendPublicReminder(timer);
                    continue;
                }

                if (timer.Weekday != "non" && timer.Weekday == DateTime.Now.DayOfWeek.ToString().ToLower())
                {
                    await SendPublicReminder(timer);
                    continue;
                }

                if (timer.Duration > 0)
                {
                    timer.Duration--;

                    if (timer.Duration > 0)
                        await UpdatePublicReminderDuration(timer);
                    else
                    {
                        removeFromPublicList.Add(timer);
                        await DeletePublicReminder(timer);
                    }

                    await SendPublicReminder(timer);
                    continue;
                }

                if (timer.Date == DateTime.Now.ToShortDateString())
                {
                    await SendPublicReminder(timer);
                    await DeletePublicReminder(timer);
                    removeFromPublicList.Add(timer);
                }
            }

            if (removeFromPublicList.Count > 0)
                publicReminderObjects.RemoveAll(t => removeFromPublicList.Contains(t));
        }

        /// <summary>
        /// Sending a message to a channel for a public reminder.
        /// </summary>
        private static async Task SendPublicReminder(PublicReminderObject reminder)
        {
            SocketGuild guild = StartBotInstance._client.GetGuild(reminder.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild id {reminder.GuildId} was not found.\n{reminder.Title}\n{reminder.Description}\n{reminder.Date}, {reminder.Time}, daily: {reminder.Daily}, " +
                    $"weekly: {reminder.Weekday}\nChannel: {reminder.ChannelId}");
                return;
            }

            ITextChannel textChannel = guild.GetChannel(reminder.ChannelId) as ITextChannel;
            if (textChannel == null)
            {
                await Utilities.SendDevLogMessage(1, $"Channel id {reminder.ChannelId} was not found.\n{reminder.Title}\n{reminder.Description}\n{reminder.Date}, {reminder.Time}, daily: {reminder.Daily}, " +
                    $"weekly: {reminder.Weekday}\nGuild: {reminder.GuildId}");
                return;
            }

            string gif = (reminder.PictureURL == "non" ? "" : reminder.PictureURL);

            string roleTags = "";

            string[] roleIds = new string[0];

            if (reminder.RoleIds != "")
                roleIds = reminder.RoleIds.Split(',');

            if (roleIds.Count() > 0)
            {
                foreach (string role in roleIds)
                {
                    role.Replace(" ", "");
                    roleTags = roleTags + $" <@&{role}>";
                }
            }

            var embedBuilder = new EmbedBuilder()
                    .WithDescription($"# {reminder.Title}\n{reminder.Description}")
                    .WithColor(await Utilities.GetColor(reminder.Color))
                    .WithImageUrl(gif);

            await textChannel.SendMessageAsync($"-# Reminder was set by user <@{reminder.UserId}>\n" + roleTags, embed: embedBuilder.Build());
        }
        #endregion



        #region Public reminder data base
        /// <summary>
        /// Deleting a public reminder from database.
        /// </summary>
        private static async Task DeletePublicReminder(PublicReminderObject reminder)
        {
            int deleteCounter = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `reminder_public` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `user_id` = @user_id AND `time` = @time AND `title` = @title AND `description` = @description " +
                "AND `picture` = @picture AND `color` = @color AND `daily` = @daily AND `weekday` = @weekday AND `date` = @date AND `role_ids` = @role_ids",
                new Dictionary<string, object>() { {"guild_id", reminder.GuildId}, {"channel_id", reminder.ChannelId}, { "user_id", reminder.UserId }, { "time", reminder.Time}, { "title", reminder.Title}, 
                    { "description", reminder.Description}, { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, { "date", reminder.Date}, 
                    {"role_ids", reminder.RoleIds} });

            if (deleteCounter < 1)
                await Utilities.SendDevLogMessage(1, $"Public reminder could not be deleted from database.\nId: {reminder.ID}, channel: {reminder.ChannelId}, date: {reminder.Date}, description: {reminder.Description}");
        }

        /// <summary>
        /// Adding a reminder to database.
        /// </summary>
        internal static async Task AddPublicReminder(PublicReminderObject reminder)
        {
            int insertCounter = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `reminder_public` (`guild_id`, `channel_id`, `user_id`, `time`, `title`, `description`, `picture`, `color`, `daily`, `weekday`, `duration`, `date`, `role_ids`) VALUES " +
                "(@guild_id, @channel_id, @user_id, @time, @title, @description, @picture, @color, @daily, @weekday, @duration, @date, @role_ids)",
                new Dictionary<string, object>() { {"guild_id", reminder.GuildId}, {"channel_id", reminder.ChannelId}, { "user_id", reminder.UserId }, { "time", reminder.Time}, { "title", reminder.Title}, 
                    { "description", reminder.Description}, { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, 
                    { "duration", reminder.Duration}, { "date", reminder.Date}, {"role_ids", reminder.RoleIds} });

            if (insertCounter <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Public reminder could not be inserted into database.\nId: {reminder.ID}, channel: {reminder.ChannelId}, date: {reminder.Date}, description: {reminder.Description}");
                return;
            }

            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `id` FROM `reminder_public` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `user_id` = @user_id AND `time` = @time AND `title` = @title AND `description` = @description " +
                "AND `picture` = @picture AND `color` = @color AND `daily` = @daily AND `weekday` = @weekday AND `duration` = @duration AND `date` = @date AND `role_ids` = @role_ids",
                new Dictionary<string, object>() { {"guild_id", reminder.GuildId}, {"channel_id", reminder.ChannelId}, { "user_id", reminder.UserId }, { "time", reminder.Time}, { "title", reminder.Title},
                    { "description", reminder.Description}, { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, 
                    { "duration", reminder.Duration}, { "date", reminder.Date}, {"role_ids", reminder.RoleIds} });

            reminder.ID = Convert.ToInt32(result);
            publicReminderObjects.Add(reminder);
        }

        /// <summary>
        /// Updating a public reminder in database.
        /// </summary>
        private static async Task UpdatePublicReminderDuration(PublicReminderObject reminder)
        {
            int updateCounter = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `reminder_public` SET `duration` = @duration WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `user_id` = @user_id AND `time` = @time AND " +
                "`title` = @title AND `description` = @description AND `picture` = @picture AND `color` = @color AND `daily` = @daily AND `weekday` = @weekday AND `date` = @date",
                new Dictionary<string, object>() { {"guild_id", reminder.GuildId}, {"channel_id", reminder.ChannelId}, { "user_id", reminder.UserId }, { "time", reminder.Time}, { "title", reminder.Title}, 
                    { "description", reminder.Description}, { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, 
                    { "duration", reminder.Duration}, { "date", reminder.Date} });

            if (updateCounter < 1)
                await Utilities.SendDevLogMessage(1, $"Public reminder could not be updated into database.\nId: {reminder.ID}, channel: {reminder.ChannelId}, date: {reminder.Date}, description: {reminder.Description}");
        }
        #endregion



        #region Private reminder functions
        /// <summary>
        /// Loading all private reminder from database. Triggerd on bot start once in <seealso cref="StartBotInstance.BotSetup"/>.
        /// </summary>
        internal static async Task LoadPrivateReminder()
        {
            List<dynamic> reminder = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `reminder_private`",
                new Dictionary<string, object>() { });

            privateReminderObjects.Clear();

            foreach (dynamic entry in reminder)
            {
                privateReminderObjects.Add(new PrivateReminderObject(entry.id, entry.user_id, entry.time, entry.title, entry.description, entry.picture, entry.color, entry.daily,
                    entry.weekday, entry.duration, entry.date));
            }
        }

        /// <summary>
        /// Checking for existing private timer and starting there handling.
        /// </summary>
        internal static async Task DoPrivateReminder(string timeNow)
        {
            int rateLimitCounter = 0;
            List<PrivateReminderObject> removeFromPrivateList = new List<PrivateReminderObject>();

            foreach (PrivateReminderObject timer in privateReminderObjects)
            {
                if (timer.Time != timeNow)
                    continue;

                if (++rateLimitCounter >= 45)
                {
                    rateLimitCounter = 0;
                    await Task.Delay(3000);
                }

                if (timer.Daily == 1)
                {
                    await SendPrivateReminder(timer);
                    continue;
                }

                if (timer.Weekday != "non" && timer.Weekday == DateTime.Now.DayOfWeek.ToString().ToLower())
                {
                    await SendPrivateReminder(timer);
                    continue;
                }

                if (timer.Duration > 0)
                {
                    timer.Duration--;

                    if (timer.Duration > 0)
                        await UpdatePrivateReminderDuration(timer);
                    else
                    {
                        removeFromPrivateList.Add(timer);
                        await DeletePrivateReminder(timer);
                    }

                    await SendPrivateReminder(timer);
                    continue;
                }

                if (timer.Date == DateTime.Now.ToShortDateString())
                {
                    await SendPrivateReminder(timer);
                    await DeletePrivateReminder(timer);
                    removeFromPrivateList.Add(timer);
                }
            }

            if (removeFromPrivateList.Count > 0)
                privateReminderObjects.RemoveAll(t => removeFromPrivateList.Contains(t));
        }

        /// <summary>
        /// Sending a direct message to a user for a reminder.
        /// </summary>
        private static async Task SendPrivateReminder(PrivateReminderObject reminder)
        {
            var user = StartBotInstance._client.GetUser(reminder.UserID);

            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"User ||{reminder.UserID}|| was not be found.");
                return;
            }

            string gif = (reminder.PictureURL == "non" ? "" : reminder.PictureURL);

            var embedBuilder = new EmbedBuilder()
                    .WithDescription($"# {reminder.Title}\n{reminder.Description}")
                    .WithColor(await Utilities.GetColor(reminder.Color))
                    .WithImageUrl(gif);

            try
            {
                await user.SendMessageAsync(embed: embedBuilder.Build());
            }
            catch (Exception ex)
            {

            }
        }
        #endregion



        #region Private reminder data base
        /// <summary>
        /// Adding PrivateReminderObjects to `reminder_private` in database.
        /// </summary>
        private static async Task AddPrivateReminder(PrivateReminderObject reminder)
        {
            int insertCounter = await MySqlWrapper.SQLExecuteNonQuery(
                @"INSERT INTO `reminder_private` (`user_id`, `time`, `title`, `description`, `picture`, `color`, `daily`, `weekday`, `duration`, `date`) VALUES 
                (@user_id, @time, @title, @description, @picture, @color, @daily, @weekday, @duration, @date)",
                new Dictionary<string, object>() { {"user_id", reminder.UserID}, { "time", reminder.Time}, { "title", reminder.Title}, { "description", reminder.Description},
                    { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, { "duration", reminder.Duration}, { "date", reminder.Date} });

            if (insertCounter <= 0)
                await Utilities.SendDevLogMessage(1, "Timer could not be inserted to database.");

            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `id` FROM `reminder_private` WHERE `user_id` = @user_id AND `time` = @time AND `title` = @title AND `description` = @description " +
                "AND `picture` = @picture AND `color` = @color AND `daily` = @daily AND `weekday` = @weekday AND `duration` = @duration AND `date` = @date",
                new Dictionary<string, object>() { {"user_id", reminder.UserID}, { "time", reminder.Time}, { "title", reminder.Title}, { "description", reminder.Description},
                    { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, { "duration", reminder.Duration}, { "date", reminder.Date} });

            if (result == null) return;

            reminder.ID = Convert.ToInt32(result);
            privateReminderObjects.Add(reminder);
        }

        /// <summary>
        /// Deleting private timer from database.
        /// </summary>
        private static async Task DeletePrivateReminder(PrivateReminderObject reminder)
        {
            int deleteCounter = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `reminder_private` WHERE `user_id` = @user_id AND `time` = @time AND `title` = @title AND `description` = @description " +
                "AND `picture` = @picture AND `color` = @color AND `daily` = @daily AND `weekday` = @weekday AND `date` = @date",
                new Dictionary<string, object>() { {"user_id", reminder.UserID}, { "time", reminder.Time}, { "title", reminder.Title}, { "description", reminder.Description},
                    { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, { "date", reminder.Date} });

            if (deleteCounter < 1)
                await Utilities.SendDevLogMessage(1, "Timer could not be deleted from database.");
        }

        /// <summary>
        /// Updating a private timer in database.
        /// </summary>
        private static async Task UpdatePrivateReminderDuration(PrivateReminderObject reminder)
        {
            int updateCounter = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `reminder_private` SET `duration` = @duration WHERE `user_id` = @user_id AND `time` = @time AND " +
                "`title` = @title AND `description` = @description AND `picture` = @picture AND `color` = @color AND `daily` = @daily AND `weekday` = @weekday AND `date` = @date",
                new Dictionary<string, object>() { {"guild_id", reminder.UserID}, { "time", reminder.Time}, { "title", reminder.Title}, { "description", reminder.Description},
                    { "picture", reminder.PictureURL}, { "color", reminder.Color}, { "daily", reminder.Daily}, { "weekday", reminder.Weekday}, { "duration", reminder.Duration}, { "date", reminder.Date} });

            if (updateCounter < 1)
                await Utilities.SendDevLogMessage(1, "Timer could not be updated.");
        }
        #endregion
    }



    /// <summary>
    /// Struct for a public reminder object.
    /// </summary>
    internal class PublicReminderObject
    {
        internal int ID { get; set; }
        internal ulong GuildId { get; set; }
        internal ulong ChannelId { get; set; }
        internal ulong UserId { get; set; }
        internal string Time { get; set; }
        internal string Title { get; set; }
        internal string Description { get; set; }
        internal string PictureURL { get; set; }
        internal string Color { get; set; }
        internal int Daily { get; set; }
        internal string Weekday { get; set; }
        internal int Duration { get; set; }
        internal string Date { get; set; }
        internal string RoleIds { get; set; }

        internal PublicReminderObject(int id, ulong guildId, ulong channelId, ulong userId, string time, string title, string description, string pictureURL, string color, int daily, string weekday, int duration, string date, string roleIds)
        {
            ID = id;
            GuildId = guildId;
            ChannelId = channelId;
            UserId = userId;
            Time = time;
            Title = title;
            Description = description;
            PictureURL = pictureURL;
            Color = color;
            Daily = daily;
            Weekday = weekday;
            Duration = duration;
            Date = date;
            RoleIds = roleIds;
        }
    }

    /// <summary>
    /// Struct for a private reminder object.
    /// </summary>
    internal class PrivateReminderObject
    {
        internal int ID { get; set; }
        internal ulong UserID { get; set; }
        internal string Time { get; set; }
        internal string Title { get; set; }
        internal string Description { get; set; }
        internal string PictureURL { get; set; }
        internal string Color { get; set; }
        internal int Daily { get; set; }
        internal string Weekday { get; set; }
        internal int Duration { get; set; }
        internal string Date { get; set; }

        internal PrivateReminderObject(int id, ulong userId, string time, string title, string description, string pictureURL, string color, int daily, string weekday, int duration, string date)
        {
            ID = id;
            UserID = userId;
            Time = time;
            Title = title;
            Description = description;
            PictureURL = pictureURL;
            Color = color;
            Daily = daily;
            Weekday = weekday;
            Duration = duration;
            Date = date;
        }
    }
}
