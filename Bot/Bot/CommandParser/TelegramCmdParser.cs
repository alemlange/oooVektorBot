using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class TelegramCmdParser
    {
        public static CmdTypes ParseUpdate(Update update)
        {
            var msgText = update.Message.Text;
            int result;

            if (msgText.ToLower() == "привет")
                return CmdTypes.Greetings;
            else if (Int32.TryParse(msgText, out result))
                return CmdTypes.TableNumber;
            else if (msgText.ToLower() == "меню")
                return CmdTypes.Menu;
            else if (msgText.ToLower() == "счет")
                return CmdTypes.Check;
            else
                return CmdTypes.Unknown;
        }
    }
}