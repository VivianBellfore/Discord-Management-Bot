
using System.Collections.Generic;



namespace LCNET_Management_Bot.Language
{
    /// <summary>
    /// Contains all german text strings for the <seealso cref="LanguageManager"/>.
    /// </summary>
    public class German
    {
        /// <summary>
        /// Contains all german text strings<para/>
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
            {"blockedTextWarning",      "# Warnung, gelöschte Nachricht!\nNutzer <@{0}> hat eine Nachricht mit gesperrtem Inhalt gesendet. Der Inhalt war " +
                                        "folgender:\n\n>>> {1}"}, // 0 = user id, 1 = message content
            {"blockedTextTitle",        "Gelöschte Nachricht"},
            {"blacklistMessageDeleted", "Eine deiner Nachrichten auf dem Server {0} wurde gelöscht, da sie gesperrte Inhalte wie Beleidigungen, gefährliche Links oder anderes " +
                                        "enthalten hat."}, // 0 = Guild Name
            {"wordfilterNotContains",   "[ :x: ] Der angegebene Text wurde nicht in der Datenbank gefunden oder konnte nicht gelöscht werden."},
            {"wordListTitle",           "# Gesperrte Texte für diesen Server:\n"},
            {"wordListEmpty",           "Keine Einträge für den Wortfilter gefunden!"},
            {"blockedinputText",        "Der angegebene Text enthält vom Server blockierte Worte oder Wortteile. Bitte wende dich bei Fragen hierzu an das Serverteam."},
            #endregion

            #region BOT TALKING
            {"botDeveloper",        "Meine Entwicklerin ist <@278561097366241300> und ich gehöre der Lost City Community."},
            {"iCanDo",              "Ich bin ein Verwaltungsbot und helfe dabei diesen Server sicherer zu machen. Zusätzlich habe ich einige nützliche Funktionen für Administratoren des Servers und auch für die " +
                                    "Nutzer. Was ich alles kann, kannst du mit dem Befehl `/use help` einsehen und auf meiner Gitbookseite: " +
                                    "[Anleitung zum Lost City Bot](<https://lost-city.gitbook.io/lcnet-discord-bot/zu-diesem-gitbook>)"},
            {"botDontUnderstand",   "Tut mir leid, ich habe nicht verstanden was du mir sagen möchtest. Ich bin leider keine AI und kann nur auf bestimmte Schlagwörter und Wortketten reagieren. Bitte versuche deine " +
                                    "Frage in einer anderen Formuliereng zu stellen."},
            {"timerStartet",        "Sieht aus als möchtest du einen Timer starten."},
            #endregion

            #region BUTTON
            {"buttonLoading",                   "Der Button wird geprüft und der Inhalt geladen.\nWenn diese Nachricht länger als 30 Sekunden stehen bleibt, dann ist ein Fehler aufgetreten!"},
            {"buttonPressedToFast",             "Woah! Langsam, du brauchst den Knopf nur einmal zu drücken. Bitte warte kurz bevor du erneut drückst."},
            {"buttonUnknown",                   "[ :x: ] Es ist ein Fehler aufgetreten und automatisch gemeldet worden. Der Button wurde vom System nicht erkannt.\nBitte warte auf eine Rückmeldung des Botentwicklers."},
            {"ticketEditButton",                "Ticket wiederherstellen"},
            {"buttonAcceptBotForGuild",         "Bot annehmen und DSGVO akzeptieren"},
            {"buttonAcceptBotForGuildRepeat",   "[ :x: ] Der Bot ist bereits registriert worden."},
            {"buttonAcceptMember",              "Zum Mitglied machen"},
            {"buttonRejectMember",              "Als Mitglied ablehnen" },
            {"buttonGetMember",                 "DSGVO akzeptieren und Mitglied werden"},
            {"buttonAcceptUserTOS",             "DSGVO akzeptieren und im Bot registrieren"},
            #endregion

            #region COMMAND HELPER
            {"command_dev_news",            "Sended ein Embed zu allen registrierten Servern."},

            {"command_guild_help",          "Erstelle ein Ticket um das Serverteam zu kontaktieren."},
            {"command_guild_register",      "Registriere deinen Server für den Bot."},
            {"command_guild_removedata",    "Löscht alle Daten zu deinem Server."},
            {"command_guild_gated",         "Sendet die Nachricht für Gated Communitys."},
            {"command_guild_channel",       "Lege die Kanäle für Botbenachrichtigungen fest."},
            {"command_guild_member",        "Lege die Rolle für deine Mitglieder fest."},
            {"command_guild_permissions",   "Lege die Rollen für Mods der Admins fest."},
            {"command_guild_pointname",     "Lege den Namen für die Serverpunkte fest."},
            {"command_guild_settings",      "Aktiviere oder deaktiviere Funktionen für den Server."},
            {"command_guild_tickets",       "Lege die Kategorie für dein Ticketsystem fest."},
            {"command_guild_language",      "Ändere die Serversprache für den Bot."},
            {"command_guild_voice",         "Lege die Kategorie für temporäre Sprachkanäle fest."},

            {"command_admin_status",        "Zeigt dir den Status deines Servers an."},
            {"command_admin_help",          "Liste mit allen Befehlen für Administratoren."},
            {"command_admin_invite",        "Lege den Invitelink für deinen Server fest."},
            {"command_admin_wordadd",       "Füge dem Wortfilter etwas hinzu."},
            {"command_admin_wordremove",    "Entferne etwas vom Wortfilter."},
            {"command_admin_sticky",        "Sendet ein sticky Embed in einen Kanal."},
            {"command_admin_stopsticky",    "Stoppt eine Sticky-Message."},
            {"command_admin_points",        "Gebe oder nehme Serverpunkte von einem Nutzer."},
            {"command_admin_roles",         "Sendet die Nachricht mit den Rollen für Nutzer."},
            {"command_admin_rolechange",    "Füge Nutzerrollen hinzu oder entferne sie."},
            {"command_admin_report",        "Melde einen Nutzer für alle verbundenen Server."},
            {"command_admin_color",         "Füge eine Farbrolle hinzu oder entferne sie."},
            {"command_admin_channel",       "Lege besondere Kanäle fest."},
            {"command_admin_remchannel",    "Lösche einen besonderen Kanal."},

            {"command_mod_embed",           "Sendet ein Embed in den Textkanal."},
            {"command_mod_ticket",          "Schließt ein Ticket für den Nutzer."},
            {"command_mod_help",            "Liste mit allen Befehlen für Moderatoren."},
            {"command_mod_seereport",       "Sehe alle Meldungen zu einem Nutzer."},
            {"command_mod_setrule",         "Erstelle Regeln für einen Kanal."},

            {"command_use_help",            "Liste mit allen Befehlen für Nutzer."},
            {"command_use_register",        "Registriere deinen Account für den Bot und seine Funktionen."},
            {"command_use_deletedata",      "Lösche unwiderruflich alle Daten zu deinem Account."},
            {"command_use_ranks",           "Zeigt dir die Topliste dieses Servers und deinen Rang an."},
            {"command_use_ticket",          "Erstelle ein Ticket um das Serverteam zu kontaktieren."},
            {"command_use_invite",          "Zeigt dir den Einladungslink des Servers an."},
            {"command_use_botdm",           "Stelle ein ob der Bot dir private Nachrichten senden darf."},
            {"command_use_language",        "Lege deine Sprache fest."},
            {"command_use_stat",            "Sehe deiner Punkte, Event Infos und mehr zu deinem Nutzerprofil."},
            {"command_use_rules",           "Zeigt dir die Regeln für einen bestimmten Kanal."},
            {"command_use_color",           "Zeigt dir alle Farbrollen an."},
            {"command_use_colorrole",       "Kaufe eine Farbrolle für dich."},
            {"command_use_pubremind",       "Setze eine öffentliche Erinnerung in den jeweiligen Kanal."},
            {"command_use_voice",           "Erstelle einen temporären Sprachkanal."},

            {"command_fact_new",            "[Serverinhaber, Admin] Lege eine neue Fraktion und ihren Anführer fest."},
            {"command_fact_guildlist",      "[Serverinhaber, Admin, Mod] Sehe alle existierenden Fraktionen auf deinem Server."},
            {"command_fact_owner",          "[Serverinhaber, Admin] Ändere den Fraktionsleiter einer existierenden Fraktion."},
            {"command_fact_remove",         "[Serverinhaber, Admin] Lösche eine Fraktion vollständig."},
            {"command_fact_removemember",   "[Anführer] Entferne einen Nutzer aus deiner Fraktion."},
            {"command_fact_addmember",      "[Anführer] Füge einen Nutzer zu deiner Fraktion hinzu."},
            {"command_fact_name",           "[Anführer] Lege den Namen und die Beschreibung für deine Fraktion fest."},
            {"command_fact_help",           "[Jeder] Zeigt dir alle Fraktionsbefehle an."},
            {"command_fact_member",         "[Anführer, Mitglieder] Zeigt dir alle Mitglieder deiner Fraktion."},

            {"command_winter_advent",       "Öffne ein Adventstürchen vom Kalender."},
            {"command_winter_work",         "Erledige Winterarbeiten um Winterpunkte zu erhalten."},

            {"command_wow_help",            "Zeigt dir alle wow Befehle an."},
            {"command_wow_addchar",         "Speicher oder update deinen wow Charakter für diesen Server."},
            {"command_wow_delchar",         "Lösche einen Charakter aus deiner Liste."},
            #endregion

            #region DM
            {"userBlocksDMs",       "[ :x: ] Es wurde versucht dem Nutzer <@{0}> eine Nachricht zu senden, doch dieser blockiert private Nachrichten. Bitte prüfe ob der Nutzer anderweitig " +
                                    "informiert werden sollte."}, // 0 = user id
            {"userBlockDMsError",   "[ :x: ] Es wurde versucht dem Nutzer <@{0}> eine Nachricht zu senden, doch dieser blockiert private Nachrichten. Da der Nutzer etwas bestätigen muss, kann " +
                                    "diese Funktion nicht genutzt werden. Der Nutzer muss seine privaten Nachrichten für den Bot öffnen."}, // 0 = user id
            #endregion

            #region FACTIONS
            {"newFactionAdminMessage",      "[ :white_check_mark: ] Du hast eine neue Fraktion erstellt. Der Besitzer der Fraktion ist <@{0}> und wurde benachrichtigt, wenn seine DMs offen sind."}, // 0 = faction owner id
            {"errorSavingNewFaction",       "[ :x: ] Etwas ist beim Erstellen der Fraktion schief gelaufen. Der Fehler wurde automatisch gemeldet."},
            {"addFactionCategoryError",     "[ :x: ] Beim Erstellen der Kategorie ist ein Fehler aufgetreten. Der Fehler wurde automatisch gemeldet."},
            {"addFactionTextChannelError",  "[ :x: ] Beim Erstellen des Textkanales ist ein Fehler aufgetreten. Der Fehler wurde automatisch gemeldet."},
            {"factionGuildListTitle",       "# Fraktionen des Servers\n"},
            {"noFactionsOnGuild",           "Auf diesem Server ist derzeit keine Fraktion registriert."},
            {"userIsAlreadyOwner",          "[ :x: ] Nutzer ist bereits Anführer eine Fraktion: {0}. Man darf nur eine Fraktion auf einmal anführen."}, // 0 = id and faction name
            {"notFactionLeader",            "[ :x: ] Dies kann nur als Fraktionsleiter benutzt werden und du bist kein Fraktionsleiter."},
            {"userRemovedFromFaction",      "[ :white_check_mark: ] Nutzer <@{0}> wurde aus deiner Fraktion entfernt."}, // 0 = user id
            {"couldNotRemovePermissions",   "**Warnung** es konnten nicht alle Kanalrechte für den Nutzer entfernt werden. Bitte prüfe deine Fraktionskanäle und entferne die Nutzerrechte selbstständig.\n\n"},
            {"userInviteSend",              "[ :white_check_mark: ] Es wurde eine Einladung an <@{0}> versendet. Die Person hat eine private Nachricht vom Bot erhalten und muss nun die Einladung bestätigen."},
            {"removedFaction",              "[ :white_check_mark: ] Die Fraktion wurde vollständig gelöscht!"},
            {"factionOwnerChanged",         "[ :white_check_mark: ] Der Fraktionsleiter wurde erfolgreich geändert."},
            {"newFactionOwnerMessage",      ":tada: Du wurdest zum Fraktionsleiter einer neuen Fraktion ernannt! Schau dir deinen ersten Fraktionskanal an: <#{0}>"}, // 0 = channel id
            {"factionOwnerTransfered",      ":tada: Du wurdest zum neuen Fraktionsleiter der Fraktion {0} ernannt!"}, // 0 = faction name
            {"factionMemberMaxCount",       "[ :x: ] Die Fraktion hat das Maximum an Mitgliedern bereits erreicht, es kann niemand mehr eingeladen werden. Ehöhe deine maximale Mitgliederanzahl."},
            {"youDenyFactionInvite",        "Du hast die Einladung zur Fraktion **{0}** abgelehnt!"}, // 0 = faction name
            {"userDenyedFactionInvite",     "Deine Einladung an **{0}** deiner Fraktion beizutreten wurde abgelehnt."}, // 0 = invited user name
            {"youAcceptedFactionInvite",    "[ :white_check_mark: ] Du hast die Einladung angenommen. Willkommen in der Fraktion **{0}**!"}, // 0 = faction name
            {"factionMemberInviteMessage",  "# Fraktionseinladung\nHallo, du wurdest von **{0}** in die Fraktion **{1}** auf dem Server {2} eingeladen.\nWenn du auf \"Annehmen\" drückst, werden dir die Rechte für " +
                                            "die Fraktion gegeben und du kannst deren Kanäle auf dem Server sehen. Wenn du kein Mitglied werden willst, kannst du diese Nachricht ignorieren oder auf \"Ablehnen\" klicken " +
                                            "damit dem Fraktionsleiter mitgeteilt wird, dass du die Einladung ablehnst."}, // 0 = faction owner name, 1 = faction name, 2 = server name
            {"userIsAlreadyFactionUser",    "Der Nutzer ist bereits Mitglied deiner Fraktion!"},
            {"userIsNotInFaction",          "[ :x: ] Funktion nicht möglich, da der Nutzer nicht zur Fraktion gehört."},
            {"factionIdDoesNotExist",       "[ :x: ] Fraktions Id ungültig, es existiert keine Fraktion mit der Id **{0}**."}, // 0 = faction id
            {"notFactionMember",            "Der Nutzer ist kein Fraktionsmitglied."},
            {"factionMemberListTitle",      "Alle Mitglieder der Fraktion **{0}**:"}, // 0 = faction name
            {"factionMemberListOwner",      "Anführer ist <@{0}>\n\n"}, // 0 = name of the faction owner
            #endregion

            #region HALLOWEEN
            {"halloweenIntroduction",       "# Halloween 2025\nDas diesjährige Event beginnt heute!\n**Süßes, sonst gibt´s Saures!**\n\nIn diesem Kanal könnt ihr ungestört das Event bespielen. Sammelt so viele " +
                                            "Süßigkeiten wie möglich und versucht Streiche auszuteilen oder Streiche die gegen dich gerichtet sind zu vermeiden."},
            {"halloweenSearchButton",       "Suche Süßigkeiten"},
            {"halloweenCandyButton",        "Zeige meine Süßigkeiten"},
            {"halloweenDoTrickhButton",     "Spiele Jemanden einen Streich!"},
            {"halloweenDefendTrickButton",  "Verteidige dich vor einem Streich"},
            {"halloweenAlreadyUsed",        "[ :x: ] Du kannst nur einmal am Tag einen Streich ausführen oder verhindern. Heute hast du bereits eine Aktion ausgeführt!"},
            {"halloweenFoundCandy",         "# Süßigkeitentour\nDu gehst von Haus zu Haus, doch nicht jeder hält etwas für dich bereit.\nDu hast **{1}**x **{0}** gefunden!"}, // 0 = candy name, 1 0 candy amount
            {"halloweenNoCandy",            "Du hast bisher noch keine Süßigkeiten gefunden."},
            {"halloweenUserCandyList",      "# Deine Süßigkeiten\nDu hast bisher folgendes eingesammelt:\n{0}\nDadurch gesammelte Punkte insgesamt: **{1}**"}, // 0 = list of candy, 1 = points
            {"halloweenCooldown",           "[ :x: ] Du kannst nur alle 30 Minuten nach Süßigkeiten suchen. Du kannst wieder um {0} suchen."}, // 0 = next possible time
            {"halloweenNoCandyStolen",      "Du konntest Niemanden finden dem du einen Streich spielen könntest."},
            {"halloweenPrankNotSuccess",    "[ :x: ] Du wolltest jemandem einen Streich spielen doch die Person war darauf vorbereitet! Dein Streich ist gescheitert und du bist leider leer ausgegangen."},
            {"halloweenStolenCandyMessage", "[ :white_check_mark: ] Du hast erfolgreich einen Streich gespielt und dabei {0} x **{1}** erbeutet!"}, // 0 = candy amount, 1 = candy name
            {"halloweenProtection",         "[ :white_check_mark: ] Du hast dich **heute** für einen Streich gewappnet und wirst **einem** Streich entkommen können."},
            #endregion

            #region HELP TEXT
            {"helpTitleGuild",      "Befehle für Serverinhaber:\n"},
            {"helpTextGuild",       "# Befehle für Serverinhaber\n`/guild channel` - Lege die Serverkanäle für Neuigkeiten und Warnungen fest.\n`/guild gated` - Lege fest ob nur " +
                                    "Mitglieder den Bot nutzen können.\n`/guild member` - Lege die Mitgliederrolle für Gated Community fest.\n`/guild permissions` - Lege die Rechte " +
                                    "für die Botnutzung fest.\n`/guild points` - Ändere den Namen für das Punktesystem.\n`/guild removedata` - Löscht alle Daten zu deinem Server.\n" +
                                    "`/guild settings` - Ändere Einstellungen für deinen Server.\n`/guild status` - Zeigt die Einstellungen deines Servers.\n`/guild tickets` - " +
                                    "Lege die Kategorie für Tickets fest.\n\n[Datenschutzerklärung und Impressum](<https://lost-city-1.gitbook.io/management-bot-de>)\n" +
                                    "[Installationsanleitung](<https://lost-city-1.gitbook.io/management-bot-de/bot-installation>)"},
            {"guildStatusText",     "## Informationen über den Server\nErstellt am: {9} von {10}.\nBeschreibung: {11}\n\nAktuell wird der Server {12} mal geboostet und ist damit auf " +
                                    "dem Boostlevel {13}.\nDer Server hat `{14}` als Land angegeben mit dem Länderflag {15} und die Sprachkanaäle werden in {16} gehostet.\nDas " +
                                    "Verifizierungslevel ist {17} und das NSFW-Level des Servers ist {18}.\n\nAnzahl der...\nEmotes: {19}\nSticker: {20}\nRollen: {21}\nKategorien: {22}\n" +
                                    "Kanäle insgesamt: {23}\nSprachkanäle: {24}\nTextkanäle: {25}\nForen: {26}\nThreads: {27}\nBühnen: {28}\n\n Es finden aktuell {29} Events statt.\n\n" +
                                    "## Bot Einstellungen\nInvitelink: {0}\nBot Adminrolle: <@&{1}>\nBot Modrolle: <@&{2}>\nBot Mitgliederrolle: <@&{3}>\nName der Punkte: {5}\nGated " +
                                    "Community: {4}\nWortfilter aktiv: {6}\nÜberprüfen gelöschter Nachrichten: {7}\nTicketsystem aktiv: {8}\n\n# Kategorien und Kanäle:\n{30}"},
                                    // 0 = InviteLink, 1 = AdminRole, 2 = ModeratorRole, 3 = MemberRole, 4 = IsGatedCommunity,
                                    // 5 = PointsName, 6 = UseWordfilter, 7 = CheckDeletedMessages, 8 = TicketsActive, 9 = creation date, 10 = owner name,
                                    // 11 = server description, 12 = amount boosts, 13 = boost level, 14 = culture name, 15 = culture id, 16 = voice region name, 17 = verification level,
                                    // 18 = nsfw level, 19 = count emotes, 20 = count sticker, 21 = count roles, 22 = count category, 23 = count channel, 24 = voice channel,
                                    // 25 = count text channel, 26 = count forum, 27 = count thread, 28 = count stages, 29 = count events, 30 = channel list
            {"helpTitleUser",       "Dies sind die Befehle für Nutzer:\n"}, // 0 = guild name
            {"imprintGDPR",         "[Datenschutzerklärung und Impressum](<https://lost-city-1.gitbook.io/management-bot-de>)"},
            {"helpTitleAdmin",      "Die Befehle für Administratoren:\n"},
            {"installationLink",    "[Funktionen des Bots](<https://lost-city-1.gitbook.io/management-bot-de/bot-installation>)"},
            {"helpTitleMod",        "Die Befehle für Moderatoren:\n"},
            {"helpTitleFaction",    "Befehle für Fraktionen:\n"},
            #endregion

            #region MODAL
            {"modalLoading",    "Das Formular wird geprüft und der Inhalt geladen.\nWenn diese Nachricht länger als 30 Sekunden stehen bleibt, dann ist ein Fehler aufgetreten!"},
            {"modalUnknown",    "Das genutzt Modal wurde nicht erkannt. Der Fehler wurde automatisch gemeldet."},
            {"embedToLong",     "Embed ist ungültig. Ein Embed kann maximal 6000 Zeichen beinhalten, dieses Embed beinhaltet zu viele Zeichen. Bitte überprüfe den Inhalte oder melde den Fehler einem Administrator."},
            #endregion

            #region PERMISSION
            {"notMember",           "[ :x: ] Du benötigst die Mitgliederrolle um Botbefehle nutzen zu können."},
            {"pickedUserNotMember", "[ :x: ] Der ausgewählte Nutzer ist kein registriertes Mitglied, daher kann die Funktion nicht ausgeführt werden."},
            {"alreadyMember",       "Du bist bereits ein Mitglied dieser Community!"},
            {"missingMemberRole",   "[ :x: ] Die Mitgliederrolle konnte nicht gefunden werden. Bitte überprüfe das du eine Mitgliederrolle eingetragen hast!"},
            {"missingPermisson",    "[ :x: ] Du hast nicht die benötigten Rechte um dies nutzen zu können."},
            {"youAreBlockedGlobal", "[ :x: ] Du bist serverübergreifend für diesen Bot gesperrt. Entsperrungen können nur vom Botentwickler vorgenommen werden."},
            #endregion

            #region POINTS
            {"notEnoughGuildPoints",        "[ :x: ] Du hast nicht genug Serverpunkte um diese Funktion nutzen zu können, du brauchst dafür **{0}** Serverpunkte."}, // 0 = needed guild points
            #endregion

            #region REGISTER USER
            {"registerMessage",         "[ :white_check_mark: ] Du hast dich für den Lost City Management Bot registriert. Dieser Bot speichert die Daten für seine Funktoinen. Um diese Daten zu deinem Account zuordnen " +
                                        "zu können, wird auch deine Discord-Id gespeichert. Du kannst jederzeit alle Daten zu dir löschen lassen. Für das Sammeln und Verwalten der Daten ist der Botentwickler verantwortlich."},
            {"alreadyRegistered",       "[ :x: ] Du bist bereits registriert."},
            {"registerCanceled",        "[ :x: ] Registrierung ist fehlgeschlagen. Der Fehler wurde automatisch gemeldet, bitte warte bis ein Administrator sich bei dir meldet."},
            {"registerNeddPermission",  "[ :x: ] Du kannst dich nur für den Bot registrieren, wenn du der DSGVO zustimmst!"},
            {"accountToJung",           "Dein Account ist jünger als 7 Tage, daher wurde dem Serverteam deine Anfrage auf eine Mitgliedschaft gesendet. Bitte öffne deine privaten Nachrichten, falls das Serverteam Rückfragen hat."},
            {"userAccountToJung",       "Achtung, der Account des Nutzers ist jünger als 7 Tage."},
            {"needToBeRegistered",      "[ :x: ] Um diese Funktion nutzen zu können müssen wir Daten speichern, daher musst du registriert sein. Benutze dazu `/use register` und akzeptiere die DSGVO."},
            {"registerUserInfoText",    "Hallo **{0}**!\nWenn du Funktionen von mir nutzen möchtest, die Daten von dir speichern, musst du zuerst zustimmen dass ich diese auch speichern darf. Ich speichere keine Daten " +
                                        "von dir, solange du dich noch nicht registriert hast. Drücke den Knopf `DSGVO akzeptieren und im Bot registrieren` um deinen Account zu registrieren und meine " +
                                        "[Nutzungsbedingungen so wie DSGVO](<https://lost-city-1.gitbook.io/management-bot-de>) zu akzeptieren."}, // 0 = user name
            #endregion

            #region REGISTER SERVER
            {"guildAlreadyRegistered",      "[ :x: ] Dein Server ist bereits registriert."},
            {"guildRegisterMessage",        "Du bist dabei deinen Server für den Lost City Bot zu registrieren.\nDrücke den Knopf `Bot annehmen und DSGVO akzeptieren` um deinen Server zu registrieren und der " +
                                            "[Nutzungsbedingungen so wie DSGVO](<https://lost-city-1.gitbook.io/management-bot-de/>) von Lost City zu akzeptieren. Die DSGVO ( Datenschutzgrundverordnung ) beschreibt welche " +
                                            "Daten unser Bot sammelt und was damit geschieht."},
            {"registrationMissingBot",      "[ :x: ] Der Bot wurde noch nicht registriert, daher kann diese Funktion nicht genutzt werden! Der Serverinhaber muss den Bot zuerst registrieren."},
            {"registerGuildDatabaseError",  "[ :x: ] Dein Server konnte nicht registriert werden! Der Fehler wurde automatisch gemeldet. Der Botentwickler wird sich bei dir melden."},
            {"guildRegisterSuccess",        "[ :white_check_mark: ] Dein Server wurde erfolgreich registriert! Bitte befolge nun die Installationsanweisungen aus unserem " +
                                            "[GitBook](<https://lost-city-1.gitbook.io/management-bot-de/bot-installation>). Wenn du Hilfe benötigst, dann wende dich bitte an Vivian."},
            {"registerGuildOwnerDM",        "Hallo {0}!\nDu hast darum gebeten mich für **{1}** hinzuzufügen.\nIch bin hier um dir bei der Serververwaltung zu helfen. Bitte schau dir meine " +
                                            "[GitBook](<https://lost-city-1.gitbook.io/management-bot-en/bot-installation>) Webseite an. Dort findest du alle Erklärungen zu meinen Funktionen.\n\n" +
                                            "Wenn du nicht darum gebeten hast mich hinzuzufügen, dann kick mich einfach von deinem Server. Ich sammle keine Daten und nehme keine EInstellungen am " +
                                            "Server vor, so lange du meine Einladung nicht bestätigt hast. Drücke den Knopf `Bot annehmen und DSGVO akzeptieren` wenn du meine Einladung " +
                                            "bestätigen willst und meine [Datenschutzverordnung & Nutzungsbedingung](<https://lost-city-1.gitbook.io/management-bot-en>) akzeptierst. " +
                                            "Du kannst auch den englischen Knopf drücken, dies legt deine persönliche und deine Serversprache fest ( beides kann später noch geändert werden )."}, 
                                            // 0 = guild owner name, 1 = guild name
            #endregion

            #region REMINDER
            {"reminderTimeNotMatching",     "[ :x: ] Falsches Zeitformat. Du musst eine Uhrzeit im 24 Stundenformat \"00:00\" angeben!"},
            {"reminderDateNoMatching",      "[ :x: ] Falsches Datumsformat. Du musst das Datum mit den Querstrichen ( slashes )angeben. Tag/Monat/Jahr also so schreiben: 28/01/2024"},
            {"reminderWeekdayNotMatching",  "[ :x: ] Ungültiger Wochentag. Du musst einen Wochentag wie \"Montag\" oder \"Mittwoch\" eintragen. Groß- und Kleinschreibung ist unwichtig."},
            {"reminderDurationNotMatch",    "[ :x: ] Die Laufzeit einer Erinnerung muss mindestens 2 Tage betragen und darf maximal 10 Tage lang sein."},

            {"dailypubreminderSaved",       "[ :white_check_mark: ] Eine tägliche Erinnerung wurde gespeichert! Jeden Tag um {0} Uhr wird die Erinnerung im Kanal <#{1}> gepostet."}, // 0 = time, 1 = channel id
            {"weeklypubreminderSaved",      "[ :white_check_mark: ] Eine wöchentliche Erinnerung wurde gespeichert! Jeden {2} um {0} Uhr wird die Erinnerung im Kanal <#{1}> gepostet."}, // 0 = time, 1 = channel id, 2 = weekday
            {"datepubreminderSaved",        "[ :white_check_mark: ] Eine Erinnerung für das Datum {0} um {1} Uhr wurde für den Kanal <#{2}> erstellt."}, // 0 = date, 1 = time, 2 = channel id
            {"durationpubreminderSaved",    "[ :white_check_mark: ] Eine laufende Erinnerung wurde erstellt für den Kanal <#{0}> mit einer Laufzeit von {1} Tagen. Die Erinnerung wird immer um {2} " +
                                            "Uhr gepostet."}, // 0 = channel id, 1 = duration time, 2 = time
            
            #endregion

            #region REPORTS
            {"noUserReports",           "Es bestehen keine Einträge für diesen Nutzer."},
            {"userReportTitle",         "# Folgende Einträge bestehen für den Nutzer <@{0}>:\n"}, // 0 = user name
            {"reportEmpty",             "Eine Meldung darf nicht leer sein und muss mindestens 4 Zeichen beinhalten, zum Beispiel \"Spam\"."},
            {"reportInserted",          "[ :white_check_mark: ] Die Meldung wurde erfolgreich eingetragen."},
            {"ReportInsertError",       "[ :x: ] Die Meldung konnte nicht gespeichert werden. Bitte versuche es erneut und kontaktiere den Botentwickler wenn der Fehler weiterhin auftaucht."},
            {"userReportEmbedTitle",    "Neuer Nutzer hat den Server betreten"},
            #endregion

            #region ROLES
            {"youGotTheRole",               "[ :white_check_mark: ] Du hast die Rolle **{0}** erhalten."}, // 0 = role name
            {"youTossedTheRole",            "[ :white_check_mark: ] Du hast die Rolle **{0}** abgelegt."}, // 0 = role name
            {"youGotTheRoleAlready",        "[ :x: ] Du hast die Rolle **{0}** bereits!"}, // 0 = role name
            {"requestMemberRole",           "Es wurde eine Anfrage an das Serverteam gesendet. Bitte habe etwas Geduld, bis du deine Rolle oder eine Rückmeldung von ihnen erhälst."},
            {"requestMemberRoleTeam",       "<@{0}> möchte als Mitglied freigeschaltet werden. **Niemals** die Rolle händisch vergeben! Klicke auf den Button um dem Nutzer die Rolle zu geben."}, // 0 = user id
            {"memberAccepted",              "Der Nutzer <@{0}> wurde als Mitglied akzeptiert von Teammitglied <@{1}>."}, // 0 = user id, 1 = team member id
            {"youGotMember",                "Du wurdest als Mitglied auf dem Server **{0}** aufgenommen und hast die Rolle **{1}** erhalten."}, // 0 = server name, 1 = role name
            {"membershipDenied",            "[ :x: ] Du wurdest als Mitglied auf dem Server **{0}** abgelehnt. Diese Entscheidung kommt vom Serverteam und nicht vom Bot."}, // 0 = server name
            {"memberDenied",                "Nutzer <@{0}> wurde als Mitglied abgelehnt von <@{1}>."}, // 0 = user id, 1 = team member id
            {"notFoundAnyUserRoles",        "Es wurden keine Rollen für diesen Server gefunden. Wenn es sich dabei um einen Fehler handelt, wende dich bitte an den Botentwickler."},
            {"allUserRolesForGuildRemoved", "[ :white_check_mark: ] Es wurden alle Nutzerrollen aus der Datenbank entfernt."},
            {"sendUserRolesMessage",        "# Nutzerrollen\nDu kannst dir jederzeit eine zusätzliche Rolle nehmen und diese wieder abgeben. Klicke dazu einfach auf die Knöpfe unter diesem Text."},
            {"userRolesTitel",              "# Nutzerrollen dieses Servers\n"},
            {"roleIsSystemRole",            "[ :x: ] Du kannst keine Systemrolle zu einer Nutzerrolle machen! Diese Rolle ist entweder Mitglied, Moderator oder Administrator im Botsystem."},
            {"userRoleAlreadyAdded",        "[ :x: ] Die Rolle **{0}** ist bereits als Nutzerrolle registriert."}, // 0 = role name
            {"roleNotSavedAsUserRole",      "[ :x: ] Die Rolle **{0}** ist nicht als Nutzerrolle gespeichert."}, // 0 = role name
            {"userRoleAdded",               "[ :white_check_mark: ] Die Rolle **{0}** wurde zur Liste der Nutzerrollen hinzugefügt."}, // 0 = role name
            {"userRoleRemoved",             "[ :white_check_mark: ] Die Rolle **{0}** wurde von der Nutzerliste entfernt."}, // 0 = role name
            {"roleAlreadyAdded",            "Die Rolle ist bereits aufgelistet."},
            {"colorRoleAdded",              "[ :white_check_mark: ] Die Rolle <@&{0}> wurde als Farbrolle hinzugefügt."}, // 0 = role id
            {"colorRoleRemoved",            "[ :white_check_mark: ] Die Rolle **{0}** wurde als Farbrolle entfernt."}, // 0 = role id
            {"noColorRolesForGuild",        "Es gibt derzeit keine gespeicherten Farbrollen."},
            {"getGuildColorList",           "# Farbrollen\nDiese Rollen sind als Farbrollen gespeichert:\n{0}"}, // 0 = string with role list
            {"roleIsNotColorRole",          "[ :x: ] Die ausgewählte Rolle ist keine Farbrolle und kann daher nicht gekauft werden."},
            {"alreadyOwnedColorRole",       "[ :x: ] Du besitzt diese Rolle bereits und kannst sie daher nicht noch einmal kaufen."},
            #endregion

            #region SELECT MENU
            {"selectMenuLoading",   "Das Auswahlmenü wird geprüft und der Inhalt geladen.\nWenn diese Nachricht länger als 30 Sekunden stehen bleibt, dann ist ein Fehler aufgetreten!"},
            {"selectMenuUnknown",   "[ :x: ] Es ist ein Fehler aufgetreten und automatisch gemeldet worden. Das Auswahlmenü wurde vom System nicht erkannt.\nBitte warte auf eine Rückmeldung des Botentwicklers."},
            #endregion

            #region SLASH COMMAND
            {"commandLoading",      "Der Befehl wird geprüft und der Inhalt geladen.\nWenn diese Nachricht länger als 30 Sekunden stehen bleibt, dann ist ein Fehler aufgetreten!"},
            {"commandNotExisting",  "[ :x: ] Dieser Befehl existiert nicht oder ist veraltet und wurde leider noch nicht von Discord entfernt."},
            {"commandNotInDM",      "[ :x: ] Du kannst keine Befehle in privaten Unterhaltungen nutzen! Benutze den Befehl auf einem Server, der unseren Bot nutzt."},
            {"commandValueInvalid", "[ :x: ] Eine oder mehrere Eingaben in den Befehlsoptionen war falsch. Bitte achte darauf, welchen Datentyp (Text, Zahl, Rolle, Kanal ect.) du angeben sollst."},
            {"commandOutdated",     "[ :x: ] Dieser Befehl existiert nicht oder ist veraltet und wurde leider noch nicht von Discord entfernt."},
            #endregion

            #region STICKY
            {"stickyMessageTitle",          "Sticky message wurde gesendet"},
            {"stickyMessageText",           "In <#{0}> wurde eine neue sticky message gesendet {1}."}, // 0 = channel id, 1 = message link
            {"stickyMessageRemoved",        "Die Sticky-Message wurde entfernt."},
            {"stickyMessageRemovedError",   "Beim entfernen der Sticky-Message ist etwas schief gelaufen."},
            #endregion

            #region SYSTEM GENERAL
            {"dataSaved",               "[ :white_check_mark: ] Deine Angaben wurden erfolgreich gespeichert!"},
            {"deletedUserData",         "[ :white_check_mark: ] Alle Daten in Verbindung zu deinem Account wurden gelöscht!"},
            {"deletedGuildData",        "[ :white_check_mark: ] Alle Daten in Verbindung zu deinem Server wurden gelöscht!"},

            {"removeGuildDataLabel",    "Lösche unwiderruflich alle Serverdaten"},
            {"removeGuildDataMessage",  "# Achtung!\nDu bist dabei **ALLE** Daten zu deinem Server zu löschen. Diese Daten können nicht wieder hergestellt werden. Alle Einstellungen, Punkte, Events und alle Informationen " +
                                        "zu und für Funktionen werden gelöscht. Wenn du dies wirklich möchtest, dann klicke auf den roten Knopf."},
            {"removeUserDataLabel",     "Lösche unwiderruflich alle deine Nutzerdaten"},
            {"removeUserDataMessage",   "# Achtung!\nDu bist dabei **ALLE** Daten zu deinem Account zu löschen. Diese Daten können nicht wieder hergestellt werden. Alle Einstellungen, Punkte, Events und alle Informationen " +
                                        "zu und für Funktionen werden gelöscht. Wenn du dies wirklich möchtest, dann klicke auf den roten Knopf."},
            {"pendingAction",           "Du hast dies bereits genutzt. Bitte warte auf eine Reaktion oder melde es dem Serverteam, wenn du denkst dies sei ein Bug."},
            {"deleteMessageTitle",      "Ein Nutzer hat eine Nachricht gelöscht. Dies waren der Nutzer und der Inhalt der Nachricht:"},
            {"userDeletedThereData",    "[ :grey_exclamation: ] Der betroffene Nutzer hat seine Daten gelöscht."},
            {"urlInvalid",              "Die angegebene URL ist ungültig, bitte kontrolliere den Link."},
            {"getInviteLink",           "Schicke diesen Link jemanden den du auf den Server einladen willst:\n```{0}```"}, // 0 = invite link url
            {"missingInvite",           "Es wurde kein Einladungslink gespeichert. Bitte wende dich an die Serverleitung."},
            {"titleReactionDeleted",    "Reactionmessage wurde gelöscht!"},
            {"messageReactionDeleted",  "Eine Reactionmessage wurde gelöscht, diese Nachrichten sind mit Funktionen des Bots verbunden. Sollte dies nicht geplant gewesen sein, dann überprüfe dies bitte.\nDie Nachricht war im " +
                                        "Kanal <#{1}> und die Id der Nachricht war: {0}."}, // 0 = message id, 1 = channel id
            {"noRulesFound",            "Für diesen Kanal wurden noch keine Regeln festgelegt."},
            {"userLeftGuild",           "Der Nutzer **{0}** mit der Id ||{1}|| hat den Server verlassen."}, // 0 = user left name, 1 = user left id
            {"userLeftGuildTitle",      "Nutzer hat den Server verlassen"},

            {"unknown",                 "Unbekannt"},
            {"deny",                    "Ablehnen"},
            {"accept",                  "Annehmen"},
            {"rank",                    "Rang"},

            {"generalError",            "[ :x: ] Es ist ein Fehler aufgetreten. Das Problem wurde automatisch gemeldet."},
            {"saveDataError",           "[ :x: ] Fehler beim Speichern der Daten. Der Fehler wurde automatisch gemeldet."},
            {"userDataError",           "[ :x: ] Fehler beim Auslesen des angegebenen Nutzers. Befindet sich der Nutzer noch auf dem Server?"},
            {"channelReadError",        "[ :x: ] Fehler beim Auslesen eines Kanals. Möglicherweise existiert der ausgewählte Kanal nicht mehr oder er hat den falschen Typ."},
            {"roleReadError",           "[ :x: ] Fehler beim Auslesen einer Rolle. Möglicherweise existiert die ausgewählte Rolle nicht mehr."},
            {"fetchGuildError",         "[ :x: ] Fehler beim Auslesen des Discordservers. Das Problem wurde automatisch gemeldet."},
            {"wrongFormatNumber",       "[ :x: ] Der angegebene Wert darf nur Zahlen enthalten!"},
            {"noUserDataFound",         "[ :x: ] Es wurden keine Daten in Verbindung zu deinem Account gefunden."},

            {"channelNotCategory",      "[ :x: ] Du musst für diese Funktion eine Kategorie auswählen. Der angegebene Kanal ist keine Kategorie, bitte überprüfe das."},
            {"functionNotWhileTimeout", "[ :x: ] Du kannst keine Funktionen benutzen während du einen Timeout laufen hast. Dein Timeout endet um {0}!"}, // 0 = time when timeout is ending
            {"functionAfter24Hour",     "[ :x: ] Du kannst den Bot erst 24 Stunden nach dem Beitritt des Servers nutzen. Wir bitten um Verständnis für diese Sicherheitsmaßnahme.\n" +
                                        "Du kannst den Bot nutzen ab {0}."}, // 0 = time when user is 24 hours on server
            {"inputTextToShort",        "[ :x: ] Der angegebene Text ist zu kurz. Er muss mindestens {0} Zeichen lang sein."}, // 0 = amount of min characters
            {"noMatchingEntryName",     "[ :x: ] Zu dem angegebenen Namen wurde kein Nutzer oder kein Charakter gefunden."},

            {"delayMessageTicket1",     "Überprüfe Serverdaten um Ticket zu schließen..."},
            {"delayMessageTicket2",     "Überprüfe Nutzerdaten um Ticket zu schließen..."},
            {"delayMessageTicket3",     "Überprüfe Kanaldaten um Ticket zu schließen..."},
            {"delayMessageTicket4",     "Nutzer wird aus dem Kanal entfernt..."},

            {"serverFunctionNotActive", "[ :x: ] Der Server hat diese Botfunktion nicht aktiviert. Bitte wende dich an einen Serverteammitglied, wenn du dies für einen Fehler hälst."},
            {"userHasAlreadyTempVoice", "[ :x: ] Du hast bereits einen Sprachkanal auf diesem Server erstellt. Jeder Nutzer darf nur einen Sprachkanal gleichzeitig erstellt haben."},
            {"missingGuildCategory",    "[ :x: ] Für diese Funktion muss eine Kanalkategorie für den Server festgelegt werden. Es ist keine Kategorie gespeichert. Bitte " +
                                        "kontaktiere einen Serveradministrator."},
            {"tempVoiceWasCreated",     "[ :white_check_mark: ] Ein neuer temporärer Sprachkanal mit dem Namen **{0}** wurde erfolgreich erstellt!"}, // 0 = voice name
            #endregion

            #region TICKET
            {"ticketAlreadyExists",     "[ :x: ] Du hast bereits ein Ticket auf diesem Server geöffnet. Es befindet sich hier: <#{0}>"}, // 0 = ticket channel id
            {"ticketNoCategory",        "[ :x: ] Es konnte keine Ticketkategorie in den Serverdaten gefunden werden. Bitte informiere das Serverteam darüber."},
            {"ticketsNotActive",        "[ :x: ] Tickets sind für diesen Server deaktiviert."},
            {"ticketBotMessage",        "# Ein neues Ticket wurde erstellt!\n<@{0}> hat eine Anfrage an das Serverteam. Bitte schreibe hier nun hinein, um was es geht."}, // 0 = user id
            {"ticketCloseText",         "# Ticket geschlossen\n<@{0}> hat das Ticket geschlossen und der Nutzer wurde entfernt.\nWenn du das Ticket wieder öffnen willst, dann klicke auf den Button."},
            {"ticketReopened",          "Das Ticket wurde wieder eröffnet."},
            {"ticketOpened",            "Du hast ein neues Ticket eröffnet. Du findest es in diesem Kanal: <#{0}>"}, // 0 = ticket channel id
            {"ticketChannelRemoved",    "# Ticketkanal wurde gelöscht\nEin Kanal mit einem offenem Ticket wurde gelöscht! Das Ticket hat dem Nutzer <@{0}> gehört."}, // 0 = ticket owner id
            #endregion

            #region USER
            {"noRanksFound",                    "Auf diesem Server hat noch niemand Punkte gesammelt."},
            {"rankListText",                    "{0}. <@{1}> mit Level {2} ( {3} Punkte )\n"}, // 0 = rank, 1 = user id, 2 = level, 3 = points
            {"ranksCheckingUser",               "Rangliste wird geladen, dies kann einige Sekunden dauern. Bitte warten..."},
            {"getMemberInfoText",               "# Willkommen auf dem Server **{0}**!\nUm unserem Server beitreten zu dürfen und die Funktionen des Management Bots nutzen zu können, brauchst du die " +
                                                "Mitgliederrolle <@&{1}>.\nMit dem " +
                                                "Annehmen der Rolle stimmst du unseren Regeln und der [DSGVO](https://lost-city.gitbook.io/lcnet-discord-bot/) des Management Bots zu."}, 
                                                // 0 = guild name, 1 = member role id
            {"getMemberClosedInfoText",         "# Willkommen auf dem Server **{0}**!\nUm unserem Server beitreten zu dürfen und die Funktionen des LC-Bots nutzen zu können, brauchst du die " +
                                                "Mitgliederrolle <@&{1}>.\n" +
                                                "Drücke auf den Knopf um eine Anfrage an das Serverteam zu senden. Sie entscheiden dann, ob du Mitglied wirst oder nicht. Mit dem Annehmen der " +
                                                "Rolle stimmst du zu, dass ein Servertteammitglied dir eine private Nachricht schicken darf ( Öffne deine Nachrichten für Servermitglieder! ) und du " +
                                                "stimmst unseren Regeln und der [DSGVO](https://lost-city.gitbook.io/lcnet-discord-bot/) des Management Bots zu."}, // 0 = guild name, 1 = member role id
            {"userStatusMessage",               "# <@{2}>\nHat **{0}** Bot-Punkte gesammelt.\nBeim Winterevent wurden **{1}** Punkte gesammelt.\n\n"}, // 0 = general bot points,
                                                // 1 = winter event points, 2 = user name
            {"userStatusNoGuildPoints",         "Noch keine Serverpunkte gesammelt."},
            {"userStatusGuildPointsCaption",    "Gesammelte Serverpunkte:\n"},
            {"userStatusGuildPoints",           "- {0}, {1}: {2}\n"}, // 0 = guild name, 1 = guild points name, 2 = guild points
            {"logTitleGuildPointsChanged",      "Serverpunkte wurden durch einen Befehl geändert"},
            {"logMessageGuildPointsChanged",    "Teammitglied **{0}** hat für den Nutzer **{1}** Serverpunkte verändert. Der Änderungstyp war \"{2}\" und die Punkte waren **{3}**."}, // 0 = admin name,
                                                                                                                                                                                       // 1 = user name, 2 = changeType, 3 = points
            #endregion         

            #region WINTER
            {"itsNotAdventTime",            "[ :x: ] Es ist keine Adventszeit. Der Kalender kann nur vom 01 - 24 Dezember genutzt werden."},
            {"itsNotWinterTime",            "[ :x: ] Diese Funktion kann nur in der Winterzeit, also im Dezember, genutzt werden."},
            {"adventAlreadyOpend",          "[ :x: ] Du hast heute bereits dein Türchen geöffnet, diese Funktion ist Server übergreifend und kann nur ein mal genutzt werden."},
            {"adventDoorDescription",       "Du öffnest dein heutiges Türchen und findest ein **Gedicht** und **100** Winterpunkte!\n\n{0}\n\n" +
                                            "Außerdem findest du ein Geschenk: **{1}**\n\n{2}"}, // 0 = poem, 1 = item name, 2 = item card url
            {"cantDoWinterWorkNow",         "[ :x: ] Du bist noch beschäftigt. Du kannst wieder arbeiten ab {0}."}, // 0 = work time
            {"doWinterWork",                "[ :white_check_mark: ] Du gehst für 15 Minuten arbeiten und wirst dafür **10** Winterpunkte erhalten."},
            #endregion

            #region WOW
            {"wowheal",     "Heiler"},
            {"wowtank",     "Tank"},
            {"wowdamage",   "Schaden"},

            {"wowdruid",        "Druide"},
            {"wowwitchdoctor",  "Hexenmeister"},
            {"wowhunter",       "Jäger"},
            {"wowwarrior",      "Krieger"},
            {"wowmage",         "Magier"},
            {"wowpaladin",      "Paladin"},
            {"wowpriest",       "Priester"},
            {"wowshaman",       "Schamane"},
            {"wowrough",        "Schurke"},

            {"wowalchemy",      "Alchemie"},
            {"wowblacksmith",   "Schmied"},
            {"wowenchanter",    "Verzauberer"},
            {"wowingenier",     "Ingenieur"},
            {"wowjuwelcraft",   "Juwelier"},
            {"wowleatherwork",  "Lederer"},
            {"wowtailor",       "Schneider"},
            {"wowherbalist",    "Kräuterkunde"},
            {"wowmining",       "Bergbau"},
            {"wowskinning",     "Kürschner"},
            #endregion
        };
    }
}
