using System.Collections.Generic;

namespace ManagerDesk.ViewModels
{
    public class DispatchListViewModel
    {
        public List<DispatchViewModel> ActiveDispatches { get; set; }

        public List<DispatchViewModel> InActiveDispatches { get; set; }
    }
}