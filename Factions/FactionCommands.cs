
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and executing the global <paramref name="fact"/> command.<para/>
    /// </summary>
    internal class FactionCommands : SlashCommand
    {
        /// <summary>
        /// Builder for a global command called <paramref name="fact"/>.<br/>
        /// Constructor executed by:
        /// <seealso cref="CommandManager.SetupCommands"/><para/>
        /// </summary>
        internal FactionCommands(string name, string description) : base(name, description)
        {
            new GetFactionHelp();
            AddOption(new SlashCommandOptionBuilder().WithName("help").WithDescription("All faction commands explained.").WithType(ApplicationCommandOptionType.SubCommand));

            new AddNewFaction();
            AddOption(new SlashCommandOptionBuilder().WithName("new").WithDescription("Create a new faction.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("owner").WithDescription("Who is the owner of the faction.").WithType(ApplicationCommandOptionType.User).WithRequired(true)));

            new GetGuildFactions();
            AddOption(new SlashCommandOptionBuilder().WithName("guildlist").WithDescription("Get all factions for your server.").WithType(ApplicationCommandOptionType.SubCommand));

            new RemoveFaction();
            AddOption(new SlashCommandOptionBuilder().WithName("remove").WithDescription("Remove a faction from server.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("id").WithDescription("Get the faction id from the faction list.").WithType(ApplicationCommandOptionType.Integer).WithRequired(true)));

            new ChangeFactionOwner();
            AddOption(new SlashCommandOptionBuilder().WithName("owner").WithDescription("Change the owner of an existing faction..").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("owner").WithDescription("Who should be the new owner?").WithType(ApplicationCommandOptionType.User).WithRequired(true))
                .AddOption(new SlashCommandOptionBuilder().WithName("id").WithDescription("Get the faction id from the faction list.").WithType(ApplicationCommandOptionType.Integer).WithRequired(true)));

            new SetFactionName();
            AddOption(new SlashCommandOptionBuilder().WithName("name").WithDescription("Set the name for your faction.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("name").WithDescription("What should your faction be named?").WithType(ApplicationCommandOptionType.String).WithRequired(true))
                .AddOption(new SlashCommandOptionBuilder().WithName("description").WithDescription("How should your faction be descriped?").WithType(ApplicationCommandOptionType.String).WithRequired(true)));

            new AddFactionMember();
            AddOption(new SlashCommandOptionBuilder().WithName("addmember").WithDescription("Add a member to your faction.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("user").WithDescription("Who should be added to the faction?").WithType(ApplicationCommandOptionType.User).WithRequired(true)));

            new RemoveFactionMember();
            AddOption(new SlashCommandOptionBuilder().WithName("removemember").WithDescription("Remove a member from your faction.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("user").WithDescription("Who should be removed from the faction?").WithType(ApplicationCommandOptionType.User).WithRequired(true)));

            new GetMemberList();
            AddOption(new SlashCommandOptionBuilder().WithName("member").WithDescription("Get all member of a faction.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("id").WithDescription("Give the faction id.").WithType(ApplicationCommandOptionType.Integer).WithRequired(true)));
        }

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task OnCommandExecute(SocketSlashCommand command)
        {
            bool isUsageValid = await CheckConditions(command);
            if ( !isUsageValid ) return;

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
        /// Checking guild settings and permissions for using fact commands.<para/>
        /// </summary>
        private static async Task<bool> CheckConditions(SocketSlashCommand command)
        {
            GuildObject guildObject = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return false;
            }

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
