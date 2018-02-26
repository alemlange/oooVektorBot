using System.Collections.Generic;
using Brains.Models;

namespace Brains.Responces
{
    public class RestaurantResponce: Responce
    {
        public bool IsOk { get; set; }
        public IEnumerable<RestaurantInfo> RestaurantsInfo { get; set; }
    }
}