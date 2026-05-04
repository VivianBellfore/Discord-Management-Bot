
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and executing the global <paramref name="winter"/> command.<para/>
    /// </summary>
    internal class WinterCommands : SlashCommand
    {
        /// <summary>
        /// Builder for a global command called <paramref name="winter"/>.<br/>
        /// Constructor executed by:
        /// <seealso cref="CommandManager.SetupCommands"/><para/>
        /// </summary>
        internal WinterCommands(string name, string description) : base(name, description)
        {
            new OpenAdventDoor();
            AddOption(new SlashCommandOptionBuilder().WithName("advent").WithDescription("Open an advent door for this day.").WithType(ApplicationCommandOptionType.SubCommand));

            new DoWinterWork();
            AddOption(new SlashCommandOptionBuilder().WithName("work").WithDescription("Do some work to earn winter points.").WithType(ApplicationCommandOptionType.SubCommand));
        }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task OnCommandExecute(SocketSlashCommand command)
        {
            bool isCommandAllowed = await CheckConditions(command);
            if (!isCommandAllowed) return;

            try
            {
                CommandObject cmd = CommandObject.commandObjectList.First(x => x.GroupName == command.Data.Name && x.Name == command.Data.Options.First().Name);
                await cmd.CommandFunction(command);
            }
            catch (Exception exceptionMessage)
            {
                await Utilities.SendDevLogMessage(1, $"Command was not fetched from commandObjectList.\n{exceptionMessage}");
                await CommandManager.CommandInvalid(command);
            }
        }

        /// <summary>
        /// Checking guild settings and permissions for using user commands.<para/>
        /// Has TOS not accepted = false<br/>
        /// No guild data = true<br/>
        /// Not gated community = true<br/>
        /// Socket user not found = false<br/>
        /// If gated and user is member = true<br/>
        /// If gated and user not member = false
        /// </summary>
        private static async Task<bool> CheckConditions(SocketSlashCommand command)
        {
            if ( await PermissionManager.HasUserAcceptTos(command.User.Id) == false)
            {
                string errorMessage = await LanguageManager.GetTranslation("needToBeRegistered", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return false;
            }           

            GuildObject guildObject = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildObject == null) return true;

            if (guildObject.IsGatedCommunity == false) return true;

            SocketGuildUser user = command.User as SocketGuildUser;
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"User was null! Id was || {command.User.Id} ||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return false;
            }

            bool isMember = await PermissionManager.HasUserBotPermissionRole("member", (ulong)command.GuildId, user);
            if (isMember) return true;

            string errorMessage2 = await LanguageManager.GetTranslation("notMember", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
            return false;
        }
    }
 }
