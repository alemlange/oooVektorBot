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
        public string Start() //http://localhost:8443/Telegram/Start
        {
            Bot.Api.SetWebhookAsync().Wait();
            Bot.Api.SetWebhookAsync("https://882ae9b1.ngrok.io/Telegram/WebHook").Wait();

            // remove all tables
            var service = new TestLiteManagerService();
            service.RemoveAllTables();

            return "Ok" ;
        }

        [HttpPost]
        public async Task<IHttpActionResult> WebHook(Update update)
        {
            if (update.Type == UpdateType.MessageUpdate)
            {
                var message = update.Message;
                var chatId = message.Chat.Id;
                var bot = BotBrains.Instance.Value;
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
                                var response = bot.ShowMenu(chatId);

                                await Bot.Api.SendTextMessageAsync(
                                    chatId,
                                    response.ResponceText,
                                    replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                break;
                            }
                        case CmdTypes.Slash:
                            {
                                var keyboard = new InlineKeyboardMarkup(
                                    new[]
                                    {
                                        new[] { new InlineKeyboardButton("Добавить в заказ") },
                                        new[] { new InlineKeyboardButton("Вернуться к меню") }
                                    });

                                var response = bot.GetMenuItem(chatId, update.Message.Text);

                                await Bot.Api.SendTextMessageAsync(
                                    chatId,
                                    response.ResponceText,
                                    replyMarkup: keyboard);
                                break;
                            }
                        case CmdTypes.Check:
                            {
                                var responce = bot.ShowCart(chatId);

                                await Bot.Api.SendTextMessageAsync(
                                    chatId,
                                    responce.ResponceText,
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
                //var message = update.Message;
                var chatId = update.CallbackQuery.From.Id;
                var bot = BotBrains.Instance.Value;
                var parser = ParserChoser.GetParser(bot.GetState(chatId));
                //var cmd = parser.ParseForCommand(update);

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        new InlineKeyboardButton(" << "),
                        new InlineKeyboardButton(" >> "),
                    }
                });

                if (update.CallbackQuery.Data.ToLower().Contains(" << "))
                {
                    await Bot.Api.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId,
                        "/Капучино\n" +
                        "/Латте\n" +
                        "/Мясная лазанья\n" +
                        "/Паста «Карбонара»\n" +
                        "/Салат по-итальянски", replyMarkup: keyboard);
                }
                else if (update.CallbackQuery.Data.ToLower().Contains(" >> "))
                {
                    await Bot.Api.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId,
                        "/Блинчики домашние\n" +
                        "/Блинчики с творогом\n" +
                        "/«Наполеон» с клубникой\n" +
                        "/Торт «Медовик»\n" +
                        "/Торт «Прага»", replyMarkup: keyboard);
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

                    await Bot.Api.SendTextMessageAsync(
                        chatId,
                        response.ResponceText,
                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                }
            }

            return Ok();
        }

        /*
        public async Task<IHttpActionResult> Old_WebHook(Update update)
        {
            if (update.Type == UpdateType.MessageUpdate)
            {
                var message = update.Message;

                if (message.Type == MessageType.TextMessage)
                {
                    var chatId = message.Chat.Id;
                    var bot = BotBrains.Instance.Value;

                    var parser = ParserChoser.GetParser(bot.GetState(chatId));

                    var cmd = parser.ParseForCommand(update);
                    //var cmd = TelegramCmdParser.ParseUpdate(update);

                    if (cmd == CmdTypes.Greetings)
                    {
                        await Bot.Api.SendTextMessageAsync(chatId, bot.Greetings(chatId).ResponceText);
                    }
                    else if (cmd == CmdTypes.TableNumber)
                    {
                        var tableNumber = Convert.ToInt32(message.Text);
                        await Bot.Api.SendTextMessageAsync(chatId, bot.Number(chatId, tableNumber).ResponceText);
                    }
                    else if (cmd == CmdTypes.Menu)
                    {
                        await Bot.Api.SendTextMessageAsync(chatId, bot.ShowMenu(chatId).ResponceText);
                    }
                    else if (cmd == CmdTypes.Check)
                    {
                        await Bot.Api.SendTextMessageAsync(chatId, bot.ShowCart(chatId).ResponceText);
                    }
                    else if (cmd == CmdTypes.InlineKeyboard)
                    {
                        await Bot.Api.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                        var keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[] // first row
                            {
                                new InlineKeyboardButton("Prev 😂"),
                                new InlineKeyboardButton("Next 😉"),
                            },
                            new[] // second row                        
                            {
                                new InlineKeyboardButton("Add to Favorites"),
                                new InlineKeyboardButton("Cancel"),
                            }
                        });

                        await Task.Delay(500); // simulate longer running task
                        await Bot.Api.SendTextMessageAsync(message.Chat.Id,
                            "1. /Капучино\n" +
                            "2. Латте /latte\n" +
                            "3. Мясная лазанья /meatlazania\n" +
                            "4. Паста «Карбонара» /carbonara\n" +
                            "5. Салат по-итальянски /italiansalad", replyMarkup: keyboard);
                    }
                    else if (cmd == CmdTypes.CustomKeyboard)
                    {
                        ReplyKeyboardMarkup myKeyboard = new ReplyKeyboardMarkup()
                        {
                            Keyboard = new KeyboardButton[][]
                            {
                                new KeyboardButton[] { "Меню", "Заказ готов!", "Счет" },
                                new KeyboardButton[] { "Позвать менеджера", "Оставить отзыв" }
                            }
                        };

                        await Bot.Api.SendTextMessageAsync(message.Chat.Id, "Выберите команду!", replyMarkup: myKeyboard);
                    }
                    else if (cmd == CmdTypes.Picture)
                    {
                        await Bot.Api.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                        string file = @"D:\Shared Projects\RestoBot\Bot\Bot\App_Data\Pictures\lazania.jpg";

                        var fileName = file.Split('\\').Last();

                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var fts = new FileToSend(fileName, fileStream);

                            await Bot.Api.SendPhotoAsync(message.Chat.Id, fts, "Лазанья");
                        }
                    }
                    else if (cmd == CmdTypes.PictureLink)
                    {
                        await Bot.Api.SendTextMessageAsync(message.Chat.Id, "https://www.instagram.com/p/BWE-azWgr4K/?taken-by=ferrari");
                    }
                    else if (cmd == CmdTypes.MenuCategories)
                    {
                        //await Bot.Api.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                        var keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new InlineKeyboardButton("Напитки"),
                                new InlineKeyboardButton("Основные блюда"),
                                new InlineKeyboardButton("Десерты"),
                            }
                        });

                        string file = @"D:\Shared Projects\RestoBot\Bot\Bot\App_Data\Pictures\tea.jpg";

                        var fileName = file.Split('\\').Last();

                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var fts = new FileToSend(fileName, fileStream);

                            await Bot.Api.SendPhotoAsync(message.Chat.Id, fts, "", replyMarkup: keyboard);
                        }

                    }
                    else if (cmd == CmdTypes.Slash)
                    {
                        await Bot.Api.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                        string food = update.Message.Text;

                        var keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new InlineKeyboardButton("Описание")
                            },
                            new[]
                            {
                                new InlineKeyboardButton("Заказать"),
                            },
                            new[]
                            {
                                new InlineKeyboardButton("Вернуться к меню"),
                            }
                        });

                        await Task.Delay(500); // simulate longer running task
                        await Bot.Api.SendTextMessageAsync(message.Chat.Id, food, replyMarkup: keyboard);
                    }
                    else if (cmd == CmdTypes.Unknown)
                    {
                        if (BotBrains.Instance.Value.DishNames.Contains(message.Text.ToLower()))
                            await Bot.Api.SendTextMessageAsync(chatId, bot.OrderMeal(chatId, message.Text).ResponceText);
                        else
                            await Bot.Api.SendTextMessageAsync(chatId, "Извините, не понял вашей просьбы :(");
                    }
                }
            }
            else if (update.Type == UpdateType.CallbackQueryUpdate)
            {
                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] // first row
                    {
                        new InlineKeyboardButton("Prev 😂"),
                        new InlineKeyboardButton("Next 😉"),
                    },
                    new[] // second row
                    {
                        new InlineKeyboardButton("Add to Favorites"),
                        new InlineKeyboardButton("Cancel"),
                    }
                });

                if (update.CallbackQuery.Data.ToLower().Contains("prev"))
                {
                    await Bot.Api.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId,
                        "/Капучино\n" +
                        "/Латте\n" +
                        "/Мясная лазанья\n" +
                        "/Паста «Карбонара»\n" +
                        "/Салат по-итальянски", replyMarkup: keyboard);
                }
                else if (update.CallbackQuery.Data.ToLower().Contains("next"))
                {
                    await Bot.Api.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId,
                        "/Блинчики домашние\n" +
                        "/Блинчики с творогом\n" +
                        "/«Наполеон» с клубникой\n" +
                        "/Торт «Медовик»\n" +
                        "/Торт «Прага»", replyMarkup: keyboard);
                }
            }

            return Ok();
        }
        */
    }
}