using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class MenuCategorySessionParser : IParser
    {
        protected List<string> Categories { get; set; }

        public MenuCategorySessionParser(List<string> categoryNames)
        {
            Categories = categoryNames;
        }

        public IReplyMarkup Keyboard
        {
            get
            {
                var keys = new List<KeyboardButton[]>();
                keys.Add(new KeyboardButton[] { "↩ Назад" });

                foreach (var cat in Categories)
                {
                    keys.Add(new KeyboardButton[] { cat });
                }

                return new ReplyKeyboardMarkup
                {
                    Keyboard = keys.ToArray()
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if(update.Type == UpdateType.CallbackQueryUpdate)
            {
                var data = update.CallbackQuery.Data;

                if (data.Contains("time"))
                {
                    return CmdTypes.TimeInput;
                }
                else if (data.Contains("dish"))
                {
                    return CmdTypes.DishDetails;
                }
                else if (data.Contains("addOrder"))
                {
                    return CmdTypes.AddToOrder;
                }
                else if (data.Contains("mod"))
                {
                    return CmdTypes.AddMod;
                }
                else
                {
                    switch (data)
                    {
                        case ("arrTime"):
                            {
                                return CmdTypes.ArrivingTime;
                            }
                        case ("payCard"):
                            {
                                return CmdTypes.CreateInvoice;
                            }
                        case ("payCash"):
                            {
                                return CmdTypes.PayCash;
                            }
                        case ("backMenu"):
                            {
                                return CmdTypes.BackToMenu;
                            }
                        default:
                            return CmdTypes.Unknown;
                    }
                }
            }
            else if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();

                if (msgText.Contains("назад"))
                    return CmdTypes.CloseMenu;
                else if (Categories.Select(o => o.ToLower()).Contains(msgText))
                    return CmdTypes.Category;
                else
                    return CmdTypes.Unknown;
            }
            else if (update.Message.Type == MessageType.SuccessfulPayment)
            {
                return CmdTypes.SuccessfulPayment;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}