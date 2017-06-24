using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace LiteDbService.Interfaces
{
    interface ICustomerDb:IDb
    {
        void OrderDish(Guid tableId, OrderedDish dish);
    }
}
