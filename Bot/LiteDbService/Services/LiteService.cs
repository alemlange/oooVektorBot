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
                var col = db.GetCollection<Menu>("Menus");
                return col.Find(o => o.Id == menuId).FirstOrDefault();

            }
        }

        public List<Menu> GetAllMenus()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                return col.FindAll().ToList();
            }
        }

        public Menu GetMenuByRestaurant(Guid restId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var restaurantCol = db.GetCollection<Restaurant>("Restaurants");
                var restaurant = restaurantCol.Find(r => r.Id == restId).FirstOrDefault();

                var menuCol = db.GetCollection<Menu>("Menus");
                return menuCol.Find(o => o.Id == restaurant.Id).FirstOrDefault();
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

        public void UpdateTableState(long chatId, SessionState state)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.Deactivated).FirstOrDefault();

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

        public void UpdateTable(Table table)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                col.Update(table);
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

        public Restaurant GetRestaurantByMenu(Guid menuId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Restaurant>("Restaurants");
                return col.Find(o => o.Menu == menuId).FirstOrDefault();
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
                /*
                var menuCol = db.GetCollection<Menu>("Menus");
                var menus = menuCol.Find(m => m.Restaurant != Guid.Empty).ToList();

                var restaurantCol = db.GetCollection<Restaurant>("Restaurants");
                var restaurants = restaurantCol.FindAll().ToList();
                return restaurants.Where(r => menus.Select(m => m.Restaurant).Contains(r.Id)).ToList();
                */
                var restaurantCol = db.GetCollection<Restaurant>("Restaurants");
                return restaurantCol.Find(r => r.Menu != Guid.Empty).ToList();
            }
        }
    }
}