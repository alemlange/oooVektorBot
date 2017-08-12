using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brains.Interfaces;
using LiteDbService;
using LiteDbService.Helpers;
using Brains.Responces;
using DataModels.Enums;

namespace Brains
{
    public class BotBrains : IMainTasks
    {
        public static readonly Lazy<BotBrains> Instance = new Lazy<BotBrains>(() => new BotBrains());

        private LiteCustomerService _service = ServiceCreator.GetCustomerService();

        public List<string> DishNames { get; set; }

        public BotBrains()
        {
            var allDishes = _service.GetAllDishes();

            DishNames = new List<string>();

            foreach(var dish in allDishes)
            {
                DishNames.Add(dish.Name.ToLower());
            }
        }

        public SessionState GetState(long chatId)
        {
            var table = _service.GetActiveTable(chatId);

            if (table != null)
                return table.State;
            else
                return SessionState.Unknown;
        }

        public Responce OrderMeal(long chatId, string dishName = "")
        {
            try
            {
                if (GetState(chatId) == SessionState.Sitted)
                {
                    var table = _service.FindTable(chatId);

                    if (string.IsNullOrWhiteSpace(dishName))
                    {
                        var lastDishName = table.StateVaribles.Where(t => t.Key == "LastDish").FirstOrDefault();
                        dishName = lastDishName.Value.ToString();
                    }

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
            if (GetState(chatId) == SessionState.Sitted)
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

        public Responce ShowMenuOnPage(long chatId) // todo
        {
            try
            {
                var menu = _service.GetAllMenus().First();

                var respText = menu.MenuName + Environment.NewLine;

                int page = 0;

                var table = _service.FindTable(chatId);

                var lastPage = table.StateVaribles.Where(t => t.Key == "LastPage").FirstOrDefault();

                if ((int)lastPage.Value > 0)
                {
                    page = (int)lastPage.Value;
                }

                var dishes = menu.DishList.Skip((page-1)*8); //8 items per page

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
                if (_service.GetTable(chatId) != Guid.Empty)
                {
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    return new Responce { ChatId = chatId, ResponceText = "Отлично! Напишите \"меню\" в чат и я принесу его вам." };
                }
                else
                {
                    throw new Exception("Не получилось создать столик.");
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
                if (_service.GetTable(chatId) != Guid.Empty)
                { 
                    return new Responce { ChatId = chatId, ResponceText = "Привет! За каким столиком вы сидите?" };
                }
                else
                    throw new Exception("Не получилось создать столик.");
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }  
        }

        public Responce CallWaiter(long chatId)
        {
            try
            {
                _service.SetHelpNeeded(chatId);

                return new Responce { ChatId = chatId, ResponceText = "Официант уже идет" };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce GetMenuItem(long chatId, string dishName)
        {
            try
            {
                var dish = _service.GetDish(dishName); // to do get dish from memory
                _service.AddLastDishToTable(chatId, dishName);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "https://www.instagram.com/p/BWE-azWgr4K/?taken-by=ferrari" + Environment.NewLine +
                    dish.Name + Environment.NewLine +
                    dish.Description
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }
    }
}