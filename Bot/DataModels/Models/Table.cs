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
        public Guid Menu { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime OrderPlaced { get; set; }
        public List<OrderedDish> Orders { get;  set; }
        public SessionState State { get; set; }
        public List<StateVarible> StateVaribles { get; set; } 
        public bool HelpNeeded { get; set; }
        public bool CheckNeeded { get; set; }
        public bool OrderProcessed { get; set; }

    }
}