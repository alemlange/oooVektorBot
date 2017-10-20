using System;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Brains;
using System.Collections.Generic;

namespace Bot.CommandParser
{
    public class InQueueSessionParser : SessionParser, IParser
    {
        public InQueueSessionParser(List<string> restaurantNames, int tablesCount) : base(restaurantNames, tablesCount)
        {
        }

        public IReplyMarkup Keyboard
        {
            get
            {
                var keys = new List<KeyboardButton[]>();
                keys.Add(new KeyboardButton[] { "Меню 📓" });

                if (TablesCount > 0)
                {
                    int table = 1;
                    while (table <= TablesCount)
                    {
                        if (table + 2 <= TablesCount)
                        {
                            keys.Add(new KeyboardButton[] { table.ToString(), (table + 1).ToString(), (table + 2).ToString() });
                            table += 3;
                        }
                        else if (table + 1 <= TablesCount)
                        {
                            keys.Add(new KeyboardButton[] { table.ToString(), (table + 1).ToString() });
                            table += 2;
                        }
                        else
                        {
                            keys.Add(new KeyboardButton[] { table.ToString() });
                            table += 1;
                        }
                    }
                }

                return new ReplyKeyboardMarkup
                {
                    Keyboard = keys.ToArray()
                };

            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            var msgText = update.Message.Text.ToLower();
            int result;

            if (Int32.TryParse(msgText, out result))
                return CmdTypes.TableNumber;
            else if (msgText.Contains("меню"))
                return CmdTypes.Menu;
            else
                return CmdTypes.Unknown;
        }
    }
}