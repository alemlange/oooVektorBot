using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using LiteDbService.Interfaces;
using LiteDB;

namespace LiteDbService
{
    public class LiteManagerService : LiteService, IManagerDb
    {

        private string _userDb { get; set; }

        public override string CurrentDb
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDb))
                {
                    _currentDb = @"C:\db\" + _userDb +".db";
                }
                return _currentDb;
            }
        }

        public LiteManagerService(string userDbName)
        {
            _userDb = userDbName;
        }

        public Menu AddDishToMenu(Menu menu, Dish dishToAdd)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                var curMenu = col.Find(o => o.Id == menu.Id).FirstOrDefault();
                if (dishToAdd.Id == Guid.Empty)
                    dishToAdd.Id = Guid.NewGuid();
                curMenu.DishList.Add(dishToAdd);
                col.Update(curMenu);
                return curMenu;
            }
        }

        public void CloseTable(Guid tableId)
        {
            //using (var db = new LiteDatabase(CurrentDb))
            //{
            //    var col = db.GetCollection<Table>("Tables");

            //    col.Delete(o => o.Id == tableId);

            //    col.Insert(menu);
            //    col.EnsureIndex(o => o.Id);

            //    return menu.Id;
            //}
        }

        public Guid CreateNewMenu(Menu menu)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                if (menu.Id == Guid.Empty)
                    menu.Id = Guid.NewGuid();
                col.Insert(menu);
                col.EnsureIndex(o => o.Id);

                if (menu.DishList.Any())
                {
                    var dishesCollestion = db.GetCollection<Dish>("Dishes");
                    var allDishesIds = dishesCollestion.FindAll().Select(o => o.Id);
                    foreach(var dish in menu.DishList)
                    {
                        if (!allDishesIds.Contains(dish.Id))
                        {
                            dishesCollestion.Insert(dish);
                            dishesCollestion.EnsureIndex(o => o.Id);
                        }
                    }

                }

                return menu.Id;
            }
        }

        public Guid CreateNewDish(Dish dish)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dish>("Dishes");
                if (dish.Id == Guid.Empty)
                    dish.Id = Guid.NewGuid();
                col.Insert(dish);

                return dish.Id;
            }
        }

        public Guid CreateRestaurant(Restaurant rest)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Restaurant>("Restaurants");
                if (rest.Id == Guid.Empty)
                    rest.Id = Guid.NewGuid();
                col.Insert(rest);

                return rest.Id;
            }
        }

        public List<Table> GetAllTables()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.FindAll().ToList();
            }
        }

        public void UpdateMenu(Menu menu)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                col.Update(menu);
            }
        }

        public void UpdateMenuInfo(Menu menu)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                var curMenu = col.Find(o => o.Id == menu.Id).FirstOrDefault();
                if(curMenu != null)
                {
                    curMenu.MenuName = menu.MenuName;
                    col.Update(curMenu);
                }
            }
        }

        public void UpdateDish(Dish dish)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colDish = db.GetCollection<Dish>("Dishes");
                colDish.Update(dish);

                var colMenu = db.GetCollection<Menu>("Menus");
                var menus = colMenu.FindAll();
                menus = menus.Where(o => o.DishList.Select(d => d.Id).Contains(dish.Id));
                foreach (var menu in menus)
                {
                    menu.DishList.RemoveAll(o => o.Id == dish.Id);
                    menu.DishList.Add(dish);
                    colMenu.Update(menu);
                }
            }
        }

        public void UpdateRestaurant(Restaurant rest)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colDish = db.GetCollection<Restaurant>("Restaurants");
                colDish.Update(rest);
            }
        }

        public void DeleteDish(Guid dishId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colDish = db.GetCollection<Dish>("Dishes");
                colDish.Delete(dishId);

                var colMenu = db.GetCollection<Menu>("Menus");
                var menus = colMenu.FindAll();
                menus = menus.Where(o => o.DishList.Select(d => d.Id).Contains(dishId));
                foreach (var menu in menus)
                {
                    menu.DishList.RemoveAll(o => o.Id == dishId);
                    colMenu.Update(menu);
                }
            }
        }

        public void DeleteMenu(Guid menuId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colDish = db.GetCollection<Menu>("Menus");
                colDish.Delete(menuId);

            }
        }

        public void DeleteTable(Guid tableId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colDish = db.GetCollection<Table>("Tables");
                colDish.Delete(tableId);

            }
        }
    }
}
