using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        public ActionResult BotStart()
        {
            var regService = new RegistrationService();
            var config = regService.FindConfiguration(User.Identity.Name);

            try
            {
                var status = new BotClient(config.TelegramBotLocation).StartBot(config.BotKey);

                if (status.Contains("Ok"))
                {
                    return Json(new { isAuthorized = true, isSuccess = true, okStatus = true, msg = "Бот зарегистрирован" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { isAuthorized = true, isSuccess = true, okStatus = false, msg = status }, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { isAuthorized = true, isSuccess = true, okStatus = false, msg = "Не получилось стартовать бота" }, JsonRequestBehavior.AllowGet);
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
                if (curRest != Guid.Empty)
                {
                    activeTables = service.GetManagerTables(curRest).OrderByDescending(o => o.OrderPlaced).ToList();
                    inActiveTables = service.GetInActiveTables(curRest).OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
                }
                else
                {
                    activeTables = service.GetManagerTables().OrderByDescending(o => o.OrderPlaced).ToList();
                    inActiveTables = service.GetInActiveTables().OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
                }    
                
            }
            else
            {
                activeTables = service.GetManagerTables().OrderByDescending(o => o.OrderPlaced).ToList();
                inActiveTables = service.GetInActiveTables().OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
            }
            var model = new AllTablesViewModel { ActiveTables = Mapper.Map<List<TableCardViewModel>>(activeTables), InActiveTables = Mapper.Map<List<TableCardViewModel>>(inActiveTables) };

            return View("TableCardList", model);
        }

        [HttpGet]
        public ActionResult UpdateTables()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var rests = service.GetAllRestaurants();
            var restCookie = Request.Cookies.Get("CurRest");

            string restOptionsView = "";
            string tablesView = "";

            var restsModel = new RestOptionsViewModel { AvailableRests = new List<Restaurant> { new Restaurant { Name = "Все", Id = Guid.Empty } } };

            if (rests != null && rests.Any())
            {
                restsModel.AvailableRests.AddRange(rests);

                if (restCookie != null)
                {
                    var curRest = restCookie.Value;
                    var rest = service.GetRestaurant(Guid.Parse(curRest));

                    if (rest == null)
                    {
                        restCookie.Value = Guid.Empty.ToString();
                        Response.Cookies.Set(restCookie);
                        restsModel.CurrentRest = Guid.Empty;
                    }
                    else
                    {
                        restsModel.CurrentRest = rest.Id;
                    }
                }
            }
            restOptionsView = RenderPartialViewToString("RestaurantOptions", restsModel);

            var activeTables = new List<Table>();
            var inActiveTables = new List<Table>();

            if (restCookie != null)
            {
                var curRest = Guid.Parse(restCookie.Value);
                if (curRest != Guid.Empty)
                {
                    activeTables = service.GetManagerTables(curRest).OrderByDescending(o => o.OrderPlaced).ToList();
                    inActiveTables = service.GetInActiveTables(curRest).OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
                }
                else
                {
                    activeTables = service.GetManagerTables().OrderByDescending(o => o.OrderPlaced).ToList();
                    inActiveTables = service.GetInActiveTables().OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
                }

            }
            else
            {
                activeTables = service.GetManagerTables().OrderByDescending(o => o.OrderPlaced).ToList();
                inActiveTables = service.GetInActiveTables().OrderByDescending(o => o.OrderPlaced).Take(20).ToList();
            }

            var newTables = activeTables.Where(o => !o.OrderProcessed).Any();
            var tablesModel = new AllTablesViewModel { ActiveTables = Mapper.Map<List<TableCardViewModel>>(activeTables), InActiveTables = Mapper.Map<List<TableCardViewModel>>(inActiveTables) };

            tablesView = RenderPartialViewToString("TableCardList", tablesModel);

            return Json(new { isAuthorized = true, isSuccess = true, tablesView = tablesView, restOptionsView = restOptionsView, newTables = newTables }, JsonRequestBehavior.AllowGet);
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

                    //if (value)
                    //{
                    //    new BotClient(config.TelegramBotLocation).SendNotification(table.ChatId, "Ваш заказ принят!");
                    //}
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
                    case CardTypes.Dispatch:
                        {
                            return View("DispatchCardEdditable", new DispatchViewModel());
                        }
                    case CardTypes.Mod:
                        {
                            return View("ModificatorsCardEdditable", new ModificatorViewModel());
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
        public ActionResult DeleteItem(string itemId, CardTypes itemType)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                switch (itemType)
                {
                    case CardTypes.Dish:
                        {
                            service.DeleteDish(Guid.Parse(itemId));
                            break;
                        }
                    case CardTypes.Menu:
                        {
                            service.DeleteMenu(Guid.Parse(itemId));
                            break;
                        }
                    case CardTypes.Table:
                        {
                            service.DeleteTable(Guid.Parse(itemId));
                            break;
                        }
                    case CardTypes.Restaurant:
                        {
                            service.DeleteRestaraunt(Guid.Parse(itemId));
                            break;
                        }
                    case CardTypes.Mod:
                        {
                            service.DeleteModificator(Int32.Parse(itemId));
                            break;
                        }
                    default:
                        break;
                }

                return Json(new { isAuthorized = true, isSuccess = true });

            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        protected string RenderPartialViewToString(string viewName, object model = null)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            }
            ViewData.Model = model;
            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                return stringWriter.GetStringBuilder().ToString();
            }
        }

    }
}