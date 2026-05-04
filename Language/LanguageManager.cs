
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Mnaging the user and guild language options.
    /// </summary>
    internal class LanguageManager
    {
        // Not everything can be automated if you add a new language!
        // You need to change some things yourself:
        // - GuildCommands: SetGatedCommunity command, add language choices into the command builder.



        /// <summary>
        /// Need to block loading on API resume connection for API on runtime.
        /// </summary>
        private static bool languagesLoaded = false;

        /// <summary>
        /// Default system language id.
        /// </summary>
		internal static string systemLanguage = Configurations.DefaultSystemLanguage;

        /// <summary>
        /// Holding user language data.<para/>
        /// </summary>
        internal static Dictionary<ulong, string> cachedUserLanguages = new Dictionary<ulong, string>();

        /// <summary>
        /// Contains all registered languages.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> languages = new Dictionary<string, Dictionary<string, string>>();



        /// <summary>
        /// Getting all langues from the assembly.
        /// </summary>
        public static void LoadLanguages()
        {
            if (languagesLoaded) return;

            Assembly assembly = Assembly.GetExecutingAssembly();

            var languageTypes = assembly.GetTypes().Where(t => t.Namespace == "LCNET_Management_Bot.Language");

            foreach (var type in languageTypes)
            {
                var field = type.GetField("LanguageDictionary", BindingFlags.Public | BindingFlags.Static);
                if (field != null && field.FieldType == typeof(Dictionary<string, string>))
                {
                    var dict = (Dictionary<string, string>)field.GetValue(null);
                    var languageName = type.Name.ToLower();

                    var duplicates = dict.GroupBy(kv => kv.Key).Where(g => g.Count() > 1).Select(g => g.Key).ToList();

                    if (duplicates.Any())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[LANGUAGE ERROR] Duplicate keys in '{languageName}':");

                        foreach (var key in duplicates)
                            Console.WriteLine($"  - {key}");

                        Console.ResetColor();
                    }

                    languages.Add(languageName, dict);
                }
            }

            languagesLoaded = true;
        }

        /// <summary>
        /// Checking if a user has selected a language and gives back the language id.<br/>
        /// </summary>
        /// <returns>int - language id</returns>
        internal static async Task<string> GetUserLanguage(ulong userId)
        {
            if (cachedUserLanguages.ContainsKey(userId))
                return cachedUserLanguages[userId];

            object language = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `language` FROM `user_profile` WHERE `user_id` = user_id",
                new Dictionary<string, object>() { { "user_id", userId } });

            if (language == null)
            {
                cachedUserLanguages.Add(userId, systemLanguage);
                return systemLanguage;
            }
            else
            {
                cachedUserLanguages.Add(userId, language.ToString());
                return language.ToString();
            }
        }

        /// <summary>
        /// Checking the user language and gives back a translation for the given text-id.<br/>
        /// Add language id and parameter after this, if you are using {0} in the translation text.
        /// </summary>
        /// <returns>string - text message</returns>
        internal static async Task<string> GetTranslation(string textId, ulong userId, string language = "_none_", params object[] args)
        {
            if ( userId != 0 )
                language = await GetUserLanguage(userId);

            Dictionary<string, string> usedLanguage;

            if (languages.ContainsKey(language))
                usedLanguage = languages[language];
            else if (languages.ContainsKey(systemLanguage))
                usedLanguage = languages[systemLanguage];
            else
                return $"Missing Translation! Text id was \"{textId}\".";

            if (usedLanguage.ContainsKey(textId))
                return args.Length > 0 ? string.Format(usedLanguage[textId], args) : usedLanguage[textId];
            else
            {
                await Utilities.SendDevLogMessage(1, $"Translation text is missing!\nText id was \"{textId}\" and user is ||{userId}||.");
                return $"[ :warning: ] Missing Translation! Id was \"{textId}\". This error was reported automatically.";
            }
        }

        /// <summary>
        /// Setting a language id for a user to the data base.
        /// </summary>
        internal static async Task SetUserLanguage(string language, ulong userId)
        {
            await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `user_profile` SET `language` = @language WHERE `user_id` = @user_id",
                new Dictionary<string, object>() { { "user_id", userId }, { "language", language } });

            if (cachedUserLanguages.ContainsKey(userId))
                cachedUserLanguages[userId] = language;
            else
                cachedUserLanguages.Add(userId, language);
        }

        /// <summary>
        /// Setting a language id for a guild to the data base.
        /// </summary>
        internal static async Task SetSystemLanguage(string language, ulong guildId)
        {
            await MySqlWrapper.SQLExecuteNonQuery(
                "UPDATE `guild_data` SET `language` = @language WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", guildId }, { "language", language } });

            systemLanguage = language;
        }
    }
}
