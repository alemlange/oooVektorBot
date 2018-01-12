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