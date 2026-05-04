
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the <paramref name="guild language"/> command.
    /// </summary>
    internal class SetGuildLanguage : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetGuildLanguage() : base("guild", "language", "command_guild_language") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildData == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string language = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            await LanguageManager.SetSystemLanguage(language, (ulong)command.GuildId);

            await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("dataSaved", command.User.Id));
        }
    }
}
