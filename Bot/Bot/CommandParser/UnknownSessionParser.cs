using System;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace Bot.CommandParser
{
    public class UnknownSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Меню" },
                        new KeyboardButton[] { "Привет"},
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text.ToLower();

            switch (msgText.ToLower())
            {
                case "меню":
                    {
                        return CmdTypes.Menu;
                    }
                case "привет":
                    {
                        return CmdTypes.Greetings;
                    }
                default:
                    {
                        return CmdTypes.Unknown;
                    }
            }
        }
    }
}