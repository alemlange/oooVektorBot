using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace Bot.CommandParser
{
    public class SittedSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Меню 📓", "Мой заказ 🍴" },
                        new KeyboardButton[] { "Убрать из заказа ❌" },
                        new KeyboardButton[] { "Попросить счет 💳" },
                        new KeyboardButton[] { "Позвать официанта 🔔" }
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text.ToLower();
            int result;

            if (msgText.Contains("меню"))
            {
                return CmdTypes.Menu;
            }
            else if (msgText.Contains("попросить счет"))
            {
                return CmdTypes.Check;
            }
            else if (msgText.Contains("позвать официанта"))
            {
                return CmdTypes.Waiter;
            }
            else if (msgText.Contains("мой заказ"))
            {
                return CmdTypes.MyOrder;
            }
            else if (msgText.Contains("убрать из заказа"))
            {
                return CmdTypes.Remove;
            }
            else if (msgText.StartsWith("/"))
            {
                return CmdTypes.Slash;
            }
            else if (Int32.TryParse(msgText, out result))
            {
                return CmdTypes.RemoveByNum;
            }
            else
            {
                return CmdTypes.Unknown;
            }
        }
    }
}