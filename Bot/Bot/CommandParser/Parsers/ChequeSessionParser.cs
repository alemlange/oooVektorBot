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
    public class ChequeSessionParser : IParser
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
                    },
                    ResizeKeyboard = true
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

                int num;
                var isNumber = Int32.TryParse(msgText, out num);

                if (msgText == "↩ Отменить") 
                    return CmdTypes.CancelTable;
                else if (isNumber)
                    return CmdTypes.InputSumm;
                else
                    return CmdTypes.Unknown;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}