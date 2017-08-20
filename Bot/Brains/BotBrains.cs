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
                //if (GetState(chatId) == SessionState.DishChoosing)
                //{
                    var table = _service.FindTable(chatId);

                    if (string.IsNullOrWhiteSpace(dishName))
                    {
                        var lastDishName = table.StateVaribles.Where(t => t.Key == "LastDish").FirstOrDefault();
                        dishName = lastDishName.Value.ToString();
                    }

                    var dish = _service.FindDish(dishName);
       
                    _service.OrderDish(table.Id,new DataModels.OrderedDish { DishFromMenu = dish});
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    return new Responce { ChatId = chatId, ResponceText = "Отличный выбор! Отношу заказ на кухню, чтонибудь еще?" };
                //}
                //else
                //{
                //    return Responce.UnknownResponce(chatId);
                //}
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce ShowCart(long chatId)
        {
            //if (GetState(chatId) == SessionState.Sitted)
            //{
                _service.UpdateTableState(chatId, SessionState.Sitted);
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
                
                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = respText,
                    State = SessionState.Sitted
                };
            //}
            //else
            //{
            //    return Responce.UnknownResponce(chatId);
            //}
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
                //_service.UpdateTableState(chatId, SessionState.Sitted);
                var menu = _service.GetAllMenus().First();

                var respText = menu.MenuName + Environment.NewLine + Environment.NewLine;

                int page = 1;
                int pageCount = 0;

                if (showPage != null)
                {
                    page = (int)showPage;
                    _service.UpdateLastPage(chatId, (int)showPage);
                }
                else
                {
                    var table = _service.FindTable(chatId);

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

                int dishNum = (page-1)*8;

                decimal d = Math.Ceiling((decimal)menu.DishList.Count / 8);
                pageCount = (int)d;
                var dishes = menu.DishList.Skip((page-1)*8).Take(8); //8 items per page

                foreach (var dish in dishes)
                {
                    respText += (dishNum += 1) + ". " + dish.Name + " " + dish.Price + "р. " + dish.SlashName + Environment.NewLine;
                }

                return new MenuResponce { ResponceText = respText, Page = page, PageCount = pageCount };
            }
            catch (Exception ex)
            {
                throw ex; // разобраться с эксепшенами и что возвращать
            }
        }

        public Responce Number(long chatId, int tableNumber)
        {
            try
            {
                if (_service.GetTable(chatId) != Guid.Empty)
                {
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Отлично! Напишите \"меню\" в чат и я принесу его вам.",
                        State = SessionState.Sitted
                    };
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
                    _service.UpdateTableState(chatId, SessionState.Queue);

                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Привет! За каким столиком вы сидите?",
                        State = SessionState.Queue
                    };
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
                _service.SetCheckNeeded(chatId);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Счет сейчас принесут",
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