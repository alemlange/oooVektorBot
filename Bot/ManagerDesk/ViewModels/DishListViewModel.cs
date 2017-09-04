using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class DishListViewModel
    {
        public string Category { get; set; }

        public List<DishViewModel> Dishes {get;set;}
    }
}