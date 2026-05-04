
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and executing the global <paramref name="admin"/> command.<para/>
    /// </summary>
    internal class AdminCommands : SlashCommand
    {
        /// <summary>
        /// Builder for a global command called <paramref name="admin"/>.<br/>
        /// Constructor executed by:
        /// <seealso cref="CommandManager.SetupCommands"/><para/>
        /// </summary>
        internal AdminCommands(string name, string description) : base(name, description)
        {
            new GetAdminHelp();
            AddOption(new SlashCommandOptionBuilder().WithName("help").WithDescription("All admin commands explained.").WithType(ApplicationCommandOptionType.SubCommand));

            new SetInviteLink();
            AddOption(new SlashCommandOptionBuilder().WithName("invite").WithDescription("Set an invite link").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("link", ApplicationCommandOptionType.String, "Insert your URL here.", isRequired: true));

            new SetUserGuildPoints();
            AddOption(new SlashCommandOptionBuilder().WithName("points").WithDescription("Set user guild points.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("type").WithDescription("Do you want to add or remove points?").WithRequired(true)
                    .AddChoice("Add", "add").AddChoice("Remove", "remove").WithType(ApplicationCommandOptionType.String))    
                .AddOption("amount", ApplicationCommandOptionType.Integer, "How much points should be chnaged?", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder().WithName("user").WithDescription("Which user should be changed?").WithType(ApplicationCommandOptionType.User).WithRequired(true)));

            new SendUserReport();
            AddOption(new SlashCommandOptionBuilder().WithName("report").WithDescription("Send a report for a user.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("user").WithDescription("Enter a user id.").WithType(ApplicationCommandOptionType.String).WithRequired(true))
                .AddOption(new SlashCommandOptionBuilder().WithName("reason").WithDescription("What kind of missbehaviour did they do?").WithRequired(true)
                    .AddChoice("Spamming", "spam").AddChoice("Scamming", "scam").AddChoice("Advertisement", "addvertise").AddChoice("Harrasment", "harrasment").AddChoice("Insulting", "insult")
                    .AddChoice("Trolling", "troll").AddChoice("Impersonating", "impersonate").AddChoice("Hate speech", "hatespeech").AddChoice("Threats", "threats")
                    .AddChoice("Doxxing", "doxxing").AddChoice("Ban evade", "banevade").AddChoice("Cheating", "cheating").AddChoice("Predatory", "predatory").AddChoice("Toxic", "toxic")
                    .WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("text").WithDescription("Explain shortly w´hat they did.").WithType(ApplicationCommandOptionType.String).WithRequired(true)));

            new SendRolesMessage();
            AddOption(new SlashCommandOptionBuilder().WithName("roles").WithDescription("Post the role message, get role list or clear all roles.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("type").WithDescription("What do you want to do?").WithRequired(true).AddChoice("Post the role message", "role")
                    .AddChoice("Get the current roles list", "roleget").AddChoice("Clear the current role list", "roleclear").WithType(ApplicationCommandOptionType.String)));

            new ChangeUserRoles();
            AddOption(new SlashCommandOptionBuilder().WithName("rolechange").WithDescription("Add or remove user roles.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("role", ApplicationCommandOptionType.Role, "What role should be changed?", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder().WithName("change").WithDescription("Do you want to add or remove the role?").WithRequired(true)
                    .AddChoice("Add", "add").AddChoice("Remove", "remove").WithType(ApplicationCommandOptionType.String)));

            new ShowGuildStatus();
            AddOption(new SlashCommandOptionBuilder().WithName("status").WithDescription("Shows all your guild data from our data base.").WithType(ApplicationCommandOptionType.SubCommand));

            new SendStickyMessage();
            AddOption(new SlashCommandOptionBuilder().WithName("sticky").WithDescription("Create a sticky Embed.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("type").WithDescription("Do you want to add or remove points?").WithRequired(true)
                    .AddChoice("Set slow mode", "slow").AddChoice("No slow mode", "none").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("color").WithDescription("In which color should the sticky embed be?").WithRequired(true).AddChoice("Teal", "teal")
                    .AddChoice("Dark teal", "darkteal").AddChoice("Green", "green").AddChoice("Dark green", "darkgreen").AddChoice("Blue", "blue").AddChoice("Dark blue", "darkblue")
                    .AddChoice("Purple", "purple").AddChoice("Dark purple", "darkpurple").AddChoice("Magenta", "magenta").AddChoice("Dark magenta", "darkmagenta").AddChoice("Gold", "gold")
                    .AddChoice("Light orange", "lightorange").AddChoice("Orange", "orange").AddChoice("Dark orange", "darkorange").AddChoice("Red", "red").AddChoice("Dark red", "darkred")
                    .AddChoice("Light grey", "lightgrey").AddChoice("Dark grey", "darkgrey").WithType(ApplicationCommandOptionType.String))
                .AddOption("picture", ApplicationCommandOptionType.String, "Enter the URL of an image.", isRequired: false));

            new StopAStickyMessage();
            AddOption(new SlashCommandOptionBuilder().WithName("stopsticky").WithDescription("Delete a sticky message.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("messageid", ApplicationCommandOptionType.String, "Message id of the sticky message.", isRequired: true));

            new AddWordfilter();
            AddOption(new SlashCommandOptionBuilder().WithName("wordadd").WithDescription("Add something to the word filter.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("text", ApplicationCommandOptionType.String, "What should be added? ( 50 character max )", isRequired: true));

            new RemoveWordfilter();
            AddOption(new SlashCommandOptionBuilder().WithName("wordremove").WithDescription("Remove something from the word filter.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("text", ApplicationCommandOptionType.String, "What should be removed?", isRequired: true));

            new SetGuildColorRoles();
            AddOption(new SlashCommandOptionBuilder().WithName("color").WithDescription("Add or remove color roles.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("role", ApplicationCommandOptionType.Role, "What role should be changed?", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder().WithName("change").WithDescription("Do you want to add or remove the role?").WithRequired(true)
                    .AddChoice("Add", "add").AddChoice("Remove", "remove").WithType(ApplicationCommandOptionType.String)));

            new SetChannel();
            AddOption(new SlashCommandOptionBuilder().WithName("channel").WithDescription("Set or add a system channel.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("wowchar", ApplicationCommandOptionType.Channel, "Where should wow member information be send to?", isRequired: false));

            new RemoveChannel();
            AddOption(new SlashCommandOptionBuilder().WithName("remchannel").WithDescription("Remove a system channel.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("select").WithDescription("Which channel should be removed?").WithRequired(true)
                    .AddChoice("WoW character", "wowchar").WithType(ApplicationCommandOptionType.String)));
        }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task OnCommandExecute(SocketSlashCommand command)
        {
            GuildObject guildObject = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (await PermissionManager.HasUserBotPermissionRole("admin", (ulong)command.GuildId, (SocketGuildUser)command.User) == false)
            {
                if (await PermissionManager.IsUserGuildOwner((ulong)command.GuildId, command.User.Id) == false)
                {
                    string message = await LanguageManager.GetTranslation("missingPermisson", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => { func.Content = message; });
                    return;
                } 
            }

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
    }
}
