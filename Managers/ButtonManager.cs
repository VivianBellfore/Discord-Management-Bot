
using Discord;
using Discord.Rest;
using Discord.WebSocket;

using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling the executed button interaction from Discord API.
    /// </summary>
    internal class ButtonManager
    {
        /// <summary>
        /// Need to block loading on API resume connection for API on runtime.
        /// </summary>
        private static bool buttonLoaded;

        /// <summary>
        /// Contains user ids and the time they have last used a button.
        /// </summary>
        private static Dictionary<ulong, DateTime> userButtonPressedTime = new Dictionary<ulong, DateTime>();

        /// <summary>
        /// Contains user id, message id and channel id for a button that needs to be deleted at some later point.
        /// </summary>
        internal static List<(ulong userId, ulong buttonMessageId, ulong channelId, ulong guildId)> deleteButtonLaterList = new List<(ulong, ulong, ulong, ulong)>();

        /// <summary>
        /// Register all button name and functions on bot start.
        /// </summary>
        internal static void SetupButtons()
        {
            if (buttonLoaded) return;

            // button custom id has to follow this schematic:
            // type_unique-id_data
            // types can be: respond, none

            MemberButton memberButton                       = new MemberButton("member");
            GuildSetupButton guildSetupButton               = new GuildSetupButton("guildsetup");
            RemoveGuildDataButton removeGuildDataButton     = new RemoveGuildDataButton("removeguilddata");
            RemoveUserDataButton removeUserDataButton       = new RemoveUserDataButton("removeuserdata");
            UserRegisterButton userRegisterButton           = new UserRegisterButton("userregister");
            TicketButton ticketButton                       = new TicketButton("ticket");
            RoleButton roleButton                           = new RoleButton("role");
            HalloweenButton halloweenButton                 = new HalloweenButton("halloween");
            FactionInviteButton factionInviteButton         = new FactionInviteButton("factioninvite");

            buttonLoaded = true;
        }



        /// <summary>
        /// Triggerd if any button is pressed.<br/>
        /// Triggerevent is from <seealso cref="Program._client"/>.<para/>
        /// Functions:<br/>
        /// - Sending respond if nessecary.<br/>
        /// - Checking if <seealso cref="IsButtonAllowedToUse(SocketMessageComponent)"/> is all true.<br/>
        /// - Checking if the button is registered internal in <seealso cref="ButtonPressed.buttonList"/>.<br/>
        /// - Checking if a user is pressing button to fast with <seealso cref="CheckButtonPressedTime(SocketMessageComponent)"/>.<br/>
        /// - Execute <seealso cref="ButtonPressed.OnButtonPressed(SocketMessageComponent)"/> function if all checks are valid.
        /// </summary>
        internal static async Task ButtonExecutedHandler(SocketMessageComponent button)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_');

            if (splitedCustomId[0] == "respond")
                await button.RespondAsync(await LanguageManager.GetTranslation("buttonLoading", button.User.Id), ephemeral: true);

            if (await IsButtonAllowedToUse(button) == false) return;

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += async (sender, e) =>
            {
                ButtonPressed cmd;

                string errorMessage = await LanguageManager.GetTranslation("buttonUnknown", button.User.Id);

                try
                {
                    cmd = ButtonPressed.buttonList.First(c => c.CustomId == splitedCustomId[1]);
                }
                catch (Exception exceptionMessage)
                {
                    if (button.HasResponded)
                        await button.ModifyOriginalResponseAsync(func => { func.Content = errorMessage; });                  
                    else
                        await button.RespondAsync(errorMessage, ephemeral: true);

                    await Utilities.SendDevLogMessage(1, $"Custom Id invalid: {button.Data.CustomId}\nUser: ||{button.User.Id}||" +
                        $"\n\n{exceptionMessage}");

                    return;
                }

                if (cmd == null)
                {
                    if (button.HasResponded)
                        await button.ModifyOriginalResponseAsync(func => { func.Content = errorMessage; });
                    else
                        await button.RespondAsync(errorMessage, ephemeral: true);

                    await Utilities.SendDevLogMessage(1, $"Button `cmd` was null. It is not recognized by our system?\nCustom Id invalid: {button.Data.CustomId}\nUser: ||{button.User.Id}||");
                    return;
                }

                try
                {
                    if (await CheckButtonPressedTime(button) == false) return;
                    await cmd.OnButtonPressed(button);
                }
                catch (Exception exceptionMessage)
                {
                    string message = await LanguageManager.GetTranslation("generalError", button.User.Id);

                    if (button.HasResponded)
                        await button.ModifyOriginalResponseAsync(func => { func.Content = message; });
                    else
                        await button.RespondAsync(message, ephemeral: true);

                    await Utilities.SendDevLogMessage(1, $"Button  interaction could not be executed!\nUser: ||{button.User.Id}||\nCustom Id: {button.Data.CustomId}\n\n{exceptionMessage}");
                    return;
                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Checking conditions to decide if a user is allowed to use a button.<para/>
        /// Conditions checked:<br/>
        /// - IsBot<br/>
        /// - IsWebhook<br/>
        /// - <seealso cref="PermissionManager.IsUserBlockedFromBotUsage(ulong, ulong)"/><br/>
        /// - <seealso cref="PermissionManager.HasUserBotPermission(int, ulong, SocketGuildUser)"/> with permission 1.<br/>
        /// - IsUserTimedOut
        /// </summary>
        internal static async Task<bool> IsButtonAllowedToUse(SocketMessageComponent button)
        {
            if (button.User.IsBot || button.User.IsWebhook) return false;

            if (button.IsDMInteraction)
                return true;

            (bool isUserBlacklisted, string blockedMessage) = await PermissionManager.IsUserBlockedFromBotUsage(button.User.Id);
            if (isUserBlacklisted)
            {
                if (button.HasResponded)
                    await button.ModifyOriginalResponseAsync(func => { func.Content = blockedMessage; });
                else
                    await button.RespondAsync(blockedMessage, ephemeral: true);

                return false;
            }

            IGuild guild = StartBotInstance._client.GetGuild((ulong)button.GuildId);
            if (guild != null)
            {
                IGuildUser guildUser = await guild.GetUserAsync(button.User.Id);

                if (guildUser != null && guildUser.TimedOutUntil.HasValue && guildUser.TimedOutUntil > DateTime.Now)
                {
                    string message = await LanguageManager.GetTranslation("functionNotWhileTimeout", button.User.Id, "", guildUser.TimedOutUntil.ToString());

                    if (button.HasResponded)
                        await button.ModifyOriginalResponseAsync(func => { func.Content = message; });
                    else
                        await button.RespondAsync(message, ephemeral: true);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checking how often and fast a button was pressed by a user.<para/>
        /// </summary>
        /// <returns>True - if user can use the button.<br/>
        /// False - if user is pressing to fast.</returns>
        internal static async Task<bool> CheckButtonPressedTime(SocketMessageComponent button)
        {
            if (userButtonPressedTime.ContainsKey(button.User.Id))
            {
                DateTime time = userButtonPressedTime.FirstOrDefault(x => x.Key == button.User.Id).Value;

                if (time.AddSeconds(3) > DateTime.Now)
                {
                    string text = await LanguageManager.GetTranslation("buttonPressedToFast", button.User.Id);

                    if (button.HasResponded)
                        await button.ModifyOriginalResponseAsync(func => { func.Content = text; });
                    else
                        await button.RespondAsync(text, ephemeral: true);

                    return false;
                }
            }

            userButtonPressedTime[button.User.Id] = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Deleting messages from a list with a matching user id.
        /// </summary>
        internal static async Task DeleteButtonMessage(ulong userId)
        {
            // checking if message content is empty? here or in parent function?
            foreach (var button in deleteButtonLaterList)
            {
                if (button.Item1 == userId)
                {
                    RestGuild guild = await StartBotInstance._client.Rest.GetGuildAsync(button.guildId);
                    if (guild == null) return;

                    ITextChannel textChannel = await guild.GetChannelAsync(Convert.ToUInt64(button.channelId)) as ITextChannel;
                    if (textChannel == null) return;

                    await textChannel.DeleteMessageAsync(button.buttonMessageId);
                }
            }

            deleteButtonLaterList.RemoveAll(x => x.userId == userId);
        }
    }



    /// <summary>
    /// Handling all incoming button pressed interactions from <seealso cref="ButtonManager.ButtonExecutedHandler"/>.
    /// </summary>
    /// <returns></returns>
    internal class ButtonPressed : ButtonBuilder
    {
        internal ButtonPressed(string customId)
        {
            WithCustomId(customId);
            buttonList.Add(this);
        }

        internal static List<ButtonPressed> buttonList = new List<ButtonPressed>();

        internal virtual async Task OnButtonPressed(SocketMessageComponent button)
        {
            await Task.FromResult(0);
        }
    }
}
