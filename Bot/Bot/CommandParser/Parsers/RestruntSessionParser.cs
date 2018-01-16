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
    public class RestruntSessionParser : IParser
    {
        protected List<string> Restaurants { get; set; }

        public RestruntSessionParser(List<string> restaurantNames)
        {
            Restaurants = restaurantNames;
        }

        public IReplyMarkup Keyboard
        {
            get
            {
                var keys = new List<KeyboardButton[]>();
                
                foreach (var res in Restaurants)
                {
                    keys.Add(new KeyboardButton[] { res });
                }

                return new ReplyKeyboardMarkup
                {
                    Keyboard = keys.ToArray()
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

                if (Restaurants.Contains(msgText))
                    return CmdTypes.Restrunt;
                else if (msgText.ToLower() == "↩ назад") // todo
                    return CmdTypes.Menu;
                else
                    return CmdTypes.Unknown;
            }
            else if (update.Message.Type == MessageType.PhotoMessage)
                return CmdTypes.QRCode;
            else
                return CmdTypes.Unknown;
        }
    }
}