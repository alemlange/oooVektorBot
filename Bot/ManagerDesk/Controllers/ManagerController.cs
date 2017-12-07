using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiteDbService.Helpers;
using ManagerDesk.ViewModels;
using ManagerDesk.ViewModels.Enums;
using AutoMapper;
using DataModels;
using DataModels.Exceptions;
using ManagerDesk.Services;
using Clients;

namespace ManagerDesk.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var regService = new RegistrationService();
                var config = regService.FindConfiguration(User.Identity.Name);
                return View();
            }
            catch (AuthException)
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        [HttpGet]
        public ActionResult Navigation()
        {
            var regService = new RegistrationService();
            var config = regService.FindConfiguration(User.Identity.Name);

            var model = new NavigationViewModel { UserPicPath = config.ProfilePicturePath, OrgName = config.OrgName };
            return View(model);
        }

        [HttpGet]
        public ActionResult BotStatus()
        {
            var regService = new RegistrationService();
            var config = regService.FindConfiguration(User.Identity.Name);

            try
            {
                var status = new BotClient(config.TelegramBotLocation).GetStatus();

                if (status.Contains("Ok"))
                {
                    return Json(new { isAuthorized = true, isSuccess = true, okStatus = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { isAuthorized = true, isSuccess = true, okStatus = false, msg = status }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { isAuthorized = true, isSuccess = true, okStatus = false, msg = "Не получилось связаться с ботом" }, JsonRequestBehavior.AllowGet);
            }
 
        }

        [HttpGet]
        public ActionResult RestaurantOptions()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var rests = service.GetAllRestaurants();
            if (rests != null && rests.Any())
            {
                rests.Add( new Restaurant { Name="Все", Id = Guid.Empty});
                var model = new RestOptionsViewModel { AvailableRests = rests };

                var restCookie = Request.Cookies.Get("CurRest");
                if (restCookie != null)
                {
                    var curRest = restCookie.Value;
                    var rest = service.GetRestaurant(Guid.Parse(curRest));

                    if(rest == null)
                    {
                        restCookie.Value = Guid.Empty.ToString();
                        Response.Cookies.Set(restCookie);
                        model.CurrentRest = Guid.Empty;
                    }
                    else
                    {
                        model.CurrentRest = rest.Id;
                    } 
                }

                return View(model);
            }
            else
            {
                var model = new RestOptionsViewModel { AvailableRests = new List<Restaurant> { new Restaurant { Name = "Все", Id = Guid.Empty } } };
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult AllTables()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);

            var activeTables = new List<Table>();
            var inActiveTables = new List<Table>();

            var restCookie = Request.Cookies.Get("CurRest");
            if (restCookie != null)
            {
                var curRest = Guid.Parse(restCookie.Value);
                var curMenu = service.GetMenuByRestaurant(curRest);           
                if (curMenu != null)
                {
                    activeTables = service.GetActiveTables(curMenu.Id).OrderByDescending(o => o.OrderPlaced).ToList();
                    inActiveTables = service.GetInActiveTables(curMenu.Id).OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
                }
                else if(curRest == Guid.Empty)
                {
                    activeTables = service.GetActiveTables().OrderByDescending(o => o.OrderPlaced).ToList();
                    inActiveTables = service.GetInActiveTables().OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
                }    
                
            }
            else
            {
                activeTables = service.GetActiveTables().OrderByDescending(o => o.OrderPlaced).ToList();
                inActiveTables = service.GetInActiveTables().OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
            }
            var model = new AllTablesViewModel { ActiveTables = Mapper.Map<List<TableCardViewModel>>(activeTables), InActiveTables = Mapper.Map<List<TableCardViewModel>>(inActiveTables) };

            return View("TableCardList", model);
        }

        [HttpGet]
        public ActionResult TableActions(Guid tableid)
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);

            var table = service.GetTable(tableid);
            if (table != null)
            {
                var tableActions = Mapper.Map<TableActionsViewModel>(table);

                return View(tableActions);
            }
            else
                return Json(new { isAuthorized = true, isSuccess = false});
        }

        [HttpPost]
        public ActionResult TableActions(Guid tableId, bool orderProc, bool helpNeeded, bool checkPlease)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var table = service.GetTable(tableId);
                if (table != null)
                {
                    table.HelpNeeded = helpNeeded;
                    table.CheckNeeded = checkPlease;
                    table.OrderProcessed = orderProc;
                    service.UpdateTable(table);
                }
                else
                    throw new ArgumentNullException("Table not found!");

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult TableSetProccesed(Guid tableId, bool value)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var config = new RegistrationService().FindConfiguration(User.Identity.Name);

                var table = service.GetTable(tableId);
                if (table != null)
                {
                    table.OrderProcessed = value;
                    service.UpdateTable(table);

                    if (value)
                    {
                        new BotClient(config.TelegramBotLocation).SendNotification(table.ChatId, "Ваш заказ уже готовится!");
                    }
                }
                else
                    throw new ArgumentNullException("Table not found!");

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult TableSetHelp(Guid tableId, bool value)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var table = service.GetTable(tableId);
                if (table != null)
                {
                    table.HelpNeeded = value;
                    service.UpdateTable(table);
                }
                else
                    throw new ArgumentNullException("Table not found!");

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult TableSetCheck(Guid tableId, bool value)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var table = service.GetTable(tableId);
                if (table != null)
                {
                    table.CheckNeeded = value;
                    service.UpdateTable(table);
                }
                else
                    throw new ArgumentNullException("Table not found!");

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CloseTable(Guid tableId)
        {
            try
            {
                if(tableId != Guid.Empty)
                {
                    var service = ServiceCreator.GetManagerService(User.Identity.Name);
                    service.CloseTable(tableId);
                    return Json(new { isAuthorized = true, isSuccess = false });

                }
                else
                    return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Add(CardTypes activeSection)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                switch (activeSection)
                {
                    case CardTypes.Dish:
                        {
                            var model = new DishViewModel();

                            var allDishes = service.GetAllDishes();
                            if (allDishes != null && allDishes.Any())
                            {
                                model.AvailableCategories = allDishes.Where(o => !string.IsNullOrEmpty(o.Category)).Select(o => o.Category).Distinct().ToList();
                            }
                            return View("DishCardEdditable", model);
                        }
                    case CardTypes.Menu:
                        {
                            var model = new MenuViewModel();

                            var rests = service.GetAllRestaurants();
                            if(rests != null && rests.Any())
                            {
                                model.AvailableRests = Mapper.Map<List<RestaurantDropDown>>(rests);
                            }

                            return View("MenuCardEdditable", model);
                        }
                    case CardTypes.Restaurant:
                        {
                            return View("RestaurantCardEdditable", new RestaurantViewModel());
                        }
                    default:
                        throw new Exception("No active section");
                }
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteItem(Guid itemId, CardTypes itemType)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                if (itemId != Guid.Empty)
                {
                    switch (itemType)
                    {
                        case CardTypes.Dish:
                            {
                                service.DeleteDish(itemId);
                                break;
                            }
                        case CardTypes.Menu:
                            {
                                service.DeleteMenu(itemId);
                                break;
                            }
                        case CardTypes.Table:
                            {
                                service.DeleteTable(itemId);
                                break;
                            }
                        case CardTypes.Restaurant:
                            {
                                //var restCookie = Request.Cookies.Get("CurRest");
                                //if(restCookie!= null)
                                //{
                                //    var curRest = restCookie.Value;
                                //    if (curRest != null)
                                //    {
                                //        var restGuid = Guid.Parse(curRest);

                                //        if(restGuid == itemId)
                                //        {
                                //            var rests = service.GetAllRestaurants();
                                //            if(rests!=null && rests.Any())
                                //            {
                                //                var notCurRest = rests.Where(o => o.Id != itemId);
                                //                if (notCurRest.Any())
                                //                    restCookie.Value = notCurRest.FirstOrDefault().Id.ToString();
                                                
                                //                else
                                //                    restCookie.Value = null;
                                                
                                //                Request.Cookies.Set(restCookie);
                                //            }  
                                //        }
                                //    }
                                //}
                                
                                service.DeleteRestaraunt(itemId);
                                break;
                            }
                        default:
                            break;
                    }
                    
                }
                else
                    throw new Exception("Object id not specified!");
                return Json(new { isAuthorized = true, isSuccess = true });

            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

    }
}