
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
    /// Building and managing the <paramref name="sticky"/> command.<para/>
    /// </summary>
    internal class SendStickyMessage : CommandObject
    {
        internal static Dictionary<ulong, string> slowModeStatus = new Dictionary<ulong, string>();

        /// <summary>
        /// Struct for the help command informations.
        /// </summary>
        internal SendStickyMessage() : base("admin", "sticky", "command_admin_sticky") { }

        /// <summary>
        /// Register the modal for embeds called <paramref name="embed_modal"/>.
        /// </summary>
        internal static StickyEmbedModal embedModal = new StickyEmbedModal("sticky_modal");

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            Utilities.tempColorChoises.TryRemove(command.User.Id, out _);
            Utilities.tempColorChoises.TryAdd(command.User.Id, command.Data.Options.First().Options.ElementAt(1).Value.ToString());

            if (command.Data.Options.First().Options.Count > 2)
            {
                if (Utilities.ValidateUrlWithUri(command.Data.Options.First().Options.ElementAt(2).Value.ToString()))
                {
                    Utilities.tempGifChoises.TryRemove(command.User.Id, out _);
                    Utilities.tempGifChoises.TryAdd(command.User.Id, command.Data.Options.First().Options.ElementAt(2).Value.ToString());
                }
            }

            await command.RespondWithModalAsync(embedModal.Build());

            if (slowModeStatus.ContainsKey(command.User.Id))
                slowModeStatus[command.User.Id] = command.Data.Options.First().Options.ElementAt(0).Value.ToString();
            else
                slowModeStatus.Add(command.User.Id, command.Data.Options.First().Options.ElementAt(0).Value.ToString());
        }

        /// <summary>
        /// Adds or updates a sticky message in the data base and sends a guild log message out.
        /// </summary>
        internal static async Task AddStickyMessage(RestUserMessage stickyMessage, ulong guildId)
        {
            if (stickyMessage == null)
            {
                await Utilities.SendDevLogMessage(1, "Sticky message was null.");
                return;
            }

            GuildObject guildData = await GuildManager.GetGuildData(guildId);
            if (guildData == null) return;

            string messageLink = $"https://discord.com/channels/{guildId}/{stickyMessage.Channel.Id}/{stickyMessage.Id}";

            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `message_id` FROM `guild_reaction_messages` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `event_type` = @event_type",
                new Dictionary<string, object>() { { "guild_id", guildId }, { "channel_id", stickyMessage.Channel.Id }, { "event_type", "sticky" } });

            if (result == null)
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `guild_reaction_messages` ( `guild_id`, `channel_id`, `message_id`, `event_type` ) VALUES ( @guild_id, @channel_id, @message_id, @event_type )",
                    new Dictionary<string, object>() { { "guild_id", guildId }, { "channel_id", stickyMessage.Channel.Id }, { "message_id", stickyMessage.Id }, { "event_type", "sticky" } });

                if(insertCount <= 0)
                {
                    await Utilities.SendDevLogMessage(1, $"Sticky message could not be saved. Message id was {stickyMessage.Id}.");
                    return;
                }

                await GuildManager.SendSystemMessageToGuild(guildId, 0, await LanguageManager.GetTranslation("stickyMessageTitle", 0, guildData.Language), 
                    await LanguageManager.GetTranslation("stickyMessageText", 0, guildData.Language, stickyMessage.Channel.Id, messageLink));

                try
                {
                    MessageManager.reactionMessages.Add(stickyMessage.Id, new ReactionMessageObject(guildId, stickyMessage.Channel.Id, stickyMessage.Id, "sticky"));
                }
                catch (Exception ex)
                {
                    await Utilities.SendDevLogMessage(1, ex.Message);
                }

                return;
            }

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_reaction_messages` SET `message_id` = @message_id WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id AND `event_type` = @event_type",
                new Dictionary<string, object>() { { "guild_id", guildId }, { "channel_id", stickyMessage.Channel.Id }, { "message_id", stickyMessage.Id }, { "event_type", "sticky" } });

            if (updateCount <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"New sticky message could not be updated. Message id was {stickyMessage.Id}");
                return;
            }

            await GuildManager.SendSystemMessageToGuild(guildId, 0, await LanguageManager.GetTranslation("stickyMessageTitle", 0, guildData.Language),
                    await LanguageManager.GetTranslation("stickyMessageText", 0, guildData.Language, stickyMessage.Channel.Id, messageLink));

            MessageManager.reactionMessages.Remove(Convert.ToUInt64(result));
            MessageManager.reactionMessages.Add(stickyMessage.Id, new ReactionMessageObject(guildId, stickyMessage.Channel.Id, stickyMessage.Id, "sticky"));
        }
    }



    /// <summary>
    /// This class is building the modal for <seealso cref="Embed"/>.
    /// </summary>
    internal class StickyEmbedModal : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="sticky_modal"/>.<para/>
        /// Modal inputs:<br/>
        /// <paramref name="Titel"/> - short<br/>
        /// <paramref name="Nachricht"/> - paragraph<para/>
        /// Connected to:<br/>
        /// <seealso cref="Embed"/>
        /// </summary>
        internal StickyEmbedModal(string customId) : base(customId)
        {
            WithTitle("Create an embed");
            AddTextInput("Titel field", "embed_title", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 250);
            AddTextInput("Message field 1", "embed_text", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 4000);
            AddTextInput("Message field 2", "embed_text2", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 3", "embed_text3", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 4", "embed_text4", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);

            CommandManager.commandsWithModal.Add("sticky");
        }

        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {
            RestUserMessage stickyMessage = null;

            try
            {
                List<SocketMessageComponentData> components = modal.Data.Components.ToList();
                string title = components.First(x => x.CustomId == "embed_title").Value;
                string text = components.First(x => x.CustomId == "embed_text").Value;
                string text2 = components.First(x => x.CustomId == "embed_text2").Value;
                string text3 = components.First(x => x.CustomId == "embed_text3").Value;
                string text4 = components.First(x => x.CustomId == "embed_text4").Value;

                Color color = await Utilities.GetColor(Utilities.tempColorChoises.First(userId => userId.Key == modal.User.Id).Value);

                string gifUrl = "";
                if (Utilities.tempGifChoises.Count > 0 && Utilities.tempGifChoises[modal.User.Id].Any() == true)
                    gifUrl = Utilities.tempGifChoises[modal.User.Id];

                var embedBuilder = new EmbedBuilder()
                    .WithTitle(title)
                    .WithDescription(text)
                    .WithColor(color)
                    .WithImageUrl(gifUrl);

                int totalLength = title.Length + text.Length;

                string[] fieldTexts = new[] { text2, text3, text4 };

                foreach (var fieldText in fieldTexts)
                {
                    if (totalLength >= 5900)
                        break;

                    if (fieldText.Length > 0 && fieldText != " ")
                    {
                        if (fieldText.Length + totalLength > 5900)
                        {

                            string trimmedText = fieldText.Substring(0, 5900 - totalLength);
                            embedBuilder.AddField("\u200B", trimmedText, false);
                            totalLength += trimmedText.Length;
                        }
                        else
                        {
                            embedBuilder.AddField("\u200B", fieldText, false);
                            totalLength += fieldText.Length;
                        }
                    }
                }

                if (totalLength >= 6000)
                    await modal.ModifyOriginalResponseAsync(m => m.Content = "❌ Embed too long. The combined embed text exceeds Discord’s 6000 character limit.");
                else
                    await modal.DeleteOriginalResponseAsync();

                stickyMessage = await modal.Channel.SendMessageAsync(embed: embedBuilder.Build());
            }
            catch (Exception exception)
            {
                string error = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                await modal.ModifyOriginalResponseAsync(func => func.Content = error);
                await Utilities.SendDevLogMessage(1, $"Building and sending the embed faild.\nException: {exception.Message}");
            }

            Utilities.tempColorChoises.TryRemove(modal.User.Id, out _);
            Utilities.tempGifChoises.TryRemove(modal.User.Id, out _);
            

            await SendStickyMessage.AddStickyMessage(stickyMessage, (ulong)modal.GuildId);

            string slowMode = SendStickyMessage.slowModeStatus[modal.User.Id];
            if (slowMode == "slow")
            {
                if (modal.Channel is ITextChannel textChannel)
                    await textChannel.ModifyAsync(prop => prop.SlowModeInterval = 300);
            }

            SendStickyMessage.slowModeStatus.Remove(modal.User.Id);
        }
    }
}
