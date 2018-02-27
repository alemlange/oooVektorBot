using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.ViewModels
{
    public class FeedbackViewModel
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
    }
}