
using Discord;
using Discord.WebSocket;

using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="status"/> command.
    /// </summary>
    internal class ShowGuildStatus : CommandObject
    {
        /// <summary>
        /// Struct for the help command informations.
        /// </summary>
        internal ShowGuildStatus() : base("admin", "status", "command_admin_status") { }

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
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            SocketGuild guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
            if (guild == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string channelInfoText = "";

            foreach (var channel in guildData.GuildChannel)
            {
                channelInfoText += $"- {channel.Key} = <#{channel.Value}>\n";
            }

            string message = await LanguageManager.GetTranslation("guildStatusText", command.User.Id, "", 
                guildData.InviteLink, guildData.AdminRole, guildData.ModeratorRole, guildData.MemberRole, guildData.IsGatedCommunity, guildData.PointsName, 
                guildData.UseWordfilter, guildData.CheckDeletedMessages, guildData.TicketsActive, guild.CreatedAt, guild.Owner.GlobalName, 
                guild.Description, guild.PremiumSubscriptionCount, guild.PremiumTier, guild.PreferredCulture.Name, guild.PreferredLocale, guild.VoiceRegionId,
                guild.VerificationLevel,  guild.NsfwLevel, guild.Emotes.Count, guild.Stickers.Count, guild.Roles.Count, guild.CategoryChannels.Count, 
                guild.Channels.Count, guild.VoiceChannels.Count, guild.TextChannels.Count, guild.ForumChannels.Count, guild.ThreadChannels.Count, guild.StageChannels.Count, 
                guild.Events.Count, channelInfoText );

            var embedBuiler = new EmbedBuilder()
                .WithDescription(message)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuiler.Build(); });
        }
    }
}
