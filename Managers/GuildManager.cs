
using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Contains functions that are handeling guild related events. 
    /// </summary>
    internal class GuildManager
    {
        /// <summary>
        /// Triggered when the bot is entering a new guild. It will send an introduction message to the guild owner.
        /// </summary>
        internal static async Task JoinedGuildHandler(SocketGuild guild)
        {
            var registerDate = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `register_date` FROM `guild_data` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", guild.Id } });

            // Bot is already registered. We dont need to send this message as the bot has just reentered the server.
            if (registerDate != null)
                return;

            var owner = await StartBotInstance._client.GetUserAsync(guild.OwnerId);

            await Task.Delay(5000);

            try
            {
                var buttonBuilder = new ComponentBuilder();
                foreach ( var dic in LanguageManager.languages)
                {
                    string label = await LanguageManager.GetTranslation("buttonAcceptBotForGuild", 0, dic.Key);
                    buttonBuilder.WithButton(label, $"respond_guildsetup_{guild.Id}_register_{dic.Key}", ButtonStyle.Success);
                }

                await owner.SendMessageAsync(await LanguageManager.GetTranslation("registerGuildOwnerDM", guild.OwnerId), components: buttonBuilder.Build());

                await Utilities.SendDevLogMessage(2, $"Bot has joined new guild {guild.Id} named {guild.Name}. Server owner {owner.GlobalName} got the information message.");
            }
            catch (Exception ex)
            {
                if (owner == null)
                    await Utilities.SendDevLogMessage(1, $"Bot joined new guild but owner was null! Guild id is {guild.Id}.\n\n{ex}");
                else
                    await Utilities.SendDevLogMessage(1, $"Bot joined new guild but owner does not accept DM´s from bot! Guild id is {guild.Id}.\n\n{ex}");
            }
        }

        /// <summary>
        /// Checking the registration of a guild and will add it to our data base if it is not registered.
        /// </summary>
        internal static async Task RegisterNewGuild(SocketMessageComponent button)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_');

            // "respond_guildsetup_{guild.Id}_register_{language}"
            ulong guildId = Convert.ToUInt64(splitedCustomId[2]);

            var registerDate = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `register_date` FROM `guild_data` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", guildId } });

            if (registerDate != null)
            {
                string message = await LanguageManager.GetTranslation("buttonAcceptBotForGuildRepeat", 0, splitedCustomId[4]);
                await button.ModifyOriginalResponseAsync(func => func.Content = message);
                await button.Message.DeleteAsync();
                return;
            }

            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `guild_data` (`guild_id`, `register_date`, `language`) VALUES (@guild_id, @register_date, @language)",
                    new Dictionary<string, object>() { { "guild_id", guildId }, { "register_date", DateTime.Now.ToShortDateString() }, { "language", splitedCustomId[4]} });

            if (insertCount <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Guild was not registered. Id is {guildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registerGuildDatabaseError", 0, splitedCustomId[4]);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
            }
            else
            {
                await Utilities.SendDevLogMessage(2, $"New guild was registered. Id is {guildId}.");
                string message = await LanguageManager.GetTranslation("guildRegisterSuccess", 0, splitedCustomId[4]);
                await button.ModifyOriginalResponseAsync(func => func.Content = message);
            }

            await button.Message.DeleteAsync();

            string language = splitedCustomId[4];

            bool isRegistered = await UserManager.RegisterUser(button.User.Id, language);
            if (isRegistered)
                await LanguageManager.SetUserLanguage(language, button.User.Id);
        }

        /// <summary>
        /// Reading guild data from data base and builds a guild object from that.<para/>
        /// Returns null, if no entry was found.
        /// </summary>
        internal static async Task<GuildObject> GetGuildData(ulong guildId)
        {
            SocketGuild guildSocket = StartBotInstance._client.GetGuild(guildId);

            string guildName;

            if (guildSocket == null)
                guildName = "Guild name not found.";
            else
                guildName = guildSocket.Name;

            List<dynamic> result = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `guild_data` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", guildId } });

            if (result == null || result.Count <= 0)
                return null;   

            bool gatedCommunity = Convert.ToInt32(result[0].gatedcommunity) == 1 ? true : false;
            bool wordfilter = Convert.ToInt32(result[0].wordfilter) == 1 ? true : false;
            bool deleteMessage = Convert.ToInt32(result[0].deletemessage) == 1 ? true : false;
            bool ticketsActive = Convert.ToInt32(result[0].ticketsactive) == 1 ? true : false;
            bool econemyActive = Convert.ToInt32(result[0].econemy) == 1 ? true : false;
            bool isHalloweenActive = Convert.ToInt32(result[0].halloween) == 1 ? true : false;
            bool tempVoice = Convert.ToInt32(result[0].tempvoice) == 1 ? true : false;

            Dictionary<string, ulong> guildChannel = new Dictionary<string, ulong>();

            List<dynamic> channelResults = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `guild_channel` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", guildId } });

            if (channelResults.Count > 0)
            {
                foreach (dynamic entry in channelResults)
                {
                    var row = (IDictionary<string, object>)entry;

                    foreach (var column in row)
                    {
                        if (column.Key == "guild_id")
                            continue;

                        if (column.Value != null)
                        {
                            guildChannel[column.Key] = Convert.ToUInt64(column.Value);
                        }
                    }
                }
            }

            return new GuildObject(guildId, guildName, Convert.ToString(result[0].register_date), result[0].language.ToString(), Convert.ToUInt64(result[0].channel_logs), 
                Convert.ToUInt64(result[0].channel_news), Convert.ToUInt64(result[0].channel_events), Convert.ToUInt64(result[0].category_ticket), 
                Convert.ToUInt64(result[0].category_voice), Convert.ToUInt64(result[0].role_admin), Convert.ToUInt64(result[0].role_mod), Convert.ToUInt64(result[0].role_member), 
                gatedCommunity, wordfilter, deleteMessage, Convert.ToString(result[0].points_name), Convert.ToString(result[0].invite_link), ticketsActive, econemyActive, 
                tempVoice, isHalloweenActive, guildChannel);
        }

        /// <summary>
        /// Fetches guild language from database and gives back system language if data is null.
        /// </summary>
        internal static async Task<string> GetGuildLanguage(ulong guildId)
        {
            GuildObject data =  await GetGuildData(guildId);

            if (data == null) return LanguageManager.systemLanguage;

            return data.Language;
        }

        /// <summary>
        /// Inserting or deleting a "pending action" entry from data base.<para/>
        /// Must contain everytime: isEntry and type<para/>
        /// Missing values can be set as 0.
        /// </summary>
        internal static async Task SetPendingAction(bool isEntry, string type, ulong guildId, ulong userId, ulong channelId)
        {
            if (isEntry)
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    $"INSERT INTO `guild_pending_actions` (`type`, `guild_id`, `user_id`, `channel_id`) VALUES (@type, @guild_id, @user_id, @channel_id)",
                    new Dictionary<string, object>() { {"type", type }, { "guild_id", guildId }, { "user_id", userId }, { "channel_id", channelId } });

                if (insertCount <= 0)
                    await Utilities.SendDevLogMessage(1, $"Inserting new pending was not completed.\nData was: type {type}, guild {guildId}, user || {userId} ||, channel {channelId}.");
                
                return;
            }

            int deleteCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "DELETE FROM `guild_pending_actions` WHERE `type` = @type AND `guild_id` = @guild_id AND `user_id` = @user_id AND `channel_id` = @channel_id",
                    new Dictionary<string, object>() { { "type", type }, { "guild_id", guildId }, { "user_id", userId }, { "channel_id", channelId } });

            if (deleteCount <= 0)
                await Utilities.SendDevLogMessage(1, $"Deleting a pending was not completed.\nData was: type {type}, guild {guildId}, user || {userId} ||, channel {channelId}.");
        }

        /// <summary>
        /// Checking the "pending actions" table in data base for an entry.
        /// </summary>
        internal static async Task<bool> GetPendingAction(string type, ulong guildId, ulong userId, ulong channelId)
        {
            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `type` FROM `guild_pending_actions` WHERE `type` = @type AND `guild_id` = @guild_id AND `user_id` = @user_id AND `channel_id` = @channel_id",
                new Dictionary<string, object>() { { "type", type }, { "guild_id", guildId }, { "user_id", userId }, { "channel_id", channelId } });

            if (result == null)
                return false;
            else 
                return true;
        }

        /// <summary>
        /// Fetching a system channel for a guild and will send a message to it as an embed.<para/>
        /// Message types:<br/>
        /// 0 = Log channel<br/>
        /// 1 = News channel<br/>
        /// 2 = Event channel<para/>
        /// Reurns:<br/>
        /// (false, errorMessage) = message was not send<br/>
        /// (true, "") = message was send
        /// </summary>
        internal static async Task<(bool isMessageSend, string errorMessage)> SendSystemMessageToGuild(ulong guildId, int messageType, string title, string message)
        {
            GuildObject guildObject = await GetGuildData(guildId);
            if (guildObject == null) return (false, "Guild not registered.");

            var guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null) return (false, "Guild was invalid.");

            ITextChannel textChannel = null;

            if (messageType == 0)
                textChannel = guild.GetChannel(guildObject.GuildChannel["system"]) as ITextChannel;
            else if (messageType == 1)
                textChannel = guild.GetChannel(guildObject.GuildChannel["news"]) as ITextChannel;
            else if (messageType == 2)
                textChannel = guild.GetChannel(guildObject.GuildChannel["events"]) as ITextChannel;

            if (textChannel == null) return (false, "Channel was not found.");

            await Utilities.SendMessageForLongText($"# {title}\n{message}", textChannel);
            return (true, "");
        }

        /// <summary>
        /// Send an embed to all registered guilds in the specific system channel.<para/>
        /// Message types:<br/>
        /// 0 = Log channel<br/>
        /// 1 = News channel<br/>
        /// 2 = Event channel<para/>
        /// Reurns:<br/>
        /// (false, errorMessage) = message was not send<br/>
        /// (true, "") = message was send
        /// </summary>
        internal static async Task <(bool isMessageSend, string errorMessage)> SendSystemMessageToAllGuilds(int messageType, EmbedBuilder embedBuilder)
        {
            string column = "system";

            if (messageType == 1)
                column = "news";

            if (messageType == 2)
                column = "events";

            List<dynamic> channelIdList = await MySqlWrapper.SQLExecuteReader( $"SELECT `{column}`, `guild_id` FROM `guild_channel`", new Dictionary<string, object>() {} );

            if ( channelIdList.Count <= 0)
                return(false, "No system channel was found in guild_data data base!");

            string errors = string.Empty;

            foreach (dynamic entry in channelIdList)
            {
                var dict = (IDictionary<string, object>)entry;
                ulong guildId = Convert.ToUInt64(dict["guild_id"]);
                ulong channelId = Convert.ToUInt64(dict[column]);

                if (channelId == 0 || guildId == 0) continue;

                SocketGuild guild = Utilities.GetGuildSocket((ulong)entry.guild_id);
                if (guild == null)
                {
                    errors = errors + $"- Guild not found: {(ulong)entry.guild_id}\n";
                    continue;
                }      

                ITextChannel textChannel = guild.GetChannel(channelId) as ITextChannel;
                if (textChannel == null)
                {
                    errors = errors + $"- Channel not found: {channelId}\n";
                    continue;
                }

                await textChannel.SendMessageAsync(embed: embedBuilder.Build());

                await Task.Delay(3000);
            }

            if (errors != string.Empty)
                await Utilities.SendDevLogMessage(2, errors);

            return (true, errors);
        }

        /// <summary>
        /// Does guild function on receiving a message in a guild.
        /// </summary>
        internal static async Task CheckMessageReceivedGuildSettings(ulong guildId, ulong userId, SocketMessage message)
        {
            GuildObject guildObject = await GetGuildData(guildId);
            if (guildObject == null) return;

            var channel = message.Channel as ITextChannel;
            if (channel == null) return;

            bool isMessageDeleted = false;

            if (guildObject.UseWordfilter)
            {
                BlockedTextManager blockedTextManager = new BlockedTextManager();
                isMessageDeleted = await blockedTextManager.DeletProhibitedMessage(message, guildObject, channel);
            }

            // NO BOTS below this point!
            if (message.Author.IsBot && message.Author.Id != 1339224661103476768)
                return;

            if (guildObject.Economy && !isMessageDeleted && await PermissionManager.HasUserAcceptTos(message.Author.Id) && message.Content.Length >= 20)
                await UserManager.SetUserGuildPoints(userId, guildId, 10, true);
        }
    }



    /// <summary>
    /// Building a button for <seealso cref="guildsetup"/>.
    /// </summary>
    internal class GuildSetupButton : ButtonPressed
    {
        /// <summary>
        /// Builder for the button with custom id <paramref name="guildsetup"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="GuildManager"/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal GuildSetupButton(string customId) : base(customId)
        {
            WithCustomId("guildsetup");
        }

        /// <summary>
        /// Handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            // "respond_guildsetup_{guild.Id}_register"
            string[] splitedCustomId = button.Data.CustomId.Split('_');

            if (await PermissionManager.IsUserGuildOwner(Convert.ToUInt64(splitedCustomId[2]), button.User.Id) == false)
            {
                string permissionMessage = await LanguageManager.GetTranslation("missingPermisson", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = permissionMessage);
                return;
            }

            switch (splitedCustomId[3])
            {
                case "register":
                    await GuildManager.RegisterNewGuild(button);
                    break;
            }
        }
    }



    /// <summary>
    /// Constructor for the guild object.
    /// </summary>
    internal class GuildObject
    {
        internal ulong GuildId { get; set; }
        internal string GuildName { get; set; }
        internal string RegisterDate { get; set; }
        internal string Language {  get; set; }
        internal ulong LogChannel { get; set; }
        internal ulong NewsChannel { get; set; }
        internal ulong EventChannel { get; set; }
        internal ulong TicketCategory { get; set; }
        internal ulong TempVoiceCategory { get; set; }
        internal ulong AdminRole { get; set; }
        internal ulong ModeratorRole { get; set; }
        internal ulong MemberRole { get; set; }
        internal bool IsGatedCommunity { get; set; }
        internal bool UseWordfilter { get; set; }
        internal bool CheckDeletedMessages { get; set; }
        internal string PointsName { get; set; }
        internal string InviteLink { get; set; }
        internal bool TicketsActive { get; set; }
        internal bool Economy {  get; set; }

        internal bool TempVoice { get; set; }
        internal bool IsHalloweenActive { get; set; }

        internal Dictionary<string, ulong> GuildChannel { get; }



        internal GuildObject(ulong guildId, string guildName, string registerDate, string language, ulong logChannel, ulong newsChannel, ulong eventChannel, ulong ticketCategory, 
            ulong tempVoiceCategory, ulong adminRole, ulong moderatorRole, ulong memberRole, bool isGatedCommunity, bool useWordfilter, bool checkDeletedMessages, string pointsName, 
            string inviteLink, bool ticketsActive, bool econemy, bool tempVoice, bool isHalloweenActive, Dictionary<string, ulong> guildChannel)
        {
            GuildId = guildId;
            GuildName = guildName;
            RegisterDate = registerDate;
            Language = language;
            LogChannel = logChannel;
            NewsChannel = newsChannel;
            EventChannel = eventChannel;
            TicketCategory = ticketCategory;
            TempVoiceCategory = tempVoiceCategory;
            AdminRole = adminRole;
            ModeratorRole = moderatorRole;
            MemberRole = memberRole;
            IsGatedCommunity = isGatedCommunity;
            UseWordfilter = useWordfilter;
            CheckDeletedMessages = checkDeletedMessages;
            PointsName = pointsName;
            InviteLink = inviteLink;
            TicketsActive = ticketsActive;
            Economy = econemy;
            TempVoice = tempVoice;
            IsHalloweenActive = isHalloweenActive;
            GuildChannel = guildChannel ?? new Dictionary<string, ulong>();
        }
    }



    /// <summary>
    /// Constructor for the pending action object.
    /// </summary>
    internal class PendingActionObject
    {
        internal string Type { get; set; }
        internal ulong GuildId { get; set;}
        internal ulong UserId { get; set;}
        internal ulong ChannelId { get; set;}


        internal PendingActionObject(string type, ulong guildId, ulong userId, ulong channelId)
        {
            Type = type;
            GuildId = guildId;
            UserId = userId;
            ChannelId = channelId;
        }
    }
}
