using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Restaurant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public int QueueNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; } 
        public string TimeZoneId { get; set; }
        public Guid Menu { get; set; }
    }
}