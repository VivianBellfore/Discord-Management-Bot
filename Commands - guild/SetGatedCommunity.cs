
using Discord;
using Discord.WebSocket;

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// This class contains the functions for the guild owner command <paramref name="gated"/>.
    /// </summary>
    internal class SetGatedCommunity : CommandObject
    {
        /// <summary>
        /// This is the strukt for the help command informations.
        /// </summary>
        internal SetGatedCommunity() : base("guild", "gated", "command_guild_gated") { }



        /// <summary>
        /// This function is handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if(guildData == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch data for guild! Id was: {(ulong)command.GuildId}.");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }
            if (guildData.MemberRole == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("missingMemberRole", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string panelLanguage = command.Data.Options.First().Options.ElementAt(0).Value.ToString();
            string memberType = command.Data.Options.First().Options.ElementAt(1).Value.ToString();

            if (panelLanguage == null || panelLanguage.Length == 0 || memberType == null || memberType.Length == 0)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch strings from command options data.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            SocketGuild guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {(ulong)command.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (memberType == "open")
                await SendOpenGatedCommunityMessage(command, panelLanguage, guild, guildData.MemberRole);
            else if (memberType == "closed")
                await SendClosedGatedCommunityMessage(command, panelLanguage, guild, guildData.MemberRole);
            else
            {
                await Utilities.SendDevLogMessage(1, $"Type of gated community was wrong! Type was {memberType}.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
            }
        }



        #region Sending messages with buttons
        /// <summary>
        /// This function is sending a message for an open gated community.
        /// </summary>
        private static async Task SendOpenGatedCommunityMessage(SocketSlashCommand command, string language, SocketGuild guild, ulong memberRoleId)
        {
            var buttonBuilder = new ComponentBuilder();
            string message;

            buttonBuilder.WithButton(await LanguageManager.GetTranslation("buttonGetMember", 0, language), $"respond_member_open_{language}", ButtonStyle.Success);
            message = await LanguageManager.GetTranslation("getMemberInfoText", 0, language, guild.Name, memberRoleId);

            await command.DeleteOriginalResponseAsync();
            var MessageObject = await command.Channel.SendMessageAsync(message, components: buttonBuilder.Build(), flags: MessageFlags.SuppressEmbeds);

            await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `guild_reaction_messages` (`guild_id`, `channel_id`, `message_id`, `event_type`) VALUES (@guild_id, @channel_id, @message_id, @event_type)",
                new Dictionary<string, object>() { { "guild_id", guild.Id }, { "channel_id", MessageObject.Channel.Id }, { "message_id", MessageObject.Id }, { "event_type", "openGated" } });
        }

        /// <summary>
        /// This function is sending a message for a closed gated community.
        /// </summary>
        private static async Task SendClosedGatedCommunityMessage(SocketSlashCommand command, string language, SocketGuild guild, ulong memberRoleId)
        {
            var buttonBuilder = new ComponentBuilder();
            string message;

            buttonBuilder.WithButton(await LanguageManager.GetTranslation("buttonGetMember", 0, language), $"respond_member_closed_{language}", ButtonStyle.Success);
            message = await LanguageManager.GetTranslation("getMemberClosedInfoText", 0, language, guild.Name, memberRoleId);

            await command.DeleteOriginalResponseAsync();
            var MessageObject = await command.Channel.SendMessageAsync(message, components: buttonBuilder.Build(), flags: MessageFlags.SuppressEmbeds);

            await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `guild_reaction_messages` (`guild_id`, `channel_id`, `message_id`, `event_type`) VALUES (@guild_id, @channel_id, @message_id, @event_type)",
                new Dictionary<string, object>() { { "guild_id", guild.Id }, { "channel_id", MessageObject.Channel.Id }, { "message_id", MessageObject.Id }, { "event_type", "closedGated" } });
        }
        #endregion



        #region Functions
        /// <summary>
        /// Register a user if all conditions are true. Else it will send a note to the server team.
        /// </summary>
        internal static async Task OpenCommunityRegister(SocketMessageComponent button, string language)
        {
            await LanguageManager.SetUserLanguage(language, button.User.Id);

            GuildObject guildObject = await GuildManager.GetGuildData((ulong)button.GuildId);
            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guildobject {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }
            if (guildObject.MemberRole == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("missingMemberRole", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var guild = StartBotInstance._client.GetGuild((ulong)button.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var role = guild.GetRole(guildObject.MemberRole);
            if (role == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find the member role for guild {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("missingMemberRole", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            SocketGuildUser user = (SocketGuildUser)button.User;
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not get socket guild user || {button.User.Id} ||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var roleList = user.Roles.Where(x => !x.IsEveryone).Select(x => x.Id).ToList();
            if (roleList.Contains(guildObject.MemberRole))
            {
                string errorMessage = await LanguageManager.GetTranslation("youGotTheRoleAlready", button.User.Id, "", role.Name);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (user.CreatedAt.AddDays(7) > DateTime.Now)
            {
                string errorMessage = await LanguageManager.GetTranslation("accountToJung", button.User.Id, "", role.Name);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                await ClosedCommunityRegister(button, language, true);
                return;
            }

            if (await PermissionManager.HasUserAcceptTos(button.User.Id) == false)
                await UserManager.RegisterUser(button.User.Id, language);

            await LanguageManager.SetUserLanguage(language, button.User.Id);
            await user.AddRoleAsync(guildObject.MemberRole);
            string message = await LanguageManager.GetTranslation("youGotTheRole", button.User.Id, "", role.Name);
            await button.ModifyOriginalResponseAsync(func => func.Content = message);
        }

        /// <summary>
        /// This function is checking conditions and sending a message with buttons to the server team.
        /// </summary>
        internal static async Task ClosedCommunityRegister(SocketMessageComponent button, string language, bool toJungForOpen)
        {
            await LanguageManager.SetUserLanguage(language, button.User.Id);

            bool isAlreadyPending = await GuildManager.GetPendingAction("membership", (ulong)button.GuildId, button.User.Id, 0);
            if (isAlreadyPending)
            {
                string errorMessage = await LanguageManager.GetTranslation("pendingAction", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (await PermissionManager.HasUserBotPermissionRole("member", (ulong)button.GuildId, button.User as SocketGuildUser))
            {
                string errorMessage = await LanguageManager.GetTranslation("alreadyMember", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            GuildObject guildObject = await GuildManager.GetGuildData((ulong)button.GuildId);
            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guildobject {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }
            if (guildObject.MemberRole == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("missingMemberRole", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var guild = StartBotInstance._client.GetGuild((ulong)button.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            ITextChannel textChannel = guild.GetChannel(guildObject.LogChannel) as ITextChannel;
            if (textChannel == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find channel {guildObject.LogChannel}!");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await GuildManager.SetPendingAction(true, "membership", (ulong)button.GuildId, button.User.Id, 0);

            if (!toJungForOpen)
            {
                string message = await LanguageManager.GetTranslation("requestMemberRole", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = message);
            }

            var buttonBuilder = new ComponentBuilder();
            buttonBuilder.WithButton(await LanguageManager.GetTranslation("buttonAcceptMember", 0, language), $"respond_member_accept_{language}_{button.User.Id}", ButtonStyle.Success);
            buttonBuilder.WithButton(await LanguageManager.GetTranslation("buttonRejectMember", 0, language), $"respond_member_deny_{language}_{button.User.Id}", ButtonStyle.Danger);

            string teamMessage = await LanguageManager.GetTranslation("requestMemberRoleTeam", button.User.Id, "", button.User.Id);
            if (button.User.CreatedAt.AddDays(7) > DateTime.Now)
                teamMessage = teamMessage + "\n" + await LanguageManager.GetTranslation("userAccountToJung", button.User.Id);
            await textChannel.SendMessageAsync(teamMessage, components: buttonBuilder.Build());
        }
        #endregion



        #region Resolve membership
        /// <summary>
        /// This function is adding a user as a member of a server.
        /// </summary>
        internal static async Task AcceptMembership(SocketMessageComponent button, string language)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_'); // respond_member_open_language_userid

            var guild = StartBotInstance._client.GetGuild((ulong)button.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            SocketGuildUser user = guild.GetUser(Convert.ToUInt64(splitedCustomId[4]));
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not get socket guild user || {splitedCustomId[4]} ||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (await PermissionManager.HasUserAcceptTos(button.User.Id) == false)
                await UserManager.RegisterUser(user.Id, splitedCustomId[3]);

            GuildObject guildObject = await GuildManager.GetGuildData((ulong)button.GuildId);
            if (guildObject == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guildobject {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("registrationMissingBot", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }
            if (guildObject.MemberRole == 0)
            {
                string errorMessage = await LanguageManager.GetTranslation("missingMemberRole", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var role = guild.GetRole(guildObject.MemberRole);
            if (role == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find the member role for guild {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("missingMemberRole", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            var roleList = user.Roles.Where(x => !x.IsEveryone).Select(x => x.Id).ToList();
            if (!roleList.Contains(guildObject.MemberRole))
                await user.AddRoleAsync(guildObject.MemberRole);

            await GuildManager.SetPendingAction(false, "membership", (ulong)button.GuildId, user.Id, 0);

            await button.Channel.SendMessageAsync(await LanguageManager.GetTranslation("memberAccepted", 0, language, user.Id, button.User.Id));

            try
            {
                await user.SendMessageAsync(string.Format(await LanguageManager.GetTranslation("youGotMember", user.Id), guild.Name, role.Name));
            }
            catch
            {
                await button.Channel.SendMessageAsync(await LanguageManager.GetTranslation("userBlocksDMs", button.User.Id));
            }

            await button.Message.DeleteAsync();
            await button.DeleteOriginalResponseAsync();
        }

        /// <summary>
        /// This function is denying a user as a member of a server.
        /// </summary>
        internal static async Task DenyMembership(SocketMessageComponent button, string language)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_'); // respond_member_open_language_userid

            var guild = StartBotInstance._client.GetGuild((ulong)button.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not find guild {(ulong)button.GuildId}!");
                string errorMessage = await LanguageManager.GetTranslation("generalError", 0, language);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            SocketGuildUser user = guild.GetUser(Convert.ToUInt64(splitedCustomId[4]));
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not get socket guild user || {button.User.Id} ||.");
                string errorMessage = await LanguageManager.GetTranslation("generalError", 0, language);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            await LanguageManager.SetUserLanguage(splitedCustomId[3], user.Id);

            await GuildManager.SetPendingAction(false, "membership", (ulong)button.GuildId, user.Id, 0);

            await button.Channel.SendMessageAsync(await LanguageManager.GetTranslation("memberDenied", 0, language, user.Id, button.User.Id));

            try
            {
                await user.SendMessageAsync(await LanguageManager.GetTranslation("membershipDenied", 0, language, guild.Name));
            }
            catch
            {
                await button.Channel.SendMessageAsync(await LanguageManager.GetTranslation("userBlocksDMs", 0, language));
            }

            await button.Message.DeleteAsync();
            await button.DeleteOriginalResponseAsync();
        }
        #endregion
    }



    /// <summary>
    /// This class is building a button for <seealso cref="member"/>.
    /// </summary>
    internal class MemberButton : ButtonPressed
    {
        /// <summary>
        /// This constructor is a builder for the button with custom id <paramref name="member"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="SetGatedCommunity"/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal MemberButton(string customId) : base(customId)
        {
            WithCustomId("member");
        }

        /// <summary>
        /// This function is handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_'); // respond_member_open_language_userid

            string memberType = splitedCustomId[2];
            string language = splitedCustomId[3];

            switch (memberType)
            {
                case "open":
                    await SetGatedCommunity.OpenCommunityRegister(button, language);
                    break;
                case "closed":
                    await SetGatedCommunity.ClosedCommunityRegister(button, language, false);
                    break;
                case "accept":
                    await SetGatedCommunity.AcceptMembership(button, language);
                    break;
                case "deny":
                    await SetGatedCommunity.DenyMembership(button, language);
                    break;
                default:
                    {
                        await Utilities.SendDevLogMessage(1, $"Member type was not valid! Id was {memberType}.");
                        string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                        await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                    }
                    return;
            }
        }
    }
}
