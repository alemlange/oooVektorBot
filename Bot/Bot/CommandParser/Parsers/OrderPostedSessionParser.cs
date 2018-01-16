using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class OrderPostedSessionParser : IParser
    {
        protected List<string> Categories { get; set; }

        public OrderPostedSessionParser(List<string> categoryNames)
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
                        new KeyboardButton[] { "📓 Меню" },
                        new KeyboardButton[] { "🍴 Мой заказ" }
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Type == UpdateType.CallbackQueryUpdate)
            {
                var data = update.CallbackQuery.Data;

                switch (data)
                {
                    case ("backMenu"):
                        {
                            return CmdTypes.BackToMenu;
                        }
                    case ("addOrder"):
                        {
                            return CmdTypes.AddToOrder;
                        }
                    default:
                        return CmdTypes.Unknown;
                }
            }
            else if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();

                if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("мой заказ"))
                    return CmdTypes.MyOrderComplete;
                else if (msgText.Contains("назад"))
                    return CmdTypes.CloseMenu;
                else if (Categories.Select(o => o.ToLower()).Contains(msgText))
                    return CmdTypes.Category;
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