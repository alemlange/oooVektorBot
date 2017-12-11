﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Cheque
    {
        public Guid Id { get; set; }

        public long ChatId { get; set; }

        public DateTime Date { get; set; }

        public List<OrderedDish> OrderedDishes { get; set; }

        public decimal Summ { get; set; }

        public decimal SummInCents
        {
            get
            {
                return Summ * 100;
            }
        }

        public string Currency { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}