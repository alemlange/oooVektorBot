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
                        new KeyboardButton[] { "🍴 Мой заказ", "❌ Убрать из заказа" }
                     }
                };
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Type == UpdateType.CallbackQueryUpdate)
            {
                var data = update.CallbackQuery.Data;

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
                    case ("addOrder"):
                        {
                            return CmdTypes.AddToOrder;
                        }
                    case ("backMenu"):
                        {
                            return CmdTypes.BackToMenu;
                        }
                    default:
                        return CmdTypes.Unknown;
                }
            }
            else if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();

                if (msgText.Contains("вернуться к меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("мой заказ"))
                    return CmdTypes.MyOrder;
                else if (msgText.Contains("убрать из заказа"))
                    return CmdTypes.Remove;
                if (msgText.StartsWith("/"))
                    return CmdTypes.Slash;
                else
                    return CmdTypes.Remark;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}