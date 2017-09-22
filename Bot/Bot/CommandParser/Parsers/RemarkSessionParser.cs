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
                default:
                    {
                        //return CmdTypes.Unknown;
                        return CmdTypes.Remark;
                    }
            }
        }
    }
}