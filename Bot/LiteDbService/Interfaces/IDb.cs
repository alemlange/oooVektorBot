using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace LiteDbService
{
    interface IDb
    {
        Menu GetMenu(Guid menuId);
        List<Menu> GetAllMenus();
        Table GetTable(Guid tableId);
    }
}
