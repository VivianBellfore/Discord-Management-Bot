
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="guildlist"/> command.
    /// </summary>
    internal class GetGuildFactions : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal GetGuildFactions() : base("fact", "guildlist", "command_fact_guildlist") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            if (await PermissionManager.HasUserBotPermissionRole("mod", (ulong)command.GuildId, (SocketGuildUser)command.User) == false)
            {
                if (await PermissionManager.HasUserBotPermissionRole("admin", (ulong)command.GuildId, (SocketGuildUser)command.User) == false)
                {
                    if (await PermissionManager.IsUserGuildOwner((ulong)command.GuildId, command.User.Id) == false)
                    {
                        string errorMessage = await LanguageManager.GetTranslation("missingPermisson", command.User.Id);
                        await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                        return;
                    }
                }
            }

            string message = await FactionManager.GetAllFactionsForGuild((ulong)command.GuildId);

            var embedBuiler = new EmbedBuilder()
                .WithDescription(message)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuiler.Build(); });
        }
    }
}
