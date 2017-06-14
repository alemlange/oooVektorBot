using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brains.Interfaces
{
    interface IMainTasks
    {
        string ShowMenu();
        string ShowCart();
        string OrderMeal(string customerId, string meal);
    }
}
