
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="news"/> command.
    /// </summary>
    internal class SendDevNews : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SendDevNews() : base("dev", "news", "command_dev_news") { }

        /// <summary>
        /// Register the modal for embeds called <paramref name="devnews_modal"/>.
        /// </summary>
        internal static DevNewsModal embedModal = new DevNewsModal("news_modal");



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            Utilities.tempColorChoises.TryRemove(command.User.Id, out _);
            Utilities.tempColorChoises.TryAdd(command.User.Id, command.Data.Options.First().Options.ElementAt(0).Value.ToString());

            if (command.Data.Options.First().Options.Count > 1)
            {
                if (Utilities.ValidateUrlWithUri(command.Data.Options.First().Options.ElementAt(1).Value.ToString()))
                {
                    Utilities.tempGifChoises.TryRemove(command.User.Id, out _);
                    Utilities.tempGifChoises.TryAdd(command.User.Id, command.Data.Options.First().Options.ElementAt(1).Value.ToString());
                }
            }

            await command.RespondWithModalAsync(embedModal.Build());
        }
    }



    /// <summary>
    /// This class is building the modal for <seealso cref="Embed"/>.
    /// </summary>
    internal class DevNewsModal : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="devnews_modal"/>.<para/>
        /// </summary>
        internal DevNewsModal(string customId) : base(customId)
        {
            WithTitle("Create a news embed");
            AddTextInput("Titel field", "embed_title", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 250);
            AddTextInput("Message field 1", "embed_text", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 4000);
            AddTextInput("Message field 2", "embed_text2", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 3", "embed_text3", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 4", "embed_text4", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);

            CommandManager.commandsWithModal.Add("news");
        }

        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {
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
                {
                    await modal.ModifyOriginalResponseAsync(m => m.Content = ":x: Embed too long. The combined embed text exceeds Discord’s 6000 character limit.");
                    return;
                }

                (bool messagesSend, string errors) = await GuildManager.SendSystemMessageToAllGuilds(1, embedBuilder);

                if (messagesSend && errors != string.Empty)
                    await modal.ModifyOriginalResponseAsync(func => func.Content = $":x: Messages are send, some errors accured:\n{errors}");
                else if (messagesSend && errors == string.Empty)
                    await modal.ModifyOriginalResponseAsync(func => func.Content = $":white_check_mark: Messages are send to all registered server with a system channel for this case.");
                else
                    await modal.ModifyOriginalResponseAsync(func => func.Content = $":x: No message was send:\n{errors}");

            }
            catch (Exception exception)
            {
                string error = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                await modal.ModifyOriginalResponseAsync(func => func.Content = error);
                await Utilities.SendDevLogMessage(1, $"Building and sending the news embed faild.\nException: {exception.Message}");
            }

            Utilities.tempColorChoises.TryRemove(modal.User.Id, out _);
            Utilities.tempGifChoises.TryRemove(modal.User.Id, out _);
        }
    }
}
