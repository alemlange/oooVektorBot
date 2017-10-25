using System;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
                        new KeyboardButton[] { "Начать" },
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();

                if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText == "начать")
                    return CmdTypes.Greetings;
                else if (msgText == "/start")
                    return CmdTypes.Start;
                else if (msgText != "/start" && msgText.StartsWith("/"))
                    return CmdTypes.Slash;
                else
                    return CmdTypes.Unknown;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}