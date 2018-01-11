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
        Category,
        CloseMenu,
        TableNumber,
        Check,
        InlineKeyboard,
        CustomKeyboard,
        Picture,
        PictureLink,
        Slash,
        Waiter,
        MyOrder,
        Remark,
        Remove,
        RemoveByNum,
        QRCode,
        CreateInvoice,
        SuccessfulPayment,
        ArrivingTime,
        TimeInput,
        CloseTimeArriving
    }
}