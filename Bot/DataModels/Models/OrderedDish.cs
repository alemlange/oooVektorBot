using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class OrderedDish
    {
        public string Remarks { get; set; }

        public Dish DishFromMenu { get; set; }
    }
}
