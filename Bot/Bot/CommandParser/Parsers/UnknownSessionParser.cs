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
                        //new KeyboardButton[] { "🛍️ Мои заказы" },
                        new KeyboardButton[] { "🗺 Адреса", "📄 О нас" },
                        new KeyboardButton[] { "🍽 Забронировать столик" },
                        new KeyboardButton[] { "📔 Меню", "🎁 Акции" },
                        new KeyboardButton[] { "👉 Заказ за столиком" },
                        new KeyboardButton[] { "📫 Оставить отзыв" }
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
                else if (msgText.Contains("заказ за столиком"))
                    return CmdTypes.Greetings;
                else if (msgText.Contains("акции"))
                    return CmdTypes.Actions;
                else if (msgText.Contains("о нас"))
                    return CmdTypes.Description;
                else if (msgText.Contains("адреса"))
                    return CmdTypes.Location;
                else if (msgText.Contains("оставить отзыв"))
                    return CmdTypes.RequestFeedback;
                else if (msgText.Contains("забронировать столик"))
                    return CmdTypes.RequestBooking;
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