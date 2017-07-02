using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brains.Interfaces;
using LiteDbService;
using LiteDbService.Helpers;
using Brains.Responces;

namespace Brains
{
    public class BotBrains : IMainTasks
    {
        public static readonly Lazy<BotBrains> Instance = new Lazy<BotBrains>(() => new BotBrains());

        private LiteCustomerService _service = ServiceCreator.GetCustomerService();

        private List<long> _newCustomerQueue = new List<long>();

        private List<long> _sittedCustomers = new List<long>();

        public List<string> DishNames { get; set; }

        public BotBrains()
        {
            var menu = _service.GetAllMenus().First();

            DishNames = new List<string>();

            foreach(var dish in menu.DishList)
            {
                DishNames.Add(dish.Name.ToLower());
            }
        }

        public Responce OrderMeal(long chatId, string dishName)
        {
            try
            {
                if (_sittedCustomers.Contains(chatId))
                {
                    var table = _service.FindTable(chatId);
                    var dish = _service.FindDish(dishName);
       
                    _service.OrderDish(table.Id,new DataModels.OrderedDish { DishFromMenu = dish});

                    return new Responce { ChatId = chatId, ResponceText = "Отличный выбор! Отношу заказ на кухню, чтонибудь еще?" };
                }
                else
                {
                    return Responce.UnknownResponce(chatId);
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce ShowCart(long chatId)
        {
            if (_sittedCustomers.Contains(chatId))
            {
                var table = _service.FindTable(chatId);
                var respText = "";
                if (table.Orders.Any())
                {
                    var tableSumm = table.Orders.Sum(o => o.DishFromMenu.Price);

                    respText = "Вы заказали:" + Environment.NewLine;
                    foreach (var dish in table.Orders)
                    {
                        respText += dish.DishFromMenu.Name + " " + dish.DishFromMenu.Price + Environment.NewLine;
                    }
                    respText += "Итого: " + tableSumm.ToString() + Environment.NewLine;
                }
                else
                {
                    respText = "Вы пока еще ничего не заказали :(";
                }
                
                return new Responce { ChatId = chatId, ResponceText = respText };
            }
            else
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce ShowMenu(long chatId)
        {
            try
            {
                var menu = _service.GetAllMenus().First();

                var respText = menu.MenuName + Environment.NewLine;
                foreach (var dish in menu.DishList)
                {
                    respText += dish.Name + " " + dish.Price + Environment.NewLine;
                }
                respText += "Хотите чтонибудь из меню? Просто напишите назавние блюда в чат." + Environment.NewLine;

                return new Responce { ResponceText = respText };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce Number(long chatId, int tableNumber)
        {
            try
            {
                if (_newCustomerQueue.Contains(chatId) && !_sittedCustomers.Contains(chatId))
                {
                    if (_service.CreateTable(chatId, tableNumber) != Guid.Empty)
                    {
                        _sittedCustomers.Add(chatId);
                        _newCustomerQueue.Remove(chatId);

                        return new Responce { ChatId = chatId, ResponceText = "Отлично! Напишите \"меню\" в чат и я принесу его вам." };
                    }
                    else
                        throw new Exception("Не получилось создать столик.");
                    
                    
                }
                else
                {
                    return Responce.UnknownResponce(chatId);
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }  
        }

        public Responce Greetings(long chatId)
        {
            try
            {
                if (!_newCustomerQueue.Contains(chatId) && !_sittedCustomers.Contains(chatId))
                {
                    _newCustomerQueue.Add(chatId);
                    return new Responce { ChatId = chatId, ResponceText = "Привет! За каким столиком вы сидите?" };
                }
                else
                {
                    return new Responce { ChatId = chatId, ResponceText = "Привет, вы уже в очереди!" };
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
               
        }
    }
}
