using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace LiteDbService.Interfaces
{
    interface IManagerDb
    {
        Menu AddDishToMenu(Menu menu, Dish dishToAdd);
        Guid CreateNewMenu(Menu menu);
        List<Table> GetAllTables();
    }
}
