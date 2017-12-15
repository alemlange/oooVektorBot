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
                        new KeyboardButton[] { "Меню 📓" },
                        new KeyboardButton[] { "Мой заказ 🍴", "Убрать из заказа ❌" },
                        new KeyboardButton[] { "Оплатить заказ💳" },
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();
                int result;

                if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("попросить счет"))
                    return CmdTypes.Check;
                else if (msgText.Contains("мой заказ"))
                    return CmdTypes.MyOrder;
                else if (msgText.Contains("оплатить заказ"))
                    return CmdTypes.CreateInvoice;
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