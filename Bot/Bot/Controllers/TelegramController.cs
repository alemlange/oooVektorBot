using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Controllers
{
    public class TelegramController : ApiController
    {
        static class Bot
        {
            public static readonly Api Api = new Api("392797621:AAECgGELrjENABjPvPnorEaE0BjnlHN-qY0");
        }

        [HttpGet]
        public IEnumerable<string> Start() //http://localhost:8443/api/Telegram/Start
        {
            var telegramService = "https://api.telegram.org/bot";
            var botToken = "392797621:AAECgGELrjENABjPvPnorEaE0BjnlHN-qY0";
            var method = "/getupdates";

            //var botClient = new TelegramBotClient(botToken);

            Bot.Api.SetWebhook().Wait();
            //Bot.Api.SetWebhook("https://YourHostname:8443/WebHook").Wait();
            Bot.Api.SetWebhook("https://0c63f8af.ngrok.io/api/Telegram/Post").Wait();

            return new string[] { "Ok" };
        }

        [HttpPost]
        public async Task<IHttpActionResult> WebHook()
        {
            var update = new Update();
            var message = update.Message;

            if (message.Type == MessageType.TextMessage)
            {
                // Echo each Message
                await Bot.Api.SendTextMessage(message.Chat.Id, message.Text);
            }
            /*
            else if (message.Type == MessageType.PhotoMessage)
            {
                // Download Photo
                var file = await Bot.Api.GetFile(message.Photo.LastOrDefault()?.FileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = File.Open(filename, FileMode.Create))
                {
                    await file.FileStream.CopyToAsync(saveImageStream);
                }

                await Bot.Api.SendTextMessage(message.Chat.Id, "Thx for the Pics");
            }
            */
            return Ok();
        }
    }
}