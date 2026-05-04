
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling the received message interaction from Discord API.
    /// </summary>
    internal class MessageManager
    {
        #region Reaction Messages
        /// <summary>
        /// Contains a list of reaction message objects with the message id as a key.<para/>
        /// Types:<br/>
        /// - openGated<br/>
        /// - closedGated<br/>
        /// - sticky
        /// </summary>
        internal static Dictionary<ulong, ReactionMessageObject> reactionMessages = new Dictionary<ulong, ReactionMessageObject>();



        #region DISCORD EVENTS
        /// <summary>
        /// Triggerd when a message is send in any channel, on any guild.
        /// </summary>
        internal static async Task MessageReceivedHandler(SocketMessage message)
        {
            if (message.Type == MessageType.ApplicationCommand) return; // ignoring any command inputs

            var channel = message.Channel as ITextChannel;
            if (channel == null) return;

            if (message.Channel.GetChannelType() == ChannelType.News)
                await PublishAnnouncement(message);

            await GuildManager.CheckMessageReceivedGuildSettings(channel.GuildId, message.Author.Id, message);

            if (ContainsChannelStickyMessage(message.Channel.Id, channel.GuildId))
                await ReplaceStickyMessage(message, channel);

            // NO OTHER BOTS below this point!
            if (message.Author.IsBot && message.Author.Id != 1339224661103476768)
                return;

            if (message.Content.Contains($"<@{StartBotInstance.botClientId}>"))
                await TalkToUser(message);
        }

        /// <summary>
        /// This function is triggered when a message was deleted.
        /// </summary>
        internal static async Task MessageDeleteHandler(Cacheable<IMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
        {
            IMessage message = null;
            try
            {
                message = arg1.HasValue ? arg1.Value : await arg1.GetOrDownloadAsync();
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch message values.\n{ex}");
                return;
            }

            if (message == null) return; // its to old, we cant read content from older messages they are just gone...

            SocketGuildChannel messageChannel = arg2.Value as SocketGuildChannel;
            if (messageChannel == null) return;

            GuildObject guildObject = await GuildManager.GetGuildData(messageChannel.Guild.Id);
            if (guildObject == null) return;

            if (reactionMessages.ContainsKey(message.Id))
            {
                string reactionTitle = await LanguageManager.GetTranslation("titleReactionDeleted", 0, guildObject.Language);
                string reactionMessage = await LanguageManager.GetTranslation("messageReactionDeleted", 0, guildObject.Language, message.Id, messageChannel.Id);
                await GuildManager.SendSystemMessageToGuild(guildObject.GuildId, 0, reactionTitle, reactionMessage);
                RemoveReactionMessage(reactionMessages[message.Id]);
            }

            // No handling of bot messages below this point.
            if (message.Author.IsBot) return;

            if (guildObject.CheckDeletedMessages)
                await LogDeletedGuildMessage(message, messageChannel, guildObject);
        }
        #endregion



        private static async Task LogDeletedGuildMessage(IMessage message, SocketGuildChannel messageChannel, GuildObject guildObject)
        {
            string messageText = GetFullMessageContent(message);

            SocketGuildChannel logChannel = messageChannel.Guild.GetChannel(guildObject.LogChannel);
            if (logChannel == null) return;

            ITextChannel logChannel2 = logChannel as ITextChannel;
            if (logChannel2 == null) return;

            string title = await LanguageManager.GetTranslation("deleteMessageTitle", messageChannel.Guild.OwnerId);

            string userMention = "";
            if (await PermissionManager.HasUserBotPermissionRole("admin", messageChannel.Guild.Id, message.Author as SocketGuildUser) ||
                await PermissionManager.IsUserGuildOwner(messageChannel.Guild.Id, message.Author.Id))
                userMention = message.Author.GlobalName;
            else
                userMention = $"<@{message.Author.Id}>";

            await Utilities.SendMessageForLongText($"{title}\nMessage author was {userMention} in {messageChannel.Name}:\n\n# Text:\n{messageText}", logChannel2);
        }



        /// <summary>
        /// Need to block loading on API resume connection for API on runtime.
        /// </summary>
        private static bool reactionsLoaded;

        /// <summary>
        /// Fetching all existing reaction messages from database and setting them up into dictonarys.<br/>
        /// Will clear the lists before adding data from database.<br/>
        /// Does clear the list if no entrys in database are found.
        /// </summary>
        internal static async void SetReactionMessageList()
        {
            if (reactionsLoaded) return;

            MessageManager.reactionMessages.Clear();

            List<dynamic> reactionMessages = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `guild_reaction_messages`",
                new Dictionary<string, object>() { });

            if (reactionMessages.Count <= 0) return;

            foreach (dynamic message in reactionMessages)
            {
                MessageManager.reactionMessages.Add(message.message_id, new ReactionMessageObject(message.guild_id, message.channel_id, message.message_id, message.event_type));
            }

            reactionsLoaded = true;
        }

        /// <summary>
        /// Removing reaction messages from <seealso cref="reactionMessages"/> and from the data base.
        /// </summary>
        internal static async void RemoveReactionMessage(ReactionMessageObject reactionMessage)
        {
            reactionMessages.Remove(reactionMessage.MessageId);

            int removeCounter = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `guild_reaction_messages` WHERE `guild_id` = @guild_id AND `message_id` = @message_id",
                new Dictionary<string, object>() { { "guild_id", reactionMessage.GuildId }, { "message_id", reactionMessage.MessageId} });

            if (removeCounter <= 0)
                await Utilities.SendDevLogMessage(1, $"Reaction message was not removed. Guild id is {reactionMessage.GuildId} and message id is {reactionMessage.MessageId}.");

        }
        #endregion

        

        /// <summary>
        /// Checks if a reaction message is existing.
        /// </summary>
        private static bool ContainsChannelStickyMessage(ulong channelId, ulong guildId)
        {
            return reactionMessages.Values.Any(prop => prop.GuildId == guildId && prop.ChannelId == channelId && prop.EventType == "sticky");
        }

        /// <summary>
        /// Removes an old reaction message that is older then 2 minutes and send a new one.
        /// </summary>
        private static async Task ReplaceStickyMessage(SocketMessage message, ITextChannel channel)
        {
            List<ReactionMessageObject> reactions = reactionMessages.Values.Where(prop => prop.GuildId == channel.GuildId && prop.ChannelId == channel.Id && prop.EventType == "sticky").ToList();
            if ( reactions.Count <= 0) return;

            var stickyMessage = await channel.GetMessageAsync(reactions[0].MessageId);
            if ( stickyMessage == null ) return;

            if (stickyMessage.CreatedAt.AddMinutes(2).ToUniversalTime() > DateTimeOffset.Now.ToUniversalTime()) return;

            List<EmbedObject> embedList = GetEmbedObjectFromMessage(stickyMessage);
            if ( embedList.Count <= 0) return;

            await channel.DeleteMessageAsync(stickyMessage);

            var embedBuilder = new EmbedBuilder()
                    .WithTitle(embedList[0].Titel)
                    .WithDescription(embedList[0].Description)
                    .WithColor(embedList[0].Color)
                    .WithImageUrl(embedList[0].ImageURL);

            if (embedList[0].Field_1 != "")
                embedBuilder.AddField("\u200B", embedList[0].Field_1, false);

            if (embedList[0].Field_2 != "")
                embedBuilder.AddField("\u200B", embedList[0].Field_2, false);

            if (embedList[0].Field_3 != "")
                embedBuilder.AddField("\u200B", embedList[0].Field_3, false);

            IMessage newSticky = await channel.SendMessageAsync(embedList[0].MessageContent, embed: embedBuilder.Build());

            reactionMessages.Remove(stickyMessage.Id);
            reactionMessages.Add(newSticky.Id, new ReactionMessageObject(channel.GuildId, channel.Id, newSticky.Id, "sticky"));

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_reaction_messages` SET `message_id` = @message_id WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `event_type` = @event_type",
                new Dictionary<string, object>() { { "guild_id", channel.GuildId }, { "channel_id", channel.Id }, { "message_id", newSticky.Id }, { "event_type", "sticky" } });

            if (updateCount <= 0)
                await Utilities.SendDevLogMessage(1, $"New sticky message could not be updated. Message id was {stickyMessage.Id}");
        }

        

        internal static List<EmbedObject> GetEmbedObjectFromMessage(IMessage message)
        {
            List<EmbedObject> embedList = new List<EmbedObject>();
            
            foreach (var embed in message.Embeds)
            {
                EmbedObject embedObject = new EmbedObject();

                embedObject.Titel = embed.Title;
                embedObject.Description = embed.Description;

                if (embed.Fields.Count() > 0)
                    embedObject.Field_1 = embed.Fields[0].Value.ToString();

                if (embed.Fields.Count() > 1)
                    embedObject.Field_2 = embed.Fields[1].Value.ToString();

                if (embed.Fields.Count() > 2)
                    embedObject.Field_3 = embed.Fields[2].Value.ToString();

                embedObject.ImageURL = embed.Url;
                embedObject.Color = embed.Color == null ? Color.Default : (Color)embed.Color;

                embedObject.MessageContent = message.Content;

                embedList.Add(embedObject);
            }

            return embedList;
        }

        /// <summary>
        /// Fetching content of a message. can have up to 6000 character!
        /// </summary>
        internal static string GetFullMessageContent(IMessage message)
        {
            if (message == null) return string.Empty;

            string embedText = "";

            if (message.Embeds.Any())
            {
                embedText = "\n\n# Embed:\n";

                foreach (var embed in message.Embeds)
                {
                    embedText += $"{embed.Title}\n{embed.Description}\n";
                    foreach (var field in embed.Fields)
                    {
                        embedText += $"{field.Value}\n";
                    }
                }
            }

            return $"{message.Content}{embedText}";
        }

        /// <summary>
        /// Is cross posting a message in an announcement channel.
        /// </summary>
        private static async Task PublishAnnouncement(SocketMessage message)
        {
            if (message.Type != MessageType.Default) return;

            IUserMessage userMessage = message as IUserMessage;

            if (userMessage.Flags.HasValue && userMessage.Flags.Value.HasFlag(MessageFlags.Crossposted)) return;

            if (userMessage == null)
            {
                await Utilities.SendDevLogMessage(1, $"Message could not be casted to IUserMessage. Id was {message.Id}.");
                return;
            }

            try
            {
                await userMessage.CrosspostAsync();
            }
            catch (Exception exception)
            {
                await Utilities.SendDevLogMessage(1, $"Tried to send a crossposting on invalid message. Id was {message.Id}.\n\n{exception}");
            }
        }

        /// <summary>
        /// Removes a message from a channel.
        /// </summary>
        internal static async Task RemoveMessage(ulong guildId, ulong channelId, ulong messageId)
        {
            SocketGuild guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null)
            {
                Console.WriteLine($"[ MessageManager, RemoveMessage ] Error, could not fetch socket guild! Guild id was {guildId}.");
                return;
            }

            var channel = guild.GetChannel(channelId) as ITextChannel;
            if (channel == null)
            {
                Console.WriteLine($"[ MessageManager, RemoveMessage ] Error, could not fetch channel! Guild was {guildId} and channel was {channelId}.");
                return;
            }

            var message = await channel.GetMessageAsync(messageId);
            if (message == null)
            {
                Console.WriteLine($"[ MessageManager, RemoveMessage ] Error, could not fetch message! Guild was {guildId} and channel was {channelId} an message was {messageId}.");
                return;
            }

            await message.DeleteAsync();
        }



        #region Talk to user
        /// <summary>
        /// Sending respond messages from key words.
        /// </summary>
        private static async Task TalkToUser(SocketMessage message)
        {
            int repliedCount = 0;
            foreach ( var talkingPoint in talkingPointList)
            {
                foreach (string text in talkingPoint.Value)
                {
                    if (message.Content.ToLower().Contains(text.ToLower()))
                    {
                        var messageReferenceBuilder = new MessageReference(message.Id);
                        await message.Channel.SendMessageAsync(await LanguageManager.GetTranslation(talkingPoint.Key, message.Author.Id), messageReference: messageReferenceBuilder);

                        repliedCount++;
                        return;
                    }
                }
            }

            if ( repliedCount <= 0)
                await message.Channel.SendMessageAsync(await LanguageManager.GetTranslation("iCanDo", message.Author.Id, "", message.Author.Username));
        }

        /// <summary>
        /// Contains key words as values, there are talking points of the bot. The translation string for the bot answers are the dictionary keys.
        /// </summary>
        private static Dictionary<string, List<string>> talkingPointList = new Dictionary<string, List<string>>()
        {
            {"botDeveloper", new List<string>(){ "gehörst du", "dein besitzer", "deinen besitzer", "besitzt dich", "dein dev", "dein developer", "dich developed", "dich developet", "dich developt", 
                "dein entwickler", "dich entwickelt", "du entwickelt", "dich erschaffen", "dein erschaffer", "dich gebaut", "du gebaut", "dich gecoded", "du gecoded", "dich programmiert", "du programmiert",
                "dich erstellt", "du erstellt", "hat dich gemacht", "dein meister", "deine meisterin", "dein gebieter", "dein eigentümer"}},
        };
        #endregion
    }



    internal class ReactionMessageObject
    {
        internal ulong GuildId { get; set; }
        internal ulong ChannelId { get; set; }
        internal ulong MessageId { get; set; }
        internal string EventType { get; set; }


        internal ReactionMessageObject(ulong guildId, ulong channelId, ulong messageId, string eventType)
        {
            GuildId = guildId;
            ChannelId = channelId;
            MessageId = messageId;
            EventType = eventType;
        }
    }
}
