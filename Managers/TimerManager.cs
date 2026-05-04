
using Discord.WebSocket;

using System;
using System.Linq;
using System.Timers;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling timer tsak functions.
    /// </summary>
    internal class TimerManager
    {
        #region Temp saves
        /// <summary>
        /// Contains the last handled timestamp for timer functions.
        /// </summary>
        private static string lastTimedEvent = "";

        /// <summary>
        /// When was the <seealso cref="Utilities.randomUserSeeds"/> last cleared.
        /// </summary>
        private static DateTime randomUserSeedlastClearTime = DateTime.Now;

        /// <summary>
        /// Key is channel id and string is time in formate DateTime.Now.ToShortDateString().
        /// </summary>
        internal static ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, DateTime>> tempVoices = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, DateTime>>();
        #endregion



        /// <summary>
        /// Adding all temp voices to the dictionary "tempVoices".
        /// </summary>
        internal static async Task FetchTempVoiceData()
        {
            List<dynamic> result = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `guild_temp_voice`",
                new Dictionary<string, object>() { });

            if ( result.Count <= 0) return;

            foreach (dynamic item in result)
            {
                ulong guildId = (ulong)item.guild_id;
                ulong channelId = (ulong)item.channel_id;
                DateTime time = DateTime.FromBinary((long)item.time);

                var guildDict = tempVoices.GetOrAdd(guildId, _ => new ConcurrentDictionary<ulong, DateTime>());

                guildDict.TryAdd(channelId, time);
            }
        }



        /// <summary>
        /// Handler for the system timer event.<para/>
        /// </summary>
        internal async void OnTimedEvent( object source, ElapsedEventArgs e )
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += async ( sender, en ) =>
            {
                try
                {
                    string timeNow = DateTime.Now.ToString("HH:mm");

                    // preventing double postings:
                    if ( lastTimedEvent == timeNow )
                        return;
                    else
                        lastTimedEvent = timeNow;

                    Console.Write( "[" );
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(DateTime.Now.ToLongTimeString());
                    Console.ResetColor();
                    Console.Write( $"] {Glados.lines[Glados.stage]}\n" );

                    Glados.stage++;
                    if (Glados.stage >= Glados.lines.Count())
                        Glados.stage = 0;

                    if ( ReminderManager.publicReminderObjects.Count >= 1 )
                        await ReminderManager.DoPublicReminder(timeNow);

                    //if ( ReminderManager.privateReminderObjects.Count >= 1 )
                    //    await ReminderManager.DoPrivateReminder(timeNow);

                    if ( DateTime.Now.Minute == 0 )
                        Utilities.randomUserSeeds.Clear();

                    if (tempVoices.Count > 0)
                        await CheckTempVoiceTimeLimit();

                }
                catch ( Exception exceptionMessage )
                {
                    await Utilities.SendDevLogMessage(1, exceptionMessage.ToString());
                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Deleting all temp voices above the time limit.
        /// </summary>
        private static async Task CheckTempVoiceTimeLimit()
        {
            Dictionary<ulong, ulong> toBeRemoved = new Dictionary<ulong, ulong>(); 

            foreach ( var guildDic in tempVoices)
            {
                SocketGuild guild = StartBotInstance._client.GetGuild(guildDic.Key);
                if (guild == null)
                {
                    await Utilities.SendDevLogMessage(1, $"Could not find guild {guildDic.Key}!");
                    continue;
                }

                foreach ( var voiceDic in guildDic.Value)
                {
                    if ( DateTime.Now < voiceDic.Value.AddMinutes(Configurations.TempVoiceIdlTimer) ) continue;

                    SocketVoiceChannel voiceChannel = guild.GetVoiceChannel(voiceDic.Key);
                    if (voiceChannel == null)
                    {
                        await Utilities.SendDevLogMessage(1, $"Could not find voice channel {voiceDic.Key}!");
                        continue;
                    }

                    if ( voiceChannel.ConnectedUsers.Count > 0 ) continue;

                    await voiceChannel.DeleteAsync();

                    int deleteCount = await MySqlWrapper.SQLExecuteNonQuery(
                        "DELETE FROM `guild_temp_voice` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id",
                        new Dictionary<string, object>() { { "channel_id", voiceDic.Key }, { "guild_id", guild.Id } });

                    if (deleteCount <= 0)
                        await Utilities.SendDevLogMessage(1, $"Could not delete temp voice channel {voiceDic.Key} from guild {guild.Id}.");

                    toBeRemoved.Add ( guild.Id, voiceDic.Key );
                }
            }

            foreach( var entry in toBeRemoved)
            {
                tempVoices[entry.Key].TryRemove(entry.Value, out _);
            }
        }
    }
}
