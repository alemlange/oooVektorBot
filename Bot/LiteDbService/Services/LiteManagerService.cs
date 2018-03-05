using System;
using System.Collections.Generic;
using System.Linq;
using DataModels;
using DataModels.Enums;
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
                    _currentDb = @"C:\db\classic\" + _userDb +".db";
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

                RefreshMenuCategories(col, menu.Id);
                return curMenu;
            }
        }

        public void RefreshMenuCategories(LiteCollection<Menu> col, Guid menuId)
        {
            var menu = col.Find(o => o.Id == menuId).FirstOrDefault();
            if (menu != null)
            {
                if (menu.DishList != null && menu.DishList.Any())
                {
                    var dishCats = menu.DishList.Select(o => o.Category).Distinct();
                    var newCats = new List<string>();

                    if (menu.CategoriesSorted != null)
                    {
                        foreach (var cat in menu.CategoriesSorted)
                        {
                            if (dishCats.Contains(cat))
                            {
                                newCats.Add(cat);
                            }
                        }
                    }


                    foreach (var cat in dishCats)
                    {
                        if (!newCats.Contains(cat))
                        {
                            newCats.Add(cat);
                        }
                    }
                    menu.CategoriesSorted = newCats;
                }
                else
                    menu.CategoriesSorted = new List<string>();
            }

            col.Update(menu);
            
        }

        public List<Table> GetActiveTables()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.State != SessionState.Closed && o.State != SessionState.OrderPosted).ToList();
            }
        }

        public List<Table> GetInActiveTables()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.State == SessionState.Closed).ToList();
            }
        }

        public List<Table> GetOrderPostedTables()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");

                return tableCol.Find(o => o.State == SessionState.OrderPosted).ToList();
            }
        }

        public List<Table> GetManagerTables()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");

                return tableCol.Find(o => o.State == SessionState.MenuCategory || o.State == SessionState.OrderPosted || o.State == SessionState.Sitted).ToList();
            }
        }

        public List<Table> GetActiveTables(Guid restId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");

                return tableCol.Find(o => o.State != SessionState.Closed && o.State != SessionState.OrderPosted && o.Restaurant == restId).ToList();
            }
        }

        public List<Table> GetOrderPostedTables(Guid restId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");

                return tableCol.Find(o => o.State == SessionState.OrderPosted && o.Restaurant == restId).ToList();
            }
        }

        public List<Table> GetManagerTables(Guid restId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");

                return tableCol.Find(o => (o.State == SessionState.MenuCategory || o.State == SessionState.OrderPosted || o.State == SessionState.Sitted) && o.Restaurant == restId).ToList();
            }
        }

        public List<Table> GetInActiveTables(Guid restId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");

                return tableCol.Find(o => o.State == SessionState.Closed  && o.Restaurant == restId).ToList();
            }
        }

        public void CloseTable(Guid tableId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");

                var table = col.Find(o => o.Id == tableId).FirstOrDefault();
                if(table != null)
                {
                    table.State = SessionState.Closed;
                    col.Update(table);
                }
                else
                {
                    throw new Exception("Table not found!");
                }
            }
        }

        public void CloseBooking(Guid bookId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Booking>("Bookings");

                var booking = col.Find(o => o.Id == bookId).FirstOrDefault();
                if (booking != null)
                {
                    booking.State = BookingState.Closed;
                    col.Update(booking);
                }
                else
                {
                    throw new Exception("Booking not found!");
                }
            }
        }

        public Guid CreateNewMenu(Menu menu)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                if (menu.Id == Guid.Empty)
                    menu.Id = Guid.NewGuid();

                if (menu.CategoriesSorted == null)
                    menu.CategoriesSorted = new List<string>();

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

                RefreshMenuCategories(col, menu.Id);

                return menu.Id;
            }
        }

        public void SetDefaultMenu(Guid menuId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                var munus = col.FindAll();

                foreach (var menu in munus)
                {
                    if (menu.Id == menuId)
                    {
                        menu.DefaultMenu = true;
                    }
                    else
                    {
                        menu.DefaultMenu = false;
                    }
                }

                col.Update(munus);
            }
        }

        public Guid CreateNewDish(Dish dish)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Dish>("Dishes");
                if (dish.Id == Guid.Empty)
                    dish.Id = Guid.NewGuid();
                if (dish.ModIds == null)
                    dish.ModIds = new List<int>();
                col.Insert(dish);

                return dish.Id;
            }
        }

        public int CreateNewModificator(Modificator mod)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Modificator>("Modificators");

                col.Insert(mod);

                return mod.Id;
            }
        }

        public Guid CreateRestaurant(Restaurant rest)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Restaurant>("Restaurants");

                if (rest.Id == Guid.Empty)
                    rest.Id = Guid.NewGuid();

                var restaurants = col.FindAll();
                
                if (restaurants.Count() == 0)
                {
                    rest.Code = (1).ToString();
                }
                else
                {
                    rest.Code = (int.Parse(restaurants.Max(c => c.Code)) + 1).ToString();
                }

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

                RefreshMenuCategories(col, menu.Id);
            }
        }

        public void UpdateMenuCategories(Guid menuId, List<string> categories)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                var curMenu = col.Find(o => o.Id == menuId).FirstOrDefault();
                curMenu.CategoriesSorted = categories;
                col.Update(curMenu);
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

                    RefreshMenuCategories(colMenu, menu.Id);
                }
            }
        }

        public void UpdateModificator(Modificator mod)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colDish = db.GetCollection<Modificator>("Modificators");
                colDish.Update(mod);
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

                    RefreshMenuCategories(colMenu, menu.Id);
                }
            }
        }

        public void DeleteMenu(Guid menuId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colMenus = db.GetCollection<Menu>("Menus");
                colMenus.Delete(menuId);

            }
        }

        public void DeleteModificator(int modId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colMenus = db.GetCollection<Modificator>("Modificators");
                colMenus.Delete(modId);

            }
        }

        public void DeleteRestaraunt(Guid restId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colRests = db.GetCollection<Restaurant>("Restaurants");
                colRests.Delete(restId);

                var colTables = db.GetCollection<Table>("Tables");
                var tables = colTables.Find(o => o.Restaurant == restId);

                if (tables.Any())
                {
                    foreach(var table in tables)
                    {
                        table.State = SessionState.Closed;
                        colTables.Update(table);
                    }
                }
            }
        }

        public void DeleteTable(Guid tableId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var colTables = db.GetCollection<Table>("Tables");
                colTables.Delete(tableId);
            }
        }

        public List<Feedback> GetAllFeedbacks()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Feedback>("Feedbacks");
                return col.FindAll().ToList();
            }
        }

        public List<Booking> GetAllBookings()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Booking>("Bookings");
                return col.FindAll().ToList();
            }
        }

        public List<Cheque> GetAllCheques()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Cheque>("Cheques");
                return col.FindAll().ToList();
            }
        }
    }
}
