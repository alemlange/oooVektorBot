using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using DataModels.Enums;

namespace ManagerDesk.ViewModels
{
    public class AllTablesViewModel
    {
        public List<TableCardViewModel> ActiveTables { get; set; }

        public List<TableCardViewModel> InActiveTables { get; set; }
    }
}