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
                        new KeyboardButton[] { "🛍️ Мои заказы" },
                        new KeyboardButton[] { "Начать Заказ" },
                        new KeyboardButton[] { "Акции" },
                        new KeyboardButton[] { "Адреса" },
                        new KeyboardButton[] { "Описание" },
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Type == UpdateType.CallbackQueryUpdate)
            {
                var data = update.CallbackQuery.Data;

                if(data == "addOrder")
                {
                    return CmdTypes.AddToOrder;
                }
                else if (data.Contains("dish"))
                {
                    return CmdTypes.DishDetails;
                }
                else if (data.Contains("addOrder"))
                {
                    return CmdTypes.AddToOrder;
                }
                else
                    return CmdTypes.Unknown;  
            }
            if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();

                if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText == "начать заказ")
                    return CmdTypes.Greetings;
                else if (msgText == "акции")
                    return CmdTypes.Actions;
                else if (msgText == "описание")
                    return CmdTypes.Description;
                else if (msgText == "адреса")
                    return CmdTypes.Location;
                else if (msgText.Contains("мои заказы"))
                    return CmdTypes.MyOrders;
                else if (msgText.Contains("назад"))
                    return CmdTypes.CloseMenu;
                else if (Categories.Select(o => o.ToLower()).Contains(msgText))
                    return CmdTypes.Category;
                else if (msgText == "/start")
                    return CmdTypes.Start;
                else
                    return CmdTypes.Unknown;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}