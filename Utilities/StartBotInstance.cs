
using Discord;
using Discord.Net;
using Discord.WebSocket;

using System;
using System.Timers;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Register bot token and client for API and doing setups.
    /// </summary>
    internal class StartBotInstance
    {
        internal static ulong botClientId = Configurations.BotClientId;
        internal static DiscordSocketClient _client;



        /// <summary>
        /// This is the async main thread for the bot.
        /// </summary>
        internal async Task MainAsync()
        {
            var _config = new DiscordSocketConfig
            {
                MessageCacheSize = 5000,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers | GatewayIntents.MessageContent
            };

            var token = Configurations.BotToken;
            if (token == "" || botClientId == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("MISSING CONFIG DATA! You need to add configuration information in Configurations.cs!");
                Console.ResetColor();
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
                return;
            }

            _client = new DiscordSocketClient(_config);

            #region HANDLER
            _client.Ready   += BotSetup;
            _client.Log     += Utilities.ConsolePrint;

            Console.CancelKeyPress += async (sender, e) =>
            {
                e.Cancel = true;
                await ShutdownAsync();
            };

            _client.ModalSubmitted          += ModalManager.ModalSubmittedHandler;
            _client.ButtonExecuted          += ButtonManager.ButtonExecutedHandler;
            _client.SelectMenuExecuted      += SelectMenuManager.SelectMenuExecutedHandler;
            _client.SlashCommandExecuted    += CommandManager.SlashCommandExecutedHandler;

            _client.UserJoined          += UserManager.UserJoinedHandler;
            _client.UserLeft            += UserManager.UserLeftHandler;
            //_client.ReactionAdded     += MessageManager.ReactionAddedHandler;
            _client.MessageReceived     += MessageManager.MessageReceivedHandler;
            _client.MessageDeleted      += MessageManager.MessageDeleteHandler;
            _client.JoinedGuild         += GuildManager.JoinedGuildHandler;
            _client.ChannelDestroyed    += ChannelManager.ChannelDestroyed;
            #endregion

            #region TIMER
            TimerManager timerManager = new TimerManager();
            Timer aTimer = new Timer();

            aTimer.Elapsed += timerManager.OnTimedEvent;
            aTimer.Interval = 55000;
            aTimer.Enabled = true;
            #endregion

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            Console.WriteLine($"{Utilities.resourceNameLogo}");

            await KeepAliveProcessConsoleInput();
        }



        /// <summary>
        /// Running a while loop to keep the bot alive.<br/>
        /// Also running the console reader to process any input.
        /// </summary>
        private static async Task KeepAliveProcessConsoleInput()
        {
            Utilities utilities = new Utilities();

            while (true)
            {
                string input = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input)) continue;


                if (input.StartsWith("disconnect") || input.StartsWith("leave"))
                {
                    ulong guildId = Convert.ToUInt64(input.Split(' ')[1]);

                    SocketGuild guild = _client.GetGuild((ulong)guildId);
                    await guild.LeaveAsync();

                    Console.WriteLine($"Left server {guild.Name} with id {guildId}.");
                }

                else if (input == "status" || input == "stat")
                    utilities.SendStatus();

                else if (input == "close" || input == "exit" || input == "quit" || input == "end")
                {
                    await ShutdownAsync();
                }

                else if (input.StartsWith("update command") || input.StartsWith("command update") || input.StartsWith("new command") || input.StartsWith("command new") ||
                    input.StartsWith("add command") || input.StartsWith("command add") || input.StartsWith("command edit") || input.StartsWith("edit command"))
                    await utilities.UpdateCommand(input.Split(' ')[2]);

                else if (input.StartsWith("delete command") || input.StartsWith("command delete") || input.StartsWith("remove command") || input.StartsWith("command remove"))
                    await utilities.DeleteCommand(input.Split(' ')[2]);

                else if (input.StartsWith("update guild command") || input.StartsWith("update guildcommand") || input.StartsWith("guildcommand update") || input.StartsWith("guild command update")
                    || input.StartsWith("new guildcommand") || input.StartsWith("new guild command") || input.StartsWith("guildcommand new") || input.StartsWith("guild command new"))
                    await utilities.UpdateGuildCommand(input);

                else if (input.StartsWith("halloween start"))
                    await HalloweenManager.StartHalloweenOnAllGuilds();

                else
                    Console.WriteLine("Invalid command.");

                await Task.Delay(10);
            }
        }

        /// <summary>
        /// Fired when <seealso cref="DiscordSocketClient.Ready"/> is executed.<para/>
        /// Functions:<br/>
        /// - Register commands.<br/>
        /// - Register buttons.<br/>
        /// - Register modals.<br/>
        /// - Register reaction messages.<br/>
        /// - Register select menus.<para/>
        /// - Load Languages.<br/>
        /// - Load word filter.
        /// </summary>
        private async Task BotSetup()
        {
            try
            {
                CommandManager.SetupCommands();
                ButtonManager.SetupButtons();
                ModalManager.SetupModals();
                MessageManager.SetReactionMessageList();
                SelectMenuManager.SetupSelectMenus();
                LanguageManager.LoadLanguages();

                BlockedTextManager blacklistManager = new BlockedTextManager();
                await blacklistManager.LoadSettingsAndContent();

                await ReminderManager.LoadPublicReminder();
                //await ReminderManager.LoadPrivateReminder();
                await InventoryManager.LoadItems();
                await TimerManager.FetchTempVoiceData();
            }
            catch (HttpException exception)
            {
                await Utilities.SendDevLogMessage(1, $"# Program, BotSetup\nException:\n{exception}");
                return;
            }
        }

        /// <summary>
        /// Shutdown function. Cancel cmd window or use shutdown console command.
        /// </summary>
        internal static async Task ShutdownAsync()
        {
            if (_client != null)
            {
                await _client.LogoutAsync();
                await _client.StopAsync();
            }

            Console.WriteLine("Bot wird heruntergefahren...");

            Environment.Exit(0);
        }
    }
}
