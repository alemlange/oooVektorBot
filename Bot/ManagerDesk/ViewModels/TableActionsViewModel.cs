using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class TableActionsViewModel
    {
        public bool HelpNeeded { get; set; }
        public bool CheckNeeded { get; set; }
        public bool OrderProcessed { get; set; }

        public string OrderProcessedIcon
        {
            get
            {
                return OrderProcessed ? "style=display:inline;" : "style=display:none;";
            }
        }

        public string OrderProcessedStyle
        {
            get
            {
                return OrderProcessed ? "menu-selected" : "";
            }
        }

        public string CheckNeededIcon
        {
            get
            {
                return CheckNeeded ? "style=display:inline;" : "style=display:none;";
            }
        }

        public string CheckNeededStyle
        {
            get
            {
                return CheckNeeded ? "menu-selected" : "";
            }
        }

        public string HelpNeededIcon
        {
            get
            {
                return HelpNeeded ? "style=display:inline;" : "style=display:none;";
            }
        }

        public string HelpNeededStyle
        {
            get
            {
                return HelpNeeded ? "menu-selected" : "";
            }
        }
    }
}