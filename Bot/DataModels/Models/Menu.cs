using System;
using System.Collections.Generic;

namespace DataModels
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string MenuName { get; set; }
        //public Guid Restaurant { get; set; }
        public List<string> CategoriesSorted { get; set; }
        public List<Dish> DishList { get;  set; }
        public bool DefaultMenu { get; set; }
    }
}
