﻿using Telegram.Bot.Types.ReplyMarkups;
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
                return new InlineKeyboardMarkup(
                    new[]
                    {
                        new[] { new InlineKeyboardButton("Вернуться к меню 📓") }
                    });
            }
        }

        public CmdTypes ParseForCommand(Update update)
        {
            if (update.Message.Type == MessageType.TextMessage)
            {
                var msgText = update.Message.Text.ToLower();

                if (msgText.Contains("вернуться к меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("меню"))
                    return CmdTypes.Menu;
                else if (msgText.Contains("попросить счет"))
                    return CmdTypes.Check;
                else if (msgText.Contains("позвать официанта"))
                    return CmdTypes.Waiter;
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