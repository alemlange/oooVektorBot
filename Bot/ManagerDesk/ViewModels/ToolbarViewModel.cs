using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels; 

namespace ManagerDesk.ViewModels
{
    public class RestOptionsViewModel
    {
        public List<Restaurant> AvailableRests { get; set; }

        public Restaurant CurrentRest { get; set; }

        public string SelectedOptionStyle(Guid restId)
        {
            return CurrentRest != null && restId == CurrentRest.Id ? "selected": "";
        }
    }
}