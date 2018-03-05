using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataModels;
using System.ComponentModel.DataAnnotations;

namespace ManagerDesk.ViewModels
{
    public class ChequeViewModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string TelegramPaymentId { get; set; }

        public long ChatId { get; set; }

        public DateTime Date { get; set; }

        public decimal Summ { get; set; }

        public string Currency { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool PaymentRecieved { get; set; }

        public string CurrencySymbol
        {
            get
            {
                switch (Currency)
                {
                    case "RUB":
                        return "₽";
                    default:
                        return "";
                }
            }
            
        }
    }
}