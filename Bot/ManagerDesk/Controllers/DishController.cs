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
    }
}