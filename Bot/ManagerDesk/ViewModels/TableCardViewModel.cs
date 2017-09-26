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
                if (State == SessionState.Closed || State == SessionState.Deactivated)
                    return "table-inactive";
                else
                    return (OrderProcessed) ? "table-processed" : "";
            }
        }

        public bool TableActive
        {
            get
            {
                return (State != SessionState.Closed && State != SessionState.Deactivated);
            }
        }
    }
}