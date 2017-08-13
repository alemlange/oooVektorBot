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
                        new KeyboardButton[] { "Меню", "Счет" },
                        new KeyboardButton[] { "Позвать официанта" }
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text.ToLower();
            switch (msgText)
            {
                case "меню":
                    {
                        return CmdTypes.Menu;
                    }
                case "позвать официанта":
                    {
                        return CmdTypes.Waiter;
                    }
                case "счет":
                    {
                        return CmdTypes.Check;
                    }
                default:
                    {
                        if (msgText.StartsWith("/"))
                        {
                            return CmdTypes.Slash;
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