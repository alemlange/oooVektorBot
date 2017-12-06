using System;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;

namespace Bot.CommandParser
{
    public class UnknownSessionParser : IParser
    {
        protected List<string> Categories { get; set; }

        public UnknownSessionParser(List<string> categoryNames)
        {
            Categories = categoryNames;
        }

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
                else if (msgText.Contains("назад"))
                    return CmdTypes.CloseMenu;
                else if (Categories.Select(o => o.ToLower()).Contains(msgText))
                    return CmdTypes.Category;
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