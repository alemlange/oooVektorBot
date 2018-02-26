using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Config
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string OrgName { get; set; }

        public string ProfilePicturePath { get; set; }

        public string TelegramBotLocation { get; set; }

        public string BotName { get; set; }

        public string BotGreeting { get; set; }

        public int DishesPerPage { get; set; }

        public bool UseQR { get; set; }

        public string BotKey { get; set; }

        public string BotToken { get; set; }

        public string PaymentToken { get; set; }

        public string Actions { get; set; }

        public string Description { get; set; }
    }
}