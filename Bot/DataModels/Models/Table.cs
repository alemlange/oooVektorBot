﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Table
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime OrderPlaced { get; set; }
        public List<OrderedDish> Orders { get;  set; }
    }
}
