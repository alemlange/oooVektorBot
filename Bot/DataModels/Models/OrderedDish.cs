using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class OrderedDish
    {
        public int Num { get; set; }

        public DateTime DateOfOrdering { get; set; }

        public Dish DishFromMenu { get; set; }

        public string Remarks { get; set; }
    }
}