using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Args;
using Bot.CommandParser;
using Bot.Tools;
using Bot.CommandParser.KeyBoards;
using Brains;
using Brains.Responces;
using DataModels.Enums;
using DataModels.Notifications;
using DataModels.Configuration;
using LiteDbService.Helpers;

namespace Bot.Controllers
{
    public class TelegramController : ApiController
    {
        static class Telegram
        {
            public static readonly TelegramBotClient Api = new TelegramBotClient("498869682:AAH-IwVHEdKM09SjjIHO7D0rx27z7-ZfCbI");
        }

        [HttpGet]
        public string Test()
        {
            try
            {
                var bot = new BotBrains();
                bot.SystemDiagnostic();

                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public string Start(string key) //http://localhost:8443/Telegram/Start?key=
        {
            var testResult = Test();
            if (testResult == "Ok")
            {
                Telegram.Api.SetWebhookAsync().Wait();
                Telegram.Api.SetWebhookAsync("https://" + key + ".ngrok.io/Telegram/WebHook").Wait();

                return "Ok";
            }
            else
                return testResult;
        }

        [HttpPost]
        public void SendMessage([FromBody]Notification msg)
        {
            try
            {
                Telegram.Api.SendTextMessageAsync(
                    msg.ChatId,
                    msg.Message,
                    parseMode: ParseMode.Html);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> WebHook(Update update)
        {
            long chatId = 0;
            try
            {
                var bot = new BotBrains(); //.Instance.Value;

                if (update.Type == UpdateType.MessageUpdate)
                {
                    var message = update.Message;
                    chatId = message.Chat.Id;
                    var parser = ParserChoser.GetParser(chatId, bot);

                    if (message.Type == MessageType.TextMessage || message.Type == MessageType.PhotoMessage)
                    {
                        var cmd = parser.ParseForCommand(update);

                        switch (cmd)
                        {
                            case CmdTypes.Start:
                                {
                                    var greetings = "Здравствуйте, меня зовут ДайнерБот! Я знаю все о местной кухне, могу рассказать вам " +
                                        "о наших блюдах, принять заказ с учетом ваших вкусов и пожеланий, а так же помочь вам " +
                                        "в любых вопросах! Для того, чтобы ознакомиться с меню напишите \"меню\", чтобы " +
                                        "сделать заказ напишите \"начать\". Приятного отдыха!";

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        bot.Config.Greetings ?? greetings,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Greetings:
                                {
                                    var responce = bot.Greetings(chatId);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Restrunt:
                                {
                                    var responce = bot.Restrunt(chatId, message.Text);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.TableNumber:
                                {
                                    var response = bot.Number(chatId, Convert.ToInt32(message.Text));

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Menu:
                                {
                                    var responce = bot.ShowMenuCategories(chatId);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: new MenuCategorySessionParser(bot.GetMenuCategoriesByChatId(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Category:
                                {
                                    var response = bot.SnowMenuByCategory(chatId, message.Text);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.CloseMenu:
                                {
                                    var response = bot.CloseMenu(chatId);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Slash:
                                {
                                    var keyboard = InlineKeyBoardManager.GetByCmnd(CmdTypes.Slash);

                                    var response = bot.GetMenuItem(chatId, update.Message.Text);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: keyboard);
                                    break;
                                }
                            case CmdTypes.Pay:
                                {
                                    var responce = bot.ShowCart(chatId);

                                    var prices = new LabeledPrice[1];
                                    prices[0] = new LabeledPrice { Amount = 10000, Label = "Товар" };

                                    await Telegram.Api.SendInvoiceAsync(
                                        chatId, "Title", "Description", "payload", "401643678:TEST:4e6bc4df-f9c2-4935-a81d-7649d848571a", "startP", "RUB", prices);

                                    break;
                                }
                            case CmdTypes.MyOrder:
                                {
                                    var responce = bot.ShowCart(chatId);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Check:
                                {
                                    var response = bot.GiveACheck(chatId);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Waiter:
                                {
                                    var response = bot.CallWaiter(chatId);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Remark:
                                {
                                    var response = bot.AddRemark(chatId, message.Text);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Remove:
                                {
                                    var response = bot.RemoveFromOrder(chatId);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.RemoveByNum:
                                {
                                    var response = bot.RemoveFromOrderByNum(chatId, message.Text);

                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Unknown:
                                {
                                    await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        "Извините, не понял вашей просьбы.",
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.QRCode:
                                {
                                    var code = "";
                                    var file = await Telegram.Api.GetFileAsync(message.Photo.LastOrDefault()?.FileId);
                                    var filename = bot.Config.PicturePath + chatId + "." + file.FilePath.Split('.').Last();

                                    using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                                    {
                                        await file.FileStream.CopyToAsync(saveImageStream);

                                        saveImageStream.Position = 0;
                                        saveImageStream.Close();
                                    }

                                    file.FileStream.Position = 0;
                                    file.FileStream.Close();

                                    code = CodeController.ReadCode(filename);

                                    if (System.IO.File.Exists(filename))
                                    {
                                        System.IO.File.Delete(filename);
                                    }

                                    if (code != null)
                                    {
                                        var response = bot.QRCode(chatId, code);

                                        await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
                                    else
                                    {
                                        await Telegram.Api.SendTextMessageAsync(
                                        chatId,
                                        "Не удалось распознать код! Попробуйте еще раз или выберите ресторан и номер стола через меню!",
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
                                    break;
                                }
                        }
                    }
                }
                else if (update.Type == UpdateType.CallbackQueryUpdate)
                {
                    chatId = update.CallbackQuery.From.Id;
                    var messageId = update.CallbackQuery.Message.MessageId;

                    if (update.CallbackQuery.Data.ToLower().Contains("добавить в заказ"))
                    {
                        var response = bot.OrderMeal(chatId);

                        await Telegram.Api.SendTextMessageAsync(
                            chatId,
                            response.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains("вернуться к меню"))
                    {
                        var responce = bot.ShowMenuCategories(chatId);

                        await Telegram.Api.SendTextMessageAsync(
                            chatId,
                            responce.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: new MenuCategorySessionParser(bot.GetMenuCategoriesByChatId(chatId)).Keyboard);

                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter().WriteException(ex.Message);

                if(chatId != 0)
                {
                    if (ex.Message.Contains("429"))
                    {
                        var excResponce = Responce.UnknownResponce(chatId);
                        await Telegram.Api.SendTextMessageAsync(
                        chatId,
                        "К сожалению Телеграм не позволяет нам так часто вам отвечать 😔, подождите пару минут пожалуйста.",
                        parseMode: ParseMode.Html);
                    }
                    else
                    {
                        var excResponce = Responce.UnknownResponce(chatId);
                        await Telegram.Api.SendTextMessageAsync(
                        chatId,
                        excResponce.ResponceText,
                        parseMode: ParseMode.Html);
                    }
                    
                }
            }

            return Ok();
        }

    }
}