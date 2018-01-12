using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Bot.CommandParser;

namespace Bot.CommandParser.KeyBoards
{
    public static class InlineKeyBoardManager
    {
        public static InlineKeyboardMarkup DescriptionKeyBoard()
        {
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[] { new InlineKeyboardCallbackButton("Добавить в заказ 🍴", "Добавить в заказ 🍴") },
                    new[] { new InlineKeyboardCallbackButton("Вернуться к меню 📓", "Вернуться к меню 📓") }
                });
        }

        public static InlineKeyboardMarkup OrderKeyBoard()
        {
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[] { new InlineKeyboardCallbackButton("Заберу через...", "arrTime") },
                    new[] { new InlineKeyboardCallbackButton("Оплатить картой и отправить заказ", "payCard") },
                    new[] { new InlineKeyboardCallbackButton("Отправить заказ, а оплачу наличными", "payCash") }
                });
        }

        public static InlineKeyboardMarkup MenuNavKeyBoard(int pageCount, int curPage)
        {
            var keyboard = new InlineKeyboardMarkup();

            if (pageCount > 1)
            {
                InlineKeyboardButton prev = new InlineKeyboardCallbackButton("", "");
                InlineKeyboardButton next = new InlineKeyboardCallbackButton("", "");

                if (curPage > 1)
                    prev = new InlineKeyboardCallbackButton((curPage - 1) + " стр.  ⬅ ", (curPage - 1) + " стр.  ⬅ ");

                if (curPage < pageCount)
                    next = new InlineKeyboardCallbackButton(" ➡  " + (curPage + 1) + " стр.", " ➡  " + (curPage + 1) + " стр.");

                if (prev.Text != "" && next.Text != "")
                    keyboard = new InlineKeyboardMarkup(new[] { prev, next });
                else if (prev.Text == "" && next.Text != "")
                    keyboard = new InlineKeyboardMarkup(new[] { next });
                else if (prev.Text != "" && next.Text == "")
                    keyboard = new InlineKeyboardMarkup(new[] { prev });
            }
            return keyboard;
        }

        public static InlineKeyboardMarkup GetByCmnd(CmdTypes command)
        {
            switch (command)
            {
                case CmdTypes.Slash:
                    return DescriptionKeyBoard();
                case CmdTypes.MyOrder:
                    return OrderKeyBoard();
                default:
                    throw new Exception("Unknown command"); 
            }
        }
    }
}