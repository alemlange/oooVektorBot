﻿using System;
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
        private LiteCustomerService _service { get; set; }

        private LiteRegistrationService _regService = ServiceCreator.GetRegistrationService();

        public Guid AccountId
        {
            get
            {
                return ConfigurationSettings.AccountId;
            }
        }

        public string BotToken
        {
            get
            {
                var dataConfig = _regService.FindConfig(AccountId);
                if (dataConfig != null)
                {
                    if (dataConfig.BotToken != null)
                        return dataConfig.BotToken;
                    else
                        throw new ConfigurationException("Не найден токен бота.");
                }
                else
                {
                    throw new ConfigurationException("Настройки для аккаунта не найдены!");
                }

            }
        }

        public string PaymentToken
        {
            get
            {
                var dataConfig = _regService.FindConfig(AccountId);
                if (dataConfig != null)
                {
                    return dataConfig.PaymentToken;
                }
                else
                {
                    throw new ConfigurationException("Настройки для аккаунта не найдены!");
                }

            }
        }

        public string GreetingsText
        {
            get
            {
                var dataConfig = _regService.FindConfig(AccountId);
                if(dataConfig != null)
                {
                    return dataConfig.BotGreeting;
                }
                else
                {
                    throw new ConfigurationException("Настройки для аккаунта не найдены!");
                }

            }
        }

        public string PicturePath
        {
            get
            {
                return ConfigurationSettings.FilePath;
            }
        }

        public List<string> RestaurantNames
        {
            get
            {
                var account = _regService.FindAccount(AccountId);

                if (account != null)
                {
                    var allRestaurants = _service.GetAllRestaurants();

                    if(allRestaurants!= null & allRestaurants.Any())
                        return allRestaurants.Select(o => o.Name).ToList();
                    else
                        throw new ConfigurationException("В системе нет ни одного ресторана!");

                }
                else
                    throw new ConfigurationException("Аккаунт в системе не найден!");
            }
        }
        
        public BotBrains()
        {
            var account = _regService.FindAccount(AccountId);

            if (account != null)
            {
                _service = ServiceCreator.GetCustomerService(account.Login);
            }
            else
                throw new ConfigurationException("Аккаунт в системе не найден!");   
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

            if (BotToken == null)
            {
                throw new ConfigurationException("Не найден токен бота.");
            }
        }

        public List<string> GetMenuCategoriesByChatId(long chatId)
        {
            var state = GetState(chatId);
            var categories = new List<string>();

            if (state == SessionState.Unknown)
            {
                var defaultMenu = _service.GetStandartMenu();

                foreach (var category in defaultMenu.CategoriesSorted)
                {
                    categories.Add(category);
                }
            }
            else
            {
                var menu = _service.GetMenuByTable(chatId);

                foreach (var category in menu.CategoriesSorted)
                {
                    categories.Add(category);
                }
            }
            return categories;
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

                    _service.OrderDish(table.Id, new OrderedDish { Num = dishNum, DishFromMenu = dish, DateOfOrdering = DateTime.UtcNow.AddHours(3) });
                    _service.UpdateTableState(chatId, SessionState.Remark);

                    return new Responce { ChatId = chatId, ResponceText = "Отличный выбор! Если у вас есть какие то пожелания к блюду, просто отправьте их сообщением!" };
                }
                else
                {
                    return new Responce { ChatId = chatId, ResponceText = "Вы не выбрали стол! Нажмите \"Начать\" в главном меню, чтобы сделать заказ!" };
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
                    return new Responce { ChatId = chatId, ResponceText = "Вы не выбрали стол! Нажмите \"Начать\" в главном меню, чтобы сделать заказ!" };
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
                respText = "Вы пока еще ничего не заказали :(";
            }
                
            return new Responce
            {
                ChatId = chatId,
                ResponceText = respText,
                //State = SessionState.Sitted
            };
        }

        public Responce SnowMenuByCategory(long chatId, string category)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table != null && (table.State == SessionState.Remark || table.State == SessionState.MenuCategory))
                {
                    _service.UpdateTableState(chatId, SessionState.Sitted);
                }

                var menu = _service.GetMenuByTable(chatId) ?? _service.GetStandartMenu();

                var respText = "<b>" + category + "</b>" + Environment.NewLine + Environment.NewLine;

                var dishes = menu.DishList.Where(m => m.Category == category);

                var dishNum = 0;

                foreach (var dish in dishes)
                {
                    respText += (dishNum += 1) + ". " + dish.Name + " <b>" +
                        dish.Price + "р.</b> " + dish.SlashName + Environment.NewLine;
                }

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = respText + Environment.NewLine +
                        "Хотите узнать про блюдо подробнее? Просто кликните по слэш-ссылке рядом с блюдом."
                };
            }
            catch
            {
                return Responce.UnknownResponce(chatId);
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
                _service.UpdateTableState(chatId, SessionState.MenuCategory);

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

        public Responce CloseMenu(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    _service.UpdateTableState(chatId, SessionState.Sitted);

                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Напишите \"Меню\" в чат и я принесу его вам.",
                    };
                }
                else
                {
                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Нажмите \"Начать\", чтобы сделать заказ!",
                    };
                }
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
                    respText = "Вы пока еще ничего не заказали :(";
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