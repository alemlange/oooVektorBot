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

        public string BotName { get; set; }

        public int DishesPerPage { get; set; }

        public int UseQR { get; set; }
    }
}