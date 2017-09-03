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
                        new KeyboardButton[] { "Меню 📓" },
                        new KeyboardButton[] { "Начать"},
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text.ToLower();

            if (msgText.Contains("меню"))
            {
                return CmdTypes.Menu;
            }
            else if (msgText == "начать")
            {
                return CmdTypes.Greetings;
            }
            else
            {
                return CmdTypes.Unknown;
            }
        }
    }
}