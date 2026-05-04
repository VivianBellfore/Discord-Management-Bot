
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Building and executing the global <paramref name="dev"/> command.<para/>
    /// </summary>
    internal class DevCommands : SlashCommand
    {
        /// <summary>
        /// Builder for a global command called <paramref name="dev"/>.<br/>
        /// Constructor executed by:
        /// <seealso cref="CommandManager.SetupCommands"/><para/>
        /// </summary>
        internal DevCommands(string name, string description) : base(name, description)
        {
            new SendDevNews();
            AddOption(new SlashCommandOptionBuilder().WithName("news").WithDescription("Create a news embed").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(new SlashCommandOptionBuilder().WithName("color").WithDescription("In which color should the embed be?").WithRequired(true).AddChoice("Teal", "teal")
                    .AddChoice("Dark teal", "darkteal").AddChoice("Green", "green").AddChoice("Dark green", "darkgreen").AddChoice("Blue", "blue").AddChoice("Dark blue", "darkblue")
                    .AddChoice("Purple", "purple").AddChoice("Dark purple", "darkpurple").AddChoice("Magenta", "magenta").AddChoice("Dark magenta", "darkmagenta").AddChoice("Gold", "gold")
                    .AddChoice("Light orange", "lightorange").AddChoice("Orange", "orange").AddChoice("Dark orange", "darkorange").AddChoice("Red", "red").AddChoice("Dark red", "darkred")
                    .AddChoice("Light grey", "lightgrey").AddChoice("Dark grey", "darkgrey").WithType(ApplicationCommandOptionType.String))
                .AddOption("picture", ApplicationCommandOptionType.String, "Enter the URL of an image.", isRequired: false));
        }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
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
