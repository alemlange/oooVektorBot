using System;

namespace ManagerDesk.ViewModels
{
    public class BookingViewModel
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
    }
}