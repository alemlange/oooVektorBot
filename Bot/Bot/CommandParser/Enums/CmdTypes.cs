using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot.CommandParser
{
    public enum CmdTypes
    {
        Start,
        Unknown,
        Greetings,
        Restrunt,
        Menu,
        MenuCategory,
        CloseMenu,
        TableNumber,
        Check,
        InlineKeyboard,
        CustomKeyboard,
        Picture,
        PictureLink,
        Dishes,
        Slash,
        Waiter,
        MyOrder,
        Remark,
        Remove,
        RemoveByNum,
        QRCode
    }
}