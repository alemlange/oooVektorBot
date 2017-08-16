﻿using DataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brains.Responces
{
    public class Responce
    {
        public long ChatId { get; set; }

        public string ResponceText { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public SessionState State { get; set; }

        public static Responce UnknownResponce(long chatId)
        {
            return new Responce { ChatId = chatId, ResponceText = "Извините, не понял вашей просьбы :(" };
        }
    }
}
