
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Contains user related functions and handler.
    /// </summary>
    internal class UserManager
    {
        #region Intents
        /// <summary>
        /// Triggered when a user is leaving a server.
        /// </summary>
        internal static async Task UserLeftHandler(SocketGuild guild, SocketUser user)
        {
            if ( user.IsBot ) return;

            GuildObject guildObject = await GuildManager.GetGuildData(guild.Id);
            if (guildObject == null || guildObject.LogChannel == 0) return;

            string message = await LanguageManager.GetTranslation("userLeftGuild", 0, guildObject.Language, user.GlobalName, user.Id);
            string title = await LanguageManager.GetTranslation("userLeftGuildTitle", 0, guildObject.Language);

            await GuildManager.SendSystemMessageToGuild(guild.Id, 0, title, message);
        }

        /// <summary>
        /// Triggered when a user is joining a server.
        /// </summary>
        internal static async Task UserJoinedHandler(SocketGuildUser user)
        {
            GuildObject guildObject = await GuildManager.GetGuildData(user.Guild.Id);
            if (guildObject == null || guildObject.LogChannel == 0) return;

            List<dynamic> userReports = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `reports` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", user.Id } });

            if (userReports.Count == 0) return;

            string message = await LanguageManager.GetTranslation("userReportTitle", 0, guildObject.Language, user.Id);
            foreach (dynamic report in userReports)
            {
                message += $"- **{report.date}** {report.reason} - {report.comment}\n";
            }

            string title = await LanguageManager.GetTranslation("userReportEmbedTitle", 0, guildObject.Language);
            await GuildManager.SendSystemMessageToGuild(user.Guild.Id, 0, title, message);
        }
        #endregion


        /// <summary>
        /// Adding a new user to the `user_profile` table in the data base.
        /// </summary>
        internal static async Task<bool> RegisterUser(ulong userId, string language)
        {
            if (await PermissionManager.HasUserAcceptTos(userId))
                return true;

            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `user_profile` (`user_id`, `language`) VALUES (@user_id, @language)",
                new Dictionary<string, object>() { { "user_id", userId }, { "language", language } });

            if (insertCount <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"User was not registered! User id is {userId} and language was {language}.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adding or removing points to existing user guild points or adding new entry to data base.<para/>
        /// <paramref name="pointsAdded"/> "true" to add points.<br/>
        /// <paramref name="pointsAdded"/> "false" to remove points.<para/>
        /// Guild points can be negative.
        /// </summary>
        internal static async Task<bool> SetUserGuildPoints(ulong userId, ulong guildId, long points, bool pointsAdded)
        {
            object currentAmount = await MySqlWrapper.SQLExecuteScalar(
                $"SELECT `points` FROM `user_guild_points` WHERE `user_id` = @user_id AND `guild_id` = @guildId",
                new Dictionary<string, object>() { { "user_id", userId }, { "guildId", guildId } });

            if ( currentAmount == null )
            {
                if (!pointsAdded)
                    points = -points;

                int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                    "INSERT INTO `user_guild_points` (`user_id`, `guild_id`, `points`) VALUES (@user_id, @guild_id, @points)",
                    new Dictionary<string, object>() { { "user_id", userId }, { "guild_id", guildId }, { "points", points } });

                if (insertCount >= 0) return true;
                
                return false;
            }

            long newAmount;

            if ( pointsAdded )
                newAmount = Convert.ToInt64(currentAmount) + points;
            else
                newAmount = Convert.ToInt64(currentAmount) - points;

            int updateCount = await MySqlWrapper.SQLExecuteNonQuery(
                $"UPDATE `user_guild_points` SET `points` = @points WHERE `user_id` = @user_id AND `guild_id` = @guildId",
                new Dictionary<string, object>() { { "points", newAmount }, { "user_id", userId }, { "guildId", guildId } });

            if (updateCount >= 0) return true;
            
            return false;
        }

        /// <summary>
        /// Fetching user data from data base and building a UserObject.
        /// </summary>
        internal static async Task<UserObject> GetUserData(ulong userId)
        {
            UserObject userObject = new UserObject();

            List<dynamic> userProfile = await MySqlWrapper.SQLExecuteReader(
                $"SELECT * FROM `user_profile` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (userProfile == null || userProfile.Count <= 0)
                return userObject;

            userObject.UserId = userId;
            userObject.Points = userProfile.First().points;
            userObject.WinterPoints = userProfile.First().winter_points;
            userObject.HalloweenDate = DateTime.Parse(userProfile.First().halloween_date);
            userObject.HalloweenAction = DateTime.Parse(userProfile.First().halloween_action);
            userObject.HalloweenProtection = DateTime.Parse(userProfile.First().halloween_protection);

            List<dynamic> guildPoints = await MySqlWrapper.SQLExecuteReader(
                $"SELECT * FROM `user_guild_points` WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (guildPoints != null || guildPoints.Count > 0)
            {
                foreach ( dynamic p in guildPoints)
                {
                    userObject.GuildPoints.Add(Convert.ToUInt64(p.guild_id), Convert.ToInt32(p.points));
                }
            }

            return userObject;
        }
    }



    /// <summary>
    /// An object that contains user data.<para/>
    /// Cant be null but the user id can be 0 if no data was found for the user.
    /// </summary>
    internal class UserObject
    {
        internal ulong UserId { get; set; } = 0;
        internal string Language { get; set; } = Configurations.DefaultUserLanguage;
        internal int Points { get; set; } = 0;
        internal int WinterPoints { get; set; } = 0;
        internal DateTime HalloweenDate { get; set; } = DateTime.Parse("2025-09-25T14:15:00");
        internal DateTime HalloweenAction { get; set; } = DateTime.Parse("2025-09-25T14:15:00");
        internal DateTime HalloweenProtection { get; set; } = DateTime.Parse("2025-09-25T14:15:00");
        internal Dictionary<ulong, int> GuildPoints { get; set; } = new Dictionary<ulong, int>();
    }
}
