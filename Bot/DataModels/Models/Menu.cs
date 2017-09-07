﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string MenuName { get; set; }
        public Guid Restaurant { get; set; }
        public List<Dish> DishList { get;  set; }
    }
}
