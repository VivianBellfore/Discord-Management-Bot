
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and managing the "/fact <paramref name="member"/>" command.<para/>
    /// </summary>
    internal class GetMemberList : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal GetMemberList() : base("fact", "member", "command_fact_member") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            int factionId = Convert.ToInt32(command.Data.Options.First().Options.ElementAt(0).Value);

            FactionObject faction = await FactionManager.GetFactionData(factionId);
            if (faction == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("factionIdDoesNotExist", command.User.Id, "", factionId);
                await command.ModifyOriginalResponseAsync(func => { func.Content = errorMessage; });
                return;
            }

            bool hasPermission = false;

            if (await PermissionManager.HasUserBotPermissionRole("admin", (ulong)command.GuildId, (SocketGuildUser)command.User) == true)
                hasPermission = true;

            if (await PermissionManager.IsUserGuildOwner((ulong)command.GuildId, command.User.Id) == true)
                hasPermission = true;

            (bool isFactionMember, string reasonTranslationId) = await FactionManager.IsUserFactionMember(command.User.Id, faction.Id);
            if (isFactionMember)
                hasPermission = true;

            if (!hasPermission)
            {
                string errorMessage = await LanguageManager.GetTranslation(reasonTranslationId, command.User.Id);
                await command.ModifyOriginalResponseAsync(func => { func.Content = errorMessage; });
                return;
            }

            string message = await LanguageManager.GetTranslation("factionMemberListOwner", command.User.Id, "", faction.OwnerId);
            string rankTranslation = await LanguageManager.GetTranslation("rank", command.User.Id);

            foreach (var member in faction.Member)
            {
                message += $"- <@{member.Key}> {rankTranslation} {member.Value}\n";
            }

            string title = await LanguageManager.GetTranslation("factionMemberListTitle", command.User.Id, "", faction.Name);

            var embedBuiler = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(message)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Content = ""; func.Embed = embedBuiler.Build(); });
        }
    }
}
