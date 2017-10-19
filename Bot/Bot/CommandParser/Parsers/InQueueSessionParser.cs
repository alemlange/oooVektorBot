using System;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Brains;
using System.Collections.Generic;

namespace Bot.CommandParser
{
    public class InQueueSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                var keys = new List<KeyboardButton[]>();
                var brains = new BotBrains();
                var tablesCount = brains.Config.TablesCount;

                keys.Add(new KeyboardButton[] { "Меню 📓" });

                if (tablesCount > 0)
                {
                    int table = 1;
                    while (table <= tablesCount)
                    {
                        if (table + 2 <= tablesCount)
                        {
                            keys.Add(new KeyboardButton[] { table.ToString(), (table + 1).ToString(), (table + 2).ToString() });
                            table += 3;
                        }
                        else if (table + 1 <= tablesCount)
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