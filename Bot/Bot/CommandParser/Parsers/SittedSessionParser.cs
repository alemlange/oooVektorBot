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

            switch (msgText)
            {
                case "меню 📓":
                    {
                        return CmdTypes.Menu;
                    }
                case "попросить счет 💳":
                    {
                        return CmdTypes.Check;
                    }
                case "позвать официанта 🔔":
                    {
                        return CmdTypes.Waiter;
                    }
                case "мой заказ 🍴":
                    {
                        return CmdTypes.MyOrder;
                    }
                case "убрать из заказа ❌":
                    {
                        return CmdTypes.Remove;
                    }
                default:
                    {
                        if (msgText.StartsWith("/"))
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
    }
}