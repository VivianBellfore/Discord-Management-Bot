
using Discord;
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="random"/> command.
    /// </summary>
    internal class Randomiser : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal Randomiser() : base("use", "random", "command_use_random") { }



        /// <summary>
        /// Register the modal for random command, called <paramref name="random_modal"/>.
        /// </summary>
        internal static RandomModal randomModal = new RandomModal("random_modal");

        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            Utilities.tempSettingChoises.TryRemove(command.User.Id, out _);
            Utilities.tempSettingChoises.TryAdd(command.User.Id, command.Data.Options.First().Options.ElementAt(0).Value.ToString());

            await command.RespondWithModalAsync(randomModal.Build());
        }
    }



    /// <summary>
    /// This class is building the modal for <seealso cref="Randomiser"/>.
    /// </summary>
    internal class RandomModal : ModalSubmit
    {
        /// <summary>
        /// This function is a builder for the modal with custom id <paramref name="random_modal"/>.<para/>
        /// </summary>
        internal RandomModal(string customId) : base(customId)
        {
            WithTitle("Randomiser");
            AddTextInput("List", "random_list", TextInputStyle.Paragraph, "Each line as a new entry for randomisation. Jede Zeile ist ein Eintrag für den Zufallsgenerator.", 1, 4000, required: true);

            CommandManager.commandsWithModal.Add("random");
        }

        /// <summary>
        /// This function is handling modal submittings and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ModalManager.ModalSubmittedHandler(SocketModal)"/>
        /// </summary>
        internal async override Task OnModalExecute(SocketModal modal)
        {
            List<SocketMessageComponentData> components = modal.Data.Components.ToList();
            string list = components.First(x => x.CustomId == "random_list").Value;

            string[] lines = list.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            Shuffle(lines);

            string text = $"Zufällig ausgewählt wurde:\n# {lines[0]}\n\nFalls du den Rest auch wissen willst, hier ist die zufällige Reihenfolge für alle anderen Einträge:\n";

            foreach (string line in lines)
            {
                if (line == lines[0]) continue;

                text += "- " + line + "\n";
            }

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Randomiser")
                .WithDescription(text);

            if (Utilities.tempSettingChoises[modal.User.Id] == "public")
            {
                await modal.Channel.SendMessageAsync(embed: embedBuilder.Build());
                await modal.DeleteOriginalResponseAsync();
            }
            else
                await modal.ModifyOriginalResponseAsync(f => { f.Content = ""; f.Embed = embedBuilder.Build(); });

            Utilities.tempSettingChoises.TryRemove(modal.User.Id, out _);
        }

        /// <summary>
        /// Shuffle a string array.
        /// </summary>
        static void Shuffle(string[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = Utilities.random.Next(0, i + 1);

                string temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
    }
}
