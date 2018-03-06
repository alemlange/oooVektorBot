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
using Brains.Models;

namespace Brains
{
    public class BotBrains 
    {
        private LiteCustomerService _service { get; set; }

        private LiteRegistrationService _regService = ServiceCreator.GetRegistrationService();

        public Guid AccountId { get; set; }

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

        public string Actions
        {
            get
            {
                var dataConfig = _regService.FindConfig(AccountId);
                if (dataConfig != null)
                {
                    return "<a href=\"" + dataConfig.Actions + "\">Акции</a>" ;
                }
                else
                {
                    throw new ConfigurationException("Настройки для аккаунта не найдены!");
                }

            }
        }

        public string Description
        {
            get
            {
                var dataConfig = _regService.FindConfig(AccountId);
                if (dataConfig != null)
                {
                    return "<a href=\"" + dataConfig.Description + "\">Описание</a>" ;
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
        
        public BotBrains(string requestHost)
        {
            var accId = _regService.AccountIdByHost(requestHost);

            if(accId != null)
            {
                AccountId = accId.Value;
                var account = _regService.FindAccount(AccountId);

                if (account != null)
                {
                    _service = ServiceCreator.GetCustomerService(account.Login);
                }
                else
                    throw new ConfigurationException("Аккаунт в системе не найден!");
            }
            else
                throw new ConfigurationException("Бот в системе не зарегистрирован!");
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

        public RemarkResponce OrderMeal(long chatId, string message)
        {
            try
            {
                var dishGuid = message.Split(new string[] { "addOrder" }, StringSplitOptions.RemoveEmptyEntries)[0];
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    if(table.Cheque != null && table.Cheque.PaymentRecieved)
                        return new RemarkResponce { ChatId = chatId, ResponceText = "Извините, но заказ уже оплачен!", IsOk = false };

                    if (table.State == SessionState.OrderPosted)
                        return new RemarkResponce { ChatId = chatId, ResponceText = "Извините, но заказ уже отправлен!", IsOk = false };

                    var dish = _service.GetMenuByRestaurant(table.Restaurant).DishList.Where(o => o.Id == Guid.Parse(dishGuid)).FirstOrDefault();
                    var dishNum = table.Orders.Count + 1;

                    var orderedDishId = Guid.NewGuid();

                    var dishMods = _service.GetDishModificators(dish.ModIds ?? new List<int>());
                    if (dishMods.Any())
                    {
                        var orderedMods = dishMods.Select(o => new OrderedModificator { Mod = o, Count = 0 }).ToList();

                        _service.OrderDish(table.Id, new OrderedDish { Num = dishNum, DishFromMenu = dish, DateOfOrdering = DateTime.Now, Id = orderedDishId, OrderedMods = orderedMods });
                        //_service.UpdateTableState(chatId, SessionState.Remark);

                        var modKeys = dishMods.Select(o => new Item { Id = o.Id.ToString() + " " + orderedDishId, Name = o.Name + " +" + o.Price + "р. " });
                        return new RemarkResponce { ChatId = chatId, ResponceText = "Добавить что-нибудь?", IsOk = true, Modificators = modKeys };
                    }
                    else
                    {
                        _service.OrderDish(table.Id, new OrderedDish { Num = dishNum, DishFromMenu = dish, DateOfOrdering = DateTime.Now, Id = orderedDishId, OrderedMods = new List<OrderedModificator>() });
                        //_service.UpdateTableState(chatId, SessionState.Remark);

                        return new RemarkResponce { ChatId = chatId, ResponceText = "Блюдо добавлено в ваш заказ!", IsOk = false };
                    }
                }
                else
                {
                    return new RemarkResponce { ChatId = chatId, ResponceText = "Нажмите \"Заказ за столиком\" в главном меню, чтобы сделать заказ!", IsOk = false };
                }
            }
            catch (Exception)
            {
                return new RemarkResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", IsOk = false };
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

                    return new Responce { ChatId = chatId, ResponceText = "Хорошо, чтобы оформить заказ перейдите в корзину." };
                }
                else
                {
                    return new Responce { ChatId = chatId, ResponceText = "Нажмите \"Заказ за столиком\" в главном меню, чтобы сделать заказ!" };
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public RemarkResponce AddModificator(long chatId, string message)
        {
            try
            {
                var modIdAndGuid = message.Split(new string[] { "mod" }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var modId = Convert.ToInt32(modIdAndGuid[0]);
                var ordereId = Guid.Parse(modIdAndGuid[1]);

                var table = _service.GetActiveTable(chatId);
                if (table != null)
                {
                    var curOrder = table.Orders.Where(o => o.Id == ordereId).FirstOrDefault();

                    if(curOrder != null)
                    {
                        var modtoPlus = curOrder.OrderedMods.Where(o => o.Mod.Id == modId).FirstOrDefault();
                        if(modtoPlus == null)
                        {
                            return new RemarkResponce { ChatId = chatId, ResponceText = "Модификатор не найден.", IsOk = false };
                        }
                        else
                        {
                            if(modtoPlus.Mod.MaxCount > modtoPlus.Count)
                            {
                                modtoPlus.Count++ ;
                                var addedMods = curOrder.OrderedMods.Where(o => o.Count > 0);

                                var resText = "<b>Вы добавили:</b>" + Environment.NewLine;
                                foreach(var m in addedMods)
                                {
                                    resText += m.Mod.Name + " " + m.Mod.Price + " р."+ " x" + m.Count + Environment.NewLine;
                                }

                                _service.UpdateTable(table);
                                var modsCanBeAdded = curOrder.OrderedMods.Where(o => o.Mod.MaxCount > o.Count);
                                if (modsCanBeAdded.Any())
                                {
                                    resText += "Добавить что-нибудь еще?";
                                    var modKeys = modsCanBeAdded.Select(o => new Item { Id = o.Mod.Id.ToString() + " " + curOrder.Id, Name = o.Mod.Name + " +" + o.Mod.Price + "р. " });
                                    return new RemarkResponce { ChatId = chatId, ResponceText = resText, IsOk = true, Modificators = modKeys };
                                }
                                else
                                {
                                    return new RemarkResponce { ChatId = chatId, ResponceText = resText, IsOk = true, Modificators = new List<Item>() };
                                }  
                            }
                            else
                            {
                                return new RemarkResponce { ChatId = chatId, ResponceText = "Больше добавить нельзя!", IsOk = false };
                            }
                        }
                    }
                    else
                    {
                        return new RemarkResponce { ChatId = chatId, ResponceText = "Блюдо не найдено.", IsOk = false };
                    }             
                }
                else
                {
                    return new RemarkResponce { ChatId = chatId, ResponceText = "Нажмите \"Заказ за столиком\" в главном меню, чтобы сделать заказ!", IsOk = false };
                }
   
            }
            catch (Exception)
            {
                return new RemarkResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", IsOk = false };
            }
        }

        public Responce RemoveFromOrder(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table.Orders.Any())
                {
                    if(table.Cheque != null &&table.Cheque.PaymentRecieved)
                        return new Responce { ResponceText = "Извините, но заказ уже оплачен!" };

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

        private decimal TableSumm(Table table)
        {
            decimal summ = 0;

            foreach(var dish in table.Orders)
            {
                summ += DishPrice(dish);
            }

            return summ;
        }

        private decimal DishPrice(OrderedDish dish)
        {
            decimal summ = dish.DishFromMenu.Price;

            foreach(var mod in dish.OrderedMods)
            {
                summ += mod.Mod.Price * mod.Count;
            }

            return summ;
        }

        public GenChequeResponce CreateInvoice(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    if (table.Orders == null || !table.Orders.Any())
                        throw new PaymentException("Извините, еще нельзя оплачивать, вы пока ничего не заказали.");

                    if (table.Cheque != null && table.Cheque.PaymentRecieved == true)
                        throw new PaymentException("Вы уже оплатили заказ.");

                    var cheque = new Cheque { ChatId = chatId, Currency = "RUB", Date = DateTime.Now, Id = Guid.NewGuid(), PaymentRecieved = false };

                    decimal summ = TableSumm(table);

                    if (summ <= 60)
                        throw new PaymentException("Сумма слишком маленькая для оплаты.");

                    cheque.Summ = summ;
                    cheque.Title = "Ваш заказ";
                    cheque.Description = "";

                    foreach (var dish in table.Orders)
                    {
                        cheque.Description += dish.Num + ". " + dish.DishFromMenu.Name + " " + DishPrice(dish) + "р. " + dish.Remarks + Environment.NewLine;

                        var allOrderedMods = dish.OrderedMods.Where(o => o.Count > 0);
                        if (allOrderedMods.Any())
                        {
                            foreach (var mod in allOrderedMods)
                            {
                                cheque.Description += " " + mod.Mod.Name + " " + mod.Mod.Price + " р." + " x" + mod.Count + Environment.NewLine;
                            }
                        }
                    }
                    cheque.Description += "Номер стола: " + table.TableNumber + Environment.NewLine;
                    _service.AssignCheque(table.Id, cheque);

                    return new GenChequeResponce { ChatId = chatId, Invoice = cheque, InvoiceReady = true };
                }
                else
                {
                    return new GenChequeResponce { ChatId = chatId, InvoiceReady = false, ResponceText = "Ошибка оплаты, столик не найден." };
                }
            }
            catch(PaymentException ex)
            {
                return new GenChequeResponce { ChatId = chatId, InvoiceReady = false, ResponceText = ex.Message };
            } 
        }

        public PreCheckResponce PreCheckout(long chatId, int preSumm, string preCur, string payload)
        {
            try
            {
                var cheque = _service.GetCheque(Guid.Parse(payload));

                if(cheque != null)
                {
                    if (cheque.SummInCents != preSumm || cheque.Currency != preCur)
                        throw new PaymentException("Произошла ошибка оплаты, суммы не совпадают.");
                }
                else
                {
                    var table = _service.GetActiveTable(chatId);

                    if (table != null)
                    {
                        if (table.Orders == null || !table.Orders.Any())
                            throw new PaymentException("Произошла ошибка оплаты, вы пока ничего не заказали.");

                        decimal summ = TableSumm(table);
                        int summInCents = Convert.ToInt32(summ * 100);

                        if (table.Cheque == null)
                            throw new PaymentException("Произошла ошибка оплаты, не найден чек!");

                        if (table.Cheque.SummInCents != preSumm || table.Cheque.Currency != preCur || preSumm != summInCents)
                            throw new PaymentException("Произошла ошибка оплаты, суммы не совпадают.");

                        if (table.Cheque.Id.ToString() != payload)
                            throw new PaymentException("Произошла ошибка оплаты, номера чеков не совпадают.");
                    }
                    else
                        return new PreCheckResponce { ChatId = chatId, IsError = true, ResponceText = "Чек не найден." };
                }

                return new PreCheckResponce { ChatId = chatId, IsError = false };
            }
            catch(PaymentException ex)
            {
                return new PreCheckResponce { ChatId = chatId, IsError = true, ResponceText = ex.Message };
            }
            catch(Exception)
            {
                return new PreCheckResponce { ChatId = chatId, IsError = true, ResponceText = "Произошла ошибка при подтверждении чека." };
            }
        }

        public Responce SuccessPayment(long chatId, string payload, int totalAmount, string telegramPaymentId, string curr)
        {
            try
            {
                var cheque = _service.GetCheque(Guid.Parse(payload));

                if (cheque != null)
                {
                    if (cheque.SummInCents != totalAmount || cheque.Currency != curr)
                        throw new PaymentException("Произошла ошибка оплаты, суммы не совпадают.");

                    _service.ChequeMarkPayed(cheque.Id, telegramPaymentId);
                }
                else
                {
                    var table = _service.GetActiveTable(chatId);

                    if (table != null)
                    {
                        if (table.Cheque == null)
                            throw new PaymentException("Произошла ошибка подтверждения оплаты, не найден чек!");

                        if (table.Cheque.SummInCents != totalAmount || table.Cheque.Currency != curr)
                            throw new PaymentException("Произошла ошибка подтверждения оплаты, суммы не совпадают.");

                        if (table.Cheque.Id.ToString() != payload)
                            throw new PaymentException("Произошла ошибка оплаты, номера чеков не совпадают.");

                        _service.TableChequeMarkPayed(table.Id, telegramPaymentId);
                        _service.SendOrderToDesk(chatId);

                    }
                    else
                        throw new PaymentException("Столик не найден.");
                }

                return new Responce { ChatId = chatId, ResponceText = "Ваш заказ успешно оплачен!" };

            }
            catch (PaymentException ex)
            {
                return new Responce { ChatId = chatId, ResponceText = ex.Message };
            }
            catch
            {
                return new Responce { ChatId = chatId, ResponceText = "Произошла ошибка при подтверждении оплаты." };
            }
        }

        public OrderResponce ShowCart(long chatId)
        {
            _service.UpdateTableState(chatId, SessionState.Sitted);

            var table = _service.GetActiveTable(chatId);
            var respText = "";

            if (table.Orders.Any())
            {
                respText += "<b>Номер вашего стола: " + table.TableNumber + "</b>" + Environment.NewLine;

                var tableSumm = TableSumm(table);

                respText += "Вы заказали:" + Environment.NewLine + Environment.NewLine;

                foreach (var dish in table.Orders)
                {
                    respText += dish.Num + ". " + dish.DishFromMenu.Name + " " + DishPrice(dish) + "р. <i>" + dish.Remarks + "</i>" + Environment.NewLine;

                    var allOrderedMods = dish.OrderedMods.Where(o => o.Count > 0);
                    if (allOrderedMods.Any())
                    {
                        foreach (var mod in allOrderedMods)
                        {
                            respText += " " + mod.Mod.Name + " " + mod.Mod.Price + " р." + " x" + mod.Count + Environment.NewLine;
                        }
                    }
                }
                respText += Environment.NewLine + "<b>Итого: " + tableSumm.ToString() + "р.</b>" + Environment.NewLine;

                return new OrderResponce { ChatId = chatId, ResponceText = respText, NeedInlineKeeyboard = true};
            }
            else
            {
                respText = "Вы пока еще ничего не заказали :(";
                return new OrderResponce { ChatId = chatId, ResponceText = respText, NeedInlineKeeyboard = false };
            }
        }

        public Responce ShowAllOrders(long chatId)
        {
            var orders = _service.AllOrders(chatId).OrderByDescending(o => o.OrderPlaced).Take(3);
            var respText = "Ваши последние заказы:" + Environment.NewLine;

            if (orders != null && orders.Any())
            {
                foreach(var order in orders)
                {
                    if (order.Orders.Any())
                    {
                        respText += "<b>Дата заказа: " + order.OrderPlaced.ToString("dd.MM.yyyy") + "</b>" + Environment.NewLine;

                        respText += "Вы заказали:" + Environment.NewLine ;

                        var tableSumm = TableSumm(order);
                        foreach (var dish in order.Orders)
                        {
                            respText += dish.Num + ". " + dish.DishFromMenu.Name + " " + DishPrice(dish) + "р. <i>" + dish.Remarks + "</i>" + Environment.NewLine;

                            var allOrderedMods = dish.OrderedMods.Where(o => o.Count > 0);
                            if (allOrderedMods.Any())
                            {
                                foreach (var mod in allOrderedMods)
                                {
                                    respText += " " + mod.Mod.Name + " " + mod.Mod.Price + " р." + " x" + mod.Count + Environment.NewLine;
                                }
                            }
                            
                        }

                        respText += "<b>Итого: " + tableSumm.ToString() + "р.</b>" + Environment.NewLine + Environment.NewLine;
                    }
                }
                return new OrderResponce { ChatId = chatId, ResponceText = respText, NeedInlineKeeyboard = true };
            }
            else
            {
                respText = "Вы пока еще ничего не заказали :(";
                return new Responce { ChatId = chatId, ResponceText = respText};
            }
        }

        public MenuResponce SnowMenuByCategory(long chatId, string category)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                //if (table != null && (table.State == SessionState.Remark || table.State == SessionState.MenuCategory))
                //{
                //    _service.UpdateTableState(chatId, SessionState.Sitted);
                //}

                var menu = _service.GetMenuByTable(chatId) ?? _service.GetStandartMenu();

                var respText = "<b>" + category + "</b>" + Environment.NewLine + Environment.NewLine;

                var dishes = menu.DishList.Where(m => m.Category == category);

                var response = new MenuResponce() { ResponceText = "<b>" + category + ": </b>" + Environment.NewLine + Environment.NewLine};

                response.Dishes = dishes.Select(o => new Models.Item { Id = o.Id.ToString(), Name = o.ShortName + " " + o.Price + "р." });

                return response;

            }
            catch
            {
                return new MenuResponce() { ChatId = chatId, ResponceText = "Упс, что-то пошло не так." };
            }
        }

        public Responce Restrunt(long chatId, string restruntName)
        {
            try
            {
                _service.AssignRestaurant(chatId, restruntName);
                _service.UpdateTableState(chatId, SessionState.InQueue);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Номер стола:"
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }  
        }

        public Responce BookingRequest(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);
                var tableId = (table == null) ? _service.CreateTable(chatId) : table.Id;

                _service.UpdateTableState(chatId, SessionState.Booking);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Пожалуйста, введите информацию о бронировании в одно сообщение (Имя, дата и время, количество человек и телефон для связи). Мы вам перезвоним!"
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce CancelTable(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);
                if (table != null)
                {
                    
                    if(table.TableNumber == null)
                        _service.DeleteTable(table.Id);
                    else
                        _service.UpdateTableState(table.Id, SessionState.Sitted);
                }

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Вы в главном меню."
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce LeaveBooking(long chatId, string text)
        {
            try
            {
                _service.CreateBooking(new Booking { ChatId = chatId, Date = DateTime.Now, Id = Guid.NewGuid(), Text = text });

                var table = _service.GetActiveTable(chatId);
                if (table != null )
                {
                    if (table.TableNumber == null)
                        _service.DeleteTable(table.Id);
                    else
                        _service.UpdateTableState(table.Id, SessionState.Sitted);
                }

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Скоро с вами свяжутся!"
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce FeedbackRequest(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);
                var tableId = (table == null) ? _service.CreateTable(chatId) : table.Id;

                _service.UpdateTableState(tableId, SessionState.Feedback);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Тут можно оставить отзыв. Уместите его в одно сообщение, пожалуйства! Мы обязательно учтем все ваши пожелания!"
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce LeaveFeedback(long chatId, string username,  string text)
        {
            try
            {
                _service.CreateFeedback(new Feedback { ChatId = chatId, Date = DateTime.Now, Id = Guid.NewGuid(), Text = text, UserName = username });

                var table = _service.GetActiveTable(chatId);
                if (table != null)
                {
                    if (table.TableNumber == null)
                        _service.DeleteTable(table.Id);
                    else
                        _service.UpdateTableState(table.Id, SessionState.Sitted);
                }

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Спасибо за ваш отзыв!"
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce AssignTableNumber(long chatId, string message)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    var isNumber = Int32.TryParse(message, out int tableNumber);

                    if (isNumber)
                    {
                        _service.AssignQueueNumber(chatId, message);
                        _service.UpdateTableState(chatId, SessionState.Sitted);

                        return new Responce { ChatId = chatId, ResponceText = "Отлично! Теперь вы можете сделать заказ" };
                    }
                    else
                    {
                        return new Responce { ChatId = chatId, ResponceText = "Нужно ввести номер стола." };
                    }

                }
                else
                    return new Responce { ChatId = chatId, ResponceText = "Стол не найден!" };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public Responce PaymentRequest(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);
                var tableId = (table == null) ? _service.CreateTable(chatId) : table.Id;

                _service.UpdateTableState(tableId, SessionState.Cheque);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Введите сумму для оплаты"
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public GenChequeResponce InputSumm(long chatId, string message, string username)
        {
            try
            {
                var isNumber = Int32.TryParse(message, out int summ);

                if (isNumber)
                {
                    var cheque = new Cheque { ChatId = chatId, Currency = "RUB", Date = DateTime.Now, Id = Guid.NewGuid(), PaymentRecieved = false, UserName = username };

                    if (summ <= 60)
                        throw new PaymentException("Сумма слишком маленькая для оплаты.");

                    cheque.Summ = summ;
                    cheque.Title = "Оплата";
                    cheque.Description += "Опалата на сумму " + summ + "руб.";

                    _service.CreateCheque(cheque);

                    var table = _service.GetActiveTable(chatId);
                    if (table != null)
                    {
                        if (table.TableNumber == null)
                            _service.DeleteTable(table.Id);
                        else
                            _service.UpdateTableState(table.Id, SessionState.Sitted);
                    }

                    return new GenChequeResponce { ChatId = chatId, Invoice = cheque, InvoiceReady = true };
                }
                else
                {
                    throw new PaymentException("Нужно ввести сумму оплаты.");
                }
            }
            catch (PaymentException ex)
            {
                return new GenChequeResponce { ChatId = chatId, InvoiceReady = false, ResponceText = ex.Message };
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
                    ResponceText = "Официант уже идет"
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
                var table = _service.GetActiveTable(chatId);

                if (table != null && table.State != SessionState.OrderPosted)
                {
                    _service.UpdateTableState(chatId, SessionState.MenuCategory);
                }

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Выберите раздел меню!"
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
                    if (table.State != SessionState.OrderPosted)
                    {
                        _service.UpdateTableState(chatId, SessionState.Sitted);
                    }
                    
                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Нажмите \"Меню\" и я принесу его вам.",
                    };
                }
                else
                {
                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Нажмите \"Заказ за столиком\", чтобы сделать заказ!",
                    };
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public TimeInlineResponce ArrivingTime(long chatId)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    return new TimeInlineResponce { ChatId = chatId, ResponceText = "Через сколько вы заберете заказ?", OkToChangeTime = true };
                }
                else
                    return new TimeInlineResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", OkToChangeTime = false };
            }
            catch (Exception)
            {
                return new TimeInlineResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", OkToChangeTime = false };
            }
        }

        public TimeInlineResponce ChangeArrivingTime(long chatId, string message)
        {
            try
            {
                var table = _service.GetActiveTable(chatId);

                if (table != null)
                {
                    var timeString = message.Split(new string[] { "time" },StringSplitOptions.RemoveEmptyEntries)[0];

                    Int32.TryParse(timeString, out int time);

                    if(time <=60 && time >= 0)
                    {
                        _service.SetArrivingTime(chatId, time);

                        return new TimeInlineResponce { ChatId = chatId, OkToChangeTime = true };
                    }
                    else
                    {
                        return new TimeInlineResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", OkToChangeTime = false };
                    }
                    
                }
                else
                    return new TimeInlineResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", OkToChangeTime = false };
            }
            catch (Exception)
            {
                return new TimeInlineResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", OkToChangeTime = false };
            }
        }

        public Responce CloseTimeArriving(long chatId)
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
                    return Responce.UnknownResponce(chatId);
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
                _service.AssignNextQueueNumber(chatId);
                _service.UpdateTableState(chatId, SessionState.Sitted);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Отлично! Напишите \"меню\" в чат и я принесу его вам."
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
                        ResponceText = "В каком вы заведении?"
                    };
                }
                else
                {
                    var rest = restaurants.FirstOrDefault();
                    _service.AssignRestaurant(chatId, rest.Name);
                    _service.UpdateTableState(chatId, SessionState.InQueue);

                    return new Responce
                    {
                        ChatId = chatId,
                        ResponceText = "Введите номер стола за которым вы сидите:"
                    };
                }
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }  
        }

        public Responce PayCash(long chatId)
        {
            try
            {
                _service.SetCashPayment(chatId);

                _service.SendOrderToDesk(chatId);

                return new Responce
                {
                    ChatId = chatId,
                    ResponceText = "Вы выбрали оплату наличными в заведении. Заказ отправлен в заведение!"
                };
            }
            catch (Exception)
            {
                return Responce.UnknownResponce(chatId);
            }
        }

        public MenuItemResponce GetMenuItem(long chatId, string message)
        {
            try
            {
                var dishGuid = message.Split(new string[] { "dish" }, StringSplitOptions.RemoveEmptyEntries)[0];

                var dish = _service.GetDish(Guid.Parse(dishGuid));

                if(dish != null)
                {
                    var needKeyboard = true;
                    var table = _service.GetActiveTable(chatId);
                    if (table == null || table.State == SessionState.Unknown)
                        needKeyboard = false;

                    return new MenuItemResponce
                    {
                        ChatId = chatId,
                        DishId = dish.Id.ToString(),
                        ResponceText = "<a href=\"" + dish.PictureUrl + "\">" + dish.Name + "</a>" + Environment.NewLine + dish.Description + Environment.NewLine + "<b>Стоимость: " + dish.Price + "р.</b>" + Environment.NewLine,
                        NeedInlineKeyboard = needKeyboard
                    };
                }
                else
                {
                    return new MenuItemResponce { ChatId = chatId, NeedInlineKeyboard = false, ResponceText = "Упс, что-то пошло не так." };
                }
            }
            catch (Exception)
            {
                return new MenuItemResponce {ChatId = chatId, NeedInlineKeyboard = false, ResponceText="Упс, что-то пошло не так." };
            }
        }

        public RestaurantResponce GetAllRestaurants(long chatId)
        {
            try
            {
                var restaurants = _service.GetAllActiveRestaurants();

                if (restaurants.Any())
                {
                    var restaurantsInfo = restaurants.Select(o => new RestaurantInfo { Info = o.Name + Environment.NewLine + o.Description, Latitude = o.Latitude, Longitude = o.Longitude });

                    return new RestaurantResponce { ChatId = chatId, IsOk = true, RestaurantsInfo = restaurantsInfo };
                }
                else
                {
                    return new RestaurantResponce { ChatId = chatId, ResponceText = "В системе не заведено ни одного ресторана!", IsOk = false };
                }
            }
            catch (Exception)
            {
                return new RestaurantResponce { ChatId = chatId, ResponceText = "Упс, что-то пошло не так.", IsOk = false };
            }
        }
    }
}