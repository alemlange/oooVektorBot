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
using ManagerDesk.Services;

namespace ManagerDesk.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Navigation()
        {
            var regService = new RegistrationService();
            var config = regService.FindConfiguration(User.Identity.Name);

            var model = new NavigationViewModel { UserPicPath = config.ProfilePicturePath, OrgName= config.OrgName };
            return View(model);
        }

        [HttpGet]
        public ActionResult AllTables()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var activeTables = service.GetActiveTables();
            var inActiveTables = service.GetInActiveTables();
            var model = new AllTablesViewModel { ActiveTables = Mapper.Map<List<TableCardViewModel>>(activeTables), InActiveTables = Mapper.Map<List<TableCardViewModel>>(inActiveTables) };

            return View("TableCardList", model);
        }

        [HttpGet]
        public ActionResult AllMenus()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var menus = service.GetAllMenus();

            var model = Mapper.Map<List<MenuViewModel>>(menus);
            model.ForEach(o => 
                {
                    if(o.Restaurant != Guid.Empty)
                    {
                        var rest = service.GetRestaurant(o.Restaurant);
                        if (rest != null)
                            o.AttachedRestaurantName = rest.Name;
                    }
                    
                });
            return View("MenuCardList", model);
        }

        [HttpGet]
        public ActionResult AllRestaurants()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var rests = service.GetAllRestaurants();

            var model = Mapper.Map<List<RestaurantViewModel>>(rests);

            return View("RestaurantCardList", model);
        }

        [HttpGet]
        public ActionResult AllDishes()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var dishes = service.GetAllDishes();

            var model = dishes.GroupBy(o => o.Category).Select(o => new DishListViewModel { Category = o.Key, Dishes = Mapper.Map<List<DishViewModel>>(o.ToList()) }).ToList();
            return View("DishCardList", model);
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
                var table = service.GetTable(tableId);
                if (table != null)
                {
                    table.OrderProcessed = value;
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
        public ActionResult MenuMoreDishes(string menuid)
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);

            var allDishes = service.GetAllDishes();
            var selectedDishes = Mapper.Map<List<SelectedDishViewModel>>(allDishes);

            var currentMenuDishes = service.GetMenu(Guid.Parse(menuid)).DishList.Select(o => o.Id);
            foreach (var dish in selectedDishes)
            {
                if (currentMenuDishes.Contains(dish.Id))
                    dish.Selected = true;
            }

            var model = selectedDishes.GroupBy(o => o.Category).Select(o => new SelectedDishListViewModel { Category = o.Key, Dishes = o.ToList() }).ToList();

            return View(model);
        }

        [HttpPost]
        public JsonResult EditMenuDishes(Guid menuId, List<Guid> allActiveDishes)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var curMenu = service.GetMenu(menuId);
                var allDishes = service.GetAllDishes();
                if (curMenu != null && allDishes != null)
                {
                    if (allActiveDishes != null)
                    {
                        var dishesForCurmenu = allDishes.Where(o => allActiveDishes.Contains(o.Id)).ToList();
                        curMenu.DishList = dishesForCurmenu;
                    }
                    else
                        curMenu.DishList = new List<Dish>();

                    service.UpdateMenu(curMenu);
                }
                else
                    throw new ArgumentNullException("Menu or list of dishes not found!");

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
        public ActionResult EditMenu(Guid menuId, string name, Guid rest)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                if (menuId == Guid.Empty)
                {
                    var menu = new Menu { MenuName = name, DishList = new List<Dish>(), Restaurant = rest };
                    service.CreateNewMenu(menu);
                }
                else
                {
                    var curMenu = service.GetMenu(menuId);
                    curMenu.MenuName = name;
                    curMenu.Restaurant = rest;
                    service.UpdateMenu(curMenu);
                }

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult EditMenu(Guid menuId)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                var menu = service.GetMenu(menuId);
                if (menu != null)
                {
                    var model = Mapper.Map<MenuViewModel>(menu);

                    var rests = service.GetAllRestaurants();
                    if (rests != null && rests.Any())
                    {
                        model.AvailableRests = Mapper.Map<List<RestaurantDropDown>>(rests);
                        if(model.Restaurant != Guid.Empty)
                        {
                            var attachedRest = rests.Where(o => o.Id == model.Restaurant).FirstOrDefault();
                            if(attachedRest != null)
                            {
                                model.AttachedRestaurantName = attachedRest.Name;
                            }
                        }
                    }

                    return View("MenuCardEdditable", model);
                }
                else
                    throw new Exception("Menu not found!");
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EditDish(Dish dish)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                if (dish.Id == Guid.Empty)
                    service.CreateNewDish(dish);
                else
                    service.UpdateDish(dish);
                
                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult EditDish(Guid dishId)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                var dish = service.GetDish(dishId);
                if (dish != null)
                {
                    var model = Mapper.Map<DishViewModel>(dish);

                    var allDishes = service.GetAllDishes();
                    if(allDishes!= null && allDishes.Any())
                    {
                        model.AvailableCategories = allDishes.Where(o => !string.IsNullOrEmpty(o.Category)).Select(o => o.Category).Distinct().ToList();
                    }

                    return View("DishCardEdditable", model);
                }
                else
                    throw new Exception("Dish not found!");
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult EditRest(Guid restId)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                var rest = service.GetRestaurant(restId);
                if (rest != null)
                {
                    var model = Mapper.Map<RestaurantViewModel>(rest);

                    return View("RestaurantCardEdditable", model);
                }
                else
                    throw new Exception("Restaraunt not found!");
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EditRest(Restaurant Rest)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                if (Rest.Id == Guid.Empty)
                    service.CreateRestaurant(Rest);
                else
                    service.UpdateRestaurant(Rest);

                return Json(new { isAuthorized = true, isSuccess = true });
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