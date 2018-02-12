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
    public class MenuController : Controller
    {
        [HttpGet]
        public ActionResult AllMenus()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var menus = service.GetAllMenus();

            var model = Mapper.Map<List<MenuViewModel>>(menus);
            model.ForEach(o =>
            {
                var rest = service.GetRestaurantByMenu(o.Id);

                if (rest != null)
                {
                    o.AttachedRestaurantName = rest.Name;
                }

                if (o.DishList != null && o.DishList.Any())
                {
                    var groupedDishes = o.DishList.GroupBy(d => d.Category).Select(d => new DishListViewModel { Category = d.Key, Dishes = Mapper.Map<List<DishViewModel>>(d.ToList()) }).ToList();
                    o.GroupedDishes = new List<DishListViewModel>();

                    foreach (var category in o.CategoriesSorted)
                    {
                        o.GroupedDishes.AddRange(groupedDishes.Where(g => g.Category == category));
                    }
                }
                else
                {
                    o.GroupedDishes = new List<DishListViewModel>();
                }


            });
            return View("MenuCardList", model);
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

        [HttpGet]
        public ActionResult MenuCatList(Guid menuid)
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var curMenu = service.GetMenu(menuid);

            var model = curMenu.CategoriesSorted;
            return View(model);
        }

        [HttpPost]
        public ActionResult EditCatList(Guid menuId, List<string> sortedCat)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                var categories = new List<string>();
                foreach (var cat in sortedCat)
                {
                    if (cat == "")
                        categories.Add(null);
                    else
                        categories.Add(cat);
                }

                service.UpdateMenuCategories(menuId, categories);
                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
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
                        var dishesForCurmenu = allActiveDishes.Select(o => allDishes.Where(a => a.Id == o).FirstOrDefault()).ToList();
                        //var dishesForCurmenu = allDishes.Where(o => allActiveDishes.Contains(o.Id)).ToList();
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

                        var rest = service.GetRestaurantByMenu(model.Id);

                        if (rest != null)
                        {
                            model.AttachedRestaurantName = rest.Name;
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
        public ActionResult EditMenu(Guid menuId, string name, bool defaultMenu, Guid rest)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var restaurant = service.GetRestaurant(rest);

                if (menuId == Guid.Empty)
                {
                    var menu = new Menu { MenuName = name, DefaultMenu = defaultMenu, DishList = new List<Dish>(), CategoriesSorted = new List<string>() };
                    var newMenu = service.CreateNewMenu(menu);
                    restaurant.Menu = newMenu;
                    service.UpdateRestaurant(restaurant);
                }
                else
                {
                    var curMenu = service.GetMenu(menuId);
                    curMenu.MenuName = name;
                    curMenu.DefaultMenu = defaultMenu;
                    service.UpdateMenu(curMenu);
                    restaurant.Menu = curMenu.Id;
                    service.UpdateRestaurant(restaurant);
                }

                if (defaultMenu == true)
                    service.SetDefaultMenu(menuId);

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }
    }
}