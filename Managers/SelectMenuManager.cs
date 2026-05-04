
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
    /// Handeling al select menu functions.
    /// </summary>
    internal class SelectMenuManager
    {
        /// <summary>
        /// Need to block loading on API resume connection for API on runtime.
        /// </summary>
        private static bool selectMenusLoaded;

        /// <summary>
        /// Triggered on bot start and builds all existing select menus.
        /// </summary>
        internal static void SetupSelectMenus()
        {
            if (selectMenusLoaded) return;

            // menu custom id has to follow this schematic:
            // type_unique-id_{menuId}_data
            // types can be: respond, none

            //WoWCharacterSelectMenu woWCharacterSelectMenu = new WoWCharacterSelectMenu("wowchar");

            selectMenusLoaded = true;
        }



        /// <summary>
        /// Holds select menu data for all opened, but not used menus.<br/>
        /// We need to close the menus after using, to prevent the user from reselcting.
        /// </summary>
        internal static Dictionary<ulong, string> openSelectMenus = new Dictionary<ulong, string>();

        /// <summary>
        /// Triggerd when a select menu was used.<br/>
        /// Triggerevent is from <seealso cref="Program._client"/>.<para/>
        /// Functions:<br/>
        /// - Checking if <seealso cref="ButtonManager.IsButtonAllowedToUse(SocketMessageComponent)"/> is all true.<br/>
        /// - Sending respond if nessecary.<br/>
        /// - Checking if the select menu is registered internal in <seealso cref="MenuSelected.selectMenuList"/>.<br/>
        /// - Checking if a user is using the select menu to fast with <seealso cref="ButtonManager.CheckButtonPressedTime(SocketMessageComponent)"/>.<br/>
        /// - Execute <seealso cref="MenuSelected.OnSelectedMenu(SocketMessageComponent)"/> function if all checks are valid.
        /// </summary>
        internal static async Task SelectMenuExecutedHandler(SocketMessageComponent selectMenu)
        {
            // reusing this function bc button and select menu are the same data type.
            if (await ButtonManager.IsButtonAllowedToUse(selectMenu) == false) return;

            string[] splitedCustomId = selectMenu.Data.CustomId.Split('_');

            if (splitedCustomId[0] == "respond")
                await selectMenu.RespondAsync(await LanguageManager.GetTranslation("selectMenuLoading", selectMenu.User.Id), ephemeral: true);

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += async (sender, e) =>
            {
                MenuSelected cmd;

                string errorMessage = await LanguageManager.GetTranslation("selectMenuUnknown", selectMenu.User.Id);

                try
                {
                    cmd = MenuSelected.selectMenuList.First(c => c.CustomId == splitedCustomId[1]);
                }
                catch (Exception exceptionMessage)
                {                   
                    if (selectMenu.HasResponded)
                        await selectMenu.ModifyOriginalResponseAsync(func => func.Content = errorMessage);    
                    else
                        await selectMenu.RespondAsync(errorMessage, ephemeral: true);

                    await Utilities.SendDevLogMessage(1, $"Custom Id invalid: {selectMenu.Data.CustomId}\nUser: " +
                        $"||{selectMenu.User.Id}||\n\n{exceptionMessage}");

                    return;
                }

                if (cmd == null)
                {
                    if (selectMenu.HasResponded)
                        await selectMenu.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    else
                        await selectMenu.RespondAsync(errorMessage, ephemeral: true);

                    await Utilities.SendDevLogMessage(1, $"SelectMenu `cmd` was null. It is not recognized by our system?\nCustom Id invalid: " +
                        $"{selectMenu.Data.CustomId}\nUser: ||{selectMenu.User.Id}||");
                    return;
                }

                try
                {
                    if (await ButtonManager.CheckButtonPressedTime(selectMenu) == false) return;
                    await cmd.OnSelectedMenu(selectMenu);
                }
                catch (Exception exceptionMessage)
                {
                    string message = await LanguageManager.GetTranslation("generalError", selectMenu.User.Id);

                    if (selectMenu.HasResponded)
                        await selectMenu.ModifyOriginalResponseAsync(func => { func.Content = message; });
                    else
                        await selectMenu.RespondAsync(message, ephemeral: true);

                    await Utilities.SendDevLogMessage(1, $"Button interaction could not be executed!\nUser: ||{selectMenu.User.Id}||\nCustom Id: " +
                        $"{selectMenu.Data.CustomId}\n\n{exceptionMessage}");
                    return;
                }
            };
            backgroundWorker.RunWorkerAsync();
        }



        /// <summary>
        /// Sending select menus.<para/>
        /// Datetime formate must be: DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
        /// </summary>
        internal static async Task SendMultipleSelectMenus(SelectMenuBuilder selectMenu, List<SelectMenuOptionBuilder> allOptions, IMessageChannel channel, string menuId)
        {
            List<SelectMenuOptionBuilder> optionList = new List<SelectMenuOptionBuilder>();

            for (int i = 0; i < 25; i++)
            {
                if (allOptions.Count == 0)
                    break;

                optionList.Add(allOptions[0]);
                allOptions.RemoveAt(0);
            }

            SelectMenuBuilder newSelectMenu = selectMenu;
            newSelectMenu.Options = optionList;

            var selectBuilder = new ComponentBuilder().WithSelectMenu(newSelectMenu);
            IUserMessage message = await channel.SendMessageAsync(components: selectBuilder.Build());
            openSelectMenus.Add(message.Id, menuId);

            if (allOptions.Count > 0)
                await SendMultipleSelectMenus(selectMenu, allOptions, channel, menuId);
        }

        /// <summary>
        /// Deleting messages with select menus by a given menu id.
        /// </summary>
        internal static async Task DeleteOldSelectMenus(string menuId, IMessageChannel channel)
        {
            List<ulong> messageIdsToDelete = new List<ulong>();

            foreach (var item in openSelectMenus)
            {
                if (item.Value == menuId)
                    messageIdsToDelete.Add(item.Key);
            }

            foreach (ulong messageId in messageIdsToDelete)
            {
                try
                {
                    await channel.DeleteMessageAsync(messageId);
                    openSelectMenus.Remove(messageId);
                }
                catch (Exception ex)
                {
                    await Utilities.SendDevLogMessage(1, $"Message id was not found in openSelectMenu list.\nKey: {messageId}\n{ex}");
                }
            }
        }
    }



    /// <summary>
    /// Constructor for the submitted select menu objects.
    /// </summary>
    internal class MenuSelected : SelectMenuBuilder
    {
        internal MenuSelected(string customId)
        {
            WithCustomId(customId);
            selectMenuList.Add(this);
        }

        internal static List<MenuSelected> selectMenuList = new List<MenuSelected>();

        internal virtual async Task OnSelectedMenu(SocketMessageComponent selectMenu)
        {
            await Task.FromResult(0);
        }
    }
}
