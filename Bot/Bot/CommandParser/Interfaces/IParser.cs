using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace Bot.CommandParser
{
    public interface IParser
    {
        IReplyMarkup Keyboard { get; }

        CmdTypes ParseForCommand(Update update);
    }
}
