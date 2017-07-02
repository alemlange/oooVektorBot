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
using Bot.CommandParser;
using Brains;

namespace Bot.Controllers
{
    public class TelegramController : ApiController
    {
        //private BotBrains Brains = new BotBrains();

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
            Bot.Api.SetWebhook("https://ef66272e.ngrok.io/Telegram/WebHook").Wait();

            return new string[] { "Ok" };
        }

        [HttpPost]
        public async Task<IHttpActionResult> WebHook(Update update)
        {
            var message = update.Message;
            var chatId = message.Chat.Id;

            if (message.Type == MessageType.TextMessage)
            {
                var cmd = TelegramCmdParser.ParseUpdate(update);
                if (cmd == CmdTypes.Greetings)
                    await Bot.Api.SendTextMessageAsync(chatId, BotBrains.Instance.Value.Greetings(chatId).ResponceText);
                else if(cmd == CmdTypes.TableNumber)
                {
                    var tableNumber = Convert.ToInt32(message.Text);
                    await Bot.Api.SendTextMessageAsync(chatId, BotBrains.Instance.Value.Number(chatId, tableNumber).ResponceText);
                }
                else if (cmd == CmdTypes.Menu)
                {
                    await Bot.Api.SendTextMessageAsync(chatId, BotBrains.Instance.Value.ShowMenu(chatId).ResponceText);
                }
                else if (cmd == CmdTypes.Check)
                {
                    await Bot.Api.SendTextMessageAsync(chatId, BotBrains.Instance.Value.ShowCart(chatId).ResponceText);
                }
                else if(cmd == CmdTypes.Unknown)
                {
                    if(BotBrains.Instance.Value.DishNames.Contains(message.Text.ToLower()))
                        await Bot.Api.SendTextMessageAsync(chatId, BotBrains.Instance.Value.OrderMeal(chatId, message.Text).ResponceText);
                    else
                        await Bot.Api.SendTextMessageAsync(chatId, "Извините, не понял вашей просьбы :(");
                }
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