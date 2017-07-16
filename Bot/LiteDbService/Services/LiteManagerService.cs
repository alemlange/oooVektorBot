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
                col.EnsureIndex(o => o.Id);

                return dish.Id;
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

        public List<Table> GetAllTables()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.FindAll().ToList();
            }
        }
    }
}
