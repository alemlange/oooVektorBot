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
                keys.Add(new KeyboardButton[] { "Закрыть меню ↩" });

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

                if (Categories.Contains(msgText))
                    return CmdTypes.MenuCategory;
                else if (msgText.ToLower().Contains("закрыть меню"))
                    return CmdTypes.CloseMenu;
                else
                    return CmdTypes.Unknown;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}