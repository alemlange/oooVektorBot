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
    public class DishController : Controller
    {
        [HttpGet]
        public ActionResult AllDishes()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var dishes = service.GetAllDishes();

            var model = dishes.GroupBy(o => o.Category).Select(o => new DishListViewModel { Category = o.Key, Dishes = Mapper.Map<List<DishViewModel>>(o.ToList()) }).ToList();
            return View("DishCardList", model);
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
                    if (allDishes != null && allDishes.Any())
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDish(Dish dish)
        {
            try
            {
                if (string.IsNullOrEmpty(dish.ShortName))
                    dish.ShortName = dish.Name;
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
        public ActionResult EditMods(string dishid)
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);

            var allMods = service.GetAllModificators();
            var selectedMods = Mapper.Map<List<SelectedModViewModel>>(allMods);

            var currentDishMods = service.GetDish(Guid.Parse(dishid)).ModIds;

            if (currentDishMods == null)
                currentDishMods = new List<int>();

            foreach (var mod in selectedMods)
            {
                if (currentDishMods.Contains(mod.Id))
                    mod.Selected = true;
            }

            return View(selectedMods);
        }

        [HttpPost]
        public ActionResult EditMods(Guid dishId, List<int> allActiveMods)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var curDish = service.GetDish(dishId);
                var allMods = service.GetAllModificators();
                if (curDish != null && allMods != null)
                {
                    if (allActiveMods != null)
                    {
                        var modsForCurDish = allMods.Where(o => allActiveMods.Contains(o.Id)).Select(o => o.Id).ToList();
                        curDish.ModIds = modsForCurDish;
                    }
                    else
                        curDish.ModIds = new List<int>();

                    service.UpdateDish(curDish);
                }
                else
                    throw new ArgumentNullException("Dish or list of dishes not found!");

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }
    }
}