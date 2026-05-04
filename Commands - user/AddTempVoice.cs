
using Discord.WebSocket;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    /// <summary>
    /// Handeling all functions for the <paramref name="voice"/> command.
    /// </summary>
    internal class AddTempVoice : CommandObject
    {
        /// <summary>
        /// Strukt for the help command informations.
        /// </summary>
        internal AddTempVoice() : base("use", "voice", "command_use_voice") { }



        /// <summary>
        /// Handling command conditions and executing other functions.<para/>
        /// Function executed by:
        /// <seealso cref="CommandManager.SlashCommandExecutedHandler(SocketSlashCommand)"/>
        /// </summary>
        internal async override Task CommandFunction(SocketSlashCommand command)
        {
            ( bool isRequestValid, string errorMessage ) = await CheckConditions(command);

            if ( !isRequestValid )
            {
                await command.ModifyOriginalResponseAsync(f => f.Content = errorMessage);
                return;
            }

            string name = command.Data.Options.First().Options.ElementAt(0).Value.ToString();

            ( bool isNameValid, string invalidMessage ) = await CheckNameCondition( name, command );

            if ( !isNameValid)
            {
                await command.ModifyOriginalResponseAsync(async f => f.Content = invalidMessage );
                return;
            }

            ( bool isChannelCreated, string errorMessage2 ) = await ChannelManager.AddTempVoice( (ulong)command.GuildId, command.User.Id, name );

            if ( !isChannelCreated)
            {
                await command.ModifyOriginalResponseAsync(async f => f.Content = errorMessage2);
                return;
            }

            await command.ModifyOriginalResponseAsync(async f => f.Content = await LanguageManager.GetTranslation("tempVoiceWasCreated", command.User.Id, "_none_", name ));
        }



        /// <summary>
        /// Checking if guild has temp voice active, has a category set and user has tos accepted and dont has a temp channel on that server.
        /// </summary>
        private async Task<( bool, string )> CheckConditions(SocketSlashCommand command)
        {
            GuildObject guildData = await GuildManager.GetGuildData((ulong)command.GuildId);
            if ( guildData == null )
            {
                await Utilities.SendDevLogMessage(1, $"Could not read guild data from DB. Guild was {(ulong)command.GuildId}.");
                return ( false, await LanguageManager.GetTranslation("fetchGuildError", command.User.Id) );
            }
            
            if ( guildData.TempVoice == false )
                return ( false, await LanguageManager.GetTranslation("serverFunctionNotActive", command.User.Id) );

            if ( guildData.TempVoiceCategory == 0 )
                return (false, await LanguageManager.GetTranslation("missingGuildCategory", command.User.Id));

            bool userIsRegistered = await PermissionManager.HasUserAcceptTos(command.User.Id);
            if ( !userIsRegistered )
                return ( false, await LanguageManager.GetTranslation("needToBeRegistered", command.User.Id) );

            bool canAddTempVoice = await ChannelManager.CheckUserTempVoice((ulong)command.GuildId, command.User.Id);
            if ( !canAddTempVoice )
                return (false, await LanguageManager.GetTranslation("userHasAlreadyTempVoice", command.User.Id));

            return ( true, "" );
        }

        /// <summary>
        /// Is name not empty and not blacklisted.
        /// </summary>
        private async Task<( bool, string )> CheckNameCondition( string name, SocketSlashCommand command)
        {
            if (name.Length < 4)
                return ( false, await LanguageManager.GetTranslation("inputTextToShort", command.User.Id, "_none_", 4) );

            List<string> forbiddenWords = BlockedTextManager.GuildBlacklists[(ulong)command.GuildId];

            if (forbiddenWords.Any(w => name.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0))
                return (false, await LanguageManager.GetTranslation( "inputTextForbidden", command.User.Id));

            return ( true, "" );
        }
    }
}
