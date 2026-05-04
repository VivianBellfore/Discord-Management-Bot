
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="setrule"/> command.
    /// </summary>
    internal class SetChannelRule : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal SetChannelRule() : base("mod", "setrule", "command_mod_setrule") { }

        /// <summary>
        /// Register the modal for embeds called <paramref name="rules_modal"/>.
        /// </summary>
        internal static EmbedModal embedModal = new EmbedModal("setrule_modal");



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            IChannel channel = command.Data.Options.First().Options.ElementAt(0).Value as IChannel;

            if (channel == null )
            {
                await Utilities.SendDevLogMessage(1, $"The channel was null!\nGuild id is {(ulong)command.GuildId} and user was || {command.User.Id} ||.");
                string errorMessage = await LanguageManager.GetTranslation("channelReadError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

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

            embedModal.CustomId = $"{embedModal.CustomId}_{channel.Id}";

            await command.RespondWithModalAsync(embedModal.Build());
        }
    }



    /// <summary>
    /// This class is building the modal for <seealso cref="Embed"/>.
    /// </summary>
    internal class RulesModal : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="rules_modal"/>.<para/>
        /// Modal inputs:<br/>
        /// <paramref name="Titel"/> - short<br/>
        /// <paramref name="Nachricht"/> - paragraph<para/>
        /// </summary>
        internal RulesModal(string customId) : base(customId)
        {
            WithTitle("Create an embed");
            AddTextInput("Titel field", "embed_title", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 250);
            AddTextInput("Message field 1", "embed_text", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 1, 4000);
            AddTextInput("Message field 2", "embed_text2", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 3", "embed_text3", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);
            AddTextInput("Message field 4", "embed_text4", TextInputStyle.Paragraph, "Embed text limit is 5900 chars total (all fields). Extra text won't be saved or sent!", 0, 1000, required: false);

            CommandManager.commandsWithModal.Add("setrule");
        }

        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {

            List<SocketMessageComponentData> components = modal.Data.Components.ToList();
            string title = components.First(x => x.CustomId == "embed_title").Value;
            string text = components.First(x => x.CustomId == "embed_text").Value;
            string field1 = components.First(x => x.CustomId == "embed_text2").Value;
            string field2 = components.First(x => x.CustomId == "embed_text3").Value;
            string field3 = components.First(x => x.CustomId == "embed_text4").Value;

            string color = Utilities.tempColorChoises.First(userId => userId.Key == modal.User.Id).Value;

            string gifUrl = "";
            if (Utilities.tempGifChoises.Count > 0 && Utilities.tempGifChoises[modal.User.Id].Any() == true)
                gifUrl = Utilities.tempGifChoises[modal.User.Id];

            string[] splitedCustomId = modal.Data.CustomId.Split('_'); // "rules_modal_channelId"

            object result = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `title` FROM `guild_channel_rules` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)modal.GuildId }, { "channel_id", Convert.ToUInt64(splitedCustomId[2]) } });

            if ( result == null )
            {
                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `guild_channel_rules` (`guild_id`, `channel_id`, `title`, `text`, `field_1`, `field_2`, `field_3`, `url_string`, `color`) VALUES (@guild_id, @channel_id, @title, @text, @field_1, @field_2, @field_3, @url_string, @color)",
                    new Dictionary<string, object>() { { "guild_id", (ulong)modal.GuildId }, { "channel_id", Convert.ToUInt64(splitedCustomId[2]) }, { "title", title },
                        { "text", text }, { "field_1", field1 }, { "field_2", field2 }, { "field_3", field3 }, { "url_string", gifUrl }, { "color", color } });

                if ( insertCount > 0)
                {
                    string message = await LanguageManager.GetTranslation("dataSaved", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = message);
                }
                else
                {
                    string errorMessage = await LanguageManager.GetTranslation("saveDataError", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    await Utilities.SendDevLogMessage(1, $"Channel rules could not be inserted. Guild {(ulong)modal.GuildId} and channel {Convert.ToUInt64(splitedCustomId[2])}.");
                }  
            }
            else
            {
                int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "UPDATE `guild_channel_rules` SET `title` = @title, `text` = @text, `field_1` = @field_1, `field_2` = @field_2, `field_3` = @field_3, `url_string` = @url_string, `color` = @color " +
                        "WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id",
                    new Dictionary<string, object>() { { "guild_id", (ulong)modal.GuildId }, { "channel_id", Convert.ToUInt64(splitedCustomId[2]) }, { "title", title },
                    { "text", text }, { "field_1", field1 }, { "field_2", field2 }, { "field_3", field3 }, { "url_string", gifUrl }, { "color", color } });

                if (updateCount > 0)
                {
                    string message = await LanguageManager.GetTranslation("dataSaved", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = message);
                }
                else
                {
                    string errorMessage2 = await LanguageManager.GetTranslation("saveDataError", modal.User.Id);
                    await modal.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                    await Utilities.SendDevLogMessage(1, $"Channel rules could not be updated. Guild {(ulong)modal.GuildId} and channel {Convert.ToUInt64(splitedCustomId[2])}.");
                }
            }

            Utilities.tempColorChoises.TryRemove(modal.User.Id, out _);
            Utilities.tempGifChoises.TryRemove(modal.User.Id, out _);
        }
    }
}
