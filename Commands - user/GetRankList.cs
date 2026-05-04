
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class is building and managing the <paramref name="use ranks"/> command.
    /// </summary>
    internal class GetRankList : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal GetRankList() : base("use", "ranks", "command_use_ranks") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            await command.ModifyOriginalResponseAsync(async func => { func.Content = await LanguageManager.GetTranslation("ranksCheckingUser", command.User.Id); });

            List<dynamic> rankUsers = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `user_guild_points` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)command.GuildId } });

            Dictionary<ulong, int> ranks = new Dictionary<ulong, int>();

            IGuild guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);

            if (rankUsers.Count <= 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("noRanksFound", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }
                
            foreach (dynamic rankUser in rankUsers)
            {
                IGuildUser user = await guild.GetUserAsync(rankUser.user_id);
                if ( user != null)
                    ranks.Add(rankUser.user_id, Convert.ToInt32(rankUser.points));
            }

            string text = await SortingRankList(command, ranks);

            var embedBuilder = new EmbedBuilder()
                .WithDescription(text)
                .WithColor(Color.Orange);

            await command.ModifyOriginalResponseAsync(func => { func.Embed = embedBuilder.Build(); func.Content = ""; });
        }

        /// <summary>
        /// This function is sorting a list of users by there points and is creating a string with the top 10.
        /// </summary>
        private static async Task<string> SortingRankList(SocketSlashCommand command, Dictionary<ulong, int> ranks)
        {
            string result = string.Empty;

            var sorted = from rank in ranks orderby rank.Value descending select rank;

            if (sorted.Count() <= 0)
            {
                result = await LanguageManager.GetTranslation("noRanksFound", command.User.Id);
                return result;
            }

            for (int i = 0; i < 10; i++)
            {
                if (i >= sorted.Count())
                    break;

                ulong userId = sorted.ElementAt(i).Key;
                int points = sorted.ElementAt(i).Value;
                int level = Utilities.CalculateLevelFromPoints(points);

                if (level < 0)
                    level = 0;

                result = result + await LanguageManager.GetTranslation("rankListText", command.User.Id, "", (i + 1), userId, level, points);
            }

            return result;
        }
    }
}
