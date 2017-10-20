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
        public static IParser GetParser(SessionState state, BotBrains bot)
        {
            switch (state)
            {
                case SessionState.Restaurant:
                    return new RestruntSessionParser(bot.DishNames, bot.Config.TablesCount);
                case SessionState.Queue:
                    return new InQueueSessionParser(bot.DishNames, bot.Config.TablesCount);
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