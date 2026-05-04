
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is handeling all functions for the <paramref name="register"/> command.
    /// </summary>
    internal class GuildRegister : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GuildRegister() : base("guild", "register", "command_guild_register") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildData != null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("guildAlreadyRegistered", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var buttonBuilder = new ComponentBuilder();

            foreach (var language in LanguageManager.languages)
            {
                buttonBuilder.WithButton(await LanguageManager.GetTranslation("buttonAcceptBotForGuild", 0, language.Key), 
                    $"respond_guildsetup_{guildData.GuildId}_register_{language.Key}", ButtonStyle.Success);
            }

            string content = await LanguageManager.GetTranslation("guildRegisterMessage", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => { func.Content = content; func.Components = buttonBuilder.Build(); });
        }
    }
}
