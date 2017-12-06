using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class MenuCategorySessionParser : IParser
    {
        protected List<string> Categories { get; set; }

        public MenuCategorySessionParser(List<string> categoryNames)
        {
            Categories = categoryNames;
        }

        public IReplyMarkup Keyboard
        {
            get
            {
                var keys = new List<KeyboardButton[]>();
                keys.Add(new KeyboardButton[] { "Назад ↩" });

                foreach (var cat in Categories)
                {
                    keys.Add(new KeyboardButton[] { cat });
                }

                return new ReplyKeyboardMarkup
                {
                    Keyboard = keys.ToArray()
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text;

                if (msgText.ToLower().Contains("назад"))
                    return CmdTypes.CloseMenu;
                else if (Categories.Contains(msgText))
                    return CmdTypes.Category;
                else
                    return CmdTypes.Unknown;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}