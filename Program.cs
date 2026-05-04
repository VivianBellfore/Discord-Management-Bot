
using System.Threading.Tasks;



namespace LCNET_Management_Bot
{
    internal class Program
    {
        /// <summary>
        /// Starting bot instance so it can be async.
        /// </summary>
        static Task Main(string[] args)
        {
            var thisInstance = new StartBotInstance();
            return thisInstance.MainAsync();
        }
    }
}
