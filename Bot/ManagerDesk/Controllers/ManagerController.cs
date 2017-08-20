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

            model.Add(new TableCardViewModel { TableNumber=13, ChatId=121123, Id = Guid.NewGuid(), State =DataModels.Enums.SessionState.Sitted,Orders =new List<OrderedDish> { new OrderedDish { DishFromMenu=new Dish { Name="Борщец",Price=123} } } });

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
        public JsonResult EditMenuDishes(Guid menuId, List<Guid> allActiveDishes)
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
        public ActionResult Add(CardTypes activeSection)
        {
            try
            {
                switch (activeSection)
                {
                    case CardTypes.Dish:
                        {
                            return View("DishCardEdditable", new DishViewModel());
                        }
                    case CardTypes.Menu:
                        {
                            return View("MenuCardEdditable", new MenuViewModel());
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
        public ActionResult EditMenu(Guid menuId, string name)
        {
            try
            {
                var service = ServiceCreator.GetManagerService();
                if (menuId == Guid.Empty)
                {
                    var menu = new Menu { MenuName = name, DishList = new List<Dish>() };
                    service.CreateNewMenu(menu);
                }
                else
                {
                    service.UpdateMenuInfo(new Menu { Id = menuId, MenuName = name });
                }

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EditDish(Guid dishId, string name, string slashName, string pictureUrl, string price, string description)
        {
            try
            {
                var service = ServiceCreator.GetManagerService();
                if (dishId == Guid.Empty)
                {
                    var dish = new Dish { Name = name, SlashName = slashName, PictureUrl = pictureUrl, Description = description, Price = Convert.ToDecimal(price) };
                    service.CreateNewDish(dish);
                }
                else
                {
                    service.UpdateDish(new Dish { Id = dishId, Name = name, SlashName = slashName, PictureUrl = pictureUrl, Description = description, Price = Convert.ToDecimal(price) });
                }

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
                var service = ServiceCreator.GetManagerService();

                var dish = service.GetDish(dishId);
                if (dish != null)
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<Dish, DishViewModel>());
                    var model = Mapper.Map<DishViewModel>(dish);

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

        [HttpPost]
        public ActionResult DeleteItem(Guid itemId, CardTypes itemType)
        {
            try
            {
                var service = ServiceCreator.GetManagerService();

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
                        default:
                            break;
                    }
                    
                }
                else
                    throw new Exception("Dish id not specified!");
                return Json(new { isAuthorized = true, isSuccess = true });

            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

    }
}