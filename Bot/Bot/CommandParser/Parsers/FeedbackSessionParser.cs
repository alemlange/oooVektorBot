using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Brains;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class FeedbackSessionParser : IParser
    {

        public IReplyMarkup Keyboard
        {
            get
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "↩ Отменить" },
                    }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Type == UpdateType.CallbackQueryUpdate)
            {
                return CmdTypes.Unknown;
            }
            else if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text;

                if (msgText == "↩ Отменить") 
                    return CmdTypes.CancelFeedback;
                else
                    return CmdTypes.LeaveFeedback;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}