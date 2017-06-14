using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brains.Interfaces;
using Brains.DbService;

namespace Brains
{
    public class BotBrains : IMainTasks
    {
        public string OrderMeal(string customerId, string meal)
        {
            throw new NotImplementedException();
        }

        public string ShowCart()
        {
            throw new NotImplementedException();
        }

        public string ShowMenu()
        {
            return new DbServiceSql().GetMenu().ToString();
        }
    }
}
