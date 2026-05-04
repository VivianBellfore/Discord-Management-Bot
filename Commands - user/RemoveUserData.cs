
using Discord;
using Discord.Rest;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <paramref name="use deletedata"/> command.<para/>
    /// </summary>
    internal class RemoveUserData : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal RemoveUserData() : base("use", "deletedata", "command_use_deletedata") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            bool isRegisterd = await PermissionManager.HasUserAcceptTos(command.User.Id);
            if (!isRegisterd)
            {
                string errorMessage = await LanguageManager.GetTranslation("noUserDataFound", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string label = await LanguageManager.GetTranslation("removeUserDataLabel", command.User.Id);
            var buttonBuilder = new ComponentBuilder().WithButton(label, $"respond_removeuserdata_{command.User.Id}", ButtonStyle.Danger);

            string content = await LanguageManager.GetTranslation("removeUserDataMessage", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => { func.Content = content; func.Components = buttonBuilder.Build(); });
        }

        /// <summary>
        /// This function is removing channel permissions of a user for a ticket channel.
        /// </summary>
        internal static async Task RemoveRelatedUserTickets(ulong userId)
        {
            List<dynamic> ticketList = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `tickets` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (ticketList.Count > 0)
            {
                foreach (dynamic ticket in ticketList)
                {
                    SocketGuild guild = StartBotInstance._client.GetGuild(Convert.ToUInt64(ticket.guild_id));
                    if (guild == null) continue;

                    SocketGuildChannel channel = guild.GetChannel(Convert.ToUInt64(ticket.channel_id));
                    if (channel == null) continue;

                    RestUser user = await StartBotInstance._client.Rest.GetUserAsync(Convert.ToUInt64(ticket.user_id));
                    if (user == null) continue;

                    await channel.RemovePermissionOverwriteAsync((IUser)user);
                    await channel.ModifyAsync(prop => prop.Name = $"closed-{user.Username}");

                    ITextChannel iTextChannel = channel as ITextChannel;
                    await iTextChannel.SendMessageAsync(await LanguageManager.GetTranslation("userDeletedThereData", guild.OwnerId));
                }
            }
        }
    }



    /// <summary>
    /// This class is building a button for <seealso cref="removeuserdata"/>.
    /// </summary>
    internal class RemoveUserDataButton : ButtonPressed
    {
        /// <summary>
        /// This constructor is a builder for the button with custom id <paramref name="removeuserdata"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="RemoveUserData/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal RemoveUserDataButton(string customId) : base(customId)
        {
            WithCustomId("removeuserdata");
        }

        /// <summary>
        /// This function is handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_'); // "respond_removeuserdata_userid"

            if (button.User.Id != Convert.ToUInt64(splitedCustomId[2])) return;

            await RemoveUserData.RemoveRelatedUserTickets(button.User.Id);

            int deleteCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `user_profile` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", button.User.Id } });

            if (deleteCount <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("deletedUserData", button.User.Id);
            await button.ModifyOriginalResponseAsync(func => func.Content = message);
        }
    }
}
