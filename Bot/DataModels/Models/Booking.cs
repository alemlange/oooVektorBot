﻿using System;

namespace DataModels
{
    public class Booking
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}
