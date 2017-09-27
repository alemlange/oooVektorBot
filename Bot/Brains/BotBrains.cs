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
using DataModels.Configuration;

namespace Brains
{
    public class BotBrains : IMainTasks
    {
        //public static readonly Lazy<BotBrains> Instance = new Lazy<BotBrains>(() => new BotBrains());

        private LiteCustomerService _service { get; set; }

        private LiteRegistrationService _regService = ServiceCreator.GetRegistrationService();

        public List<string> DishNames { get; set; }

        public List<string> RestaurantNames { get; set; }

        public int DishesPerPage { get; set; }

        public BotBrains()
        {
            var accountId = ConfigurationSettings.AccountId;

            if (accountId != Guid.Empty)
            {
                var account = _regService.FindAccount(accountId);
                _service = ServiceCreator.GetCustomerService(account.Login);

                var config = _regService.FindConfig(accountId);
                DishesPerPage = config.DishesPerPage;
            }
            else
                throw new Exception("AccountId setting not found webconfig.");

            var allDishes = _service.GetAllDishes();

            DishNames = new List<string>();

            foreach(var dish in allDishes)
            {
                DishNames.Add(dish.Name.ToLower());
            }

            var allRestaurants = _service.GetAllRestaurants();

            RestaurantNames = new List<string>();

            foreach (var restrunt in allRestaurants)
            {
                RestaurantNames.Add(restrunt.Name);
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
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    if (string.IsNullOrWhiteSpace(dishName))
                    {
                        var lastDishName = table.StateVaribles.Where(t => t.Key == "LastDish").FirstOrDefault();
                        dishName = lastDishName.Value.ToString();
                    }

                    var dish = _service.FindDish(dishName);
                    var dishNum = table.Orders.Count + 1;

                    _service.OrderDish(table.Id, new DataModels.OrderedDish { Num = dishNum, DishFromMenu = dish });
                    _service.UpdateTableState(chatId, SessionState.Remark);

                    //return new Responce { ChatId = chatId, ResponceText = "Отличный выбор! Отношу заказ на кухню, чтонибудь еще?" };
                    return new Responce { ChatId = chatId, ResponceText = "Отличный выбор! Если у Вас есть какие то пожелания к блюду, просто отправьте их сообщением!" };
                }
                else
                {
                    return new Responce { ChatId = chatId, ResponceText = "Вы не выбрали стол! Нажмите 'Начать' в главном меню, чтобы сделать заказ!" };
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce AddRemark(long chatId, string message)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    var lastDishName = table.StateVaribles.Where(t => t.Key == "LastDish").FirstOrDefault();

                    _service.UpdateDishRemark(table.Id, lastDishName.Value.ToString(), message);
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    return new Responce { ChatId = chatId, ResponceText = "Отличный выбор! Отношу заказ на кухню, чтонибудь еще?" };
                }
                else
                {
                    return new Responce { ChatId = chatId, ResponceText = "Вы не выбрали стол! Нажмите 'Начать' в главном меню, чтобы сделать заказ!" };
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce RemoveFromOrder(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table.Orders.Any())
                {
                    var resp = ShowCart(chatId);
                    resp.ResponceText += Environment.NewLine + "Отправьте ообщением номер блюда, которое вы хотите убрать из заказа";
                    
                    return resp;
                }
                else
                {
                    return new Responce { ResponceText = "Вы пока еще ничего не заказали :(" };
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce RemoveFromOrderByNum(long chatId, string message)
        {
            try
            {
                var dishNum = int.Parse(message);

                _service.RemoveDishFromOrder(chatId, dishNum);

                return new Responce { ResponceText = "Блюдо успешно удалено из Вашего заказа!" }; // todo Вас Вашего Вам...
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce ShowCart(long chatId)
        {
            _service.UpdateTableState(chatId, SessionState.Sitted);

            var table = _service.GetActiveTable(chatId);
            var respText = "";

            if (table.Orders.Any())
            {
                var tableSumm = table.Orders.Sum(o => o.DishFromMenu.Price);

                respText = "Вы заказали:" + Environment.NewLine + Environment.NewLine;

                //int num = 0;
                foreach (var dish in table.Orders)
                {
                    //num += 1;
                    respText += dish.Num + ". " + dish.DishFromMenu.Name + " " + dish.DishFromMenu.Price + "р. <i>" + dish.Remarks + "</i>" + Environment.NewLine;
                }
                respText += Environment.NewLine + "<b>Итого: " + tableSumm.ToString() + "р.</b>" + Environment.NewLine;
            }
            else
            {
                respText = "Вы пока еще ничего не заказали :(";
            }
                
            return new Responce
            {
                ChatId = chatId,
                ResponceText = respText,
                State = SessionState.Sitted
            };
        }

        public Responce ShowMenu(long chatId)
        {
            try
            {
                _service.UpdateTableState(chatId, SessionState.Sitted);
                var menu = _service.GetAllMenus().First(); // to do
                int dishNum = 0;

                var respText = menu.MenuName + Environment.NewLine;

                foreach (var dish in menu.DishList)
                {
                    respText += (dishNum += 1) + ". " + dish.Name + " " + dish.Price + "р. " + dish.SlashName + Environment.NewLine;
                }
                respText += "Хотите чтонибудь из меню? Просто напишите назавние блюда в чат." + Environment.NewLine;

                return new Responce { ResponceText = respText };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public MenuResponce ShowMenuOnPage(long chatId, int? showPage = null) // todo
        {
            try
            {
                var table = _service.GetActiveTable(chatId);
                int dishesOnPage = 8;

                if (DishesPerPage > 0)
                {
                    dishesOnPage = DishesPerPage;
                }

                if (table != null && table.State == SessionState.Remark)
                {
                    _service.UpdateTableState(chatId, SessionState.Sitted);
                }

                //_service.UpdateTableState(chatId, SessionState.Sitted);
                var menu = _service.GetMenuByTable(chatId);

                var respText = "<b>" + menu.MenuName + "</b>" + Environment.NewLine + Environment.NewLine;

                int page = 1;
                int pageCount = 0;

                if (showPage != null)
                {
                    page = (int)showPage;
                    _service.UpdateLastPage(chatId, (int)showPage);
                }
                else
                {
                    //var table = _service.FindTable(chatId);

                    if (table != null)
                    {
                        // todo move to LiteService
                        var lastPage = table.StateVaribles.Where(t => t.Key == "LastPage").FirstOrDefault();

                        if (lastPage != null && (int)lastPage.Value > 0)
                        {
                            page = (int)lastPage.Value;
                        }
                    }
                }

                int dishNum = (page-1)*dishesOnPage;

                decimal d = Math.Ceiling((decimal)menu.DishList.Count / dishesOnPage);
                pageCount = (int)d;
                var dishes = menu.DishList.Skip((page-1)*dishesOnPage).Take(dishesOnPage);

                foreach (var dish in dishes)
                {
                    respText += (dishNum += 1) + ". " + dish.Name + " <b>" + dish.Price + "р.</b> " + dish.SlashName + Environment.NewLine;
                }

                return new MenuResponce { ResponceText = respText, Page = page, PageCount = pageCount };
            }
            catch (Exception ex)
            {
                throw ex; // разобраться с эксепшенами и что возвращать
            }
        }

        public Responce Restrunt(long chatId, string restruntName)
        {
            try
            {
                _service.AssignMenu(chatId, restruntName);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Отлично! Выберите столик!",
                    State = SessionState.Queue
                };

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
                _service.AssignNumber(chatId, tableNumber);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Отлично! Напишите \"меню\" в чат и я принесу его вам.",
                    State = SessionState.Sitted
                };

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
                var newTable = _service.CreateTable(chatId);

                if (newTable != Guid.Empty)
                {
                    var restaurants = _service.GetAllActiveRestaurants();

                    if (restaurants.Count > 1)
                    {
                        return new Responce
                        {
                            ChatId = chatId,
                            ResponceText = "Привет! В каком вы ресторане?",
                            State = SessionState.Restaurant
                        };
                    }
                    else
                    {
                        _service.UpdateTableState(chatId, SessionState.Queue);

                        return new Responce
                        {
                            ChatId = chatId,
                            ResponceText = "Привет! За каким столиком вы сидите",
                            State = SessionState.Queue
                        };
                    }
                }
                else
                    throw new Exception("Не получилось определить ресторан!");
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }  
        }

        public Responce TableChoose(long chatId)
        {
            try
            {
                //if (_service.CreateTable(chatId) != Guid.Empty)
                //{
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Привет! За каким столиком вы сидите?",
                        State = SessionState.Queue
                    };
                //}
                //else
                //    throw new Exception("Не получилось создать столик!");
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
                _service.UpdateTableState(chatId, SessionState.Sitted);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Официант уже идет",
                    State = SessionState.Sitted
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce GiveACheck(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);
                var respText = "";

                if (table.Orders.Any())
                {
                    _service.SetCheckNeeded(chatId);
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    respText = "Счет сейчас принесут";
                }
                else
                {
                    respText = "Вы пока еще ничего не заказали :(";
                }

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = respText,
                    State = SessionState.Sitted
                };
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
                //_service.UpdateTableState(chatId, SessionState.DishChoosing);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = dish.Name + Environment.NewLine + dish.Description + Environment.NewLine +
                    dish.PictureUrl
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }
    }
}