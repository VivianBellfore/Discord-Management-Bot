
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="points"/> command.
    /// </summary>
    internal class SetUserGuildPoints : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal SetUserGuildPoints() : base("admin", "points", "command_admin_points") { }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            string changeType = command.Data.Options.First().Options.ElementAt(0).Value.ToString();
            long points = Convert.ToInt64(command.Data.Options.First().Options.ElementAt(1).Value);
            SocketGuildUser user = (SocketGuildUser)command.Data.Options.First().Options.ElementAt(2).Value;

            bool isUserRegistered = await PermissionManager.HasUserAcceptTos(user.Id);
            if (!isUserRegistered)
            {
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("pickedUserNotMember", command.User.Id));
                return;
            }

            bool pointsChanged;
            GuildObject guild = await GuildManager.GetGuildData((ulong)command.GuildId);

            if ( changeType == "add") 
                pointsChanged = await UserManager.SetUserGuildPoints(user.Id, (ulong)command.GuildId, points, true);
            else
                pointsChanged = await UserManager.SetUserGuildPoints(user.Id, (ulong)command.GuildId, points, false);

            if (pointsChanged)
            {
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("dataSaved", command.User.Id));

                if (guild != null && guild.LogChannel != 0)
                {
                    string title = await LanguageManager.GetTranslation("logTitleGuildPointsChanged", 0, guild.Language);
                    string message = await LanguageManager.GetTranslation("logMessageGuildPointsChanged", 0, guild.Language, command.User.GlobalName, user.GlobalName, changeType, points);
                    await GuildManager.SendSystemMessageToGuild((ulong)command.GuildId, 0, title, message);
                }
            }      
            else
                await command.ModifyOriginalResponseAsync(async func => func.Content = await LanguageManager.GetTranslation("saveDataError", command.User.Id));
        }
    }
}
