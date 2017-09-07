using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class MenuViewModel: Menu
    {
        public string AttachedRestaurantName { get; set; }
        public List<RestaurantDropDown> AvailableRests { get; set; }
    }
}