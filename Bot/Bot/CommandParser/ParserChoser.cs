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
            switch (state)
            {
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
                    return new UnknownSessionParser();
                default:
                    return new UnknownSessionParser();
            }
        }
    }
}