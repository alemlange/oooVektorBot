using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using DataModels.Enums;

namespace ManagerDesk.ViewModels
{
    public class BookingListViewModel
    {
        public IEnumerable<BookingViewModel> ActiveBookings { get; set; }

        public IEnumerable<BookingViewModel> ClosedBookings { get; set; }
    }
}