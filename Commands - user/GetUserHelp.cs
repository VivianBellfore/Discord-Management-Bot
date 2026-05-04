
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <paramref name="use help"/> command.<para/>
    /// </summary>
    internal class GetUserHelp : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GetUserHelp() : base("use", "help", "command_use_help") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            ulong userId = command.User.Id;
            string message = "";

            foreach ( CommandObject obj in commandObjectList )
            {
                if (obj.GroupName == "use")
                    message += $"\n`/use {obj.Name}` - {await LanguageManager.GetTranslation(obj.TranslationId, userId)}";
            }

            message += $"\n\n" + await LanguageManager.GetTranslation("imprintGDPR", userId);

            string title = await LanguageManager.GetTranslation("helpTitleUser", userId, "", StartBotInstance._client.GetGuild((ulong)command.GuildId).Name);

            var embedBuiler = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(message)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuiler.Build(); });
        }
    }
}
