using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class TableCardViewModel: Table
    {
        public string TableStyles
        {
            get
            {
                return (OrderProcessed) ? "table-processed" : "";
            }
        }
    }
}