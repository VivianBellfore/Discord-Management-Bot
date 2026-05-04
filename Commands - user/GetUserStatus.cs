
using Discord;
using Discord.WebSocket;

using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <b>"use stat"</b> command.<para/>
    /// </summary>
    internal class GetUserStatus : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GetUserStatus() : base("use", "stat", "command_use_stat") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            SocketGuildUser user = (SocketGuildUser)command.User;

            if (command.Data.Options.First().Options.Count > 0)
                user = (SocketGuildUser)command.Data.Options.First().Options.ElementAt(0).Value;

            UserObject userData = await UserManager.GetUserData(user.Id);

            string message = await LanguageManager.GetTranslation("userStatusMessage", command.User.Id, "", userData.Points, userData.WinterPoints, user.Id);
            string guildPoints;

            if ( userData.GuildPoints.Count == 0)
                guildPoints = await LanguageManager.GetTranslation("userStatusNoGuildPoints", command.User.Id);
            else
            {
                guildPoints = await LanguageManager.GetTranslation("userStatusGuildPointsCaption", command.User.Id);

                foreach (var guild in userData.GuildPoints)
                {
                    GuildObject thisGuild = await GuildManager.GetGuildData(guild.Key);
                    guildPoints = guildPoints + await LanguageManager.GetTranslation("userStatusGuildPoints", command.User.Id, "", thisGuild.GuildName, thisGuild.PointsName, guild.Value.ToString());
                }
            }

            var embedBuilder = new EmbedBuilder()
                .WithDescription(message + guildPoints)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Embed = embedBuilder.Build(); func.Content = ""; }); 
        }
    }
}
