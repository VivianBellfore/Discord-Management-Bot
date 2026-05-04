
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="embed"/> command.
    /// </summary>
    internal class SendEmbed : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SendEmbed() : base("mod", "embed", "command_mod_embed") { }

        /// <summary>
        /// Register the modal for embeds called <paramref name="embed_modal"/>.
        /// </summary>
        internal static EmbedModal embedModal = new EmbedModal("embed_modal");



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
    internal class EmbedModal : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="embed_modal"/>.<para/>
        /// Modal inputs:<br/>
        /// <paramref name="Titel"/> - short<br/>
        /// <paramref name="Nachricht"/> - paragraph<para/>
        /// Connected to:<br/>
        /// <seealso cref="Embed"/>
        /// </summary>
        internal EmbedModal(string customId) : base(customId)
        {
            WithTitle("Create an embed");
            AddTextInput("Titel field", "embed_title", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 250, required: false);
            AddTextInput("Message field 1", "embed_text", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 4000, required: true);
            AddTextInput("Message field 2", "embed_text2", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 3", "embed_text3", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 4", "embed_text4", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);

            CommandManager.commandsWithModal.Add("embed");
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
                    .WithDescription(text)
                    .WithColor(color)
                    .WithImageUrl(gifUrl);

                if (title != "")
                    embedBuilder.Title = title;

                int totalLength = title.Length + text.Length;

                string[] fieldInput = new[] { text2, text3, text4 };

                foreach (var input in fieldInput)
                {
                    if (string.IsNullOrWhiteSpace(input) || totalLength >= 5900)
                        continue;

                    var parsedFields = ParseFieldsFromMarkdown(input);

                    foreach (var field in parsedFields)
                    {
                        if (totalLength >= 5900)
                            break;

                        string name = string.IsNullOrWhiteSpace(field.Title) ? "\u200B" : field.Title;
                        string value = string.IsNullOrWhiteSpace(field.Content) ? "\u200B" : field.Content;

                        if (name.Length + value.Length + totalLength > 5900)
                        {
                            int allowed = 5900 - totalLength;
                            value = value.Substring(0, Math.Max(0, allowed));
                        }

                        embedBuilder.AddField(name, value, false);
                        totalLength += name.Length + value.Length;
                    }
                }

                if (totalLength >= 6000)
                {
                    string errorMessage = await LanguageManager.GetTranslation("embedToLong", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(m => m.Content = errorMessage );
                    return;
                }
                else
                    await modal.DeleteOriginalResponseAsync();

                await modal.Channel.SendMessageAsync(embed: embedBuilder.Build());
            }
            catch (Exception exception)
            {
                string error = await LanguageManager.GetTranslation("generalError", modal.User.Id);
                await modal.ModifyOriginalResponseAsync(func => func.Content = error);
                await Utilities.SendDevLogMessage(1, $"Building and sending the embed faild.\nException: {exception.Message}");
            }

            Utilities.tempColorChoises.TryRemove(modal.User.Id, out _);
            Utilities.tempGifChoises.TryRemove(modal.User.Id, out _);
        }

        private List<(string Title, string Content)> ParseFieldsFromMarkdown(string input)
        {
            var result = new List<(string Title, string Content)>();
            if (string.IsNullOrWhiteSpace(input))
                return result;

            string currentTitle = null;
            var currentLines = new List<string>();

            foreach (var rawLine in input.Split('\n'))
            {
                var line = rawLine.TrimEnd();

                if (line.StartsWith("#"))
                {
                    if (currentLines.Count > 0 || currentTitle != null)
                    {
                        result.Add((
                            currentTitle ?? "\u200B",
                            string.Join("\n", currentLines)
                        ));
                        currentLines.Clear();
                    }

                    currentTitle = line.TrimStart('#').Trim();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        currentLines.Add(line);
                }
            }

            if (currentLines.Count > 0 || currentTitle != null)
            {
                result.Add((
                    currentTitle ?? "\u200B",
                    string.Join("\n", currentLines)
                ));
            }

            return result;
        }

    }
}
