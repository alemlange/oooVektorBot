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
        private TelegramBotClient Telegram;

        [HttpGet]
        public string Test()
        {
            try
            {
                var bot = new BotBrains(Request.Headers.Host);
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
                var bot = new BotBrains(Request.Headers.Host);
                Telegram = new TelegramBotClient(bot.BotToken);

                Telegram.SetWebhookAsync().Wait();
                Telegram.SetWebhookAsync("https://" + key + "/Telegram/WebHook").Wait();

                return "Ok";
            }
            else
                return testResult;
        }

        [HttpPost]
        public string SendMessage([FromBody]Notification msg)
        {
            try
            {
                var bot = new BotBrains(Request.Headers.Host);
                Telegram = new TelegramBotClient(bot.BotToken);

                Telegram.SendTextMessageAsync(
                    msg.ChatId,
                    msg.Message,
                    parseMode: ParseMode.Html);

                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> WebHook(Update update)
        {
            long chatId = 0;
            try
            {
                var bot = new BotBrains(Request.Headers.Host);
                Telegram = new TelegramBotClient(bot.BotToken);

                if (update.Type == UpdateType.MessageUpdate)
                {
                    var message = update.Message;
                    chatId = message.Chat.Id;
                    var parser = ParserChoser.GetParser(chatId, bot);

                    if (message.Type == MessageType.TextMessage || message.Type == MessageType.PhotoMessage || message.Type == MessageType.SuccessfulPayment)
                    {
                        var cmd = parser.ParseForCommand(update);

                        switch (cmd)
                        {
                            case CmdTypes.Start:
                                {
                                    var greetings = "Добро пожаловать в наш отель! Я знаю все о местной кухне, могу рассказать вам " +
                                        "о наших блюдах, принять заказ с учетом ваших вкусов и пожеланий, а так же помочь вам " +
                                        "в любых вопросах! Для того, чтобы ознакомиться с меню напишите \"меню\", чтобы " +
                                        "сделать заказ напишите \"начать\". Приятного отдыха!";

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        bot.GreetingsText ?? greetings,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Greetings:
                                {
                                    var responce = bot.Greetings(chatId);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Restrunt:
                                {
                                    var responce = bot.Restrunt(chatId, message.Text);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.TableNumber:
                                {
                                    var response = bot.Number(chatId, Convert.ToInt32(message.Text));

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Menu:
                                {
                                    var responce = bot.ShowMenuCategories(chatId);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: new MenuCategorySessionParser(bot.GetMenuCategoriesByChatId(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Category:
                                {
                                    var response = bot.SnowMenuByCategory(chatId, message.Text);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.CloseMenu:
                                {
                                    var response = bot.CloseMenu(chatId);

                                    await Telegram.SendTextMessageAsync(
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

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: keyboard);
                                    break;
                                }
                            case CmdTypes.CreateInvoice:
                                {
                                    var response = bot.CreateInvoice(chatId);

                                    if (response.InvoiceReady)
                                    {
                                        var prices = new LabeledPrice[1];
                                        prices[0] = new LabeledPrice { Amount = response.Invoice.SummInCents, Label = "Итого" };

                                        await Telegram.SendInvoiceAsync(
                                            chatId, response.Invoice.Title, response.Invoice.Description, response.Invoice.Id.ToString(), bot.PaymentToken, "startP", response.Invoice.Currency, prices);
                                    }
                                    else
                                    {
                                        await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
                                    break;
                                }
                            case CmdTypes.SuccessfulPayment:
                                {
                                    var payment = message.SuccessfulPayment;

                                    var responce = bot.SuccessPayment(chatId, payment.InvoicePayload, payment.TotalAmount, payment.TelegramPaymentChargeId, payment.Currency);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.MyOrder:
                                {
                                    var responce = bot.ShowCart(chatId);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Check:
                                {
                                    var response = bot.GiveACheck(chatId);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Waiter:
                                {
                                    var response = bot.CallWaiter(chatId);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Remark:
                                {
                                    var response = bot.AddRemark(chatId, message.Text);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Remove:
                                {
                                    var response = bot.RemoveFromOrder(chatId);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.RemoveByNum:
                                {
                                    var response = bot.RemoveFromOrderByNum(chatId, message.Text);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Unknown:
                                {
                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        "Извините, не понял вашей просьбы.",
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.QRCode:
                                {
                                    var code = "";
                                    var file = await Telegram.GetFileAsync(message.Photo.LastOrDefault()?.FileId);
                                    var filename = bot.PicturePath + chatId + "." + file.FilePath.Split('.').Last();

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

                                        await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
                                    else
                                    {
                                        await Telegram.SendTextMessageAsync(
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

                    if (update.CallbackQuery.Data.ToLower().Contains("добавить в заказ"))
                    {
                        var response = bot.OrderMeal(chatId);

                        await Telegram.SendTextMessageAsync(
                            chatId,
                            response.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains("вернуться к меню"))
                    {
                        var responce = bot.ShowMenuCategories(chatId);

                        await Telegram.SendTextMessageAsync(
                            chatId,
                            responce.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: new MenuCategorySessionParser(bot.GetMenuCategoriesByChatId(chatId)).Keyboard);

                    }
                }
                else if (update.Type == UpdateType.PreCheckoutQueryUpdate)
                {
                    chatId = update.PreCheckoutQuery.From.Id;
                    var preCheck = update.PreCheckoutQuery;

                    var response = bot.PreCheckout(chatId, preCheck.TotalAmount, preCheck.Currency, preCheck.InvoicePayload);

                    if (!response.IsError)
                    {
                        await Telegram.AnswerPreCheckoutQueryAsync(preCheck.Id, true);
                    }
                    else
                    {
                        await Telegram.AnswerPreCheckoutQueryAsync(preCheck.Id, false, errorMessage: response.ResponceText);
                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter().WriteException(ex.Message);

                if(chatId != 0 && Telegram != null)
                {
                    if (ex.Message.Contains("429"))
                    {
                        var excResponce = Responce.UnknownResponce(chatId);
                        await Telegram.SendTextMessageAsync(
                        chatId,
                        "К сожалению Телеграм не позволяет нам так часто вам отвечать 😔, подождите пару минут пожалуйста.",
                        parseMode: ParseMode.Html);
                    }
                    else
                    {
                        var excResponce = Responce.UnknownResponce(chatId);
                        await Telegram.SendTextMessageAsync(
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