using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.CommandParser
{
    public class TimeArrivingSessionParser : IParser
    {
        public IReplyMarkup Keyboard
        {
            get
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new KeyboardButton[][]
                     {
                        new KeyboardButton[] { "↩ Назад" },
                        new KeyboardButton[] { "5 минут" },
                        new KeyboardButton[] { "10 минут" },
                        new KeyboardButton[] { "15 минут" },
                        new KeyboardButton[] { "20 минут" },
                        new KeyboardButton[] { "30 минут "}
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

                if (msgText.Contains("минут"))
                    return CmdTypes.TimeInput;
                else if (msgText.Contains("назад"))
                    return CmdTypes.CloseTimeArriving;
                else
                    return CmdTypes.Unknown;
            }
            else
                return CmdTypes.Unknown;
        }
    }
}