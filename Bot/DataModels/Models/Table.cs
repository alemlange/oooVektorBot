using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataModels.Enums;
using System.Threading.Tasks;

namespace DataModels
{
    public class Table
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public int TableNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime OrderPlaced { get; set; }
        public Dish DishBuffer { get; set; }
        public List<OrderedDish> Orders { get;  set; }
        public SessionState State { get; set; }
        public bool HelpNeeded { get; set; }
    }
}
