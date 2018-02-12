using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Dish
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string PictureUrl { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public decimal Price { get;  set; }

        public List<int> ModIds { get; set; }
    }
}
