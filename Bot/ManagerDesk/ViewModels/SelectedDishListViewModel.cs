using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class SelectedDishListViewModel
    {
        public string Category { get; set; }

        public List<SelectedDishViewModel> Dishes {get;set;}
    }
}