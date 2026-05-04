
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <paramref name="guild help"/> command.
    /// </summary>
    internal class GetGuildHelp : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GetGuildHelp() : base("guild", "help", "command_guild_help") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            string language = await LanguageManager.GetUserLanguage(command.User.Id);

            string message = "";

            foreach (CommandObject obj in commandObjectList)
            {
                if (obj.GroupName == "guild")
                    message += $"\n`/guild {obj.Name}` - {await LanguageManager.GetTranslation(obj.TranslationId, command.User.Id)}";
            }

            message += $"\n\n" + await LanguageManager.GetTranslation("imprintGDPR", command.User.Id) + "\n" + await LanguageManager.GetTranslation("installationLink", command.User.Id);

            string title = await LanguageManager.GetTranslation("helpTitleGuild", command.User.Id, "", StartBotInstance._client.GetGuild((ulong)command.GuildId).Name);

            var embedBuiler = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(message)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuiler.Build(); });
        }
    }
}
