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
                                    var greetings = "Здравствуйте, меня зовут ДайнерБот! Я помогу вам сделать заказ. " +
                                        "Для того, чтобы ознакомиться с меню нажмите \"Меню\", чтобы " +
                                        "сделать заказ нажмите \"Заказать\".";

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
                                    var responce = bot.AssignTableNumber(chatId, message.Text);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.RequestPayment:
                                {
                                    var responce = bot.PaymentRequest(chatId);

                                    await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.InputSumm:
                                {
                                    var response = bot.InputSumm(chatId, message.Text, message.Chat.Username);

                                    if (response.InvoiceReady)
                                    {
                                        var prices = new LabeledPrice[1];
                                        prices[0] = new LabeledPrice { Amount = response.Invoice.SummInCents, Label = "Итого" };

                                        await Telegram.SendInvoiceAsync(
                                            chatId, response.Invoice.Title, response.Invoice.Description, response.Invoice.Id.ToString(), bot.PaymentToken, "startP", response.Invoice.Currency, prices);
                                    }
                                    else
                                    {
                                        await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
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
                            case CmdTypes.RequestFeedback:
                                {
                                    var responce = bot.FeedbackRequest(chatId);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.LeaveFeedback:
                                {
                                    var responce = bot.LeaveFeedback(chatId, message.Chat.Username, message.Text);

                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.RequestBooking:
                                {
                                    var responce = bot.BookingRequest(chatId);

                                    await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.LeaveBooking:
                                {
                                    var responce = bot.LeaveBooking(chatId, message.Text);

                                    await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.CancelTable:
                                {
                                    var responce = bot.CancelTable(chatId);

                                    await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Category:
                                {
                                    var response = bot.SnowMenuByCategory(chatId, message.Text);

                                    if(response.Dishes != null)
                                    {
                                        await Telegram.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: InlineKeyBoardManager.MenuKeyBoard(response.Dishes));
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
                            case CmdTypes.Cart:
                                {
                                    var responce = bot.ShowCart(chatId);

                                    if (responce.NeedInlineKeeyboard)
                                    {
                                        var keyboard = InlineKeyBoardManager.GetByCmnd(CmdTypes.Cart);

                                        await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: keyboard);
                                    }
                                    else
                                    {
                                        await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
                                    break;
                                }
                            case CmdTypes.MyOrders:
                                {
                                    var responce = bot.ShowAllOrders(chatId);

                                    await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    
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
                            case CmdTypes.CloseTimeArriving:
                                {
                                    var response = bot.CloseTimeArriving(chatId);

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
                            case CmdTypes.Actions:
                                {
                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        bot.Actions,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Description:
                                {
                                    await Telegram.SendTextMessageAsync(
                                        chatId,
                                        bot.Description,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    break;
                                }
                            case CmdTypes.Location:
                                {
                                    var responce = bot.GetAllRestaurants(chatId);

                                    if (responce.IsOk)
                                    {
                                        foreach (var restaurant in responce.RestaurantsInfo)
                                        {
                                            await Telegram.SendTextMessageAsync(chatId, restaurant.Info, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);

                                            await Telegram.SendLocationAsync(chatId, restaurant.Latitude, restaurant.Longitude, replyMarkup: InlineKeyBoardManager.TaxiKeyboard());
                                        }
                                    }
                                    else
                                    {
                                        await Telegram.SendTextMessageAsync(chatId, responce.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
                                    
                                    break;
                                }
                        }
                    }
                }
                else if (update.Type == UpdateType.CallbackQueryUpdate)
                {
                    chatId = update.CallbackQuery.Message.Chat.Id;
                    var parser = ParserChoser.GetParser(chatId, bot);

                    var cmd = parser.ParseForCommand(update);

                    switch (cmd)
                    {
                        case CmdTypes.AddToOrder:
                            {
                                var response = bot.OrderMeal(chatId, update.CallbackQuery.Data);

                                if (response.IsOk)
                                {
                                    var keyboard = InlineKeyBoardManager.RemarkKeyBoard(response.Modificators);
                                    await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: keyboard);
                                }
                                else
                                {
                                    await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                }
                                break;
                            }
                        case CmdTypes.AddMod:
                            {
                                var response = bot.AddModificator(chatId, update.CallbackQuery.Data);

                                if (response.IsOk)
                                {
                                    if (response.Modificators.Any())
                                    {
                                        var keyboard = InlineKeyBoardManager.RemarkKeyBoard(response.Modificators);
                                        await Telegram.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: keyboard);
                                    }
                                    else
                                    {
                                        await Telegram.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, response.ResponceText, parseMode: ParseMode.Html);
                                    }
                                }
                                else
                                {
                                    await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                }
                                break;
                            }
                        case CmdTypes.BackToMenu:
                        {
                            var response = bot.ShowMenuCategories(chatId);

                            await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: new MenuCategorySessionParser(bot.GetMenuCategoriesByChatId(chatId)).Keyboard);
                            break;
                        }
                        case CmdTypes.DishDetails:
                            {
                                var response = bot.GetMenuItem(chatId, update.CallbackQuery.Data);

                                if (response.NeedInlineKeyboard)
                                {
                                    var keyboard = InlineKeyBoardManager.DescriptionKeyBoard(response.DishId);

                                    await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: keyboard);
                                }
                                else
                                {
                                    await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                }
                                break;
                            }
                        case CmdTypes.ArrivingTime:
                        {
                            var response = bot.ArrivingTime(chatId);

                            if (response.OkToChangeTime)
                            {
                                await Telegram.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: InlineKeyBoardManager.GetByCmnd(CmdTypes.ArrivingTime));
                            }
                            else
                            {
                                await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                            }
                            break;
                        }
                        case CmdTypes.TimeInput:
                            {
                                var timeResp = bot.ChangeArrivingTime(chatId, update.CallbackQuery.Data);

                                if (!timeResp.OkToChangeTime)
                                {
                                    await Telegram.SendTextMessageAsync(chatId, timeResp.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                }
                                else
                                {
                                    var orderResp = bot.ShowCart(chatId);
                                    if (orderResp.NeedInlineKeeyboard)
                                    {
                                        var keyboard = InlineKeyBoardManager.GetByCmnd(CmdTypes.Cart);

                                        await Telegram.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, orderResp.ResponceText, parseMode: ParseMode.Html, replyMarkup: keyboard);
                                    }
                                    else
                                    {
                                        await Telegram.SendTextMessageAsync(chatId, orderResp.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                                    }
                                }

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
                                await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
                            }
                            break;
                        }
                        case CmdTypes.PayCash:
                        {
                            var response = bot.PayCash(chatId);

                            await Telegram.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: ParserChoser.GetParser(chatId, bot).Keyboard);
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