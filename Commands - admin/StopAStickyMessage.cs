
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="stopsticky"/> command.<para/>
    /// </summary>
    internal class StopAStickyMessage : CommandObject
    {
        /// <summary>
        /// Struct for the help command informations.
        /// </summary>
        internal StopAStickyMessage() : base("admin", "stopsticky", "command_admin_stopsticky") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            ulong messageId = Convert.ToUInt64(command.Data.Options.First().Options.ElementAt(0).Value);
            if ( messageId == 0)
            {
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("wrongFormatNumber", command.User.Id));
                return;
            }

            ReactionMessageObject reaction = MessageManager.reactionMessages[messageId];

            int deleteCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `guild_reaction_messages` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `message_id` = @message_id AND `event_type` = @event_type",
                new Dictionary<string, object>() { { "guild_id", reaction.GuildId }, { "channel_id", reaction.ChannelId }, { "message_id", messageId }, { "event_type", "sticky" } });

            SocketGuild guild = StartBotInstance._client.GetGuild(reaction.GuildId);
            if (guild == null)
            {
                Console.WriteLine($"[ MessageManager, RemoveMessage ] Error, could not fetch socket guild! Guild id was {reaction.GuildId}.");
                return;
            }

            var channel = guild.GetChannel(reaction.ChannelId) as ITextChannel;
            if (channel == null)
            {
                Console.WriteLine($"[ MessageManager, RemoveMessage ] Error, could not fetch channel! Guild was {guild.Id} and channel was {reaction.ChannelId}.");
                return;
            }

            await channel.DeleteMessageAsync(messageId);

            MessageManager.reactionMessages.Remove(messageId);

            if (deleteCount > 0)
            {
                if (command.Channel is ITextChannel textChannel)
                    await textChannel.ModifyAsync(prop => prop.SlowModeInterval = 0);

                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("stickyMessageRemoved", command.User.Id));
            }
            else
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("stickyMessageRemovedError", command.User.Id));
        }
    }
}
