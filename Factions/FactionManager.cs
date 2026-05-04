
using Discord;
using Discord.Rest;
using Discord.WebSocket;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    internal class FactionManager
    {
        /// <summary>
        /// Creates a new faction in data base and is creating a new category in the discord server with a text channel and permissions for the owner.
        /// </summary>
        internal static async Task CreateNewGuildFaction(SocketSlashCommand command, SocketGuildUser owner)
        {
            SocketGuild guild = Utilities.GetGuildSocket((ulong)command.GuildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Guild could not be fetched. Guild {(ulong)command.GuildId}");
                string errorMessage3 = await LanguageManager.GetTranslation("fetchGuildError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage3);
                return;
            }

            RestCategoryChannel newCategory = await guild.CreateCategoryChannelAsync("New Faction");
            if (newCategory == null)
            {
                await Utilities.SendDevLogMessage(1, $"Category could not be created. Guild {(ulong)command.GuildId}");
                string errorMessage4 = await LanguageManager.GetTranslation("addFactionCategoryError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage4);
                return;
            }

            await Task.Delay(3000);

            RestTextChannel chatChannel = await guild.CreateTextChannelAsync("chat", func => { func.CategoryId = newCategory.Id; func.IsNsfw = true;  });
            if (chatChannel == null)
            {
                await Utilities.SendDevLogMessage(1, $"Text channel could not be created. Guild {(ulong)command.GuildId}");
                string errorMessage4 = await LanguageManager.GetTranslation("addFactionTextChannelError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage4);
                return;
            }
            
            await Task.Delay(3000);

            await newCategory.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(newCategory));

            await Task.Delay(3000);

            await chatChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(chatChannel));

            await Task.Delay(3000);

            await chatChannel.AddPermissionOverwriteAsync(owner, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                  embedLinks: PermValue.Allow, attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Allow, manageMessages: PermValue.Allow,
                  createPublicThreads: PermValue.Allow, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Allow, useApplicationCommands: PermValue.Allow));

            await Task.Delay(3000);

            string message = await AddFactionIntoDB(command.User.Id, owner.Id, guild.Id, newCategory.Id, chatChannel.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);

            if ( await PermissionManager.HasUserAcceptTos(owner.Id) && await PermissionManager.IsUserBlockingBotDM(owner.Id) == false)
            {
                string messageOwner = await LanguageManager.GetTranslation("newFactionOwnerMessage", owner.Id, "", chatChannel.Id);
                await owner.SendMessageAsync(messageOwner);
            }
        }

        /// <summary>
        /// Saves a new faction into database.
        /// </summary>
        internal static async Task<string> AddFactionIntoDB(ulong adminId, ulong ownerId, ulong guildId, ulong categoryId, ulong channel)
        {
            int factionId = Convert.ToInt32( await MySqlWrapper.SQLExecuteScalar(
                "INSERT INTO `factions` (`owner_id`, `admin_id`, `guild_id`, `category_id`) VALUES (@owner_id, @admin_id, @guild_id, @category_id); SELECT LAST_INSERT_ID()",
                new Dictionary<string, object>() { { "admin_id", adminId }, { "owner_id", ownerId }, { "guild_id", guildId }, { "category_id", categoryId} }));

            if (factionId <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Faction was not saved to database! Guild: {guildId}, owner: {ownerId}, admin: {adminId} and category: {categoryId}");
                return await LanguageManager.GetTranslation("errorSavingNewFaction", adminId);
            }

            await Task.Delay(1000); // Making sure the faction is inserted and the id is existing because of foreign keys.

            await AddFactionChannelIntoDB(factionId, channel, "text", 1);

            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `factions_user` (`faction_id`, `user_id`, `rank_id`) VALUES (@faction_id, @user_id, @rank_id)",
                new Dictionary<string, object> { { "user_id", ownerId }, { "faction_id", factionId }, { "rank_id", -1 } });

            if (insertCount <= 0)
                await Utilities.SendDevLogMessage(1, $"Could not add faction owner ||{ownerId}|| for faction id {factionId} into data base as member!");

            return await LanguageManager.GetTranslation("newFactionAdminMessage", adminId, "", ownerId);
        }

        /// <summary>
        /// Removes a faction from database and fetches the category channel id first to send it back.
        /// </summary>
        internal static async Task<(ulong, string)> RemoveFaction(int factionId, ulong userId)
        {
            object categoryId = await MySqlWrapper.SQLExecuteScalar(
                "SELECT `category_id` FROM `factions` WHERE `id` = @id",
                new Dictionary<string, object>() { { "id", factionId } });

            if ( categoryId == null )
                return (0, await LanguageManager.GetTranslation("channelReadError", userId));

            int deleteCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `factions` WHERE `id` = @id",
                new Dictionary<string, object>() { { "id", factionId } });

            if ( deleteCount <= 0 )
                return (0, await LanguageManager.GetTranslation("generalError", userId));

            return ( Convert.ToUInt64(categoryId), "" );
        }

        /// <summary>
        /// Fetching all guild factions and gives back a formated list as string.
        /// </summary>
        internal static async Task<string> GetAllFactionsForGuild(ulong guildId)
        {
            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT `id`, `name`, `owner_id` FROM `factions` WHERE `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "guild_id", guildId } });

            if (results == null || results.Count <= 0)
                return await LanguageManager.GetTranslation("noFactionsOnGuild", 0, await GuildManager.GetGuildLanguage(guildId));

            string message = await LanguageManager.GetTranslation("factionGuildListTitle", 0, await GuildManager.GetGuildLanguage(guildId));

            foreach (dynamic result in results)
                message += $"- **{result.id}** {result.name}: <@{result.owner_id}>\n";

            return message;
        }

        /// <summary>
        /// Checks if a user is owner of a faction and gives back false or true and a string with the faction id and name.
        /// </summary>
        internal static async Task<(bool, string)> IsUserFactionOwner(ulong userId, ulong guildId)
        {
            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT `id`, `name` FROM `factions` WHERE `owner_id` = @owner_id AND `guild_id` = @guild_id",
                new Dictionary<string, object>() { { "owner_id", userId }, { "guild_id", guildId } });

            if (results == null || results.Count <= 0)
                return (false, "");

            return (true, $"{results[0].id} - {results[0].name}");
        }

        /// <summary>
        /// Gives back true if user is member or owner in the given faction. Else gives back false and a reason translation id.
        /// </summary>
        internal static async Task<(bool, string)> IsUserFactionMember(ulong userId, int factionId)
        {
            FactionObject faction = await GetFactionData(factionId);
            if (faction == null) return (false, "factionIdDoesNotExist");

            if (faction.OwnerId == userId) return (true, "");

            if (faction.Member.ContainsKey(userId)) return (true, "");
            else return (false, "notAFactionMember");
        }

        /// <summary>
        /// Send a user a faction invite if a private message can be send.
        /// </summary>
        internal static async Task AddFactionUser(SocketSlashCommand command, string factionOwnerString, SocketGuildUser user)
        {
            bool isUserRegistered = await PermissionManager.HasUserAcceptTos(user.Id);

            if (!isUserRegistered)
            {
                string errorMessage = await LanguageManager.GetTranslation("pickedUserNotMember", command.User.Id, "", user.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            if (isUserRegistered && await PermissionManager.IsUserBlockingBotDM(user.Id))
            {
                string errorMessage = await LanguageManager.GetTranslation("userBlockDMsError", command.User.Id, "", user.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            try
            {
                var buttonBuilder = new ComponentBuilder();
                buttonBuilder.WithButton(await LanguageManager.GetTranslation("deny", user.Id), $"respond_factioninvite_deny_{factionOwnerString.Split(' ')[0]}", ButtonStyle.Danger);
                buttonBuilder.WithButton(await LanguageManager.GetTranslation("accept", user.Id), $"respond_factioninvite_accept_{factionOwnerString.Split(' ')[0]}", ButtonStyle.Success);

                string guildName = await LanguageManager.GetTranslation("unknown", user.Id);

                SocketGuild guild = StartBotInstance._client.GetGuild((ulong)command.GuildId);
                if (guild != null)
                    guildName = guild.Name;

                string inviteMessage = await LanguageManager.GetTranslation("factionMemberInviteMessage", user.Id, "", command.User.GlobalName, factionOwnerString.Split(' ')[2], guildName);

                await user.SendMessageAsync(inviteMessage, components: buttonBuilder.Build());
            }
            catch
            {
                string errorMessage = await LanguageManager.GetTranslation("userBlockDMsError", command.User.Id, "", user.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            string message = await LanguageManager.GetTranslation("userInviteSend", command.User.Id, "", user.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);

            await AddPublicChannelPermission((ulong)command.GuildId, Convert.ToInt32(factionOwnerString.Split(' ')[0]), user.Id);
        }

        /// <summary>
        /// Removes a user from a faction and removes also his channel permissions for faction channel.
        /// </summary>
        internal static async Task RemoveFactionUser(SocketSlashCommand command, string factionOwnerString, SocketGuildUser user)
        {
            string[] splitOwnerMatch = factionOwnerString.Split(' ');
            string message = "";

            (bool isPermissionRemoved, string errorMessagePermissions) = await RemoveAllChannelPermission((ulong)command.GuildId, Convert.ToInt32(splitOwnerMatch[0]), user.Id, command.User.Id);
            if (!isPermissionRemoved)
            {
                await Utilities.SendDevLogMessage(1, $"Could not remove user channel permissions. Command used by ||<@{command.User.Id}>||\n{errorMessagePermissions}");
                message = await LanguageManager.GetTranslation("couldNotRemovePermissions", command.User.Id);
            }

            int deleteCount = await MySqlWrapper.SQLExecuteNonQuery(
                "DELETE FROM `faction_user` WHERE `user_id` = @user_id AND `faction_id` = @faction_id",
                new Dictionary<string, object>() { { "user_id", user.Id }, { "faction_id", Convert.ToInt32(splitOwnerMatch[0]) } });

            if (deleteCount <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"Could not delete faction user. Command used by ||<@{command.User.Id}>||");
                string errorMessage = await LanguageManager.GetTranslation("generalError", command.User.Id);
                await command.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                return;
            }

            message += await LanguageManager.GetTranslation("userRemovedFromFaction", command.User.Id, "", user.Id);
            await command.ModifyOriginalResponseAsync(func => func.Content = message);
        }

        /// <summary>
        /// Fetches all public channel for a faction and adds basic channel permissions for the given user.
        /// </summary>
        internal static async Task AddPublicChannelPermission(ulong guildId, int factionId, ulong userId)
        {
            SocketGuild guild = Utilities.GetGuildSocket(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild from id {guildId}.");
                return;
            }

            SocketGuildUser user = guild.GetUser(userId);
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild user from id ||{userId}||.");
                return;
            }

            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `factions_channel` WHERE `faction_id` = @faction_id",
                new Dictionary<string, object>() { { "faction_id", factionId } });

            if (results.Count <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"No faction channel was found? Faction is is {factionId}.");
                return;
            }
            
            foreach (dynamic result in results)
            {
                if (result.is_public == 0)
                    continue;

                ITextChannel textChannel = null;
                IVoiceChannel voiceChannel = null;

                if (result.channel_type == "text")
                    textChannel = guild.GetChannel(result.channel_id) as ITextChannel;
                else
                    voiceChannel = guild.GetChannel(result.channel_id) as IVoiceChannel;

                if (textChannel == null && voiceChannel == null)
                {
                    await Utilities.SendDevLogMessage(1, $"Text and voice channel type was both null? Faction is is {factionId} and guild was {guildId}.");
                    continue;
                }

                if (textChannel != null)
                {
                    await textChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                        embedLinks: PermValue.Deny, attachFiles: PermValue.Deny, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny,
                        createPublicThreads: PermValue.Deny, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Deny, useApplicationCommands: PermValue.Deny));
                }
                else
                {
                    await voiceChannel.AddPermissionOverwriteAsync(user, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                        embedLinks: PermValue.Deny, attachFiles: PermValue.Deny, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Deny, manageMessages: PermValue.Deny,
                        createPublicThreads: PermValue.Deny, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Deny, useApplicationCommands: PermValue.Deny,
                        speak: PermValue.Allow, useVoiceActivation: PermValue.Allow));
                }

                await Task.Delay(3000);
            }
        }

        /// <summary>
        /// Fetches all faction channel from data base and removes all permission for the given user.
        /// </summary>
        internal static async Task<(bool, string)> RemoveAllChannelPermission(ulong guildId, int factionId, ulong userId, ulong exeUser)
        {
            SocketGuild guild = Utilities.GetGuildSocket(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild from id {guildId}.");
                return (false, await LanguageManager.GetTranslation("fetchGuildError", exeUser));
            }

            SocketGuildUser user = guild.GetUser(userId);
            if (user == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild user from id ||{userId}||.");
                return (false, await LanguageManager.GetTranslation("userDataError", exeUser));
            }

            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT `channel_id`, `channel_type` FROM `factions_channel` WHERE `faction_id` = @faction_id",
                new Dictionary<string, object>() { { "faction_id", factionId } });

            if (results.Count <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"No faction channel was found? Faction is is {factionId}.");
                return (false, await LanguageManager.GetTranslation("channelReadError", exeUser));
            }

            try
            {
                foreach (dynamic result in results)
                {
                    ITextChannel textChannel = null;
                    IVoiceChannel voiceChannel = null;

                    if (result.channel_type == "text")
                        textChannel = guild.GetChannel(result.channel_id) as ITextChannel;
                    else
                        voiceChannel = guild.GetChannel(result.channel_id) as IVoiceChannel;

                    if (textChannel == null && voiceChannel == null)
                    {
                        await Utilities.SendDevLogMessage(1, $"Text and voice channel type was both null? Faction is is {factionId} and guild was {guildId}.");
                        continue;
                    }

                    if (textChannel != null)
                        await textChannel.RemovePermissionOverwriteAsync(user);
                    else
                        await voiceChannel.RemovePermissionOverwriteAsync(user);

                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"Error while removing channel permissions from user ||{userId}|| for faction {factionId}.\n{ex}");
                return (false, await LanguageManager.GetTranslation("generalError", exeUser));
            }
            

            return (true, "");
        }

        /// <summary>
        /// Returns null if no data is found / faction does not exist.
        /// </summary>
        internal static async Task<FactionObject> GetFactionData(int factionId)
        {
            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `factions` WHERE `id` = @id",
                new Dictionary<string, object>() { { "id", factionId } });

            if (results.Count <= 0)
                return null;

            FactionObject faction = new FactionObject(factionId, results[0].name, results[0].description, results[0].owner_id, results[0].admin_id, results[0].guild_id, results[0].max_member, results[0].max_channel,
                results[0].max_ranks, results[0].points, null, null, results[0].category_id);

            faction.Member.Add(faction.OwnerId, -1);

            List<dynamic> memberList = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `factions_user` WHERE `faction_id` = @faction_id",
                new Dictionary<string, object>() { { "faction_id", factionId } });

            if (memberList.Count <= 0) return faction;

            foreach (dynamic member in memberList)
            {
                faction.Member.Add(Convert.ToUInt64(member.user_id), Convert.ToInt32(member.rank_id));
            }

            return faction;
        }

        /// <summary>
        /// Fetches all faction channel from data base and removes all permissions from the given owner. Does not remove the owner from faction data base table as owner!
        /// </summary>
        internal static async Task<string> RemoveOwnerPermissionFromAllFactionChannel(int factionId, ulong ownerId, ulong userId, ulong guildId)
        {
            SocketGuild guild = Utilities.GetGuildSocket(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild from id {guildId}.");
                return await LanguageManager.GetTranslation("fetchGuildError", userId);
            }

            SocketGuildUser owner = guild.GetUser(ownerId);
            if (owner == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild user from id {ownerId}.");
                return await LanguageManager.GetTranslation("userDataError", userId);
            }

            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT `channel_id`, `channel_type` FROM `factions_channel` WHERE `faction_id` = @faction_id",
                new Dictionary<string, object>() { { "faction_id", factionId } });

            if (results.Count <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"No faction channel was found? Faction is is {factionId} and guild was {guildId}.");
                return await LanguageManager.GetTranslation("channelReadError", userId);
            }

            try
            {
                foreach (dynamic result in results)
                {
                    ITextChannel textChannel = null;
                    IVoiceChannel voiceChannel = null;

                    if (result.channel_type == "text")
                        textChannel = guild.GetChannel(result.channel_id) as ITextChannel;
                    else
                        voiceChannel = guild.GetChannel(result.channel_id) as IVoiceChannel;

                    if (textChannel == null && voiceChannel == null)
                    {
                        await Utilities.SendDevLogMessage(1, $"Text and voice channel type was both null? Faction is is {factionId} and guild was {guildId}.");
                        continue;
                    }

                    if (textChannel != null)
                        await textChannel.RemovePermissionOverwriteAsync(owner);
                    else
                        await voiceChannel.RemovePermissionOverwriteAsync(owner);

                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                await Utilities.SendDevLogMessage(1, $"Error while removing owner permissions from ||{ownerId}|| for faction {factionId}.\n{ex}");
                return await LanguageManager.GetTranslation("generalError", userId);
            }

            return "";
        }

        /// <summary>
        /// Gives a user faction owner permission for all existing faction channel.
        /// </summary>
        internal static async Task<string> AddOwnerPermissionForAllFactionChannel(int factionId, ulong ownerId, ulong userId, ulong guildId)
        {
            SocketGuild guild = Utilities.GetGuildSocket(guildId);
            if (guild == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild from id {guildId}.");
                return await LanguageManager.GetTranslation("fetchGuildError", userId);
            }    

            SocketGuildUser owner = guild.GetUser(ownerId);
            if (owner == null)
            {
                await Utilities.SendDevLogMessage(1, $"Could not fetch socket guild user from id {ownerId}.");
                return await LanguageManager.GetTranslation("userDataError", userId);
            }

            List<dynamic> results = await MySqlWrapper.SQLExecuteReader(
                "SELECT `channel_id`, `channel_type` FROM `factions_channel` WHERE `faction_id` = @faction_id",
                new Dictionary<string, object>() { { "faction_id", factionId } });

            if (results.Count <= 0)
            {
                await Utilities.SendDevLogMessage(1, $"No faction channel was found? Faction is is {factionId} and guild was {guildId}.");
                return await LanguageManager.GetTranslation("channelReadError", userId);
            }
            
            foreach (dynamic result in results)
            {
                ITextChannel textChannel = null;
                IVoiceChannel voiceChannel = null;

                if (result.channel_type == "text")
                    textChannel = guild.GetChannel(result.channel_id) as ITextChannel;
                else
                    voiceChannel = guild.GetChannel(result.channel_id) as IVoiceChannel;

                if (textChannel == null && voiceChannel == null)
                {
                    await Utilities.SendDevLogMessage(1, $"Text and voice channel type was both null? Faction is is {factionId} and guild was {guildId}.");
                    return await LanguageManager.GetTranslation("generalError", userId);
                }       

                if ( textChannel != null)
                    await textChannel.AddPermissionOverwriteAsync(owner, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                        embedLinks: PermValue.Allow, attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Allow, manageMessages: PermValue.Allow,
                        createPublicThreads: PermValue.Allow, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Allow, useApplicationCommands: PermValue.Allow));
                else
                    await voiceChannel.AddPermissionOverwriteAsync(owner, new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow,
                        embedLinks: PermValue.Allow, attachFiles: PermValue.Allow, readMessageHistory: PermValue.Allow, manageChannel: PermValue.Allow, manageMessages: PermValue.Allow,
                        createPublicThreads: PermValue.Allow, manageWebhooks: PermValue.Deny, manageThreads: PermValue.Allow, useApplicationCommands: PermValue.Allow,
                        speak: PermValue.Allow, useVoiceActivation: PermValue.Allow));

                await Task.Delay(3000);
            }

            return "";
        }

        /// <summary>
        /// Add faction channel data into database.
        /// </summary>
        internal static async Task<int> AddFactionChannelIntoDB(int factionId, ulong channelId, string channelType, int isPublic)
        {
            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `factions_channel` (`faction_id`, `channel_id`, `channel_type`, `is_public`) VALUES (@faction_id, @channel_id, @channel_type, @is_public)",
                new Dictionary<string, object>() { { "faction_id", factionId }, { "channel_id", channelId }, { "channel_type", channelType }, { "is_public", isPublic } });

            return insertCount;
        }
    }



    /// <summary>
    /// This class is building a button for <seealso cref="AddFactionMember"/>.
    /// </summary>
    internal class FactionInviteButton : ButtonPressed
    {
        /// <summary>
        /// This constructor is a builder for the button with custom id <paramref name="factioninvite"/>.<para/>
        /// Connected to:<br/>
        /// <seealso cref="FactionManager"/><br/>
        /// <seealso cref="ButtonManager"/>
        /// </summary>
        internal FactionInviteButton(string customId) : base(customId)
        {
            WithCustomId("factioninvite");
        }

        /// <summary>
        /// This function is handling the button pressed event and executing additional functions.<para/>
        /// Function executed by:
        /// <seealso cref="ButtonManager.ButtonExecutedHandler(SocketMessageComponent)"/>
        /// </summary>
        internal async override Task OnButtonPressed(SocketMessageComponent button)
        {
            string[] splitedCustomId = button.Data.CustomId.Split('_'); // respond_factioninvite_deny_factionid
            int factionId = Convert.ToInt32(splitedCustomId[3]);

            FactionObject faction = await FactionManager.GetFactionData(factionId);
            if ( faction == null)
            {
                string errorMessage = await LanguageManager.GetTranslation("generalError", button.User.Id);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                await button.DeleteOriginalResponseAsync();

                await Task.Delay(3000);
                await Utilities.SendDevLogMessage(1, $"Could not fetch faction data from id {factionId}. Invite for user ||{button.User.Id}|| was revoked.");

                return;
            }

            if (splitedCustomId[2] == "deny")
            {
                try
                {
                    IUser owner = await StartBotInstance._client.GetUserAsync(faction.OwnerId);
                    if (owner == null) return;

                    string denyMessage = await LanguageManager.GetTranslation("userDenyedFactionInvite", owner.Id, "", button.User.GlobalName);
                    await owner.SendMessageAsync(denyMessage);
                }
                catch (Exception ex)
                {
                    await Utilities.SendDevLogMessage(2, $"User denyed faction member and owner could not be fetched. Owner id is ||{faction.OwnerId}||.\n{ex}");
                }

                await Task.Delay(3000);

                string errorMessage = await LanguageManager.GetTranslation("youDenyFactionInvite", button.User.Id, "", faction.Name);
                await button.ModifyOriginalResponseAsync(func => func.Content = errorMessage);
                await button.Message.DeleteAsync();

                return;
            }

            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `factions_user` (`faction_id`, `user_id`, `rank_id`) VALUES (@faction_id, @user_id, @rank_id)",
                new Dictionary<string, object> { { "user_id", button.User.Id }, { "faction_id", factionId }, { "rank_id", 0 } });

            if (insertCount <= 0)
                await Utilities.SendDevLogMessage(1, $"User was not added to faction in DB! User id is ||{button.User.Id}|| and faction is {factionId}.");

            await FactionManager.AddPublicChannelPermission((ulong)button.GuildId, factionId, button.User.Id);

            string acceptMessage = await LanguageManager.GetTranslation("youAcceptedFactionInvite", button.User.Id, "", faction.Name);
            await button.ModifyOriginalResponseAsync(func => func.Content = acceptMessage);
            await button.Message.DeleteAsync();
        }
    }



    public class FactionObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ulong OwnerId { get; set; }
        public ulong AdminId { get; set; }
        public ulong GuildId { get; set; }
        public int MaxMember {  get; set; }
        public int MaxChannel { get; set; }
        public int MaxRanks { get; set; }
        public long Points { get; set; }
        public Dictionary<ulong, int> Member { get; set; } = new Dictionary<ulong, int>();
        public Dictionary<int, FactionRankObject> Ranks {  get; set; } = new Dictionary<int, FactionRankObject>();
        public ulong CategoryId { get; set; }

        internal FactionObject(int id, string name, string description, ulong ownerId, ulong adminId, ulong guildId, int maxMember, int maxChannel, int maxRanks, long points, 
            Dictionary<ulong, int> member, Dictionary<int, FactionRankObject> ranks, ulong categoryId)
        {
            Id = id;
            Name = name;
            Description = description;
            OwnerId = ownerId;
            AdminId = adminId;
            GuildId = guildId;
            MaxMember = maxMember;
            MaxChannel = maxChannel;
            MaxRanks = maxRanks;
            Points = points;
            Member = member ?? new Dictionary<ulong, int>();
            Ranks = ranks ?? new Dictionary<int, FactionRankObject>();
            CategoryId = categoryId;
        }
    }

    public class FactionRankObject
    {
        public int FactionId { get; set; }
        public int RankId { get; set; }
        public string Name { get; set; }


        public bool View {  get; set; }
        public bool Write { get; set; }
        public bool ReadHystorie { get; set; }

        public FactionRankObject(bool view, bool write, bool readHystorie)
        {
            View = view;
            Write = write;
            ReadHystorie = readHystorie;
        }

    }
}
