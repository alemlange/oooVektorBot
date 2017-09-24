using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDbService.Interfaces;
using DataModels;
using DataModels.Enums;
using LiteDB;
using DataModels.Exceptions;

namespace LiteDbService
{
    public class LiteCustomerService : LiteService, ICustomerDb
    {
        private string _userDb { get; set; }

        public override string CurrentDb
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDb))
                {
                    _currentDb = @"C:\db\" + _userDb + ".db";
                }
                return _currentDb;
            }
        }

        public LiteCustomerService(string userDbName)
        {
            _userDb = userDbName;
        }

        public void OrderDish(Guid tableId, OrderedDish dish)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.Id == tableId).FirstOrDefault();
                if (table == null)
                    throw new TableNotFoundException();
                else
                {
                    table.Orders.Add(dish);
                    table.OrderPlaced = DateTime.Now;
                    tableCol.Update(table);
                }
            }
        }

        public void UpdateDishRemark(Guid tableId, string lastDishName, string message) // toddo одинаковые блюда в заказе
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.Id == tableId).FirstOrDefault();
                var orders = table.Orders;
                var dish = orders.LastOrDefault(o => o.DishFromMenu.SlashName == lastDishName); // todo last

                if (dish != null)
                    dish.Remarks = message;

                table.Orders = orders;
                //table.OrderPlaced = DateTime.Now;
                tableCol.Update(table);
            }
        }

        public void AssignMenu(long chatId, string restruntName)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var restCol = db.GetCollection<Restaurant>("Restaurants");
                var restaurant = restCol.Find(o => o.Name == restruntName).FirstOrDefault();

                var menuCol = db.GetCollection<Menu>("Menus");
                var menu = menuCol.Find(o => o.Restaurant == restaurant.Id).FirstOrDefault();

                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId).FirstOrDefault();

                if (table != null)
                {
                    table.Menu = menu.Id; // todo check isnull
                    tableCol.Update(table);
                }
            }
        }

        public void AssignNumber(long chatId, int tableNumber)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId).FirstOrDefault();

                var restCol = db.GetCollection<Restaurant>("Restaurants");
                var restaurant = restCol.FindAll().FirstOrDefault();

                if (table != null)
                {
                    table.TableNumber = tableNumber;
                    table.State = SessionState.Sitted;
                    tableCol.Update(table);

                    if (table.Menu == Guid.Empty)
                    {
                        AssignMenu(chatId, restaurant.Name);
                    }
                }
            }
        }

        public Table FindTable(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.ChatId == chatId).FirstOrDefault();
            }
        }

        public Dish FindDish(string dishSlashName)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Menu>("Menus");
                var dishList = col.FindAll().First().DishList.Where(o => o.SlashName == dishSlashName);

                if (dishList.Any())
                {
                    return dishList.First();
                }
                else
                    throw new Exception("Блюдо не найдено");
            }
        }

        public void RemoveDishFromOrder(long chatId, int dishNum)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId).FirstOrDefault();

                if (table == null)
                    throw new TableNotFoundException();
                else
                {
                    table.Orders.RemoveAll(o => o.Num == dishNum);
                    tableCol.Update(table);
                }
            }
        }
    }
}