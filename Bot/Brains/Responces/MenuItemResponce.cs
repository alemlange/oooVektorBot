using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace Brains.Responces
{
    public class MenuItemResponce: Responce
    {
        public string DishId { get; set; }
        public bool NeedInlineKeyboard { get; set; }
    }
}
