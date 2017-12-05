using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brains.Interfaces;
using LiteDbService;
using LiteDbService.Helpers;
using Brains.Responces;
using DataModels;
using DataModels.Enums;
using DataModels.Exceptions;
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

        public List<string> MenuCategories { get; set; }

        public BrainsConfig Config { get; set; }

        public BotBrains()
        {
            var accountId = ConfigurationSettings.AccountId;

            if (accountId != Guid.Empty)
            {
                var account = _regService.FindAccount(accountId);

                _service = ServiceCreator.GetCustomerService(account.Login);

                var dataConfig = _regService.FindConfig(accountId);
                if (dataConfig != null)
                {
                    Config = new BrainsConfig
                    {
                        DishesPerPage = dataConfig.DishesPerPage,
                        Greetings = dataConfig.BotGreeting,
                        PicturePath = ConfigurationSettings.FilePath
                    };

                    var allDishes = _service.GetAllDishes();
                    DishNames = new List<string>();
                    foreach (var dish in allDishes)
                    {
                        DishNames.Add(dish.Name.ToLower());
                    }

                    var allRestaurants = _service.GetAllRestaurants();
                    RestaurantNames = new List<string>();
                    foreach (var restrunt in allRestaurants)
                    {
                        RestaurantNames.Add(restrunt.Name);
                    }

                    var defaultMenu = _service.GetStandartMenu();
                    MenuCategories = new List<string>();
                    foreach (var category in defaultMenu.CategoriesSorted)
                    {
                        MenuCategories.Add(category);
                    }
                }
                else
                    throw new ConfigurationException("Настройки для аккаунта не найдены!");
            }
            else
                throw new ConfigurationException("Не заполнено поле AccountId в боте!");   
        }

        public void SystemDiagnostic()
        {
            var menus = _service.GetAllMenus();
            if (!menus.Any())
            {
                throw new ConfigurationException("В системе нет ни одного активного меню!");
            }
            var restaurants = _service.GetAllRestaurants();
            if (!restaurants.Any())
            {
                throw new ConfigurationException("В системе нет ни одного ресторана!");
            }
            else
            {
                var restsWithMenu = restaurants.Where(r => r.Menu != Guid.Empty);
                if (!restsWithMenu.Any())
                    throw new ConfigurationException("В системе нет ресторана с привязанным меню!");
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

        public int RestTableCount(long chatId)
        {
            var table = _service.GetActiveTable(chatId);

            if (table != null)
            {
                var rest = _service.GetRestaurantByTable(table.Id);
                return rest.TableCount == 0 ? 50 : rest.TableCount;
            }
            else
                throw new TableNotFoundException("Table not found!");
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

                    _service.OrderDish(table.Id, new DataModels.OrderedDish { Num = dishNum, DishFromMenu = dish, DateOfOrdering = DateTime.Now });
                    _service.UpdateTableState(chatId, SessionState.Remark);

                    return new Responce { ChatId = chatId, ResponceText = "Отличный выбор! Если у вас есть какие то пожелания к блюду, просто отправьте их сообщением!" };
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
                    _service.UpdateDishRemark(table.Id, message);
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    return new Responce { ChatId = chatId, ResponceText = "Отношу заказ на кухню, чтонибудь еще?" };
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
                    resp.ResponceText += Environment.NewLine + "Отправьте сообщением номер блюда, которое вы хотите убрать из заказа";
                    
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

                return new Responce { ResponceText = "Блюдо успешно удалено из вашего заказа!" };
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

                foreach (var dish in table.Orders)
                {
                    respText += dish.Num + ". " + dish.DishFromMenu.Name + " " + dish.DishFromMenu.Price + "р. <i>" + dish.Remarks + "</i>" + Environment.NewLine;
                }
                respText += Environment.NewLine + "<b>Итого: " + tableSumm.ToString() + "р.</b>" + Environment.NewLine;
            }
            else
            {
                respText = "Вы пока еще ничего не заказали.";
            }
                
            return new Responce
            {
                ChatId = chatId,
                ResponceText = respText,
                //State = SessionState.Sitted
            };
        }

        public MenuResponce ShowMenuOnPage(long chatId, int? showPage = null) // todo
        {
            try
            {
                var table = _service.GetActiveTable(chatId);
                int dishesOnPage = 8;

                if (Config.DishesPerPage > 0)
                {
                    dishesOnPage = Config.DishesPerPage;
                }

                if (table != null && table.State == SessionState.Remark)
                {
                    _service.UpdateTableState(chatId, SessionState.Sitted);
                }

                var menu = _service.GetMenuByTable(chatId) ?? _service.GetStandartMenu();

                var respText = "<b>" + menu.MenuName + "</b>" + Environment.NewLine;

                int page = 1;
                int pageCount = 0;

                if (showPage != null)
                {
                    page = (int)showPage;
                    _service.UpdateLastPage(chatId, (int)showPage);
                }
                else
                {

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

                var dishlist = menu.DishList.Where(m => m.Category != null).OrderBy(m => m.Category).Concat(menu.DishList.Where(m => m.Category == null));

                foreach (var dish in dishlist)
                {
                    if (dish.Category == null)
                    {
                        dish.Category = "Другое";
                    }
                }

                var dishes = dishlist.Skip((page-1)*dishesOnPage).Take(dishesOnPage);

                string category = "";

                foreach (var dish in dishes)
                {
                    if (dish.Category != null && category != dish.Category)
                    {
                        respText += Environment.NewLine + "<i>" + dish.Category + "</i>" + Environment.NewLine +
                            (dishNum += 1) + ". " + dish.Name + " <b>" +
                            dish.Price + "р.</b> " + dish.SlashName + Environment.NewLine;

                        category = dish.Category;
                    }
                    else
                    {
                        respText += (dishNum += 1) + ". " + dish.Name + " <b>" +
                            dish.Price + "р.</b> " + dish.SlashName + Environment.NewLine;
                    }
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
                _service.AssignRestaurant(chatId, restruntName);
                _service.UpdateTableState(chatId, SessionState.Queue);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Отлично! Выберите столик!",
                    //State = SessionState.Queue
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }  
        }

        public Responce ShowMenuCategories(long chatId)
        {
            try
            {
                //_service.UpdateTableState(chatId, SessionState.MenuCategory);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Выберите раздел меню!",
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
                _service.UpdateTableState(chatId, SessionState.Sitted);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Отлично! Напишите \"меню\" в чат и я принесу его вам.",
                    //State = SessionState.Sitted
                };

            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce QRCode(long chatId, string code)
        {
            try
            {
                string restruntCode = code.Substring(1, 3);
                int tableNumber = int.Parse(code.Substring(5));

                _service.AssignRestaurantByCode(chatId, restruntCode);
                _service.AssignNumber(chatId, tableNumber);
                _service.UpdateTableState(chatId, SessionState.Sitted);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Отлично! Напишите \"меню\" в чат и я принесу его вам.",
                    //State = SessionState.Sitted
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

                var restaurants = _service.GetAllActiveRestaurants();

                if (restaurants.Count > 1)
                {
                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Привет! В каком вы ресторане?",
                        //State = SessionState.Restaurant
                    };
                }
                else
                {
                    var rest = restaurants.FirstOrDefault();
                    _service.AssignRestaurant(chatId, rest.Name);
                    _service.UpdateTableState(chatId, SessionState.Queue);

                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Напишите номер столика, за которым вы сидите",
                        //State = SessionState.Queue
                    };
                }
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
                _service.UpdateTableState(chatId, SessionState.Sitted);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Привет! За каким столиком вы сидите?",
                    //State = SessionState.Queue
                };
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
                    //State = SessionState.Sitted
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
                    respText = "Вы пока еще ничего не заказали.";
                }

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = respText,
                    //State = SessionState.Sitted
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