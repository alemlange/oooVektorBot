using System.Text;
using System.Net;
using DataModels.Email;
using DataModels.Configuration;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Clients
{
    public class BotClient
    {
        private string BotLocation { get; set; }

        public BotClient(string botUrl)
        {
            BotLocation = botUrl;
        }
        public string GetStatus()
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            return client.DownloadString(BotLocation + "Telegram/Test/");
        }
    }
}
