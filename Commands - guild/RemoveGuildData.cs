
using Discord;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class contains the functions for the guild owner command <paramref name="removedata"/>.
    /// </summary>
    internal class RemoveGuildData : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal RemoveGuildData() : base("guild", "removedata", "command_guild_removedata") { }

        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if (guildData == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string label = await LanguageManager.GetTranslation("removeGuildDataLabel", command.User.Id);
            var buttonBuilder = new ComponentBuilder().WithButton(label, $"respond_removeguilddata_{(ulong)command.GuildId}", ButtonStyle.Danger);

            string content = await LanguageManager.GetTranslation("removeGuildDataMessage", command.User.Id);
            await command.ModifyOriginalResponseAsync(func => { func.Content = content; func.Components = buttonBuilder.Build(); });
        }
    }



    /// <summary>
    /// This class is building a button for <seealso cref="removeguilddata"/>.
    /// </summary>
    internal class RemoveGuildDataButton : ButtonPressed
    {
        /// <summary>
        /// This constructor is a builder for the button with custom id <paramref name="removeguilddata"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="RemoveGuildData/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal RemoveGuildDataButton(string customId) : base(customId)
        {
            WithCustomId("removeguilddata");
        }

        /// <summary>
        /// This function is handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_'); // "respond_removeguilddata_{guild.Id}"

            if (await PermissionManager.IsUserGuildOwner(Convert.ToUInt64(splitedCustomId[2]), button.User.Id) == false)
            {
                string permissionMessage = await LanguageManager.GetTranslation("missingPermisson", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = permissionMessage);
                return;
            }

            GuildObject guildObject = await GuildManager.GetGuildData((ulong)button.GuildId);
            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)button.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            List<ulong> toRemovedReactions = new List<ulong>();

            foreach ( var message in MessageManager.reactionMessages)
            {
                if (message.Value.GuildId == guildObject.GuildId && message.Value.EventType == "closedGated" || 
                    message.Value.GuildId == guildObject.GuildId && message.Value.EventType == "openGated")
                {
                    await MessageManager.RemoveMessage(guildObject.GuildId, message.Value.ChannelId, message.Value.MessageId);
                    toRemovedReactions.Add(message.Key);
                }
            }

            foreach (ulong id  in toRemovedReactions)
            {
                MessageManager.reactionMessages.Remove(id);
            }

            int deleteCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `guild_data` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", (ulong)button.GuildId } });

            if (deleteCount > 0)
            {
                string message = await LanguageManager.GetTranslation("deletedGuildData", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = message);
                return;
            }

            await Utilities.SendDevLogMessage(1, $"Data could not be removed. Guild id was {(ulong)button.GuildId}.");
            string errorMessage2 = await LanguageManager.GetTranslation("generalError", button.User.Id);
            await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage2);
        }
    }
}
