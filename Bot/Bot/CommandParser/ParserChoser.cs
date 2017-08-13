using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels.Enums;

namespace Bot.CommandParser
{
    public static class ParserChoser
    {
        public static IParser GetParser(SessionState state)
        {
            switch (state)
            {
                case SessionState.Queue:
                    return new InQueueSessionParser();
                case SessionState.Sitted:
                    return new SittedSessionParser();
                //case SessionState.DishChoosing:
                //    return new DishChoosingSessionParser();
                case SessionState.Unknown:
                    return new UnknownSessionParser();
                default:
                    return new UnknownSessionParser();
            }
        }
    }
}