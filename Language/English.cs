
using System.Collections.Generic;



namespace LCNET_Management_Bot.Language
{
    /// <summary>
    /// Contains all english text strings for the <seealso cref="LanguageManager"/>.
    /// </summary>
    public class English
    {
        /// <summary>
        /// Contains all english text strings<para/>
        /// Key - is a name string.<br/>
        /// Value - is a string, the actuall text output in this language.
        /// </summary>
        public static Dictionary<string, string> LanguageDictionary = new Dictionary<string, string>()
        {
            // The first string is the name-id of this translation text, dont change it!
            // The second string is the text output, you can change that at any time. But you have to restart the bot to update the text.
            // Is you use string.Formate you can add variables to the text with {0} in text line.
            // Please write informations to this inputs so it can be understand here without searching the origin.

            #region BLOCKED CONTENT
            {"blockedTextWarning",      "# Warning, deleted message!\nUser <@{0}> sent a message with blocked content. The content was the " +
                                        "following:\n\n>>> {1}"}, // 0 = user id, 1 = message content
            {"blockedTextTitle",        "Deleted message"},
            {"blacklistMessageDeleted", "A message on the server {0} has been deleted because it contains blocked content such as insults, dangerous links, or other content."}, // 0 = Guild Name
            {"wordfilterNotContains",   "[ :x: ] The specified text was not found in the database or could not be deleted."},
            {"wordListTitle",           "# Blocked content for this server:\n" },
            {"wordListEmpty",           "No entrys for the word filter found!"},
            {"blockedinputText",        "The text you provided contains words or parts of words that have been blocked by the server. Please contact the server team if you have any questions."},
            #endregion

            #region BOT TALKING
            {"botDeveloper",        "My developer is <@278561097366241300> and I am owned by the Lost City community."},
            {"iCanDo",              "I am an administration bot and my duty is to help to make this server more secure. I also have some useful functions for server administrators and users. You can see what I can " +
                                    $"do with the command `/use help` and on my Gitbook page: [Instructions for the Lost City Bot](<{Configurations.DocsUrl}>)"},
            {"botDontUnderstand",   "I'm sorry, I didn't understand what you wanted to tell me. Unfortunately, I'm not an AI and can only react to certain keywords and word chains. Please try to phrase your question differently."},
            {"timerStartet",        "Looks like you want to start a timer."},
            #endregion

            #region BUTTON
            {"buttonLoading",                   "The button is being checked and the content is being loaded.\nIf this message remains for more than 30 seconds, then an error has occurred!"},
            {"buttonPressedToFast",             "Woah! Slowly, you only need to press the button once. Please wait a moment before pressing again."},
            {"buttonUnknown",                   "[ :x: ] An error occurred and was reported automatically. The button was not recognized by the system.\nPlease wait for the response from the bot developer."},
            {"ticketEditButton",                "Reopen ticket"},
            {"buttonAcceptBotForGuild",         "Add bot and accept GDPR"},
            {"buttonAcceptBotForGuildRepeat",   "[ :x: ] The bot has already been registered."},
            {"buttonAcceptMember",              "Accept as member"},
            {"buttonRejectMember",              "Reject as a member" },
            {"buttonGetMember",                 "Accept GDPR and get member"},
            {"buttonAcceptUserTOS",             "Accept GDPR and register for bot"},
            #endregion
            
            #region COMMAND HELPER
            {"command_dev_news",            "Sends an embed to all registered server."},

            {"command_guild_help",          "Create a ticket to contact the server team."},
            {"command_guild_register",      "Register your server for the bot."},
            {"command_guild_removedata",    "Removes all data related to your server."},
            {"command_guild_gated",         "Sends the message for gated communities."},
            {"command_guild_channel",       "Set the channel for bot messages."},
            {"command_guild_member",        "Set the role for your member."},
            {"command_guild_permissions",   "Set the role for mods or admins."},
            {"command_guild_pointname",     "Set the name for your server points."},
            {"command_guild_settings",      "Activate or deactivate functions for your server."},
            {"command_guild_tickets",       "Set the category for your ticket system."},
            {"command_guild_language",      "Change the server language for the bot."},
            {"command_guild_voice",         "Set the category for temporary voice channels."},

            {"command_admin_status",        "Shows the status of your server."},
            {"command_admin_help",          "List of all commands for admins."},
            {"command_admin_invite",        "Set the invite link for your server."},
            {"command_admin_wordadd",       "Add something to the word filter."},
            {"command_admin_wordremove",    "Remove something from the word filter."},
            {"command_admin_sticky",        "Sends a sticky embed into a channel."},
            {"command_admin_stopsticky",    "Stops a sticky message."},
            {"command_admin_points",        "Give or take server points from a user."},
            {"command_admin_roles",         "Sends the message for user roles."},
            {"command_admin_rolechange",    "Add or remove user roles."},
            {"command_admin_report",        "Report a user for all connected guilds."},
            {"command_admin_color",         "Add or remove a color role."},
            {"command_admin_channel",       "Set special channel."},
            {"command_admin_remchannel",    "Delete a special channel."},

            {"command_mod_embed",           "Send an embed in the text channel."},
            {"command_mod_ticket",          "Closes a ticket for the user."},
            {"command_mod_help",            "List of all commands for mods."},
            {"command_mod_seereport",       "See all reports for a user."},
            {"command_mod_setrule",         "Set rules for a channel."},

            {"command_use_help",            "List of all commands for users."},
            {"command_use_register",        "Register your discord account for the bot and its functions."},
            {"command_use_deletedata",      "Permanently delete all data for your account."},
            {"command_use_ranks",           "Shows the top list of this server and your rank."},
            {"command_use_ticket",          "Create a ticket to contact the server team."},
            {"command_use_invite",          "Shows you the invite link to this server."},
            {"command_use_botdm",           "Set if the bot is allowed to send you private messages."},
            {"command_use_language",        "Set your language."},
            {"command_use_stat",            "Get your points, event information and user stats."},
            {"command_use_rules",           "Shows you the rules for a specific channel."},
            {"command_use_color",           "Shows all color roles."},
            {"command_use_colorrole",       "Buy a color role for yourself."},
            {"command_use_pubremind",       "Set a public reminder in the used channel."},
            {"command_use_voice",           "Add a temp voice channel."},

            {"command_fact_new",            "[Owner, Admin] Create a new faction and its leader."},
            {"command_fact_guildlist",      "[Owner, Admin, Mod] See all existing factions on your server."},
            {"command_fact_owner",          "[Owner, Admin] Change the faction leader of an existing faction."},
            {"command_fact_remove",         "[Owner, Admin] Delete a faction completely."},
            {"command_fact_removemember",   "[Leader] Remove a member from your faction."},
            {"command_fact_addmember",      "[Leader] Add a member to your faction."},
            {"command_fact_name",           "[Leader] Set the name and description for your faction."},
            {"command_fact_help",           "[Anyone] Shows you all faction commands."},

            {"command_winter_advent",       "Open a door on your advent calender."},
            {"command_winter_work",         "Do some winter work to earn winter points."},

            {"command_wow_help",            "Shows all wow commands."},
            {"command_wow_addchar",         "Save or update your WoW character for this server."},
            {"command_wow_delchar",         "Delete a character from your list."},
            #endregion

            #region DM
            {"userBlocksDMs",       "[ :x: ] An attempt was made to send a message to user <@{0}>. However, this user has not approved his messages. Please check whether the user " +
                                    "should be informed in another way."}, // 0 = user id
            {"userBlockDMsError",   "[ :x: ] An attempt was made to send a message to user <@{0}>, but they block private messages. Since the user must confirm something, this function cannot " +
                                    "be used. The user must open their private messages for the bot."}, // 0 = user id
            #endregion

            #region FACTIONS
            {"newFactionAdminMessage",      "[ :white_check_mark: ] You have created a new faction. The faction owner is <@{0}> and has been notified when their DMs are open."},
            {"errorSavingNewFaction",       "[ :x: ] Something went wrong while creating the faction. The error was reported automatically."},
            {"addFactionCategoryError",     "[ :x: ] An error occurred while creating the category. The error was reported automatically."},
            {"addFactionTextChannelError",  "[ :x: ] An error occurred while creating the channel. The error was reported automatically."},
            {"factionGuildListTitle",       "# Server factions\n"},
            {"noFactionsOnGuild",           "There is currently no faction registered on this server."},
            {"userIsAlreadyOwner",          "[ :x: ] User is already the leader of a faction: {0}. User can only lead one faction at a time."}, // 0 = id and faction name
            {"notFactionLeader",            "[ :x: ] This can only be used as a faction leader and you are not a faction leader."},
            {"userRemovedFromFaction",      "[ :white_check_mark: ] User <@{0}> has been removed from your faction."}, // 0 = user id
            {"couldNotRemovePermissions",   "**Warning** Not all channel permissions could be removed for this user. Please check your faction channels and remove the user permissions yourself.\n\n"},
            {"userInviteSend",              "[ :white_check_mark: ] An invitation has been sent to <@{0}>. The person has received a private message from the bot and must now confirm the invitation."},
            {"removedFaction",              "[ :white_check_mark: ] Faction was removed completly!"},
            {"factionOwnerChanged",         "[ :white_check_mark: ] The faction leader was successfully changed."},
            {"newFactionOwnerMessage",      ":tada: You've been appointed faction leader of a new faction! Check out your first faction channel: <#{0}>"}, // 0 = channel id
            {"factionOwnerTransfered",      ":tada: You become the new owner of the faction {0}!"}, // 0 = faction name
            {"factionMemberMaxCount",       "[ :x: ] The faction has already reached its maximum number of members. No more can be invited. Increase your maximum number of members."},
            {"youDenyFactionInvite",        "You denyed the invite to the faction **{0}**!"}, // 0 = faction name
            {"userDenyedFactionInvite",     "Your invitation to **{0}** to join your faction was declined."}, // 0 = invited user name
            {"youAcceptedFactionInvite",    "[ :white_check_mark: ] You have accepted the invitation. Welcome to the **{0}** faction!"}, // 0 = faction name
            {"factionMemberInviteMessage",  "# Faction Invitation\\nHello, you have been invited by **{0}** to the faction **{1}** on the server {2}.\\nIf you click \"Accept\", you will be granted faction " +
                                            "rights and will be able to view its channels on the server. If you do not wish to become a member, you can ignore this message or click \"Decline\" to inform the " +
                                             "faction leader that you decline the invitation."}, // 0 = faction owner name, 1 = faction name, 2 = server name
            {"userIsAlreadyFactionUser",    "User is already faction member!"},
            {"userIsNotInFaction",          "[ :x: ] Function not possible because the user does not belong to the faction."},
            {"factionIdDoesNotExist",       "[ :x: ] Faction id invalid, there is no faction with the id **{0}**."}, // 0 = faction id
            {"notFactionMember",            "The user is not a faction member."},
            {"factionMemberListTitle",      "All member of the faction **{0}**:"}, // 0 = faction name
            {"factionMemberListOwner",      "Leade is <@{0}>\n\n"}, // 0 = name of the faction owner
            #endregion

            #region HALLOWEEN
            {"halloweenIntroduction",       "# Halloween 2025\nThis year's event starts today!\n**Trick or treat!**\n\nYou can play the event undisturbed in this channel. Collect as many sweets as possible and try " +
                                            "to play pranks or avoid pranks directed at you."},
            {"halloweenSearchButton",       "Search for candy"},
            {"halloweenCandyButton",        "Show my candys"},
            {"halloweenDoTrickhButton",     "Play a prank on someone!"},
            {"halloweenDefendTrickButton",  "Defend yourself for a prank"},
            {"halloweenAlreadyUsed",        "[ :x: ] You can only perform or prevent a prank once per day. You've already performed one action today!"},
            {"halloweenFoundCandy",         "# Candy Tour\nYou go from house to house, but not everyone has something ready for you.\nYou found **{1}**x **{0}**!"}, // 0 = candy name, 1 0 candy amount
            {"halloweenNoCandy",            "You haven't found any candy yet."},
            {"halloweenUserCandyList",      "# Your candys\nYou have collected the following so far:\n{0}\nTotal points collected: **{1}**"}, // 0 = list of candy, 1 = points
            {"halloweenCooldown",           "[ :x: ] You can only search for sweets every 30 minutes. You can search again at {0}."}, // 0 = next possible time
            {"halloweenNoCandyStolen",      "You couldn't find anyone to play a prank on."},
            {"halloweenPrankNotSuccess",    "[ :x: ] You wanted to play a prank on someone, but they were prepared! Your prank failed, and unfortunately, you got nothing."},
            {"halloweenStolenCandyMessage", "[ :white_check_mark: ] You have successfully played a prank and looted {0} x **{1}**!"}, // 0 = candy amount, 1 = candy name
            {"halloweenProtection",         "[ :white_check_mark: ] You have prepared yourself for a prank **today** and will be able to escape **a** prank."},
            #endregion

            #region HELP TEXT
            {"helpTitleGuild",      "Commands for server owner:\n"},
            {"helpTextGuild",       "# Commands for guild owner\n`/guild channel` - Set the guild channel for news and logs.\n`/guild gated` - Send the gated community message.\n`/guild member` - Set the member role for gated community.\n" +
                                    "`/guild permissions` - Set the permissions for bot usage.\n`/guild points` - Change the name of the points system.\n`guild removedata` - Removing all date relevant to your server.\n" +
                                    "`/guild settings` - Change settings for your server.\n`/guild status` - Shows your server settings.\n`/guild tickets` - Set the category for tickets."},
            {"guildStatusText",     "## Informations about the server\nCreated on: {9} by {10}.\nDescription: {11}\n\nThe server is currently boosted {12} times and is therefore at " +
                                    "boost level {13}.\nThe server has `{14}` specified as the country with the country flag {15} and the voice channels are hosted in {16}.\nThe " +
                                    "verification level is {17} and the server's NSFW level is {18}.\n\nNumber of...\nEmotes: {19}\nStickers: {20}\nRoles: {21}\nCategories: {22}\n" +
                                    "Total channels: {23}\nVoice channels: {24}\nText channels: {25}\nForums: {26}\nThreads: {27}\nStages: {28}\n\nThere are currently {29} events taking " +
                                    "place.\n\n## Bot settings\nInvitelink: {0}\nBot Adminrole: <@&{1}>\nBot Modrole: <@&{2}>\nBot Memberrole: <@&{3}>\nName for points: {5}\nGated " +
                                    "Community: {4}\nWordfilter active: {6}\nCheck deleted messages: {7}\nTickets active: {8}\n\n# Category and channel:\n{30}"}, 
                                    // 0 = InviteLink, 1 = AdminRole, 2 = ModeratorRole, 3 = MemberRole, 4 = IsGatedCommunity,
                                    // 5 = PointsName, 6 = UseWordfilter, 7 = CheckDeletedMessages, 8 = TicketsActive, 9 = creation date, 10 = owner name,
                                    // 11 = server description, 12 = amount boosts, 13 = boost level, 14 = culture name, 15 = culture id, 16 = voice region name, 17 = verification level,
                                    // 18 = nsfw level, 19 = count emotes, 20 = count sticker, 21 = count roles, 22 = count category, 23 = count channel, 24 = voice channel,
                                    // 25 = count text channel, 26 = count forum, 27 = count thread, 28 = count stages, 29 = count events, 30 = channel list
            {"helpTitleUser",       "This are the user commands:\n"}, // 0 = guild name
            {"imprintGDPR",         $"[Data protection and imprint](<{Configurations.GDPRUrl}>)"},
            {"helpTitleAdmin",      "Commands for administrator:\n"},
            {"installationLink",    $"[Bot functions](<{Configurations.DocsUrl}>)"},
            {"helpTitleMod",        "Commands for moderator:\n"},
            {"helpTitleFaction",    "Commands for factions:\n"},
            #endregion

            #region MODAL
            {"modalLoading",    "The modal is being checked and the content is being loaded.\nIf this message remains for more than 30 seconds, then an error has occurred!"},
            {"modalUnknown",    "The used modal was not recognised. The error was reported automaticly."},
            {"embedToLong",     "Embed could not be validated, character limit is 6000 and the given embed has more characters in it. Please check the entry or report this to an admin."},
            #endregion

            #region PERMISSION
            {"notMember",           "[ :x: ] You need the member role to be able to use bot commands."},
            {"pickedUserNotMember", "[ :x: ] The selected user is not a member, so the function cannot be performed."},
            {"alreadyMember",       "You are already a member of this community!"},
            {"missingMemberRole",   "[ :x: ] The member role could not be found. Please check that you have setup a member role!"},
            {"missingPermisson",    "[ :x: ] You do not have the necessary permissions to use this."},
            {"youAreBlockedGlobal", "[ :x: ] You are banned from this bot across all servers. Unbanning can only be done by the bot developer."},
            #endregion

            #region POINTS
            {"notEnoughGuildPoints",        "[ :x: ] You dont have enough server points to do thid action. You need **{0}** server points to do this."}, // 0 = needed guild points
            #endregion

            #region REGISTER USER
            {"registerMessage",         "[ :white_check_mark: ] You have registered for the Lost City Management bot. This bot stores the data for its functions. In order to be able to assign this data " +
                                        "to your account, your Discord ID is also stored. You can have all data about you deleted at any time. The bot developer is responsible for collecting and managing the data."},
            {"alreadyRegistered",       "[ :x: ] You are already registered."},
            {"registerCanceled",        "[ :x: ] Registration failed. The error was reported automatically, please wait until an administrator contacts you."},
            {"registerNeddPermission",  "[ :x: ] You can only register for the bot if you agree to the GDPR!"},
            {"accountToJung",           "The account is less than 7 days old, so your membership request has been sent to the server team. Please open your private messages in case the server team has any questions."},
            {"userAccountToJung",       "Attention, the user's account is less than 7 days old."},
            {"needToBeRegistered",      "[ :x: ] To use this function we need to store data, so you must be registered. To do so, use `/use register` and accept the GDPR."},
            {"registerUserInfoText",    "Hello **{}**!\nIf you want to use my functions that store your data, you must first agree that I am allowed to store it. I will not read or store any of your data " +
                                        "until you have registered. Push the `Accept GDPR and register for bot` button if you want to register your account and accepting my " +
                                        $"[terms of service and GDPR](<{Configurations.GDPRUrl}>)."}, // 0 = user name
            #endregion

            #region REGISTER SERVER
            {"guildAlreadyRegistered",      "[ :x: ] Your server is already registered."},
            {"guildRegisterMessage",        "You are about to register your server for the Lost City Bot.\nPress the `Add bot and accept GDPR` button to register your server and accept the " +
                                            $"[Terms of Use and GDPR](<{Configurations.GDPRUrl}>) of Lost City."},
            {"registrationMissingBot",      "[ :x: ] The bot has not been registered yet, so this function cannot be used! The server owner must register the bot first."},
            {"registerGuildDatabaseError",  "[ :x: ] Your server could not be registered! The error was reported automatically. The bot developer will contact you."},
            {"guildRegisterSuccess",        "[ :white_check_mark: ] Your server has been successfully registered! Please follow now the installation instructions from our " +
                                            $"[GitBook](<{Configurations.DocsUrl}>). If you need help, please contact Vivian."},
            {"registerGuildOwnerDM",        "Hello {0}!\nYou asked to add me to your server **{1}**.\nI am now here to help you manage your server. " +
                                            $"Please have a look at my [GitBook](<{Configurations.DocsUrl}>). There you will find all the explanations of my functions.\n\n" +
                                            "If you did not ask to invite me to your server, you can simply kick me. I do not read any data and do nothing as long as you dont have registered me. Push the " +
                                            $"`Add bot and accept GDPR` button if you want to register me and if you are accepting my [terms of service and GDPR](<{Configurations.GDPRUrl}>). " +
                                            "With pressing the button, you will also set your personal and the server language ( you can also change both later )."}, // 0 = guild owner name, 1 = guild name
            #endregion

            #region REMINDER
            {"reminderTimeNotMatching",     "[ :x: ] Wrong time format. You need to use a 24 hour format like \"00:00\"!"},
            {"reminderDateNoMatching",      "[ :x: ] Falsches Datumsformat. Du musst das Datum mit den Querstrichen ( slashes )angeben. Tag/Monat/Jahr also so schreiben: 28/01/2024"},
            {"reminderWeekdayNotMatching",  "[ :x: ] Invalid weekday. You need to write a weekday like \"Monday\" or \"Saturday\". Capitalization is irrelevant."},
            {"reminderDurationNotMatch",    "[ :x: ] The duration of a reminder must be at least 2 days and may be a maximum of 10 days."},

            {"dailypubreminderSaved",       "[ :white_check_mark: ] A daily reminder is saved! Every day at {0} o´clock a reminder will be posted in the channel <#{1}>."}, // 0 = time, 1 = channel id
            {"weeklypubreminderSaved",      "[ :white_check_mark: ] A weekly reminder is saved! Every {2}} at {0} o´clock a reminder will be posted in the channel <#{1}>."}, // 0 = time, 1 = channel id, 2 = weekday
            {"datepubreminderSaved",        "[ :white_check_mark: ] A reminder for the date {0} at {1} o´clock is saved and will be posted on the channel <#{2}>."}, // 0 = date, 1 = time, 2 = channel id
            {"durationpubreminderSaved",    "[ :white_check_mark: ] A running reminder has been created for the channel <#{0}> with a duration of {1} days. The reminder will always be posted at {2} " +
                                            "o´clock."}, // 0 = channel id, 1 = duration time, 2 = time
            #endregion

            #region REPORTS
            {"noUserReports",           "No entrys found for this user."},
            {"userReportTitle",         "# The user <@{0}> has the following reports:\n"}, // 0 = user name
            {"reportEmpty",             "A report can not be empty and needs to be at least 4 character long, an example: \"spam\"."},
            {"reportInserted",          "[ :white_check_mark: ] The report was successfully send."},
            {"ReportInsertError",       "[ :x: ] Report could not be saved. Please try again and contact the bot developer if this error remains."},
            {"userReportEmbedTitle",    "New user has joined the server"},
            #endregion

            #region ROLES
            {"youGotTheRole",               "[ :white_check_mark: ] You have been assigned the role **{0}**."}, // 0 = role name
            {"youTossedTheRole",            "[ :white_check_mark: ] You have been tossed the role **{0}**."}, // 0 = role name
            {"youGotTheRoleAlready",        "[ :x: ] You already have the role **{0}**!"}, // 0 = role name
            {"requestMemberRole",           "A request has been sent to the server team. Please be patient until you receive your role or a response from them."},
            {"requestMemberRoleTeam",       "<@{0}> would like to be activated as a member. **Never** assign the role manually! Click on the button to give the role to the user."}, // 0 = user id
            {"memberAccepted",              "User <@{0}> has been accepted as a member by team member <@{1}>."}, // 0 = user id, 1 = team member id
            {"youGotMember",                "You have been accepted as a member of the server **{0}** and have been assigned the role **{1}**."}, // 0 = server name, 1 = role name
            {"membershipDenied",            "[ :x: ] You have been rejected as a member on the server **{0}**. This decision comes from the server team and not the bot."}, // 0 = server name
            {"memberDenied",                "User <@{0}> was rejected as a member by <@{1}>."}, // 0 = user id, 1 = team member id
            {"notFoundAnyUserRoles",        "No user roles found for this server. Please send a message to the bot developer if this is an error."},
            {"allUserRolesForGuildRemoved", "[ :white_check_mark: ] All user roles are removed for this server."},
            {"sendUserRolesMessage",        "# User roles\nYou can take an additional role and give it back at any time. To do this, simply click on the buttons below this text."},
            {"userRolesTitel",              "# User roles of this server\n"},
            {"roleIsSystemRole",            "[ :x: ] You can't make a system role a user role! This role is either a member, moderator, or administrator in the bot system."},
            {"userRoleAlreadyAdded",        "[ :x: ] The role **{0}** is already registered as user role."}, // 0 = role name
            {"roleNotSavedAsUserRole",      "[ :x: ] The role **{0}** is not a user role."}, // 0 = role name
            {"userRoleAdded",               "[ :white_check_mark: ] The role **{0}** was added to the user roles."}, // 0 = role name
            {"userRoleRemoved",             "[ :white_check_mark: ] The role **{0}** was removed from user roles."}, // 0 = role name
            {"roleAlreadyAdded",            "The role is already added to the list."},
            {"colorRoleAdded",              "[ :white_check_mark: ] The role <@&{0}> was added as color role."}, // 0 = role id
            {"colorRoleRemoved",            "[ :white_check_mark: ] The role **{0}** was removed as color role."}, // 0 = role id
            {"noColorRolesForGuild",        "There are no color roles saved for this server."},
            {"getGuildColorList",           "# Color roles\nThis roles are saved as color roles:\n{0}"}, // 0 = string with role list
            {"roleIsNotColorRole",          "[ :x: ] The selected role is not a color role and cant be bougth."},
            {"alreadyOwnedColorRole",       "[ :x: ] You own this role already. No need to buy it again."},
            #endregion

            #region SELECT MENU
            {"selectMenuLoading",   "The select menu is being checked and the content is being loaded.\nIf this message remains for more than 30 seconds, then an error has occurred!"},
            {"selectMenuUnknown",   "[ :x: ] An error occurred and was reported automatically. The select menu was not recognized by the system.\nPlease wait for the response from the bot developer."},
            #endregion

            #region SLASH COMMAND
            {"commandLoading",      "The command is being checked and the content is being loaded.\nIf this message remains for more than 30 seconds, then an error has occurred!"},
            {"commandNotExisting",  "[ :x: ] This command is not existing or deprecated and unfortunately has not yet been removed from Discord."},
            {"commandNotInDM",      "[ :x: ] You cannot use commands in private conversations! Use the command on a server that uses our bot."},
            {"commandValueInvalid", "[ :x: ] One or more entries in the command options were incorrect. Please pay attention to which data type (text, number, role, channel, etc.) you are asked to specify."},
            {"commandOutdated",     "[ :x: ] This command is not existing or deprecated and unfortunately has not yet been removed from Discord."},
            #endregion

            #region STICKY
            {"stickyMessageTitle",          "Sticky message was send"},
            {"stickyMessageText",           "In <#{0}> was added a new sticky message {1}."}, // 0 = channel id, 1 = message link
            {"stickyMessageRemoved",        "The sticky message was removed!"},
            {"stickyMessageRemovedError",   "Something went wrong while removing the sticky message."},
            #endregion

            #region SYSTEM GENERAL
            {"dataSaved",               "[ :white_check_mark: ] Your data have been saved successfully!"},
            {"deletedUserData",         "[ :white_check_mark: ] All data associated with your account has been deleted!"},
            {"deletedGuildData",        "[ :white_check_mark: ] All data related to your server has been deleted!"},
            
            {"removeGuildDataLabel",    "Delete all server data irrevocably"},
            {"removeGuildDataMessage",  "# Attention!\nYou are about to delete **ALL** data on your server. This data cannot be restored. All settings, points, events and all information about and for functions " +
                                        "will be deleted. If you really want to do this, then click on the red button."},
            {"removeUserDataLabel",     "Delete irrevocably all your user data"},
            {"removeUserDataMessage",   "# Attention!\nYou are about to delete **ALL** data on your account. This data cannot be restored. All settings, points, events and all information about and for functions " +
                                        "will be deleted. If you really want to do this, then click on the red button."},
            {"pendingAction",           "You have already used this. Please wait for a response or report it to the server team if you think this is a bug."},
            {"deleteMessageTitle",      "A user deleted a message. This was the user and the content of the message:"},
            {"userDeletedThereData",    "[ :grey_exclamation: ] The affected user has deleted there data."},
            {"urlInvalid",              "The url is invalid. Please check the link."},
            {"getInviteLink",           "Send this link to someone you want to invite to the server:\n```{0}```"}, // 0 = invite link url
            {"missingInvite",           "There is no invite link saved. Please ask the server staff about the invite."},
            {"titleReactionDeleted",    "Reactionmessage was deleted!"},
            {"messageReactionDeleted",  "A reaction message was deleted. These messages are linked to bot functions. If this was not planned, please check.\nThe message was in channel <#{1}> and the message ID was: {0}."}, // 0 = message id, 1 = channel id
            {"noRulesFound",            "No rules found for this channel."},
            {"userLeftGuild",           "The user **{0}** with Id ||{1}|| has left the server."}, // 0 = user left name, 1 = user left id
            {"userLeftGuildTitle",      "User left the server"},

            {"unknown",                 "Unknown"},
            {"deny",                    "Decline"},
            {"accept",                  "Accept"},
            {"rank",                    "Rank"},

            {"generalError",            "[ :x: ] An error occurred. The error was reported automatically."},
            {"saveDataError",           "[ :x: ] An error occurred while saving data. The error was reported automatically."},
            {"userDataError",           "[ :x: ] An error occurred while reading user data. Is the given user still on the server?"},
            {"channelReadError",        "[ :x: ] An error occurred while reading a channel. The selected channel may no longer exist or may be the wrong type."},
            {"roleReadError",           "[ :x: ] An error occurred while reading a role. The selected role may no longer exist."},
            {"fetchGuildError",         "[ :x: ] An error occurred while reading discord server data. The error was reported automatically."},
            {"wrongFormatNumber",       "[ :x: ] The given value can only contain numbers!"},
            {"noUserDataFound",         "[ :x:] No data was found associated with your account."},

            {"channelNotCategory",      "[ :x: ] You must select a category for this feature. The specified channel is not a category, please check."},
            {"functionNotWhileTimeout", "[ :x: ] You cannot use functions while you have a timeout running. Your timeout ends at {0}!"}, // 0 = time when timeout is ending
            {"functionAfter24Hour",     "[ :x: ] You can only use the bot 24 hours after joining the server. We ask for your understanding for this security measure.\n" +
                                        "You can use the bot past {0}."}, // 0 = time when user is 24 hours on server
            {"inputTextToShort",        "[ :x: ] The specified text is too short. It must be at least {0} characters long."}, // 0 = amount of min characters
            {"noMatchingEntryName",     "[ :x: ] No user or character was found for the specified name."},

            {"delayMessageTicket1",     "Checking server data for closing ticket..."},
            {"delayMessageTicket2",     "Checking user data for closing ticket..."},
            {"delayMessageTicket3",     "Check channel data for closing ticket..."},
            {"delayMessageTicket4",     "Removing user from channel..."},

            {"serverFunctionNotActive", "[ :x: ] The server has not enabled this bot function. Please contact a server team member if you believe this is a bug."},
            {"userHasAlreadyTempVoice", "[ :x: ] You have already created a voice channel on this server. Each user may only have one voice channel created at a time."},
            {"missingGuildCategory",    "[ :x: ] This function requires a channel category to be set for the server. No category is saved. Please contact a server administrator."},
            {"tempVoiceWasCreated",     "[ :white_check_mark: ] A new temporary voice channel named **{0}** has been successfully created!"}, // 0 = voice name
            #endregion

            #region TICKETS
            {"ticketAlreadyExists",     "[ :x: ] You already have a ticket opened on this server. It is located here: <#{0}>"}, // 0 = ticket channel id
            {"ticketNoCategory",        "[ :x: ] No ticket category could be found in the server data. Please inform the server team about this."},
            {"ticketsNotActive",        "[ :x: ] Tickets are disabled for this server."},
            {"ticketBotMessage",        "# A new ticket has been created!\n<@{0}> has a request for the server team. Please write here what it is about."}, // 0 = user id
            {"ticketCloseText",         "# Ticket closed\n<@{0}> has closed the ticket and the user has been removed.\nIf you want to reopen the ticket, click the button."},
            {"ticketReopened",          "The ticket has been reopened."},
            {"ticketOpened",            "You have opened a new ticket. You can find it in this channel: <#{0}>"}, // 0 = ticket channel id
            {"ticketChannelRemoved",    "# Ticket channel was removed\nA channel with an open ticket was removed! The ticket belonged to the user <@{0}>."}, // 0 = ticket owner id
            #endregion

            #region USER
            {"noRanksFound",                    "Nobody has gotten points on this server so far."},
            {"rankListText",                    "{0}. <@{1}> with level {2} ( {3} Points )\n"}, // 0 = rank, 1 = user id, 2 = level, 3 = points
            {"ranksCheckingUser",               "Ranking is loading, this may take a few seconds. Please wait..."},
            {"getMemberInfoText",               "# Welcome to the server **{0}**!\nTo join our server and use the functions of the LCNET bot, you need the member role <@&{1}>.\nBy accepting the " +
                                                $"role, you agree to our rules and the [GDPR](<{Configurations.GDPRUrl}>) of the LCNET bot."}, // 0 = guild name, 1 = member role id
            {"getMemberClosedInfoText",         "# Welcome to the server **{0}**!\nTo join our server and use the functions of the LCNET bot, you need the member role <@&{1}>.\nPress the button to " +
                                                "send a request to the server team. They will then decide whether you will become a member or not. By accepting the role, you agree that a server team " +
                                                "member can send you a private message ( Open your messages to server members! ) and you agree to our rules and the " +
                                                $"[GDPR](<{Configurations.GDPRUrl}>) of the LCNET bot."}, // 0 = guild name, 1 = member role id
            {"userStatusMessage",               "# <@{2}>\nGot **{0}** bot points.\nCollected **{1}** winter event points.\n\n"}, // 0 = general bot points, 1 = winter event points
            {"userStatusNoGuildPoints",         "Did not collect any server points jet."},
            {"userStatusGuildPointsCaption",    "Collected server points:\n"},
            {"userStatusGuildPoints",           "- {0}, {1}: {2}\n"}, // 0 = guild name, 1 = guild points name, 2 = guild points
            {"logTitleGuildPointsChanged",      "Server points was changed by a command"},
            {"logMessageGuildPointsChanged",    "Team member **{0}** changed server points for user **{1}**. The change type was \"{2}\" and the points were **{3}**."}, // 0 = admin name,
                                                // 1 = user name, 2 = changeType, 3 = points
            #endregion 

            #region WINTER
            {"itsNotAdventTime",            "[ :x: ] Its not christmas time. The advanet calender can only be used from 01 - 24 December."},
            {"itsNotWinterTime",            "[ :x: ] This function can only be used in winter time, december."},
            {"adventAlreadyOpend",          "[ :x: ] You already opende your advent door for today. This function is global and can only be used once on any server."},
            {"adventDoorDescription",       "You open your door today and find a **poem** and **100** winter points!\n\n{0}\n\n" +
                                            "You will also find a gift: **{1}**\n\n{2}"}, // 0 = poem, 1 = item name, 2 = item card url
            {"cantDoWinterWorkNow",         "[ :x: ] Your busy now. You can work at {0} again."}, // 0 = work time
            {"doWinterWork",                "[ :white_check_mark: ] You are working für 15 minutes and will get **10** winter points for it."},
            #endregion

            #region WOW
            {"wowheal",     "Healer"},
            {"wowtank",     "Tank"},
            {"wowdamage",   "Damagedealer"},

            {"wowdruid",        "Druid"},
            {"wowwitchdoctor",  "Witchdoctor"},
            {"wowhunter",       "Hunter"},
            {"wowwarrior",      "Warrior"},
            {"wowmage",         "Mage"},
            {"wowpaladin",      "Paladin"},
            {"wowpriest",       "Priest"},
            {"wowshaman",       "Shaman"},
            {"wowrough",        "Rough"},

            {"wowalchemy",      "Alchemy"},
            {"wowblacksmith",   "Blacksmith"},
            {"wowenchanter",    "Enchanter"},
            {"wowingenier",     "Ingenier"},
            {"wowjuwelcraft",   "Juwelcraft"},
            {"wowleatherwork",  "Leatherwork"},
            {"wowtailor",       "Tailor"},
            {"wowherbalist",    "Herbalist"},
            {"wowmining",       "Mining"},
            {"wowskinning",     "Skinning"},
            #endregion
        };
    }
}
