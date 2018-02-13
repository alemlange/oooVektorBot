using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using DataModels.Enums;

namespace ManagerDesk.ViewModels
{
    public class TableCardViewModel: Table
    {
        public decimal DishPrice(OrderedDish dish)
        {
            decimal summ = dish.DishFromMenu.Price;

            foreach (var mod in dish.OrderedMods)
            {
                summ += mod.Mod.Price * mod.Count;
            }

            return summ;
        }

        public string DishMods(OrderedDish dish)
        {
            var respText = "";
            var allOrderedMods = dish.OrderedMods.Where(o => o.Count > 0);
            if (allOrderedMods.Any())
            {
                foreach (var mod in allOrderedMods)
                {
                    respText += " " + mod.Mod.Name + " " + mod.Mod.Price + " р." + " x" + mod.Count + Environment.NewLine;
                }
            }
            return respText;
        }
        

        public string TableStyles
        {
            get
            {
                if (State == SessionState.Closed)
                    return "table-inactive";
                else
                    return (OrderProcessed) ? "table-processed" : "";
            }
        }

        public bool TableActive
        {
            get
            {
                return (State != SessionState.Closed);
            }
        }

        public string OrderProcessedStyle
        {
            get
            {
                return OrderProcessed ? "" : "un-checked";
            }
        }

        public string CheckNeededStyle
        {
            get
            {
                return CheckNeeded ? "" : "un-checked";
            }
        }

        public string HelpNeededStyle
        {
            get
            {
                return HelpNeeded ? "" : "un-checked";
            }
        }
    }
}