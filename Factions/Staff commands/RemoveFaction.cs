
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="remove"/> command.
    /// </summary>
    internal class RemoveFaction : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal RemoveFaction() : base("fact", "remove", "command_fact_remove") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            if (await PermissionManager.HasUserBotPermissionRole("admin", (ulong)command.GuildId, (SocketGuildUser)command.User) == false)
            {
                if (await PermissionManager.IsUserGuildOwner((ulong)command.GuildId, command.User.Id) == false)
                {
                    string errorMessage = await LanguageManager.GetTranslation("missingPermisson", command.User.Id);
                    await command.ModifyOriginalResponseAsync(func => { func.Content = errorMessage; });
                    return;
                }
            }

            (ulong categoryId, string errorMessage2) = await FactionManager.RemoveFaction(Convert.ToInt32(command.Data.Options.First().Options.ElementAt(0).Value), command.User.Id);

            if ( errorMessage2 != "")
            {
                await Utilities.SendDevLogMessage(1, $"Database error: {errorMessage2}. Guild {(ulong)command.GuildId}");
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
                return;
            }

            SocketGuild guild = Utilities.GetGuildSocket((ulong)command.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild could not be fetched. Guild {(ulong)command.GuildId}");
                string errorMessage3 = await LanguageManager.GetTranslation("fetchGuildError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
                return;
            }

            try
            {
                SocketCategoryChannel channelList = guild.GetCategoryChannel(categoryId);

                foreach (SocketGuildChannel channel in channelList.Channels)
                    await channel.DeleteAsync();

                await channelList.DeleteAsync();
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"Command was most likly used in the channel to be removed...\n{ex}");
            }

            string message = await LanguageManager.GetTranslation("removedFaction", command.User.Id);
            await command.ModifyOriginalResponseAsync( func => func.Content = message );
        }
    }
}
