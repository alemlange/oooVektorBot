using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.ViewModels
{
    public class MenuViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Нужно ввести название меню")]
        public string MenuName { get; set; }

        //public Guid Restaurant { get; set; }

        public List<string> CategoriesSorted { get; set; }

        public List<Dish> DishList { get; set; }

        public string AttachedRestaurantName { get; set; }

        public bool DefaultMenu { get; set; }

        public string DefaultMenuStyle
        {
            get
            {
                return DefaultMenu ? "checked='checked'" : "";
            }
        }

        public List<RestaurantDropDown> AvailableRests { get; set; }

        public List<DishListViewModel> GroupedDishes { get; set; }
    }
}