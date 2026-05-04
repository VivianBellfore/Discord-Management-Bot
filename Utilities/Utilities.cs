
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Contains a varity of functions used by many other classes.
    /// </summary>
    internal class Utilities
    {
        /// <summary>
        /// An ascii artstyle title for the bot console.
        /// </summary>
        internal static string resourceNameLogo =
 @"
███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗███╗   ███╗███████╗███╗   ██╗████████╗
████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝████╗ ████║██╔════╝████╗  ██║╚══██╔══╝
██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██╔████╔██║█████╗  ██╔██╗ ██║   ██║   
██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██║╚██╔╝██║██╔══╝  ██║╚██╗██║   ██║   
██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║ ╚═╝ ██║███████╗██║ ╚████║   ██║   
╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝   ╚═╝   
                                                                                                                                           
";

        #region Randomiser
        /// <summary>
        /// Global random variable to use on any function.<br/>
        /// Is using a new seed every time.
        /// </summary>
        internal static Random random = new Random(Convert.ToInt32($"{DateTime.Now.Minute}{DateTime.Now.Second}{DateTime.Now.Millisecond}"));

        /// <summary>
        /// Contains user ids and random seeds for them.<br/>
        /// Gets resetted every hour by <seealso cref="TimerManager.OnTimedEvent"/>.
        /// </summary>
        internal static ConcurrentDictionary<ulong, Random> randomUserSeeds = new ConcurrentDictionary<ulong, Random>();

        /// <summary>
        /// Getting a new random for a user by a specific seed.
        /// </summary>
        internal static Random GetRandomByUserSeed(ulong userId)
        {
            if (randomUserSeeds.ContainsKey(userId))
                return randomUserSeeds[userId];

            randomUserSeeds.TryAdd(userId, new Random(Convert.ToInt32($"{DateTime.Now.Minute}{DateTime.Now.Second}{DateTime.Now.Millisecond}")));

            return randomUserSeeds[userId];
        }
        #endregion


        /// <summary>
        /// Contains data from RatelimitChecker request options.
        /// </summary>
        internal static RequestOptions requestOptions = new RequestOptions() { RatelimitCallback = RatelimitChecker};

        /// <summary>
        /// Sending rate limit request data into bot console.
        /// </summary>
        internal static async Task RatelimitChecker(IRateLimitInfo info)
        {
            Console.WriteLine($"{info.IsGlobal} {info.Limit} {info.Remaining} {info.RetryAfter} {info.Reset} {info.ResetAfter} {info.Bucket} {info.Lag} {info.Endpoint}");
        }

        #region temporarily stored
        /// <summary>
        /// Saves a color for an embed creation with user id as key.
        /// </summary>
        internal static ConcurrentDictionary<ulong, string> tempColorChoises = new ConcurrentDictionary<ulong, string>();

        /// <summary>
        /// Saves a gif url for an embed creation with user id as key.
        /// </summary>
        internal static ConcurrentDictionary<ulong, string> tempGifChoises = new ConcurrentDictionary<ulong, string>();

        /// <summary>
        /// Saves a string for an modal creation with user id as key.
        /// </summary>
        internal static ConcurrentDictionary<ulong, string> tempSettingChoises = new ConcurrentDictionary<ulong, string>();
        #endregion



        #region Console commands
        /// <summary>
        /// Triggered by console input and will send a status message back to console.
        /// </summary>
        internal void SendStatus()
        {
            IReadOnlyCollection<SocketGuild> connectedGuilds = StartBotInstance._client.Guilds;

            if ( connectedGuilds == null || connectedGuilds.Count == 0)
            {
                Console.WriteLine("No connected guilds found!");
                return;
            }

            string message = $"[{DateTime.Now.ToShortTimeString()}] Connected server:\n";

            foreach (SocketGuild guild in connectedGuilds)
            {
                string guildName = "not found";
                string ownerName = "not found";
                string memberCount = "not found";

                if (guild.Name != null)
                    guildName = guild.Name;

                ownerName = guild.OwnerId.ToString();

                if (guild.MemberCount > 0)
                    memberCount = guild.MemberCount.ToString();
                message = message + $"- {guild.Name} ({guild.Id}), owner: {ownerName} with {memberCount} members.\n";
            }

            Console.WriteLine(message);
        }

        /// <summary>
        /// Triggered by console input and will update a command for commandBuilder from discord.
        /// </summary>
        internal async Task UpdateCommand(string commandName)
        {
            if (SlashCommand.slashCommandList.Where(cmd => cmd.Name == commandName).Any() == false)
            {
                Console.WriteLine($"[ UpdateCommand ] Command name {commandName} is not found in our system and cant be registered or updated!");
                return;
            }

            IReadOnlyCollection<SocketApplicationCommand> commands = await StartBotInstance._client.GetGlobalApplicationCommandsAsync();
            SlashCommand myCommand = SlashCommand.slashCommandList.Where(cmd => cmd.Name == commandName).First();

            if (commands.Count <= 0 || commands.Where(cmd => cmd.Name == commandName).Any() == false)
            {
                await StartBotInstance._client.Rest.CreateGlobalCommand(myCommand.Build());
                Console.WriteLine($"[ UpdateCommand ] Command {commandName} was added.");
                return;
            }

            SocketApplicationCommand thisCommand = commands.Where(cmd => cmd.Name == commandName).First();
            await thisCommand.DeleteAsync();

            await StartBotInstance._client.Rest.CreateGlobalCommand(myCommand.Build());
            Console.WriteLine($"[ UpdateCommand ] Command {commandName} was updated.");
        }

        /// <summary>
        /// Triggered by console input and will delete a command from commandBuilder from discord.
        /// </summary>
        internal async Task DeleteCommand(string commandName)
        {
            IReadOnlyCollection<SocketApplicationCommand> commands = await StartBotInstance._client.GetGlobalApplicationCommandsAsync();

            if (commands.Count <= 0 || commands.Where(cmd => cmd.Name == commandName).Any() == false)
            {
                Console.WriteLine($"[ DeleteCommand ] Command name {commandName} is not existing!");
                return;
            }

            SocketApplicationCommand thisCommand = commands.Where(cmd => cmd.Name == commandName).First();

            await thisCommand.DeleteAsync();

            Console.WriteLine($"[ DeleteCommand ] Command {commandName} was deleted.");
        }

        /// <summary>
        /// Updating guild application commands for the dev server.
        /// </summary>
        internal async Task UpdateGuildCommand(string input)
        {
            List<string> startWith = new List<string>() { "update guild command ", "update guildcommand ", "guildcommand update ", "guild command update ", "new guildcommand ", 
                "new guild command ", "guildcommand new ", "guild command new " };

            string commandName = "";

            foreach (string line in startWith)
            {
                if ( input.StartsWith(line))
                {
                    commandName = input.Substring(line.Length);
                }
            }

            if (SlashCommand.slashCommandList.Where(cmd => cmd.Name == commandName).Any() == false)
            {
                Console.WriteLine($"[ UpdateCommand ] Command name {commandName} is not found in our system and cant be registered or updated!");
                return;
            }

            SocketGuild devGuild = StartBotInstance._client.GetGuild(679750021406785549);

            IReadOnlyCollection<SocketApplicationCommand> commands = await devGuild.GetApplicationCommandsAsync();
            SlashCommand myCommand = SlashCommand.slashCommandList.Where(cmd => cmd.Name == commandName).First();

            if (commands.Count <= 0 || commands.Where(cmd => cmd.Name == commandName).Any() == false)
            {
                await devGuild.CreateApplicationCommandAsync(myCommand.Build());
                Console.WriteLine($"[ UpdateCommand ] Command {commandName} was added.");
                return;
            }

            SocketApplicationCommand thisCommand = commands.Where(cmd => cmd.Name == commandName).First();
            await thisCommand.DeleteAsync();

            await devGuild.CreateApplicationCommandAsync(myCommand.Build());
            Console.WriteLine($"[ UpdateCommand ] Command {commandName} was updated.");
        }

        /// <summary>
        /// Will print Discord LogMessages to the console.
        /// </summary>
        internal static Task ConsolePrint(LogMessage msg)
        {
            // dont send anoying intents message...
            if (msg.ToString().Contains("You're using the GuildScheduledEvents") || msg.ToString().Contains("You're using the GuildInvites"))
                return Task.CompletedTask;

            // useless reconnect error message...
            if (msg.ToString().Contains("Discord.WebSocket.GatewayReconnectException") || msg.ToString().Contains("System.Net.WebSockets.WebSocketException"))
                return Task.CompletedTask;

            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        #endregion



        #region Messages
        /// <summary>
        /// Sending embeds to log channels on the developer server.<para/>
        /// Log types are:<br/>
        /// 1 = Error<br/>
        /// 2 = Warning<br/>
        /// 3 = User action
        /// </summary>
        internal static async Task SendDevLogMessage(int logChannelType, string message, [CallerMemberName] string memberName = "", 
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            ulong devGuildId = Configurations.DevGuildId;
            ulong errorLogs = Configurations.ErrorLogChannel;
            ulong userActions = Configurations.UserActionLogChannel;

            if (devGuildId == 0 || errorLogs == 0 || userActions == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("MISSING CONFIG DATA! You need to add configuration information in Configurations.cs!");
                Console.ResetColor();
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
                return;
            }

            SocketGuild guild = StartBotInstance._client.GetGuild(devGuildId);
            if (guild == null)
            {
                Console.WriteLine($"[ SendDevLogMessage ] Error, could not fetch guild for debug messages!\n{message}");
                return;
            }

            ulong logChannelId;
            string title;

            switch (logChannelType)
            {
                case 1:
                    { logChannelId = errorLogs; title = "Error"; }
                    break;
                case 2:
                    { logChannelId = userActions; title = "User action"; }
                    break;
                default:
                    { logChannelId = errorLogs; title = "Internal error, wrong log channel type!"; }
                    break;
            }

            ITextChannel textChannel = guild.GetChannel(logChannelId) as ITextChannel;

            if (textChannel == null)
            {
                Console.WriteLine($"[ SendDevLogMessage ] Error, could not fetch channel for debug message! Message was: {message}");
                return;
            }

            string className = System.IO.Path.GetFileNameWithoutExtension(filePath);

            if (title == "Error")
            {
                if (message.Length > 1900)
                    await SendMessageForLongText($"# **{className}, {memberName}:{lineNumber}**\n{message}", textChannel);
                else
                    await textChannel.SendMessageAsync($"# **{className}, {memberName}:{lineNumber}**\n{message}");
            }
            else
            {
                if (message.Length > 3900)
                    await SendEmbedForLongText($"# **{className}, {memberName}:{lineNumber}**\n{message}", textChannel, Color.Red);
                else
                {
                    var embedBuilder = new EmbedBuilder()
                        .WithTitle(title)
                        .WithDescription($"# **{className}, {memberName}:{lineNumber}**\n{message}")
                        .WithColor(Color.Red);

                    await textChannel.SendMessageAsync(embed: embedBuilder.Build());
                }
            }
        }

        /// <summary>
        /// Splitting up text messages and sending them seperate if the text is still to long for a discord message (2000 character).<para/>
        /// The message is send as open message, ephermals are not supported.
        /// </summary>
        internal static async Task SendMessageForLongText(string messageText, ITextChannel channel)
        {
            string[] subStrings = messageText.Split(' ');

            string textToSend = string.Empty;
            string overflow = string.Empty;

            // Textmessages can be 2000 character long, we build up a safezone of 100 character.
            foreach (string word in subStrings)
            {
                if (textToSend.Length < 1900)
                    textToSend = $"{textToSend} {word}";
                else
                    overflow = $"{overflow} {word}";
            }

            await channel.SendMessageAsync(textToSend);

            if (overflow.Length > 0)
                await SendMessageForLongText(overflow, channel);
        }

        /// <summary>
        /// Splitting up text messages and sending them seperate as embed if the text is still to long for a discord embed (4000 character).<para/>
        /// The embed is send as open message, ephermals are not supported. Can contain 5900 chars per embed.
        /// </summary>
        internal static async Task SendEmbedForLongText(string embedMainText, ITextChannel channel, Color color)
        {
            int MaxEmbedLength = 5900;
            int MaxDescriptionLength = 3900;
            int MaxFieldLength = 900;
            int MaxFields = 3;

            List<string> words = embedMainText.Split(' ').ToList();
            int totalLength = 0;

            string description = ExtractTextBlock(ref words, MaxDescriptionLength, ref totalLength);

            var embedBuilder = new EmbedBuilder()
                .WithDescription(description)
                .WithColor(color);

            for (int i = 0; i < MaxFields && words.Count > 0; i++)
            {
                if (totalLength >= MaxEmbedLength)
                    break;

                string fieldText = ExtractTextBlock(ref words, MaxFieldLength, ref totalLength);
                if (!string.IsNullOrWhiteSpace(fieldText))
                    embedBuilder.AddField("\u200B", fieldText, false);
            }

            await channel.SendMessageAsync(embed: embedBuilder.Build());

            if (words.Count > 0)
            {
                string remainingText = string.Join(" ", words);
                await SendEmbedForLongText(remainingText, channel, color);
            }
        }

        /// <summary>
        /// Extracts a block of text from the list of words, up to the max length, updating the total length counter.
        /// </summary>
        private static string ExtractTextBlock(ref List<string> words, int maxLength, ref int totalLength)
        {
            List<string> blockWords = new List<string>();
            int currentLength = 0;

            while (words.Count > 0)
            {
                string nextWord = words[0];
                int nextLength = nextWord.Length + 1; // +1 for space

                if ((currentLength + nextLength > maxLength) || (totalLength + nextLength > 5900))
                    break;

                blockWords.Add(nextWord);
                currentLength += nextLength;
                totalLength += nextLength;
                words.RemoveAt(0);
            }

            return string.Join(" ", blockWords);
        }
        #endregion



        /// <summary>
        /// Calculating a user level from there account points.
        /// </summary>
        internal static int CalculateLevelFromPoints(int points)
        {
            return (int)Math.Floor(Math.Sqrt(points) * 0.3f - 4f);
        }

        /// <summary>
        /// Checking if url is valid and reachable with Uri.TryCreate().
        /// </summary>
        internal static bool ValidateUrlWithUri(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Parse a string like "green" or "darkgreen" into a Discord.Color object.
        /// </summary>
        internal static async Task<Color> GetColor(string colorName)
        {
            var normalized = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(colorName.Trim().ToLower());

            var colorField = typeof(Color)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(f => f.Name.Equals(normalized, StringComparison.OrdinalIgnoreCase));

            if (colorField != null && colorField.FieldType == typeof(Color))
                return (Color)colorField.GetValue(null);

            await SendDevLogMessage(1, $"The color {colorName} was invalid.");

            return Color.Default;
        }



        /// <summary>
        /// Returns null if no valid SocketGuild was found.
        /// </summary>
        public static SocketGuild GetGuildSocket(ulong guildId)
        {
            ulong? validGuildId = guildId;

            if (validGuildId == null)
                return null;

            SocketGuild guild = StartBotInstance._client.GetGuild(guildId);

            return guild;
        }



        /// <summary>
        /// Using a regex to match the date formate.
        /// </summary>
        internal static bool CheckFormateDate(string timeString)
        {
            var regex = @"^[0-3][0-9]/[0-1][0-9]/202[4-5]$";
            Match match = Regex.Match(timeString, regex);

            if (match.Success)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if time format is "00:00".
        /// </summary>
        internal static bool CheckTimeFormate(string timeString)
        {
            var regex = @"^[0-2][0-9]:[0-6][0-9]$";
            Match match = Regex.Match(timeString, regex);

            if (match.Success)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a string is a valid week day and translate it to english.
        /// </summary>
        internal static (bool isCorrect, string englishDay) CheckFormateWeekday(string timeString)
        {
            List<string> dayNames = new List<string>()
            {
                "montag", "dienstag", "mittwoch", "donnerstag", "freitag", "samstag", "sonntag",
                "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"
            };

            bool isCorrect = false;
            if (dayNames.Contains(timeString.ToLower()))
                isCorrect = true;

            string englishDay = timeString.ToLower();
            switch (timeString.ToLower())
            {
                case "montag":
                    englishDay = "monday";
                    break;

                case "dienstag":
                    englishDay = "tuesday";
                    break;

                case "mittwoch":
                    englishDay = "wednesday";
                    break;

                case "donnerstag":
                    englishDay = "thursday";
                    break;

                case "freitag":
                    englishDay = "friday";
                    break;

                case "samstag":
                    englishDay = "saturday";
                    break;

                case "sonntag":
                    englishDay = "sunday";
                    break;
            }

            return (isCorrect, englishDay);
        }

        /// <summary>
        /// Checks the duration length.
        /// </summary>
        internal static int CheckFormateDuration(string durationString)
        {
            if (Int32.TryParse(durationString, out int result))
            {
                if (result > 10 || result < 2) // Reminder must be positive and no more then a week long.
                    return -1;
                else
                    return result;
            }
            else
                return -1;
        }
    }



    /// <summary>
    /// Contains all data needed to build an embed.
    /// </summary>
    internal class EmbedObject
    {
        internal Color Color { get; set; } = Color.Default;
        internal string ImageURL { get; set; } = "";
        internal string Titel { get; set; } = "";
        internal string Description { get; set; } = "";
        internal string Field_1 { get; set; } = "";
        internal string Field_2 { get; set; } = "";
        internal string Field_3 { get; set; } = "";
        internal string MessageContent { get; set; } = "";
    }
}
