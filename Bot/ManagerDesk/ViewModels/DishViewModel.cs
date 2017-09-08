﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class DishViewModel: Dish
    {
        public List<string> AvailableCategories { get; set; }

        public string PriceString
        {
            get
            {
                return Price == 0 ? "" : Price.ToString();
            }
        }
    }
}