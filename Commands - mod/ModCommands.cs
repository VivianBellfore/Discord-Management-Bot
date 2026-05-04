
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and executing the global <paramref name="mod"/> command.<para/>
    /// </summary>
    internal class ModCommands : SlashCommand
    {
        /// <summary>
        /// Called <paramref name="mod"/>.<br/>
        /// Constructor executed by:
        /// <seealso cref="CommandManager.SetupCommands"/><para/>
        /// </summary>
        internal ModCommands(string name, string description) : base(name, description)
        {
            new SendEmbed();
            AddOption(new SlashCommandOptionBuilder().WithName("embed").WithDescription("Create an Embed").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("color").WithDescription("In which color should the embed be?").WithRequired(true).AddChoice("Teal", "teal")
                    .AddChoice("Dark teal", "darkteal").AddChoice("Green", "green").AddChoice("Dark green", "darkgreen").AddChoice("Blue", "blue").AddChoice("Dark blue", "darkblue")
                    .AddChoice("Purple", "purple").AddChoice("Dark purple", "darkpurple").AddChoice("Magenta", "magenta").AddChoice("Dark magenta", "darkmagenta").AddChoice("Gold", "gold")
                    .AddChoice("Light orange", "lightorange").AddChoice("Orange", "orange").AddChoice("Dark orange", "darkorange").AddChoice("Red", "red").AddChoice("Dark red", "darkred")
                    .AddChoice("Light grey", "lightgrey").AddChoice("Dark grey", "darkgrey").WithType(ApplicationCommandOptionType.String))
                .AddOption("picture", ApplicationCommandOptionType.String, "Enter the URL of an image.", isRequired: false));

            new GetModHelp();
            AddOption(new SlashCommandOptionBuilder().WithName("help").WithDescription("Shows all mod commands and there functions.").WithType(ApplicationCommandOptionType.SubCommand));

            new GetUserReports();
            AddOption(new SlashCommandOptionBuilder().WithName("seereport").WithDescription("See reports for this user.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("user").WithDescription("Enter a user id.").WithType(ApplicationCommandOptionType.String).WithRequired(true)));

            new SetChannelRule();
            AddOption(new SlashCommandOptionBuilder().WithName("setrule").WithDescription("Set rules for the channel the command is used in.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("channel", ApplicationCommandOptionType.Channel, "Which channel should get new rules?", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder().WithName("color").WithDescription("In which color should the embed be?").WithRequired(true).AddChoice("Teal", "teal")
                    .AddChoice("Dark teal", "darkteal").AddChoice("Green", "green").AddChoice("Dark green", "darkgreen").AddChoice("Blue", "blue").AddChoice("Dark blue", "darkblue")
                    .AddChoice("Purple", "purple").AddChoice("Dark purple", "darkpurple").AddChoice("Magenta", "magenta").AddChoice("Dark magenta", "darkmagenta").AddChoice("Gold", "gold")
                    .AddChoice("Light orange", "lightorange").AddChoice("Orange", "orange").AddChoice("Dark orange", "darkorange").AddChoice("Red", "red").AddChoice("Dark red", "darkred")
                    .AddChoice("Light grey", "lightgrey").AddChoice("Dark grey", "darkgrey").WithType(ApplicationCommandOptionType.String))
                .AddOption("picture", ApplicationCommandOptionType.String, "Enter the URL of an image.", isRequired: false));

            new EditTicket();
            AddOption(new SlashCommandOptionBuilder().WithName("ticket").WithDescription("Close a ticket ( Will not delete the channel! ).").WithType(ApplicationCommandOptionType.SubCommand));

            new GetWordfilterList();
            AddOption(new SlashCommandOptionBuilder().WithName("wordlist").WithDescription("Shows you all blocked content for this server.").WithType(ApplicationCommandOptionType.SubCommand));
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

            if (await PermissionManager.HasUserBotPermissionRole("mod", (ulong)command.GuildId, (SocketGuildUser)command.User) == false)
            {
                if (await PermissionManager.HasUserBotPermissionRole("admin", (ulong)command.GuildId, (SocketGuildUser)command.User) == false)
                {
                    if (await PermissionManager.IsUserGuildOwner((ulong)command.GuildId, command.User.Id) == false)
                    {
                        string message = await LanguageManager.GetTranslation("missingPermisson", command.User.Id);
                        await command.ModifyOriginalResponseAsync(func => { func.Content = message; });
                        return;
                    }
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
