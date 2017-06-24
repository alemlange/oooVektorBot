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
        protected internal string _currentDb { get; set; }
        public virtual string CurrentDb
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDb))
                {
                    _currentDb = @"C:\db\MyData.db";
                }
                return _currentDb;
            }
        }
        public Menu GetMenu(Guid menuId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                // Получаем коллекцию
                var col = db.GetCollection<Menu>("Menus");
                return col.Find(o => o.Id == menuId).FirstOrDefault();

            }
        }
        public List<Menu> GetAllMenus()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                // Получаем коллекцию
                var col = db.GetCollection<Menu>("Menus");
                return col.FindAll().ToList();
            }
        }
        public Guid CreateTable()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var table = new Table { Id = Guid.NewGuid(), CreatedOn = DateTime.Now, Orders = new List<OrderedDish>() };

                var col = db.GetCollection<Table>("Tables");
                col.Insert(table);
                col.EnsureIndex(o => o.Id);
                return table.Id;
            }
        }
        public Table GetTable(Guid tableId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.Id == tableId).FirstOrDefault();

            }
        }
        
    }
}
