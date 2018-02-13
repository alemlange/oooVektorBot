using System.Collections.Generic;
using Brains.Models;

namespace Brains.Responces
{
    public class RemarkResponce: Responce
    {
        public bool IsOk { get; set; }
        public IEnumerable<Item> Modificators { get; set; }
    }
}
