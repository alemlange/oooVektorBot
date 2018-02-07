﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Bot.CommandParser;
using Brains.Models;

namespace Bot.CommandParser.KeyBoards
{
    public static class InlineKeyBoardManager
    {
        public static InlineKeyboardMarkup MenuKeyBoard(IEnumerable<Item> Dishes)
        {
            var arrayDishes = Dishes.ToArray();
            var inKeyboardRows = new InlineKeyboardCallbackButton[arrayDishes.Length][];

            for (var i= 0; i < arrayDishes.Length; i++)
            {
                inKeyboardRows[i] = new[] { new InlineKeyboardCallbackButton(arrayDishes[i].Name, "dish "+ arrayDishes[i].Id) };
            }

            return new InlineKeyboardMarkup(inKeyboardRows);
        }

        public static InlineKeyboardMarkup DescriptionKeyBoard()
        {
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[] { new InlineKeyboardCallbackButton("Добавить в заказ 🍴", "addOrder") }
                });
        }

        public static InlineKeyboardMarkup OrderKeyBoard()
        {
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[] { new InlineKeyboardCallbackButton("⌚ Заберу через...", "arrTime") },
                    new[] { new InlineKeyboardCallbackButton("💳 Оплатить картой и отправить заказ", "payCard") },
                    new[] { new InlineKeyboardCallbackButton("💰 Отправить заказ, оплачу наличными", "payCash") }
                });
        }

        public static InlineKeyboardMarkup TimeKeyBoard()
        {
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[] { new InlineKeyboardCallbackButton("Сейчас", "time 0") },
                    new[] { new InlineKeyboardCallbackButton("5 минут.", "time 5") },
                    new[] { new InlineKeyboardCallbackButton("10 минут.", "time 10") },
                    new[] { new InlineKeyboardCallbackButton("15 минут.", "time 15") },
                    new[] { new InlineKeyboardCallbackButton("20 минут.", "time 20") },
                    new[] { new InlineKeyboardCallbackButton("30 минут.", "time 30") }
                });
        }

        public static InlineKeyboardMarkup GetByCmnd(CmdTypes command)
        {
            switch (command)
            {
                case CmdTypes.DishDetails:
                    return DescriptionKeyBoard();
                case CmdTypes.Cart:
                    return OrderKeyBoard();
                case CmdTypes.ArrivingTime:
                    return TimeKeyBoard();
                default:
                    throw new Exception("Unknown command"); 
            }
        }
    }
}