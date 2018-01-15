using System.Text;
using System.Net;
using DataModels.Notifications;
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

        public string StartBot(string botKey)
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            return client.DownloadString(BotLocation + "Telegram/Start?key=" + botKey);
        }

        public string GetStatus()
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            return client.DownloadString(BotLocation + "Telegram/Test/");
        }

        public string SendNotification(long chatId, string notification)
        {
            var client = new WebClient();

            client.Headers["Content-type"] = "application/json";
            client.Encoding = Encoding.UTF8;

            var request = new Notification { ChatId = chatId, Message = notification };

            var serializer = new DataContractJsonSerializer(typeof(Notification));
            MemoryStream mem = new MemoryStream();
            serializer.WriteObject(mem, request);

            var data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);

            var responce = client.UploadString(BotLocation + "Telegram/SendMessage/", "POST", data);

            return responce;
        }
    }
}