﻿using System;
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
            
            switch (state)
            {
                case SessionState.MenuCategory:
                    {
                        var categories = bot.GetMenuCategoriesByChatId(chatId);
                        return new MenuCategorySessionParser(categories);
                    }
                case SessionState.Restaurant:
                    {
                        return new RestruntSessionParser(bot.RestaurantNames);
                    }
                case SessionState.Sitted:
                    return new SittedSessionParser();
                case SessionState.Remark:
                    return new RemarkSessionParser();
                case SessionState.TimeChoosing:
                    return new TimeArrivingSessionParser();
                case SessionState.OrderPosted:
                    {
                        var categories = bot.GetMenuCategoriesByChatId(chatId);
                        return new OrderPostedSessionParser(categories);
                    }
                case SessionState.Unknown:
                    {
                        var categories = bot.GetMenuCategoriesByChatId(chatId);
                        return new UnknownSessionParser(categories);
                    }
                default:
                    {
                        var categories = bot.GetMenuCategoriesByChatId(chatId);
                        return new UnknownSessionParser(categories);
                    }
            }
        }
    }
}