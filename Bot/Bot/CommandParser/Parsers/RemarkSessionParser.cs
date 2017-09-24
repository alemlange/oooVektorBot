using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace Bot.CommandParser
{
    public class RemarkSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] { new InlineKeyboardButton("Вернуться к меню 📓") }
                    });
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text.ToLower();

            switch (msgText)
            {
                case "вернуться к меню 📓":
                    {
                        return CmdTypes.Menu;
                    }
                case "меню 📓":
                    {
                        return CmdTypes.Menu;
                    }
                case "попросить счет 💳":
                    {
                        return CmdTypes.Check;
                    }
                case "позвать официанта 🔔":
                    {
                        return CmdTypes.Waiter;
                    }
                case "мой заказ 🍴":
                    {
                        return CmdTypes.MyOrder;
                    }
                case "убрать из заказа ❌":
                    {
                        return CmdTypes.Remove;
                    }
                default:
                    {
                        if (msgText.StartsWith("/"))
                        {
                            return CmdTypes.Slash;
                        }
                        //else if (Int32.TryParse(msgText, out result))
                        //{
                        //    return CmdTypes.RemoveByNum;
                        //}
                        else
                        {
                            return CmdTypes.Remark;
                        }
                    }
            }
        }
    }
}