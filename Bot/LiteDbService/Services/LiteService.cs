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
                var col = db.GetCollection<Table>("Tables");

                var table = new Table
                {
                    Id = Guid.NewGuid(),
                    ChatId = chatId,
                    State = SessionState.Restaurant,
                    CreatedOn = DateTime.Now,
                    Orders = new List<OrderedDish>(),
                    StateVaribles = new List<StateVarible>()
                };

                col.Insert(table);
                col.EnsureIndex(o => o.Id);
                return table.Id;

            }
        }

        public Dish GetDish(string dishSlashName)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dish>("Dishes");

                return col.Find(o => o.SlashName == dishSlashName).FirstOrDefault();
            }
        }

        public Dish GetDish(Guid dishId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dish>("Dishes");

                return col.Find(o => o.Id == dishId).FirstOrDefault();
            }
        }

        public void AddLastDishToTable(long chatId, string dishName)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var dishCol = db.GetCollection<Dish>("Dishes");
                var dish = dishCol.Find(o => o.SlashName == dishName).FirstOrDefault();

                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId).FirstOrDefault();

                if (table != null)
                {
                    var stateVarible = new StateVarible();
                    stateVarible.Key = "LastDish";
                    stateVarible.Value = dish.SlashName;

                    if (table.StateVaribles != null)
                    {
                        table.StateVaribles.RemoveAll(s => s.Key == "LastDish");
                    }
                    table.StateVaribles.Add(stateVarible);

                    tableCol.Update(table);
                }
            }
        }

        public void UpdateTableState(long chatId, SessionState state)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.ChatId == chatId).FirstOrDefault();

                if (table != null)
                {
                    table.State = state;
                    col.Update(table);
                }
            }
        }

        public void RemoveAllTables()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                db.DropCollection("Tables");
                //db.DropCollection("Restaurants");
            }
        }

        public void UpdateLastPage(long chatId, int lastPage)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId).FirstOrDefault();

                if (table != null)
                {
                    var stateVarible = new StateVarible();
                    stateVarible.Key = "LastPage";
                    stateVarible.Value = lastPage;

                    table.StateVaribles.RemoveAll(s => s.Key == "LastPage");
                    table.StateVaribles.Add(stateVarible);

                    tableCol.Update(table);
                }
            }
        }

        public void SetHelpNeeded(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.ChatId == chatId).FirstOrDefault();

                table.HelpNeeded = true;
                col.Update(table);
            }
        }

        public void SetCheckNeeded(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.ChatId == chatId).FirstOrDefault();

                table.CheckNeeded = true;
                col.Update(table);
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

        public List<Dish> GetAllDishes()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dish>("Dishes");
                return col.FindAll().ToList();
            }
        }

        public Restaurant GetRestaurant(Guid restId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Restaurant>("Restaurants");
                return col.Find(o => o.Id == restId).FirstOrDefault();
            }
        }

        public List<Restaurant> GetAllRestaurants()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Restaurant>("Restaurants");
                return col.FindAll().ToList();
            }
        }

        public List<Restaurant> GetAllActiveRestaurants()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var menuCol = db.GetCollection<Menu>("Menus");
                var menus = menuCol.Find(m => m.Restaurant != Guid.Empty).ToList();

                var restaurantCol = db.GetCollection<Restaurant>("Restaurants");
                var restaurants = restaurantCol.FindAll().ToList();
                return restaurants.Where(r => menus.Select(m => m.Restaurant).Contains(r.Id)).ToList();
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