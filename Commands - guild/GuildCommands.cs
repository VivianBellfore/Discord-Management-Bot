
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and executing the global <paramref name="guild"/> command.
    /// </summary>
    internal class GuildCommands : SlashCommand
    {
        /// <summary>
        /// Builder for a global command called <paramref name="guild"/>.<br/>
        /// Constructor executed by:
        /// <seealso cref="CommandManager.SetupCommands"/><para/>
        /// Calling the following command classes:<br/>
        /// <seealso cref="GetGuildHelp"/><br/>
        /// <seealso cref="RemoveGuildData"/><br/>
        /// <seealso cref="SetGuildMember"/><br/>
        /// <seealso cref="SetGuildChannel"/><br/>
        /// <seealso cref="SetGuildPermissions"/><br/>
        /// <seealso cref="SetGuildPointsName"/><br/>
        /// <seealso cref="SetGuildSettings"/><br/>
        /// <seealso cref="SetGatedCommunity"/><br/>
        /// </summary>
        internal GuildCommands(string name, string description) : base(name, description)
        {
            new SetGuildChannel();
            AddOption(new SlashCommandOptionBuilder().WithName("channel").WithDescription("Select your system channel for news and warnings.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("logs", ApplicationCommandOptionType.Channel, "This channel is for team intern (private) logs, errors and warnings.", isRequired: true)
                .AddOption("news", ApplicationCommandOptionType.Channel, "This channel is for news around the bot for all user (public).", isRequired: true)
                .AddOption("event", ApplicationCommandOptionType.Channel, "This channel is for user events (public).", isRequired: true));

            new SetGatedCommunity();
            AddOption(new SlashCommandOptionBuilder().WithName("gated").WithDescription("Send gated community message.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("language").WithDescription("In which language should the embed be?").WithRequired(true)
                    .AddChoice("Deutsch", "german").AddChoice("English", "english").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("type").WithDescription("What kind of gated community?").WithRequired(true)
                    .AddChoice("Open", "open").AddChoice("Closed", "closed").WithType(ApplicationCommandOptionType.String)));

            new GetGuildHelp();
            AddOption(new SlashCommandOptionBuilder().WithName("help").WithDescription("Shows all guild owner commands.").WithType(ApplicationCommandOptionType.SubCommand));

            new SetGuildLanguage();
            AddOption(new SlashCommandOptionBuilder().WithName("language").WithDescription("What language shall the bot use for your server?").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("language").WithDescription("Select a langauge.").WithRequired(true)
                    .AddChoice("Deutsch", "german").AddChoice("English", "english").WithType(ApplicationCommandOptionType.String)));

            new SetGuildMember();
            AddOption(new SlashCommandOptionBuilder().WithName("member").WithDescription("Setup gated community role for member.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("role", ApplicationCommandOptionType.Role, "Which role should be the member role of the gated community?", isRequired: true));            

            new SetGuildPermissions();
            AddOption(new SlashCommandOptionBuilder().WithName("permissions").WithDescription("Select an admin or moderator role.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("admin", ApplicationCommandOptionType.Role, "Which role should be admin?", isRequired: true)
                .AddOption("mod", ApplicationCommandOptionType.Role, "Which role should be moderator?", isRequired: true));

            new SetGuildPointsName();
            AddOption(new SlashCommandOptionBuilder().WithName("pointname").WithDescription("Set the name for your points.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("name", ApplicationCommandOptionType.String, "What should the points be named? (30 character max)", isRequired: true));

            new GuildRegister();
            AddOption(new SlashCommandOptionBuilder().WithName("register").WithDescription("Register your server for our bot and system.").WithType(ApplicationCommandOptionType.SubCommand));

            new RemoveGuildData();
            AddOption(new SlashCommandOptionBuilder().WithName("removedata").WithDescription("Removing all data relevant to your server.").WithType(ApplicationCommandOptionType.SubCommand));

            new SetGuildSettings();
            AddOption(new SlashCommandOptionBuilder().WithName("settings").WithDescription("Change your server settings.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("wordfilter").WithDescription("Should the bot filter all messages for bad words?")
                    .AddChoice("Yes", "1").AddChoice("No", "0").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("deletemessage").WithDescription("Should the bot log deleted messages?")
                    .AddChoice("Yes", "1").AddChoice("No", "0").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("ticketsactive").WithDescription("Should the ticktes system be active?")
                    .AddChoice("Yes", "1").AddChoice("No", "0").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("gatedcommunity").WithDescription("Should the gated community function be active?")
                    .AddChoice("Yes", "1").AddChoice("No", "0").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("econemy").WithDescription("Can user collect server points for the econemy?")
                    .AddChoice("Yes", "1").AddChoice("No", "0").WithType(ApplicationCommandOptionType.String))
                .AddOption(new SlashCommandOptionBuilder().WithName("tempvoice").WithDescription("Can user add tempt voice channel?")
                    .AddChoice("Yes", "1").AddChoice("No", "0").WithType(ApplicationCommandOptionType.String)));

            new SetTicketCategory();
            AddOption(new SlashCommandOptionBuilder().WithName("tickets").WithDescription("Select a categorie for the ticket system.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("categorie", ApplicationCommandOptionType.Channel, "This must be a categorie! And this should only be used for tickets.", isRequired: true));

            new SetTempVoiceCategory();
            AddOption(new SlashCommandOptionBuilder().WithName("voice").WithDescription("Select a categorie for the temp voices.").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption("categorie", ApplicationCommandOptionType.Channel, "This must be a categorie! And this should only be used for voices.", isRequired: true));
        }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task OnCommandExecute(SocketSlashCommand command)
        {
            if (await PermissionManager.IsUserGuildOwner((ulong)command.GuildId, command.User.Id) == false)
            {
                string message = await LanguageManager.GetTranslation("missingPermisson", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => { func.Content = message; });
                return;
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
