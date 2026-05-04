
using Discord;
using Discord.Rest;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;



namespace LCNET_Management_Bot
{
    internal class ChannelManager
    {
        internal static async Task ChannelDestroyed(SocketChannel channel)
        {
            foreach (var reaction in MessageManager.reactionMessages)
            {
                if (reaction.Value.ChannelId == channel.Id)
                    await GuildManager.SendSystemMessageToGuild(reaction.Value.GuildId, 0, "Reaction message deleted!",
                        $"A channel with a reaction message in it was deleted! ChannelId: {channel.Id} MessageId: {reaction.Value.MessageId} EventType: {reaction.Value.EventType}\n" +
                        $"Please check if this messages was still needed. The system has deleted this reaction message and all assosiated functions.");
            }

            // checking if the channel was an open ticket. Has to be the last check on a deleted channel.
            SocketGuildChannel guildChannel = channel as SocketGuildChannel;
            if (guildChannel == null)
            {
                await Utilities.SendDevLogMessage(1, $"A channel was destroyed but the guild could not be read! This may have been a ticket channel: {channel.Id}.");
                return;
            }

            var userID = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `user_id` FROM `tickets` WHERE `channel_id` = @channel_id AND `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "channel_id", channel.Id }, { "guild_id", guildChannel.Guild.Id } });

            if (userID != null)
            {
                GuildObject guildObject = await GuildManager.GetGuildData(guildChannel.Guild.Id);
                if (guildObject == null)
                {
                    await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {guildChannel.Guild.Id}.");
                    await Utilities.SendDevLogMessage(1, $"Guild object was null! Id was {guildChannel.Guild.Id}.");
                    return;
                }

                RestGuild guild = await StartBotInstance._client.Rest.GetGuildAsync(guildObject.GuildId);
                if (guild == null)
                {
                    await Utilities.SendDevLogMessage(1, $"Guild was null! Id was {guildChannel.Guild.Id}.");
                    return;
                }

                ITextChannel systemChannel = await guild.GetChannelAsync(guildObject.LogChannel) as ITextChannel;

                if (systemChannel == null)
                {
                    await Utilities.SendDevLogMessage(1, $"System channel was null! Guild id was {guildChannel.Guild.Id} and channel {guildChannel.Id}.");
                    return;
                }

                await systemChannel.SendMessageAsync(await LanguageManager.GetTranslation("ticketChannelRemoved", guild.OwnerId, "", userID));

                await MySqlWrapper.SQLExecuteNonQuery(
                    "DELETE FROM `tickets` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `user_id` = @user_id",
                    new Dictionary<string, object>() { { "channel_id", channel.Id }, { "guild_id", guild.Id }, { "user_id", userID } });
            }
        }

        internal static async Task RemoveChannel(ulong guildId, ulong channelId)
        {
            SocketGuild guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null)
            {
                Console.WriteLine($"[ ChannelManager, RemoveChannel ] Error, could not fetch socket guild! Guild id was {guildId}.");
                return;
            }

            SocketGuildChannel channel = guild.GetChannel(channelId);
            if (channel == null)
            {
                Console.WriteLine($"[ ChannelManager, RemoveChannel ] Error, could not fetch socket guild channel! Guild was {guildId} and channel was {channelId}.");
                return;
            }

            await channel.DeleteAsync();
        }

        internal static async Task<( bool, string )> AddTempVoice( ulong guildId, ulong userId, string name )
        {
            // Adding this only on the "AddTempVoice" command. There are already checks for existing guild and user data!

            GuildObject guildData = await GuildManager.GetGuildData(guildId);
            if (guildData == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not read guild data from DB. Guild was {guildId}.");
                return ( false, await LanguageManager.GetTranslation("fetchGuildError", userId) );
            }

            SocketGuild guild = StartBotInstance._client.GetGuild(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {guild}!");
                return ( false, await LanguageManager.GetTranslation("generalError", userId) );
            }

            IVoiceChannel newChannel = await guild.CreateVoiceChannelAsync(name, new Action<VoiceChannelProperties>(target => target.CategoryId = guildData.TempVoiceCategory));
            if (newChannel == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not create voice channel in guild {guild}!");
                return ( false, await LanguageManager.GetTranslation("generalError", userId) );
            }

            await Task.Delay(3000);
            await newChannel.SyncPermissionsAsync();

            long time = DateTime.Now.ToBinary();

            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO  `guild_temp_voice` (`guild_id`, `channel_id`, `user_id`, `time`) VALUES (@guild_id, @channel_id, @user_id, @time)",
                new Dictionary<string, object>() { { "guild_id", guildId }, { "channel_id", newChannel.Id }, { "user_id", userId }, { "time", time } });

            if (insertCount == 0)
                await Utilities.SendDevLogMessage(1, $"Could not save user into `guild_temp_voice` db for guild {guild}!");

            if (!TimerManager.tempVoices.ContainsKey(guildId))
                TimerManager.tempVoices.TryAdd(guildId, new ConcurrentDictionary<ulong, DateTime>());

            TimerManager.tempVoices[guildId].TryAdd(newChannel.Id, DateTime.FromBinary(time));

            return (true, "");
        }

        /// <summary>
        /// Every user can only add one temp voice per server.<br/>
        /// Returns true if user has no temp on a server.
        /// </summary>
        internal static async Task<bool> CheckUserTempVoice( ulong guildId, ulong userId )
        {
            List<dynamic> result = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `guild_temp_voice` WHERE `guild_id` = @guild_id AND `user_id` = @user_id",
                new Dictionary<string, object>() { { "guild_id", guildId }, { "user_id", userId } });

            if (result.Count == 0) return true;

            return false;
        }
    }
}
