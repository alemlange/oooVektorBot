using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using DataModels;

namespace LiteDbService
{
    public abstract class LiteService: IDb
    {
        public Menu GetMenu(Guid menuId)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Получаем коллекцию
                var col = db.GetCollection<Menu>("Menus");
                return col.Find(o => o.Id == menuId).FirstOrDefault();

            }
        }
        public List<Menu> GetAllMenus()
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Получаем коллекцию
                var col = db.GetCollection<Menu>("Menus");
                return col.FindAll().ToList();
            }
        }
    }
}
