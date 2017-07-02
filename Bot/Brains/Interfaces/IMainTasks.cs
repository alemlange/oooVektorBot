using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brains.Responces;

namespace Brains.Interfaces
{
    interface IMainTasks
    {
        Responce ShowMenu(long chatId);
        Responce ShowCart(long chatId);
        Responce OrderMeal(long chatId, string dishName);
    }
}
