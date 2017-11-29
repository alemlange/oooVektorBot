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

        public Guid CurrentRest { get; set; }

        public string SelectedOptionStyle(Guid restId)
        {
            return restId == CurrentRest ? "selected": "";
        }
    }
}