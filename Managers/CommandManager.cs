
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all slash command related functions.
    /// </summary>
    internal class CommandManager
    {
        /// <summary>
        /// Need to block loading on API resume connection for API on runtime.
        /// </summary>
        private static bool commandsLoaded;


        /// <summary>
        /// Register slash commands in our system and adding them to: <seealso cref="SlashCommand.slashCommandList"/>
        /// </summary>
        internal static void SetupCommands()
        {
            if (commandsLoaded) return;

            new AdminCommands("admin", "Using server admin functions.");
            new ModCommands("mod", "Using server mod functions.");
            new GuildCommands("guild", "Change server settings.");
            new UserCommands("use", "Use bot functions.");

            new DevCommands("dev", "Commands for the dev only.");
            new FactionCommands("fact", "Commands for factions.");
            new WinterCommands("winter", "Commands for the winter event.");

            commandsLoaded = true;
        }

        /// <summary>
        /// Contains strings with command names that are using modals.<br/>
        /// We can not respond on a command that will use a modal, so we have to check this in <seealso cref="SlashCommandExecutedHandler"/> first. The modal will do the respond.
        /// </summary>
        internal static List<string> commandsWithModal = new List<string>();

        /// <summary>
        /// Handeling the SlashCommandExecuted from all slash commands incoming from the API.<para/>
        /// Functions:<br/>
        /// - Sending respond.<br/>
        /// - Checking if <seealso cref="IsCommandAllowedToUse(SocketSlashCommand)"/> is all true.<br/>
        /// - Execute <seealso cref="SlashCommand.OnCommandExecute(SocketSlashCommand)"/> function if all checks are valid.
        /// </summary>
        internal static async Task SlashCommandExecutedHandler(SocketSlashCommand command)
        {
            if (!commandsWithModal.Contains(command.Data.Options.First().Name))
                await command.RespondAsync(await LanguageManager.GetTranslation("commandLoading", command.User.Id), ephemeral: true);

            if (await IsCommandAllowedToUse(command) == false) return;

            SlashCommand cmd = SlashCommand.slashCommandList.First(c => c.Name == command.Data.Name);
            if (cmd == null) await CommandInvalid(command);

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += async (sender, e) =>
            {
                try
                {
                    await cmd.OnCommandExecute(command);
                }
                catch (Exception exceptionMessage)
                {
                    string messageText = await LanguageManager.GetTranslation("generalError", command.User.Id);

                    if (command.HasResponded)
                        await command.ModifyOriginalResponseAsync(func => func.Content = messageText);
                    else
                        await command.RespondAsync(messageText, ephemeral: true);

                    await Utilities.SendDevLogMessage(1, exceptionMessage.ToString());
                }
            };
            backgroundWorker.RunWorkerAsync();

            await Task.Delay(1);
        }

        /// <summary>
        /// Checking conditions for a user to decide if he is allowed to use a command.<para/>
        /// Conditions checked:<br/>
        /// - IsBot<br/>
        /// - IsWebhook<br/>
        /// - IsDMInteraction<br/>
        /// - <seealso cref="PermissionManager.IsUserBlockedFromBotUsage(ulong, ulong)"/><br/>
        /// - <seealso cref="PermissionManager.HasUserBotPermission(int, ulong, SocketGuildUser)"/> with permission 1.<br/>
        /// - IsUserTimedOut<br/>
        /// - Is user account on guild longer then 24 hours.
        /// </summary>
        private static async Task<bool> IsCommandAllowedToUse(SocketSlashCommand command)
        {
            if (command.User.IsBot || command.User.IsWebhook)
                return false;

            if (command.IsDMInteraction)
            {
                string message = await LanguageManager.GetTranslation("commandNotInDM", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => { func.Content = message; });
                return false;
            }

            (bool isUserBlacklisted, string blockedMessage) = await PermissionManager.IsUserBlockedFromBotUsage(command.User.Id);
            if (isUserBlacklisted)
            {
                await command.ModifyOriginalResponseAsync(func => { func.Content = blockedMessage; });
                return false;
            }

            IGuild guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
            if (guild != null)
            {
                IGuildUser guildUser = await guild.GetUserAsync(command.User.Id);

                if (guildUser != null && guildUser.TimedOutUntil.HasValue && guildUser.TimedOutUntil > DateTime.Now)
                {
                    string message = await LanguageManager.GetTranslation("functionNotWhileTimeout", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => { func.Content = message; });
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Triggered if no valid command name was given. This function is sending a response.
        /// </summary>
        internal static async Task CommandInvalid(SocketSlashCommand command)
        {
            string message = await LanguageManager.GetTranslation("commandOutdated", command.User.Id);

            if (command.HasResponded)
                await command.ModifyOriginalResponseAsync(func => { func.Content = message; });
            else
                await command.RespondAsync(message, ephemeral: true);

            await Utilities.SendDevLogMessage(1, $"Command name was outdated! No such command is registered.\nName was: /{command.Data.Name} {command.Data.Options.First().Name}");
        }
    }



    /// <summary>
    /// Register and handling slash commands for our internal system.
    /// </summary>
    internal class SlashCommand : SlashCommandBuilder
    {
        /// <summary>
        /// Contains all registered commands.
        /// </summary>
        internal static List<SlashCommand> slashCommandList = new List<SlashCommand>();

        /// <summary>
        /// Constructor for the SlashCommand.
        /// </summary>
        internal SlashCommand(string name, string description)
        {
            WithName(name);
            WithDescription(description);
            slashCommandList.Add(this);
        }

        /// <summary>
        /// Executing the command.
        /// </summary>
        internal virtual async Task OnCommandExecute(SocketSlashCommand command)
        {
            await Task.FromResult(0);
        }
    }



    /// <summary>
    /// Constructor for command objects that are setting help texts for commands.
    /// </summary>
    internal class CommandObject
    {
        /// <summary>
        /// Contains all registered command objects.
        /// </summary>
        internal static List<CommandObject> commandObjectList = new List<CommandObject>();

        internal string GroupName { get; set; }
        internal string Name { get; set; }
        internal string TranslationId { get; set; }


        internal CommandObject(string groupName, string name, string translationId)
        {
            GroupName = groupName;
            Name = name;
            TranslationId = translationId;
            commandObjectList.Add(this);
        }

        internal virtual async Task CommandFunction(SocketSlashCommand command)
        {
            await Task.FromResult(0);
        }
    }
}
