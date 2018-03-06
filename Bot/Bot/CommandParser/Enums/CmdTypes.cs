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
        DishDetails,
        Waiter,
        Cart,
        OrderComplete,
        Remark,
        Remove,
        RemoveByNum,
        QRCode,
        CreateInvoice,
        SuccessfulPayment,
        ArrivingTime,
        TimeInput,
        CloseTimeArriving,
        PayCash,
        AddToOrder,
        BackToMenu,
        MyOrders,
        AddMod,
        Actions,
        Description,
        Location,
        RequestFeedback,
        LeaveFeedback,
        RequestBooking,
        LeaveBooking,
        CancelTable,
        InputSumm,
        RequestPayment
    }
}