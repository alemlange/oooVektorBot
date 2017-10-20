using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Brains;

namespace Bot.CommandParser
{
    public class RestruntSessionParser : SessionParser, IParser
    {
        public RestruntSessionParser(List<string> restaurantNames, int tablesCount) : base(restaurantNames, tablesCount)
        {
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
            var msgText = update.Message.Text;
            var brains = new BotBrains();
            Restaurants = brains.RestaurantNames;

            if (Restaurants.Contains(msgText))
                return CmdTypes.Restrunt;
            else if (msgText.ToLower() == "назад ↩") // todo
                return CmdTypes.Menu;
            else
                return CmdTypes.Unknown;
        }
    }
}