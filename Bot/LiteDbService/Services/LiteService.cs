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

                if (restaurant != null)
                {
                    var menuCol = db.GetCollection<Menu>("Menus");
                    return menuCol.Find(o => o.Id == restaurant.Menu).FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public Cheque GetCheque(Guid chequeId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Cheque>("Cheques");

                return col.Find(o => o.Id == chequeId).FirstOrDefault();
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

        public Modificator GetModificator(int modId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Modificator>("Modificators");

                return col.Find(o => o.Id == modId).FirstOrDefault();
            }
        }

        public List<Modificator> GetDishModificators(List<int> modIds)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Modificator>("Modificators");

                var dishMods = col.FindAll();
                return dishMods.Where(o => modIds.Contains(o.Id)).ToList();
            }
        }

        public void UpdateTableState(long chatId, SessionState state)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                if (table != null)
                {
                    table.State = state;
                    col.Update(table);
                }
            }
        }

        public void UpdateTableState(Guid tableId, SessionState state)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.Id == tableId ).FirstOrDefault();

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

        public void DeleteTable(Guid tableId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                col.Delete(o => o.Id == tableId);
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

        public List<Modificator> GetAllModificators()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Modificator>("Modificators");
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

        public List<string> GetMenuCategories()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var dishCol = db.GetCollection<Dish>("Dishes");
                var dishes = dishCol.FindAll().ToList();
                List<string> menuCategories = new List<string>();

                foreach (var dish in dishes)
                {
                    menuCategories.Add(dish.Category);
                }

                return menuCategories;
            }
        }

        public List<Restaurant> GetAllActiveRestaurants()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var restaurantCol = db.GetCollection<Restaurant>("Restaurants");
                return restaurantCol.Find(r => r.Menu != Guid.Empty).ToList();
            }
        }
    }
}