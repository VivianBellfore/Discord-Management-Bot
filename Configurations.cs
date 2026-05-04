
namespace LCNET_Management_Bot
{
    internal class Configurations
    {
        // Add your database connection string here!
        // Like this: "Server=___;Database=___;Uid=___;Pwd=___;"
        internal static string SqlConnection = "";


        // Add your discord bot client ID here!
        internal static ulong BotClientId = 0;
        // Add your discord bot token here!
        internal static string BotToken = "";


        // Add the guild id of your developer or main server.
        // This will be used for developer information messages ect.
        internal static ulong DevGuildId = 0;


        // Add the channel id for the developer error log channel.
        internal static ulong ErrorLogChannel = 0;
        // Add the channel id for the developer user interaction log channel.
        internal static ulong UserActionLogChannel = 0;


        // Change the default user language here, needs to be the name of the language cs in lower case.
        internal static string DefaultUserLanguage = "english";
        // Change the default system language here, needs to be the name of the language cs in lower case.
        internal static string DefaultSystemLanguage = "english";


        // Change to set the maximum time, an empty temp voice channel will stay, until it is deleted.
        internal static int TempVoiceIdlTimer = 20;


        // The image shown on the halloween event info embed.
        internal static string HalloweenEventPictureUrl = "https://www.publicdomainpictures.net/pictures/540000/nahled/exploring-the-colors-of-halloween.jpg";
    }
}
