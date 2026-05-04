
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all submitted modals.
    /// </summary>
    internal class ModalManager
    {
        /// <summary>
        /// Need to block loading on API resume connection for API on runtime.
        /// </summary>
        private static bool modalLoaded;

        /// <summary>
        /// Register all modal name and functions on bot start.
        /// </summary>
        internal static void SetupModals()
        {
            if (modalLoaded) return;

            EmbedModal embedModal           = new EmbedModal("embed");
            DevNewsModal devNewsModal       = new DevNewsModal("news");
            StickyEmbedModal stickyModal    = new StickyEmbedModal("sticky");
            RulesModal ruleModal            = new RulesModal("setrule");
            RandomModal randomModal         = new RandomModal("random");

            PublicReminderModalDaily reminderDailyModal         = new PublicReminderModalDaily("pubreminddaily");
            PublicReminderModalWeekly reminderWeeklyModal       = new PublicReminderModalWeekly("pubremindweekly");
            PublicReminderModalDate reminderDateModal           = new PublicReminderModalDate("pubreminddate");
            PublicReminderModalDuration reminderDurationModal   = new PublicReminderModalDuration("pubremindduration");

            modalLoaded = true;
        }



        /// <summary>
        /// Triggered when a modal is submitted.<br/>
        /// Triggerevent is from <seealso cref="Program._client"/>.<para/>
        /// Functions:<br/>
        /// - Sending respond if nessecary..<br/>
        /// - Execute <seealso cref="ModalButtons.OnButtonPressed(SocketMessageComponent)"/> function if all checks are valid.
        /// </summary>
        internal static async Task ModalSubmittedHandler(SocketModal modal)
        {
            await modal.RespondAsync(await LanguageManager.GetTranslation("modalLoading", modal.User.Id), ephemeral: true);

            string[] splitedCustomId = modal.Data.CustomId.Split('_');

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += async (sender, e) =>
            {
                ModalSubmit cmd;
                string errorMessage = await LanguageManager.GetTranslation("modalUnknown", modal.User.Id);

                try
                {
                    cmd = ModalSubmit.modalCommandList.First(c => c.CustomId == splitedCustomId[0]);
                }
                catch (Exception exceptionMessage)
                {
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    await Utilities.SendDevLogMessage(1, exceptionMessage.ToString());
                    return;
                }

                if (cmd == null)
                {
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    await Utilities.SendDevLogMessage(1, $"Modal `cmd` was null. It is not recognized by our system?\nModal was: {modal.Id}");
                    return;
                }

                try
                {
                    await cmd.OnModalExecute(modal);
                }
                catch (Exception ex)
                {
                    string message = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => { func.Content = message; });
                    await Utilities.SendDevLogMessage(1, $"Modal interaction could not be executed!\n{ex}");
                    return;
                }
            };
            backgroundWorker.RunWorkerAsync();
        }
    }



    /// <summary>
    /// Building types of modals and will send the right modal type to user.
    /// </summary>
    internal class ModalButtons : ButtonPressed
    {
        /// <summary>
        /// Builder for the button with custom id <paramref name="modal"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal ModalButtons(string customId) : base(customId)
        {
            WithCustomId("modal");
        }

        /// <summary>
        /// This function is handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            string[] buttonParts = button.Data.CustomId.Split('_');

            switch (buttonParts[1])
            {
                default:
                    await Utilities.SendDevLogMessage(1, $"Modal name was not regconized. Name: {buttonParts[1]}");
                    await button.RespondAsync(await LanguageManager.GetTranslation("generelError", button.User.Id));
                    break;
            }
        }
    }



    /// <summary>
    /// Constructor for modal submitte objects.
    /// </summary>
    internal class ModalSubmit : ModalBuilder
    {
        internal ModalSubmit(string customId)
        {
            WithCustomId(customId);
            modalCommandList.Add(this);
        }

        internal static List<ModalSubmit> modalCommandList = new List<ModalSubmit>();

        internal virtual async Task OnModalExecute(SocketModal modal)
        {
            await Task.FromResult(0);
        }
    }
}
