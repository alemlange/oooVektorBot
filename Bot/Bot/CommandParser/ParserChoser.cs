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
                case SessionState.Sitted:
                    return new SittedSessionParser();
                default:
                    return new UnknownSessionParser();
            }
        }
    }
}