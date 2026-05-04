
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the "/use <paramref name="pubremind"/>" command.
    /// </summary>
    internal class SetPublicReminder : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal SetPublicReminder() : base("use", "pubremind", "command_use_pubremind") { }



        internal static PublicReminderModalDaily reminderDailyModal         = new PublicReminderModalDaily("pubreminddaily_modal");
        internal static PublicReminderModalWeekly reminderWeeklyModal       = new PublicReminderModalWeekly("pubremindweekly_modal");
        internal static PublicReminderModalDate reminderDateModal           = new PublicReminderModalDate("pubreminddate_modal");
        internal static PublicReminderModalDuration reminderDurationModal   = new PublicReminderModalDuration("pubremindduration_modal");
        


        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            bool userIsRegistered = await PermissionManager.HasUserAcceptTos(command.User.Id);
            if ( !userIsRegistered)
            {
                string errorMessage = await LanguageManager.GetTranslation("needToBeRegistered", command.User.Id);
                await command.RespondAsync(errorMessage);
                return;
            }

            // TODO:
            // max user reminder?
            // max guild reminder?

            PublicReminderObject reminder = new PublicReminderObject(0, (ulong)command.GuildId, 0, 0, "00:00", "", "", "non", "non", 0, "non", 0, "non", "");

            reminder.Color = command.Data.Options.First().Options.ElementAt(1).Value.ToString();

            IGuildChannel channel = command.Data.Options.First().Options.ElementAt(2).Value as IGuildChannel;
            reminder.ChannelId = channel.Id;

            if (command.Data.Options.First().Options.Count == 4 && command.Data.Options.First().Options.ElementAt(3).Name == "picture")
                reminder.PictureURL = command.Data.Options.First().Options.ElementAt(3).Value.ToString();

            if (command.Data.Options.First().Options.Count == 4 && command.Data.Options.First().Options.ElementAt(3).Name == "roleids")
                reminder.RoleIds = command.Data.Options.First().Options.ElementAt(3).Value.ToString();

            if (command.Data.Options.First().Options.Count == 5 && command.Data.Options.First().Options.ElementAt(4).Name == "roleids")
                reminder.RoleIds = command.Data.Options.First().Options.ElementAt(3).Value.ToString();

            ReminderManager.publicReminderObjectsDictionary.Add(command.User.Id, reminder);

            switch (command.Data.Options.First().Options.ElementAt(0).Value.ToString())
            {
                case "daily":
                    await command.RespondWithModalAsync(reminderDailyModal.Build());
                    break;

                case "weekly":
                    await command.RespondWithModalAsync(reminderWeeklyModal.Build());
                    break;

                case "date":
                    await command.RespondWithModalAsync(reminderDateModal.Build());
                    break;

                case "duration":
                    await command.RespondWithModalAsync(reminderDurationModal.Build());
                    break;
            }
        }
    }



    /// <summary>
    /// Modal for daily public reminder.
    /// </summary>
    internal class PublicReminderModalDaily : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="pubremind_daily"/>.<para/>
        /// Modal inputs:<br/>
        /// <paramref name="Titel"/> - short<br/>
        /// <paramref name="Message"/> - paragraph<br/>
        /// <paramref name="Time"/> - short<para/>
        /// Connected to:<br/>
        /// <seealso cref="SetPublicReminder"/>
        /// </summary>
        internal PublicReminderModalDaily(string customId) : base(customId)
        {
            WithTitle("Create a reminder");
            AddTextInput("Titel",   "reminder_title",   TextInputStyle.Short,       "The title of the reminder.",           required: true);
            AddTextInput("Message", "reminder_text",    TextInputStyle.Paragraph,   "Write the content for the reminder.",  required: true);
            AddTextInput("Time",    "reminder_time",    TextInputStyle.Short,       "Reminder time. Musst be `00:00`.",     required: true);

            CommandManager.commandsWithModal.Add("pubremind");
        }



        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {
            try
            {
                List<SocketMessageComponentData> components = modal.Data.Components.ToList();
                string title        = components.First(x => x.CustomId == "reminder_title").Value;
                string description  = components.First(x => x.CustomId == "reminder_text").Value;
                string time         = components.First(x => x.CustomId == "reminder_time").Value;

                if (!Utilities.CheckTimeFormate(time))
                {
                    ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
                    string errorMessage = await LanguageManager.GetTranslation("reminderTimeNotMatching", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                PublicReminderObject reminder = ReminderManager.publicReminderObjectsDictionary[modal.User.Id];

                reminder.Title          = title;
                reminder.Description    = description;
                reminder.Time           = time;
                reminder.Daily          = 1;
                reminder.UserId         = modal.User.Id;

                await ReminderManager.AddPublicReminder(reminder);
                string message = await LanguageManager.GetTranslation("dailypubreminderSaved", modal.User.Id, "", reminder.Time, reminder.ChannelId);
                await modal.ModifyOriginalResponseAsync(func => func.Content = message);
            }
            catch (Exception exception)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                await Utilities.SendDevLogMessage(1, $"Building and sending the embed failed.\nException: {exception.Message}");
            }

            ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
        }
    }

    /// <summary>
    /// Modal for reminder function with weekly.
    /// </summary>
    internal class PublicReminderModalWeekly : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="pubremind_weekly"/>.<para/>
        /// Modal inputs:<br/>
        /// <paramref name="Titel"/> - short<br/>
        /// <paramref name="Message"/> - paragraph<br/>
        /// <paramref name="Time"/> - short<br/>
        /// <paramref name="Weekly"/> - short<para/>
        /// Connected to:<br/>
        /// <seealso cref="SetPublicReminder"/>
        /// </summary>
        internal PublicReminderModalWeekly(string customId) : base(customId)
        {
            WithTitle("Create a reminder");
            AddTextInput("Titel",   "reminder_title",   TextInputStyle.Short,       "The title of the reminder.",           required: true);
            AddTextInput("Message", "reminder_text",    TextInputStyle.Paragraph,   "Write the content for the reminder.",  required: true);
            AddTextInput("Time",    "reminder_time",    TextInputStyle.Short,       "Reminder time. Musst be `00:00`.",     required: true);
            AddTextInput("Weekday", "reminder_weekday", TextInputStyle.Short,       "Write like: friday or freitag",        required: true);

            CommandManager.commandsWithModal.Add("pubremind");
        }



        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {
            try
            {
                List<SocketMessageComponentData> components = modal.Data.Components.ToList();
                string title        = components.First(x => x.CustomId == "reminder_title").Value;
                string description  = components.First(x => x.CustomId == "reminder_text").Value;
                string time         = components.First(x => x.CustomId == "reminder_time").Value;
                string weekday      = components.First(x => x.CustomId == "reminder_weekday").Value.ToLower();

                if (!Utilities.CheckTimeFormate(time))
                {
                    ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
                    string errorMessage = await LanguageManager.GetTranslation("reminderTimeNotMatching", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                (bool isDayCorrect, string englishDayName) = Utilities.CheckFormateWeekday(weekday);
                if (!isDayCorrect)
                {
                    ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
                    string errorMessage = await LanguageManager.GetTranslation("reminderWeekdayNotMatching", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                PublicReminderObject reminder = ReminderManager.publicReminderObjectsDictionary[modal.User.Id];

                reminder.Title          = title;
                reminder.Description    = description;
                reminder.Time           = time;
                reminder.Weekday        = englishDayName;
                reminder.UserId         = modal.User.Id;

                await ReminderManager.AddPublicReminder(reminder);
                string message = await LanguageManager.GetTranslation("weeklypubreminderSaved", modal.User.Id, "", reminder.Time, reminder.ChannelId, reminder.Weekday);
                await modal.ModifyOriginalResponseAsync(func => func.Content = message);
            }
            catch (Exception exception)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                await Utilities.SendDevLogMessage(1, $"Building and sending the embed failed.\nException: {exception.Message}");
            }

            ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
        }
    }

    /// <summary>
    /// Modal for reminder function with date.
    /// </summary>
    internal class PublicReminderModalDate : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="pubremind_date"/>.<para/>
        /// Modal inputs:<br/>
        /// <paramref name="Titel"/> - short<br/>
        /// <paramref name="Message"/> - paragraph<br/>
        /// <paramref name="Time"/> - short<br/>
        /// <paramref name="Date" - short<para/>
        /// Connected to:<br/>
        /// <seealso cref="SetPublicReminder"/>
        /// </summary>
        internal PublicReminderModalDate(string customId) : base(customId)
        {
            WithTitle("Create a reminder");
            AddTextInput("Titel",   "reminder_title",   TextInputStyle.Short,       "The title of the reminder.",                   required: true);
            AddTextInput("Message", "reminder_text",    TextInputStyle.Paragraph,   "Write the content for the reminder.",          required: true);
            AddTextInput("Time",    "reminder_time",    TextInputStyle.Short,       "Reminder time. Musst be writen like `00:00`.", required: true);
            AddTextInput("Date",    "reminder_date",    TextInputStyle.Short,       "On which date? Must be: 28/01/2024",           required: true);

            CommandManager.commandsWithModal.Add("pubremind");
        }

        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {
            try
            {
                List<SocketMessageComponentData> components = modal.Data.Components.ToList();
                string title        = components.First(x => x.CustomId == "reminder_title").Value;
                string description  = components.First(x => x.CustomId == "reminder_text").Value;
                string time         = components.First(x => x.CustomId == "reminder_time").Value;
                string date         = components.First(x => x.CustomId == "reminder_date").Value;

                if (!Utilities.CheckTimeFormate(time))
                {
                    ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
                    string errorMessage = await LanguageManager.GetTranslation("reminderTimeNotMatching", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                if (!Utilities.CheckFormateDate(date))
                {
                    ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
                    string errorMessage = await LanguageManager.GetTranslation("reminderDateNoMatching", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                PublicReminderObject reminder = ReminderManager.publicReminderObjectsDictionary[modal.User.Id];

                reminder.Title         = title;
                reminder.Description   = description;
                reminder.Time          = time;
                reminder.Date          = date;
                reminder.UserId        = modal.User.Id;

                await ReminderManager.AddPublicReminder(reminder);
                string message = await LanguageManager.GetTranslation("datepubreminderSaved", modal.User.Id, "", reminder.Date, reminder.Time, reminder.ChannelId);
                await modal.ModifyOriginalResponseAsync(func => func.Content = message);
            }
            catch (Exception exception)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                await Utilities.SendDevLogMessage(1, $"Building and sending the embed failed.\nException: {exception.Message}");
            }

            ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
        }
    }

    /// <summary>
    /// Modal für reminder function with duration.
    /// </summary>
    internal class PublicReminderModalDuration : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="pubremind_duration"/>.<para/>
        /// Modal inputs:<br/>
        /// <paramref name="Titel"/> - short<br/>
        /// <paramref name="Message"/> - paragraph<br/>
        /// <paramref name="Time"/> - short<br/>
        /// <paramref name="Duration"/> - short<para/>
        /// Connected to:<br/>
        /// <seealso cref="SetPublicReminder"/>
        /// </summary>
        internal PublicReminderModalDuration(string customId) : base(customId)
        {
            WithTitle("Create a reminder");
            AddTextInput("Titel",       "reminder_title",       TextInputStyle.Short,       "The title of the reminder.",               required: true);
            AddTextInput("Message",     "reminder_text",        TextInputStyle.Paragraph,   "Write the content for the reminder.",      required: true);
            AddTextInput("Time",        "reminder_time",        TextInputStyle.Short,       "Reminder time. Musst be `00:00`.",         required: true);
            AddTextInput("Duration",    "reminder_duration",    TextInputStyle.Short,       "How many days you want to be reminded?",   required: true);

            CommandManager.commandsWithModal.Add("pubremind");
        }

        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {
            try
            {
                List<SocketMessageComponentData> components = modal.Data.Components.ToList();
                string title            = components.First(x => x.CustomId == "reminder_title").Value;
                string description      = components.First(x => x.CustomId == "reminder_text").Value;
                string time             = components.First(x => x.CustomId == "reminder_time").Value;
                string durationString   = components.First(x => x.CustomId == "reminder_duration").Value.ToLower();

                if (!Utilities.CheckTimeFormate(time))
                {
                    ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
                    string errorMessage = await LanguageManager.GetTranslation("reminderTimeNotMatching", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                int durationTime = Utilities.CheckFormateDuration(durationString);
                if (durationTime == -1)
                {
                    ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
                    string errorMessage = await LanguageManager.GetTranslation("reminderDurationNotMatch", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    return;
                }

                PublicReminderObject reminder = ReminderManager.publicReminderObjectsDictionary[modal.User.Id];

                reminder.Title          = title;
                reminder.Description    = description;
                reminder.Time           = time;
                reminder.Duration       = durationTime;
                reminder.UserId         = modal.User.Id;

                await ReminderManager.AddPublicReminder(reminder);
                string message = await LanguageManager.GetTranslation("durationpubreminderSaved", modal.User.Id, "", reminder.ChannelId, reminder.Duration, reminder.Time);
                await modal.ModifyOriginalResponseAsync(func => func.Content = message);
            }
            catch (Exception exception)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                await Utilities.SendDevLogMessage(1, $"Building and sending the embed failed.\nException: {exception.Message}");
            }

            ReminderManager.publicReminderObjectsDictionary.Remove(modal.User.Id);
        }
    }
}
