
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="rules"/> command.
    /// </summary>
    internal class GetChannelRules : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GetChannelRules() : base("use", "rules", "command_use_rules") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            IChannel channel = command.Data.Options.First().Options.ElementAt(0).Value as IChannel;

            if (channel == null)
            {
                await Utilities.SendDevLogMessage(1, $"The channel was null!\nGuild id is {(ulong)command.GuildId} and user was || {command.User.Id} ||.");
                string errorMessage = await LanguageManager.GetTranslation("channelReadError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `guild_channel_rules` WHERE `guild_id` = @guild_id AND `channel_id` = @channel_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId }, { "channel_id", channel.Id } });

            if (results == null || results.Count <= 0)
            {
                string message = await LanguageManager.GetTranslation("noRulesFound", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = message);
                return;
            }

            await SendRulesEmbed(command, results);
        }

        private async Task SendRulesEmbed(SocketSlashCommand command, List<dynamic> results)
        {
            string title = results[0].title;
            string text = results[0].text;
            string field1 = results[0].field_1;
            string field2 = results[0].field_2;
            string field3 = results[0].field_3;

            Color color = await Utilities.GetColor(results[0].color);

            string gifUrl = results[0].url_string;
            ulong channelId = results[0].channel_id;

            var embedBuilder = new EmbedBuilder()
                    .WithTitle(title)
                    .WithDescription(text)
                    .WithColor(color)
                    .WithImageUrl(gifUrl);

            int totalLength = title.Length + text.Length;

            string[] fieldTexts = new[] { field1, field2, field3 };

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
                string errorMessage = await LanguageManager.GetTranslation("embedToLong", command.User.Id, "", channelId);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuilder.Build(); }); 
        }
    }
}
