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
                        new KeyboardButton[] { "📄 О заведении" },
                        new KeyboardButton[] { "📔 Меню", "🎁 Акции","💸 Оплата" },
                        new KeyboardButton[] { "🛒 Мой заказ", "🙋🏼‍♂️ Вызов официанта" },
                        new KeyboardButton[] { "🍽 Забронировать столик", "📫 Оставить отзыв" },
                        new KeyboardButton[] { "🗺 Адреса и часы работы" }
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Type == UpdateType.CallbackQueryUpdate)
            {
                var data = update.CallbackQuery.Data;

                if (data.Contains("time"))
                {
                    return CmdTypes.TimeInput;
                }
                else if (data.Contains("dish"))
                {
                    return CmdTypes.DishDetails;
                }
                else if (data.Contains("addOrder"))
                {
                    return CmdTypes.AddToOrder;
                }
                else if (data.Contains("mod"))
                {
                    return CmdTypes.AddMod;
                }
                else
                {
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
                        case ("backMenu"):
                            {
                                return CmdTypes.BackToMenu;
                            }
                        default:
                            return CmdTypes.Unknown;
                    }
                }
            }
            else if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();
                int result;

                if (msgText == "📔 меню")
                    return CmdTypes.Menu;
                else if (msgText.Contains("мой заказ"))
                    return CmdTypes.Cart;
                else if (msgText.Contains("убрать из заказа"))
                    return CmdTypes.Remove;
                else if (msgText.Contains("вызов официанта"))
                    return CmdTypes.Waiter;
                else if (msgText.Contains("о заведении"))
                    return CmdTypes.Description;
                else if (msgText.Contains("адреса"))
                    return CmdTypes.Location;
                else if (msgText.Contains("оплата"))
                    return CmdTypes.RequestPayment;
                else if (msgText.Contains("акции"))
                    return CmdTypes.Actions;
                else if (msgText.Contains("оставить отзыв"))
                    return CmdTypes.RequestFeedback;
                else if (msgText.Contains("забронировать столик"))
                    return CmdTypes.RequestBooking;
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