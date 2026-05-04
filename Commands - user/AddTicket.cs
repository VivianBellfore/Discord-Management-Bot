
using Discord;
using Discord.Rest;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="use ticket"/> command.
    /// </summary>
    internal class AddTicket : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal AddTicket() : base("use", "ticket", "command_use_ticket") { }

        /// <summary>
        /// Contains ticket objects that ar waiting to be send in the ticket queue function.
        /// </summary>
        internal static List<TicketObject> ticketCreationList = new List<TicketObject>();


        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            bool isRegistered = await PermissionManager.HasUserAcceptTos(command.User.Id);
            if (!isRegistered)
            {
                string errorMessage = await LanguageManager.GetTranslation("needToBeRegistered", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            (bool isTicketValid, string errorMessage2, ulong ticketCategorie) = await CheckConditions((ulong)command.GuildId, command.User.Id);
            if ( !isTicketValid )
            {
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            SocketGuild guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {(ulong)command.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            ITextChannel newChannel = await guild.CreateTextChannelAsync($"{command.User.Username}", new Action<TextChannelProperties>(target => target.CategoryId = ticketCategorie));
            if (newChannel == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            ticketCreationList.Add(new TicketObject(command.User.Id, newChannel, guild, command));
            await TicketCreationQueue();
        }

        /// <summary>
        /// Handeling the queue for the ticket generation.
        /// </summary>
        private static async Task TicketCreationQueue()
        {
            if (ticketCreationList.Count <= 0) return;

            List<TicketObject> ticketList = new List<TicketObject>();
            foreach (TicketObject ticketItem in ticketCreationList)
            {
                ticketList.Add(ticketItem);
            }

            ticketCreationList.Clear();

            try
            {
                foreach (TicketObject ticket in ticketList)
                {
                    await Task.Delay(3000);
                    await AddNewTicketChannelWithPermission(ticket.UserId, ticket.Channel, ticket.Guild, ticket.Command);
                }
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"Ticket was not send correctly:\n{ex}");
            }
        }

        /// <summary>
        /// Checking for ticket conditions.<para/>
        /// </summary>
        private static async Task<(bool isValid, string message, ulong ticketCategorie)> CheckConditions(ulong guildId, ulong userId)
        {
            GuildObject guild = await GuildManager.GetGuildData(guildId);
            if (guild == null)
                return (false, await LanguageManager.GetTranslation("registrationMissingBot", userId), 0);

            if (!guild.TicketsActive)
                return (false, await LanguageManager.GetTranslation("ticketsNotActive", userId), 0);

            if (guild.TicketCategory == 0)
                return (false, await LanguageManager.GetTranslation("ticketNoCategory", userId), 0);

            var ticketChannel = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `channel_id` FROM `tickets` WHERE `user_id` = @user_id AND `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "user_id", userId }, { "guild_id", guildId } });

            if (ticketChannel != null)
                return (false, await LanguageManager.GetTranslation("ticketAlreadyExists", userId, "", Convert.ToUInt64(ticketChannel)), 0);

            return (true, "", guild.TicketCategory);
        }

        /// <summary>
        /// Saving a new ticket channel to data base and will add permissions given.
        /// </summary>
        internal static async Task AddNewTicketChannelWithPermission(ulong userId, ITextChannel newChannel, SocketGuild guild, SocketSlashCommand command)
        {
            await newChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(newChannel));

            RestUser user = await StartBotInstance._client.Rest.GetUserAsync(userId);

            await Task.Delay(3000);

            await newChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                  embedLinks: PermValue.Allow, attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny,
                  createPublicThreads: PermValue.Deny, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Deny, useApplicationCommands: PermValue.Deny));

            await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO  `tickets` (`guild_id`, `channel_id`, `user_id`) VALUES (@guild_id, @channel_id, @user_id)",
                new Dictionary<string, object>() { { "guild_id", guild.Id }, { "channel_id", newChannel.Id }, { "user_id", userId } });

            GuildObject guildObject = await GuildManager.GetGuildData(guild.Id);

            await Task.Delay(3000);

            if (guildObject != null && guildObject.ModeratorRole != 0)
                await newChannel.AddPermissionOverwriteAsync(guild.GetRole(guildObject.ModeratorRole), new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                  embedLinks: PermValue.Allow, attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny,
                  createPublicThreads: PermValue.Deny, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Deny, useApplicationCommands: PermValue.Allow));

            if (guildObject != null && guildObject.AdminRole != 0)
                await newChannel.AddPermissionOverwriteAsync(guild.GetRole(guildObject.AdminRole), OverwritePermissions.AllowAll(newChannel));

            await Task.Delay(3000);

            string message = await LanguageManager.GetTranslation("ticketBotMessage", userId, "", userId);
            await newChannel.SendMessageAsync(message);

            string ephermalMessage = await LanguageManager.GetTranslation("ticketOpened", userId, "", newChannel.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = ephermalMessage);
        }
    }



    /// <summary>
    /// Building a nobject for tickets.
    /// </summary>
    internal class TicketObject
    {
        internal ulong UserId { get; set; }
        internal ITextChannel Channel { get; set; }
        internal SocketGuild Guild { get; set; }
        internal SocketSlashCommand Command { get; set; }


        internal TicketObject(ulong userId, ITextChannel channel, SocketGuild guild, SocketSlashCommand command)
        {
            UserId = userId;
            Channel = channel;
            Guild = guild;
            Command = command;
        }
    }
}
