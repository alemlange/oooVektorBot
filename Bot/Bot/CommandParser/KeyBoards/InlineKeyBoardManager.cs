using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
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
                    new[] { new InlineKeyboardButton("Добавить в заказ") },
                    new[] { new InlineKeyboardButton("Вернуться к меню") }
                });
        }

        public static InlineKeyboardMarkup MenuNavKeyBoard(int pageCount, int curPage)
        {
            var keyboard = new InlineKeyboardMarkup();

            if (pageCount > 1)
            {
                InlineKeyboardButton prev = new InlineKeyboardButton("");
                InlineKeyboardButton next = new InlineKeyboardButton("");

                if (curPage > 1)
                    prev = new InlineKeyboardButton((curPage - 1) + "  << ");

                if (curPage < pageCount)
                    next = new InlineKeyboardButton(" >>  " + (curPage + 1));

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
                default:
                    throw new Exception("Unknown command"); 
            }
        }
    }
}