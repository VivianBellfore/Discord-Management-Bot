
using Discord;
using Discord.Rest;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="ticket"/> command.
    /// </summary>
    internal class EditTicket : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal EditTicket() : base("mod", "ticket", "command_mod_ticket") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            if ( await CheckConditions(command) == false ) return;

            string delayMessage1 = await LanguageManager.GetTranslation("delayMessageTicket1", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = delayMessage1);
            await Task.Delay(3000); // rate limits!

            var ticketOwnerId = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `user_id` FROM `tickets` WHERE `channel_id` = @channel_id AND `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "channel_id", command.Channel.Id }, { "guild_id", (ulong)command.GuildId } });

            if (ticketOwnerId == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("noUserDataFound", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            IUser ticketOwner = await StartBotInstance._client.GetUserAsync(Convert.ToUInt64(ticketOwnerId));
            if (ticketOwner == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("noUserDataFound", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string delayMessage2 = await LanguageManager.GetTranslation("delayMessageTicket2", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = delayMessage2);
            await Task.Delay(3000); // rate limits!

            ITextChannel channel = command.Channel as ITextChannel;

            await channel.RemovePermissionOverwriteAsync((IUser)ticketOwner);

            string delayMessage3 = await LanguageManager.GetTranslation("delayMessageTicket3", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = delayMessage3);
            await Task.Delay(3000); // rate limits!

            // tried to change the ticket name here got a rate limet all the time. Dont change channel names... its limited to 2 changes in ten minutes per channel.

            await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `tickets` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `user_id` = @user_id",
                new Dictionary<string, object>() { { "channel_id", command.Channel.Id }, { "guild_id", (ulong)command.GuildId }, { "user_id", ticketOwner.Id } });

            var buttonBuilder = new ComponentBuilder().WithButton(await LanguageManager.GetTranslation("ticketEditButton", command.User.Id), $"respond_ticket_{ticketOwner.Id}", ButtonStyle.Success);

            string delayMessage4 = await LanguageManager.GetTranslation("delayMessageTicket4", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = delayMessage4);
            await Task.Delay(3000); // rate limits!

            string message = await LanguageManager.GetTranslation("ticketCloseText", command.User.Id, "", command.User.Id);
            await channel.SendMessageAsync(message, components: buttonBuilder.Build());

            await Task.Delay(3000); // rate limits!

            await command.DeleteOriginalResponseAsync();
        }

        /// <summary>
        /// Checking conditions for using the command.
        /// </summary>
        private static async Task<bool> CheckConditions(SocketSlashCommand command)
        {
            GuildObject guildObject = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild object was null! Id was {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return false;
            }

            SocketGuild guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {(ulong)command.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return false;
            }

            SocketCategoryChannel category = guild.GetCategoryChannel(guildObject.TicketCategory);
            if (category == null)
            {
                await Utilities.SendDevLogMessage(1, $"Ticket category was null. Guild was {(ulong)command.GuildId} and category {guildObject.TicketCategory}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return false;
            }

            if (!category.Channels.Contains(command.Channel as SocketGuildChannel))
            {
                await Utilities.SendDevLogMessage(1, $"Ticket channel was not in ticket category.\nGuild was {(ulong)command.GuildId} and category {guildObject.TicketCategory}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return false;
            }

            return true;
        }
    }



    /// <summary>
    /// Building a button for <seealso cref="ticket"/>.
    /// </summary>
    internal class TicketButton : ButtonPressed
    {
        /// <summary>
        /// Builder for the button with custom id <paramref name="ticket"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="EditTicket"/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal TicketButton(string customId) : base(customId)
        {
            WithCustomId("ticket");
        }

        /// <summary>
        /// Handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            if (await PermissionManager.HasUserBotPermissionRole("mod", (ulong)button.GuildId, (SocketGuildUser)button.User) == false)
            {
                if (await PermissionManager.HasUserBotPermissionRole("admin", (ulong)button.GuildId, (SocketGuildUser)button.User) == false)
                {
                    if (await PermissionManager.IsUserGuildOwner((ulong)button.GuildId, button.User.Id) == false)
                    {
                        string errorMessage = await LanguageManager.GetTranslation("missingPermisson", button.User.Id);
                        await button.ModifyOriginalResponseAsync(func => { func.Content = errorMessage; });
                        return;
                    }
                }
            }

            string[] splitedCustomId = button.Data.CustomId.Split('_'); // respond_ticket_{ticketOwner.Id}

            ITextChannel channel = button.Channel as ITextChannel;
            RestUser user = await StartBotInstance._client.Rest.GetUserAsync(Convert.ToUInt64(splitedCustomId[2]));

            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                  embedLinks: PermValue.Allow, attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny,
                  createPublicThreads: PermValue.Deny, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Deny, useApplicationCommands: PermValue.Deny));

            await Task.Delay(3000); // rate limits!

            await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO  `tickets` (`guild_id`, `channel_id`, `user_id`) VALUES (@guild_id, @channel_id, @user_id)",
                new Dictionary<string, object>() { { "guild_id", (ulong)button.GuildId }, { "channel_id", channel.Id }, { "user_id", Convert.ToUInt64(splitedCustomId[2]) } });

            string message = await LanguageManager.GetTranslation("ticketReopened", button.User.Id);
            await button.Channel.SendMessageAsync(message);

            await button.Message.DeleteAsync();
        }
    }
}
