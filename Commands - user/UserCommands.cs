
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and executing the global <paramref name="use"/> command.<para/>
    /// </summary>
    internal class UserCommands : SlashCommand
    {
        /// <summary>
        /// Builder for a global command called <paramref name="use"/>.<br/>
        /// Constructor executed by:
        /// <seealso cref="CommandManager.SetupCommands"/><para/>
        /// </summary>
        internal UserCommands(string name, string description) : base(name, description)
        {
            new BlockBotDM();
            AddOption(new SlashCommandOptionBuilder().WithName("botdm").WithDescription("Set the bot DM behaviour to you.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("setting").WithDescription("The bot can send you private messages?").WithRequired(true)
                    .AddChoice("Allow DM", "yes").AddChoice("Deny DM", "no").WithType(ApplicationCommandOptionType.String)));

            new GetUserHelp();
            AddOption(new SlashCommandOptionBuilder().WithName("help").WithDescription("All user commands explained.").WithType(ApplicationCommandOptionType.SubCommand));

            new RemoveUserData();
            AddOptions(new SlashCommandOptionBuilder().WithName("deletedata").WithDescription("Delete all data related to your account.").WithType(ApplicationCommandOptionType.SubCommand));

            new GetInviteLink();
            AddOptions(new SlashCommandOptionBuilder().WithName("invite").WithDescription("Shows you the invite link to this server.").WithType(ApplicationCommandOptionType.SubCommand));

            new Randomiser();
            AddOption(new SlashCommandOptionBuilder().WithName("random").WithDescription("What should be randomised?").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("public").WithDescription("Select if it should be private or public.").WithRequired(true)
                    .AddChoice("Public", "public").AddChoice("Privat", "privat").WithType(ApplicationCommandOptionType.String)));

            new SetUserLanguage();
            AddOption(new SlashCommandOptionBuilder().WithName("language").WithDescription("What language shall the bot use for your server?").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("language").WithDescription("Select a langauge.").WithRequired(true)
                    .AddChoice("Deutsch", "german").AddChoice("English", "english").WithType(ApplicationCommandOptionType.String)));

            new GetRankList();
            AddOptions(new SlashCommandOptionBuilder().WithName("ranks").WithDescription("Shows the top list of this server and your rank, level and points.").WithType(ApplicationCommandOptionType.SubCommand));

            new UserRegister();
            AddOption(new SlashCommandOptionBuilder().WithName("register").WithDescription("Register your account and allow the bot to save your data.").WithType(ApplicationCommandOptionType.SubCommand));

            new GetChannelRules();
            AddOption(new SlashCommandOptionBuilder().WithName("rules").WithDescription("Shows you the rules of a channel.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("channel", ApplicationCommandOptionType.Channel, "Which channels rules?", isRequired: true));

            new GetUserStatus();
            AddOption(new SlashCommandOptionBuilder().WithName("stat").WithDescription("Get your user status, points and event info.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("user").WithDescription("The user you want to see the stats of.").WithType(ApplicationCommandOptionType.User)));

            new AddTempVoice();
            AddOption(new SlashCommandOptionBuilder().WithName("voice").WithDescription("Add a temp voice.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("name", ApplicationCommandOptionType.String, "Give the voice channel a name! (30 character max)", isRequired: true));

            new AddTicket();
            AddOption(new SlashCommandOptionBuilder().WithName("ticket").WithDescription("Open a ticket to talk to the server team about a problem or ask them something.")
                .WithType(ApplicationCommandOptionType.SubCommand));

            new GetColorRoles();
            AddOption(new SlashCommandOptionBuilder().WithName("color").WithDescription("Get a list of the server color roles.").WithType(ApplicationCommandOptionType.SubCommand));

            new BuyColorRole();
            AddOption(new SlashCommandOptionBuilder().WithName("colorrole").WithDescription("Buy a new color role.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("role", ApplicationCommandOptionType.Role, "Choose your color role.", isRequired: true));

            new SetPublicReminder();
            AddOption(new SlashCommandOptionBuilder().WithName("pubremind").WithDescription("Add a public reminder.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("reminder").WithDescription("Which type of reminder do you want?").WithRequired(true).AddChoice("Daily reminder", "daily")
                    .AddChoice("Weekly reminder", "weekly").AddChoice("On date", "date").AddChoice("Duration of days", "duration").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("color").WithDescription("In which color should the embed be?").WithRequired(true).AddChoice("Teal", "teal")
                    .AddChoice("Dark teal", "darkteal").AddChoice("Green", "green").AddChoice("Dark green", "darkgreen").AddChoice("Blue", "blue").AddChoice("Dark blue", "darkblue")
                    .AddChoice("Purple", "purple").AddChoice("Dark purple", "darkpurple").AddChoice("Magenta", "magenta").AddChoice("Dark magenta", "darkmagenta").AddChoice("Gold", "gold")
                    .AddChoice("Light orange", "lightorange").AddChoice("Orange", "orange").AddChoice("Dark orange", "darkorange").AddChoice("Red", "red").AddChoice("Dark red", "darkred")
                    .AddChoice("Light grey", "lightgrey").AddChoice("Dark grey", "darkgrey").WithType(ApplicationCommandOptionType.String))
                .AddOption("channel", ApplicationCommandOptionType.Channel, "In which channel will the reminder be posted?", isRequired: true)
                .AddOption("picture", ApplicationCommandOptionType.String, "Enter the URL of an image.", isRequired: false)
                .AddOption("roleids", ApplicationCommandOptionType.String, "Enter role ID´s. Seperate them with a `,`.", isRequired: false));
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
        /// No guild data = true<br/>
        /// Not gated community = true<br/>
        /// Socket user not found = false<br/>
        /// If gated and user is member = true<br/>
        /// If gated and user not member = false
        /// </summary>
        private static async Task<bool> CheckConditions(SocketSlashCommand command)
        {
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
