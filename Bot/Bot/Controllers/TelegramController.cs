﻿using System;
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
            Bot.Api.SetWebhook("https://9fecb480.ngrok.io/Telegram/WebHook").Wait();


            //Bot.Api.StopReceiving();

            Bot.Api.OnCallbackQuery += BotOnCallbackQueryReceived;
            //Bot.Api.OnMessage += BotOnMessageReceived;
            //Bot.Api.OnMessageEdited += BotOnMessageReceived;
            Bot.Api.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.Api.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.Api.OnReceiveError += BotOnReceiveError;

            //Bot.Api.StartReceiving();

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
                else if (cmd == CmdTypes.TableNumber)
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
                else if (cmd == CmdTypes.InlineKeyboard)
                {
                    await Bot.Api.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                    var keyboard = new InlineKeyboardMarkup(new[]
                    {
                        new[] // first row
                        {
                            new InlineKeyboardButton("Prev"),
                            new InlineKeyboardButton("Next"),
                        },
                        new[] // second row                        
                        {
                            new InlineKeyboardButton("Add to Favorites"),
                            new InlineKeyboardButton("Cancel"),
                        }
                    });

                    await Task.Delay(500); // simulate longer running task
                    await Bot.Api.SendTextMessageAsync(message.Chat.Id, "Choose", replyMarkup: keyboard);
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
                


                else if (cmd == CmdTypes.MenuPages)
                {
                    //await Bot.Api.EditInlineMessageTextAsync() SendTextMessageAsync(chatId, "");

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
                    await Bot.Api.SendTextMessage(message.Chat.Id, "https://www.instagram.com/p/BWE-azWgr4K/?taken-by=ferrari");
                }
                else if (cmd == CmdTypes.Unknown)
                {
                    if (BotBrains.Instance.Value.DishNames.Contains(message.Text.ToLower()))
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

        public static InlineKeyboardMarkup InlineKeyboardMarkupMaker(Dictionary<int, string> items)
        {
            InlineKeyboardButton[][] ik = items.Select(item => new[]
            {
                new InlineKeyboardButton(item.Key.ToString(), item.Value)
            }).ToArray();

            return new InlineKeyboardMarkup(ik);
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {

        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            //Console.WriteLine($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.Api.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id, //"QQ");
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            InlineQueryResult[] results = {
                new InlineQueryResultLocation
                {
                    Id = "1",
                    Latitude = 40.7058316f, // displayed result
                    Longitude = -74.2581888f,
                    Title = "New York",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Latitude = 40.7058316f,
                        Longitude = -74.2581888f,
                    }
                },

                new InlineQueryResultLocation
                {
                    Id = "2",
                    Longitude = 52.507629f, // displayed result
                    Latitude = 13.1449577f,
                    Title = "Berlin",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Longitude = 52.507629f,
                        Latitude = 13.1449577f
                    }
                }
            };

            await Bot.Api.AnswerInlineQueryAsync(inlineQueryEventArgs.InlineQuery.Id, results, isPersonal: true, cacheTime: 0);
        }
    }
}