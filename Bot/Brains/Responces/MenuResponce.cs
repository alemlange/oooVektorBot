using System.Collections.Generic;
using Brains.Models;

namespace Brains.Responces
{
    public class MenuResponce: Responce
    {
        public IEnumerable<Item> Dishes { get; set; }
    }
}
