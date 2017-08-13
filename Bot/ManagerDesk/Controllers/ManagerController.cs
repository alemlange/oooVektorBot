using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiteDbService.Helpers;
using ManagerDesk.ViewModels;
using AutoMapper;
using DataModels;

namespace ManagerDesk.Controllers
{
    public class ManagerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AllTables()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Table, TableCardViewModel>());

            var service = ServiceCreator.GetManagerService();
            var tables = service.GetAllTables();

            var model = Mapper.Map<List<TableCardViewModel>>(tables);

            return View("TableCardList", model);
        }

        [HttpGet]
        public ActionResult AllMenus()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Menu, MenuViewModel>());

            var service = ServiceCreator.GetManagerService();
            var menus = service.GetAllMenus();

            var model = Mapper.Map<List<MenuViewModel>>(menus);
            return View("MenuCardList", model);
        }

        [HttpGet]
        public ActionResult AllDishes()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Dish, DishViewModel>());

            var service = ServiceCreator.GetManagerService();
            var dishes = service.GetAllDishes();

            var model = Mapper.Map<List<DishViewModel>>(dishes);
            return View("DishCardList", model);
        }

        [HttpGet]
        public ActionResult MenuMoreDishes(string menuid)
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Dish, SelectedDishViewModel>());
            var service = ServiceCreator.GetManagerService();

            var allDishes = service.GetAllDishes();
            var selectedDishes = Mapper.Map<List<SelectedDishViewModel>>(allDishes);

            var currentMenuDishes = service.GetMenu(Guid.Parse(menuid)).DishList.Select(o => o.Id);
            foreach (var dish in selectedDishes)
            {
                if (currentMenuDishes.Contains(dish.Id))
                    dish.Selected = true;
            }

            return View(selectedDishes);
        }

        [HttpPost]
        public JsonResult SaveMenu(Guid menuId, List<Guid> allActiveDishes)
        {
            try
            {
                var service = ServiceCreator.GetManagerService();
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
        public ActionResult Add(string activeSection)
        {
            try
            {
                //var service = ServiceCreator.GetManagerService();
                //var curMenu = service.GetMenu(menuId);
                //var allDishes = service.GetAllDishes();
                //if (curMenu != null && allDishes != null)
                //{
                //    if (allActiveDishes != null)
                //    {
                //        var dishesForCurmenu = allDishes.Where(o => allActiveDishes.Contains(o.Id)).ToList();
                //        curMenu.DishList = dishesForCurmenu;
                //    }
                //    else
                //        curMenu.DishList = new List<Dish>();

                //    service.UpdateMenu(curMenu);
                //}
                //else
                //    throw new ArgumentNullException("Menu or list of dishes not found!");

                return View("DishCardEdditable");
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult NewDish(string name, string slashName, string price, string description)
        {
            try
            {
                var dish = new Dish { Name = name, SlashName = slashName, Description = description, Price = Convert.ToDecimal(price) };
                var service = ServiceCreator.GetManagerService();
                service.CreateNewDish(dish);

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }
    }
}