using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Brains;
using DataModels.Enums;

namespace Bot.CommandParser
{
    public static class ParserChoser
    {
        public static IParser GetParser(long chatId, BotBrains bot)
        {
            var state = bot.GetState(chatId);
            var categories = bot.GetMenuCategoriesByChatId(chatId);

            switch (state)
            {
                case SessionState.MenuCategory:
                    {
                        return new MenuCategorySessionParser(categories);
                    }
                case SessionState.Restaurant:
                    {
                        return new RestruntSessionParser(bot.RestaurantNames);
                    }
                case SessionState.Queue:
                    {
                        var tableCount = bot.RestTableCount(chatId);
                        return new InQueueSessionParser(tableCount);
                    }
                case SessionState.Sitted:
                    return new SittedSessionParser();
                case SessionState.Remark:
                    return new RemarkSessionParser();
                case SessionState.Unknown:
                    return new UnknownSessionParser(categories);
                default:
                    return new UnknownSessionParser(categories);
            }
        }
    }
}