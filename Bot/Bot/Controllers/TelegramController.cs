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
using Bot.CommandParser.KeyBoards;
using Brains;
using DataModels.Enums;
using LiteDbService.Helpers;
using DataModels.Configuration;

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
            var accountId = ConfigurationSettings.AccountId;
            var regService = ServiceCreator.GetRegistrationService();
            var account = regService.FindAccount(accountId);
            var service = ServiceCreator.GetCustomerService(account.Login);
            service.RemoveAllTables();

            return "Ok" ;
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendMessage(long chatId, string message)
        {
            try
            {
                await Bot.Api.SendTextMessageAsync(
                    chatId,
                    message,
                    parseMode: ParseMode.Html);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }

            return Ok();
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
                    var state = bot.GetState(chatId);
                    var parser = ParserChoser.GetParser(state);
                    var cmd = parser.ParseForCommand(update);

                    if (message.Type == MessageType.TextMessage)
                    {
                        switch (cmd)
                        {
                            case CmdTypes.Start:
                                {
                                    // todo bot name from config
                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        "Здравствуйте, меня зовут РестоБот! Я знаю все о местной кухне, могу рассказать вам " +
                                        "о наших блюдах, принять заказ с учетом ваших вкусов и пожеланий, а так же помочь вам " +
                                        "в любых вопросах! Для того, чтобы ознакомиться с меню напишите \"меню\", чтобы " +
                                        "сделать заказ напишите \"начать\". Приятного отдыха!",
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(SessionState.Unknown).Keyboard);
                                    break;
                                }
                            case CmdTypes.Greetings:
                                {
                                    var responce = bot.Greetings(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Restrunt:
                                {
                                    var responce = bot.Restrunt(chatId, message.Text);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.TableNumber:
                                {
                                    var response = bot.Number(chatId, Convert.ToInt32(message.Text));

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Menu:
                                {
                                    var response = bot.ShowMenuOnPage(chatId);

                                    if (response.PageCount > 1)
                                    {
                                        var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                                        await Bot.Api.SendTextMessageAsync(chatId, response.ResponceText, parseMode: ParseMode.Html, replyMarkup: keyboard);
                                    }
                                    else
                                        await Bot.Api.SendTextMessageAsync(chatId, response.ResponceText);
                                    
                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        "Хотите чтонибудь из меню? Просто кликните по нему!",
                                        parseMode: ParseMode.Html,
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
                                        parseMode: ParseMode.Html,
                                        replyMarkup: keyboard);
                                    break;
                                }
                            case CmdTypes.MyOrder:
                                {
                                    var responce = bot.ShowCart(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        responce.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Check:
                                {
                                    var response = bot.GiveACheck(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Waiter:
                                {
                                    var response = bot.CallWaiter(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Remark:
                                {
                                    var response = bot.AddRemark(chatId, message.Text);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Remove:
                                {
                                    var response = bot.RemoveFromOrder(chatId);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.RemoveByNum:
                                {
                                    var response = bot.RemoveFromOrderByNum(chatId, message.Text);

                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        response.ResponceText,
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                                    break;
                                }
                            case CmdTypes.Unknown:
                                {
                                    await Bot.Api.SendTextMessageAsync(
                                        chatId,
                                        "Извините, не понял вашей просьбы :(",
                                        parseMode: ParseMode.Html);
                                    break;
                                }
                        }
                    }
                    else if (message.Type == MessageType.PhotoMessage)
                    {
                        var file = await Bot.Api.GetFileAsync(message.Photo.LastOrDefault()?.FileId);
                        var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                        //using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                        //{
                        //    await file.FileStream.CopyToAsync(saveImageStream);
                        //}

                        await Bot.Api.SendTextMessageAsync(
                            chatId,
                            ":)",
                            parseMode: ParseMode.Html);
                    }
                }
                else if (update.Type == UpdateType.CallbackQueryUpdate)
                {
                    var chatId = update.CallbackQuery.From.Id;
                    var messageId = update.CallbackQuery.Message.MessageId;

                    if (update.CallbackQuery.Data.ToLower().Contains("  ⬅ "))
                    {
                        var page = int.Parse(update.CallbackQuery.Data.Trim('⬅', ' ', 'с', 'т', 'р', '.'));
                        var response = bot.ShowMenuOnPage(chatId, page);

                        var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                        await Bot.Api.EditMessageTextAsync(
                            chatId,
                            messageId,
                            response.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains(" ➡  "))
                    {
                        var page = int.Parse(update.CallbackQuery.Data.Trim('➡', ' ', 'с', 'т', 'р', '.'));

                        var response = bot.ShowMenuOnPage(chatId, page);

                        var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                        await Bot.Api.EditMessageTextAsync(
                            chatId,
                            messageId,
                            response.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains("добавить в заказ"))
                    {
                        var response = bot.OrderMeal(chatId);

                        await Bot.Api.SendTextMessageAsync(
                            chatId,
                            response.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                    }
                    else if (update.CallbackQuery.Data.ToLower().Contains("вернуться к меню"))
                    {
                        var response = bot.ShowMenuOnPage(chatId);

                        var keyboard = InlineKeyBoardManager.MenuNavKeyBoard(response.PageCount, response.Page);

                        await Bot.Api.SendTextMessageAsync(
                            chatId,
                            response.ResponceText,
                            parseMode: ParseMode.Html,
                            replyMarkup: keyboard);

                        await Bot.Api.SendTextMessageAsync(
                            chatId,
                            "Хотите чтонибудь из меню? Просто кликните по нему!",
                            parseMode: ParseMode.Html,
                            replyMarkup: ParserChoser.GetParser(bot.GetState(chatId)).Keyboard);
                    }
                }
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                StreamWriter file = new System.IO.StreamWriter("c:\\db\\errors.txt", true);
                file.WriteLine(mes);

                file.Close();
            }

            return Ok();
        }

    }
}