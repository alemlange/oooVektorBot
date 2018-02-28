using System;
using DataModels.Enums;

namespace ManagerDesk.ViewModels
{
    public class BookingViewModel
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public BookingState State { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }

        public bool Active
        {
            get
            {
                return (State != BookingState.Closed);
            }
        }
    }
}