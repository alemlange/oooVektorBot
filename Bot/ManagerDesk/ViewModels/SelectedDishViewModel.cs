using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class SelectedDishViewModel: Dish
    {
        public bool Selected { get; set; }

        public string SelectedStyle
        {
            get
            {
                if (Selected)
                    return "menu-selected";
                else
                    return "";
            }
        }

        public string SelectedIcoStyle
        {
            get
            {
                if (Selected)
                    return "style=display:inline;";
                else
                    return "style=display:none;";
            }
        }
    }
}