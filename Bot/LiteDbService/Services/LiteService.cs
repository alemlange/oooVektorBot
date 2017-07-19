using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using DataModels;
using DataModels.Enums;

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

        public Guid CreateTable(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var table = new Table { Id = Guid.NewGuid(), ChatId = chatId, State = SessionState.Queue, CreatedOn = DateTime.Now, Orders = new List<OrderedDish>() };

                var col = db.GetCollection<Table>("Tables");
                col.Insert(table);
                col.EnsureIndex(o => o.Id);
                return table.Id;
            }
        }

        //public Guid CreateTable(long chatId, int tableNumber)
        //{
        //    using (var db = new LiteDatabase(CurrentDb))
        //    {
        //        var table = new Table { Id = Guid.NewGuid(), ChatId = chatId, TableNumber = tableNumber, CreatedOn = DateTime.Now, Orders = new List<OrderedDish>() };

        //        var col = db.GetCollection<Table>("Tables");
        //        col.Insert(table);
        //        col.EnsureIndex(o => o.Id);
        //        return table.Id;
        //    }
        //}

        public Table GetTable(Guid tableId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.Id == tableId).FirstOrDefault();

            }
        }

        public List<Dish> GetAllDishes()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dish>("Dishes");
                return col.FindAll().ToList();
            }
        }

        public Table GetActiveTable(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.Deactivated).FirstOrDefault();
            }
        }
    }
}
