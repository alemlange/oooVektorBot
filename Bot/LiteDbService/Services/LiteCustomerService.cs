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
                    _currentDb = @"C:\db\classic\" + _userDb + ".db";
                }
                return _currentDb;
            }
        }

        public LiteCustomerService(string userDbName)
        {
            _userDb = userDbName;
        }

        #region Dish

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
                    table.OrderProcessed = false;
                    table.Cheque = null;
                    tableCol.Update(table);
                }
            }
        }

        public void RemoveDishFromOrder(long chatId, int dishNum)
        {
            var table = GetActiveTable(chatId);
            if (table != null)
            {
                using (var db = new LiteDatabase(CurrentDb))
                {

                    table.Orders.RemoveAll(o => o.Num == dishNum);

                    int num = 1;
                    foreach (var order in table.Orders)
                    {
                        order.Num = num;
                        num += 1;
                    }

                    table.OrderProcessed = false;
                    table.Cheque = null;

                    UpdateTable(table);
                }
            }
            else
                throw new TableNotFoundException();
        }

        public void UpdateDishRemark(Guid tableId, string message) 
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.Id == tableId).FirstOrDefault();

                var lastOrder = table.Orders.OrderBy(o => o.DateOfOrdering).LastOrDefault();
                if (lastOrder != null)
                {
                    lastOrder.Remarks = message;

                    table.OrderPlaced = DateTime.Now;
                    table.OrderProcessed = false;
                    tableCol.Update(table);
                }
            }
        }
        #endregion

        #region Table
        public Table GetActiveTable(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();
            }
        }

        public IEnumerable<Table> AllOrders(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                return col.Find(o => o.ChatId == chatId && (o.State == SessionState.Closed || o.State == SessionState.OrderPosted));
            }
        }

        public void AssignNextQueueNumber(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                if (table != null)
                {
                    var restCol = db.GetCollection<Restaurant>("Restaurants");
                    var restaurant = restCol.Find(o => o.Id == table.Restaurant).FirstOrDefault();
                    
                    if(restaurant != null)
                    {
                        var nextNumber = restaurant.QueueNumber + 1;
                        if (nextNumber >= 1000)
                            nextNumber = 0;

                        table.TableNumber = "T"+ nextNumber.ToString("000");

                        restaurant.QueueNumber += 1;
                        tableCol.Update(table);
                        restCol.Update(restaurant);
                    }
                }
            }
        }

        public void AssignQueueNumber(long chatId, string tableNumber)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                if (table != null)
                {
                    table.TableNumber = tableNumber;
                    tableCol.Update(table);
                }
            }
        }

        public void SetHelpNeeded(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                table.HelpNeeded = true;
                col.Update(table);
            }
        }

        public void SetArrivingTime(long chatId, int time)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                if (table != null)
                {
                    table.TimeArriving = time;
                    tableCol.Update(table);
                }
            }
        }

        public void SendOrderToDesk(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                if (table != null)
                {
                    table.OrderPlaced = DateTime.Now;
                    table.State = SessionState.OrderPosted;
                    tableCol.Update(table);
                }
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

        public void SetCashPayment(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                table.CashPayment = true;
                col.Update(table);
            }
        }

        public void AssignCheque(Guid tableId, Cheque cheque)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.Id == tableId).FirstOrDefault();

                if(table != null)
                {
                    table.Cheque = cheque;
                    col.Update(table);
                }
                else
                {
                    throw new Exception("Стол не найден");
                }
            }
        }

        public void TableChequeMarkPayed(Guid tableId, string paymentId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Table>("Tables");
                var table = col.Find(o => o.Id == tableId).FirstOrDefault();

                if (table != null && table.Cheque != null)
                {
                    table.Cheque.TelegramPaymentId = paymentId;
                    table.Cheque.PaymentRecieved = true;
                    col.Update(table);
                }
                else
                {
                    throw new Exception("Стол либо чек не найдены.");
                }
            }
        }

        #endregion

        #region Menu
        public void AssignRestaurant(long chatId, string restruntName)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var restCol = db.GetCollection<Restaurant>("Restaurants");
                var restaurant = restCol.Find(o => o.Name == restruntName).FirstOrDefault();

                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                if (table != null)
                {
                    table.TimeZoneId = restaurant.TimeZoneId;
                    table.Restaurant = restaurant.Id;
                    tableCol.Update(table);
                }
            }
        }

        public void AssignRestaurantByCode(long chatId, string restruntCode)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var restCol = db.GetCollection<Restaurant>("Restaurants");
                var restaurant = restCol.Find(o => o.Code == restruntCode).FirstOrDefault();

                var tableCol = db.GetCollection<Table>("Tables");
                var table = tableCol.Find(o => o.ChatId == chatId && o.State != SessionState.Closed && o.State != SessionState.OrderPosted).FirstOrDefault();

                if (table != null)
                {
                    table.TimeZoneId = restaurant.TimeZoneId;
                    table.Restaurant = restaurant.Id;
                    tableCol.Update(table);
                }
            }
        }

        public Menu GetMenuByTable(long chatId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var menuCol = db.GetCollection<Menu>("Menus");
                var table = GetActiveTable(chatId);

                if (table == null)
                {
                    return null;
                }
                else
                {
                    var restaurantCol = db.GetCollection<Restaurant>("Restaurants");
                    var restaurant = restaurantCol.Find(r => r.Id == table.Restaurant).FirstOrDefault();

                    if(restaurant != null)
                    {
                        var menuId = restaurant.Menu;
                        return menuCol.Find(m => m.Id == menuId).FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }

            }
        }

        public Menu GetStandartMenu()
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var menuCol = db.GetCollection<Menu>("Menus");

                return menuCol.Find(m => m.DefaultMenu).FirstOrDefault() ?? menuCol.FindAll().FirstOrDefault();
            }
        }
        #endregion

        public Guid CreateFeedback(Feedback feed)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Feedback>("Feedbacks");

                col.Insert(feed);
                return feed.Id;
            }
        }

        public Guid CreateBooking(Booking booking)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Booking>("Bookings");

                col.Insert(booking);
                return booking.Id;
            }
        }

        public Guid CreateCheque(Cheque cheque)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Cheque>("Cheques");

                col.Insert(cheque);
                return cheque.Id;
            }
        }

        public void ChequeMarkPayed(Guid chequeId, string paymentId)
        {
            using (var db = new LiteDatabase(CurrentDb))
            {
                var col = db.GetCollection<Cheque>("Cheques");
                var cheque = col.Find(o => o.Id == chequeId).FirstOrDefault();

                if (cheque != null)
                {
                    cheque.TelegramPaymentId = paymentId;
                    cheque.PaymentRecieved = true;
                    col.Update(cheque);
                }
                else
                {
                    throw new Exception("Чек не найден.");
                }
            }
        }
    }
}