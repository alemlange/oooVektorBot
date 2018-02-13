using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class RemarkSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new KeyboardButton[][]
                     {
                        new KeyboardButton[] { "📓 Меню" },
                        new KeyboardButton[] { "🛒 Корзина", "❌ Убрать из заказа" }
                     }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Type == UpdateType.CallbackQueryUpdate)
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

                if (msgText.Contains("вернуться к меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("корзина"))
                    return CmdTypes.Cart;
                else if (msgText.Contains("убрать из заказа"))
                    return CmdTypes.Remove;
                else
                    return CmdTypes.Remark;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}