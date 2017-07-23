using System;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace Bot.CommandParser
{
    public class InQueueSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Меню", "1", "2", "3" },
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text;
            int result;

            if (Int32.TryParse(msgText, out result))
                return CmdTypes.TableNumber;
            else if (msgText.ToLower() == "меню")
                return CmdTypes.Menu;
            else
                return CmdTypes.Unknown;
        }
    }
}