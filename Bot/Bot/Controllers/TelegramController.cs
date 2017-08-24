using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Args;
using Bot.CommandParser;
using Bot.CommandParser.KeyBoards;
using Brains;
using LiteDbService;
using DataModels.Enums;

namespace Bot.Controllers
{
    public class TelegramController : ApiController
    {

        static class Bot
        {
            public static readonly TelegramBotClient Api = new TelegramBotClient("392797621:AAECgGELrjENABjPvPnorEaE0BjnlHN-qY0");
        }

        [HttpGet]
        public string Start(string key) //http://localhost:8443/Telegram/Start?key=
        {
            Bot.Api.SetWebhookAsync().Wait();
            Bot.Api.SetWebhookAsync("https://" + key + ".ngrok.io/Telegram/WebHook").Wait();

            // remove all tables
            var service = new TestLiteManagerService();
            service.RemoveAllTables();

            return "Ok" ;
        }

        [HttpPost]
        public async Task<IHttpActionResult> WebHook(Update update)
        {
            try
            {
                var bot = new BotBrains(); //.Instance.Value;

                if (update.Type == UpdateType.MessageUpdate)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var parser = ParserChoser.GetParser(bot.GetState(chatId));
                    var cmd = parser.ParseForCommand(update);

                    if (message.Type == MessageType.TextMessage)
                    {
                        switch (cmd)
                        {
                            case CmdTypes.Greetings:
                                {
                                    var responce = bot.Greetings(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.TableNumber:
                                {
                                    var response = bot.Number(chatId, Convert.ToInt32(message.Text));

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Menu: 
                                {
                                    var response = bot.ShowMenuOnPage(chatId);

                                    var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        replyMarkup: keyboard);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        "Хотите чтонибудь из меню? Просто кликните по нему!",
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Slash:
                                {
                                    var keyboard = InlineKeyBoardManager.GetByCmnd(CmdTypes.Slash);

                                    var response = bot.GetMenuItem(chatId, update.Message.Text);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        replyMarkup: keyboard);
                                    break;
                                }
                            case CmdTypes.MyOrder:
                                {
                                    var responce = bot.ShowCart(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Check:
                                {
                                    var response = bot.GiveACheck(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Waiter:
                                {
                                    var response = bot.CallWaiter(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Unknown:
                                {
                                    await Bot.Api.SendTextMessageAsync(chatId, "Извините, не понял вашей просьбы :(");
                                    break;
                                }
                        }
                    }
                }
                else if (update.Type == UpdateType.CallbackQueryUpdate)
                {
                    var chatId = update.CallbackQuery.From.Id;
                    var messageId = update.CallbackQuery.Message.MessageId;

                    if (update.CallbackQuery.Data.ToLower().Contains("  << ")) // todo keyboard
                    {
                        var page = int.Parse(update.CallbackQuery.Data.Trim('<', ' '));
                        var response = bot.ShowMenuOnPage(chatId, page);

                        var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                        await Bot.Api.EditMessageTextAsync(
                            chatId,
                            messageId,
                            response.ResponceText,
                            replyMarkup: keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains(" >>  ")) // todo keyboard
                    {
                        var page = int.Parse(update.CallbackQuery.Data.Trim('>', ' '));

                        var response = bot.ShowMenuOnPage(chatId, page);

                        var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                        await Bot.Api.EditMessageTextAsync(
                            chatId,
                            messageId,
                            response.ResponceText,
                            replyMarkup: keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains("добавить в заказ"))
                    {
                        var response = bot.OrderMeal(chatId);

                        await Bot.Api.SendTextMessageAsync(
                            chatId,
                            response.ResponceText,
                            replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains("вернуться к меню"))
                    {
                        var response = bot.ShowMenuOnPage(chatId);

                        var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                        await Bot.Api.SendTextMessageAsync(
                            chatId,
                            response.ResponceText,
                            replyMarkup: keyboard);

                        await Bot.Api.SendTextMessageAsync(
                            chatId,
                            "Хотите чтонибудь из меню? Просто кликните по нему!",
                            replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                    }
                }
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                StreamWriter file = new System.IO.StreamWriter("c:\\db\\test.txt",true);
                file.WriteLine(mes);

                file.Close();
            }

            return Ok();
        }

    }
}