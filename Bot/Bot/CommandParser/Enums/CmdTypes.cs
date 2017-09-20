using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot.CommandParser
{
    public enum CmdTypes
    {
        Unknown,
        Greetings,
        Restrunt,
        Menu,
        TableNumber,
        Check,
        InlineKeyboard,
        CustomKeyboard,
        Picture,
        PictureLink,
        MenuCategories,
        Slash,
        Waiter,
        MyOrder,
        Remark
    }
}