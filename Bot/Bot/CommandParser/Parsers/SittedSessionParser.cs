using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class SittedSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "📓 Меню" },
                        new KeyboardButton[] { "🛒 Корзина", "❌ Убрать из заказа" }
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
                    case ("arrTime"):
                        {
                            return CmdTypes.ArrivingTime;
                        }
                    case ("payCard"):
                        {
                            return CmdTypes.CreateInvoice;
                        }
                    case ("payCash"):
                        {
                            return CmdTypes.PayCash;
                        }
                    case ("addOrder"):
                        {
                            return CmdTypes.AddToOrder;
                        }
                    case ("backMenu"):
                        {
                            return CmdTypes.BackToMenu;
                        }
                    default:
                        return CmdTypes.Unknown;
                }
            }
            else if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();
                int result;

                if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("корзина"))
                    return CmdTypes.Cart;
                else if (msgText.Contains("убрать из заказа"))
                    return CmdTypes.Remove;
                else if (msgText.StartsWith("/"))
                    return CmdTypes.Slash;
                else if (Int32.TryParse(msgText, out result))
                    return CmdTypes.RemoveByNum;
                else
                    return CmdTypes.Unknown;
            }
            else if (update.Message.Type == MessageType.SuccessfulPayment)
            {
                return CmdTypes.SuccessfulPayment;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}