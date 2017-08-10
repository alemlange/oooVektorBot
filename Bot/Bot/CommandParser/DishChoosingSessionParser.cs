using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace Bot.CommandParser
{
    public class DishChoosingSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] { new InlineKeyboardButton("Заказать") },
                        new[] { new InlineKeyboardButton("Вернуться к меню") }
                    });
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text.ToLower();
            switch (msgText)
            {
                case "меню":
                    {
                        return CmdTypes.Menu;
                    }
                case "официант":
                    {
                        return CmdTypes.Waiter;
                    }
                case "счет":
                    {
                        return CmdTypes.Check;
                    }
                default:
                    {
                        return CmdTypes.Unknown;
                    }
            }
        }
    }
}