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
    public class RestaurantController : Controller
    {
        [HttpGet]
        public ActionResult AllRestaurants()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var rests = service.GetAllRestaurants();

            //for(var i = 0; i<100; i++)
            //{
            //    service.CreateNewDish(new Dish { Category= "Закуски", Description ="Заебато пиздато", Id = Guid.NewGuid(), Price =123, Name ="Закуска"+i.ToString(), SlashName = "/disha"+i.ToString()});
            //    service.CreateNewDish(new Dish { Category = "Основные", Description = "Заебато пиздато", Id = Guid.NewGuid(), Price = 123, Name = "Стейк" + i.ToString(), SlashName = "/dishm" + i.ToString() });
            //    service.CreateNewDish(new Dish { Category = "Десерты", Description = "Заебато пиздато", Id = Guid.NewGuid(), Price = 123, Name = "Пирог" + i.ToString(), SlashName = "/dishd" + i.ToString() });
            //    service.CreateNewDish(new Dish { Category = "Напитки", Description = "Заебато пиздато", Id = Guid.NewGuid(), Price = 123, Name = "Колла" + i.ToString(), SlashName = "/dishdr" + i.ToString() });
            //    service.CreateNewDish(new Dish { Category = "Супы", Description = "Заебато пиздато", Id = Guid.NewGuid(), Price = 123, Name = "Борщ" + i.ToString(), SlashName = "/dishso" + i.ToString() });
            //}

            var model = Mapper.Map<List<RestaurantViewModel>>(rests);

            return View("RestaurantCardList", model);
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
        [ValidateAntiForgeryToken]
        public ActionResult EditRest(RestaurantViewModel RestModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var service = ServiceCreator.GetManagerService(User.Identity.Name);
                    var rest = new Restaurant
                    {
                        Address = RestModel.Address,
                        Code = RestModel.Code,
                        Description = RestModel.Description,
                        Id = RestModel.Id,
                        Latitude = RestModel.Latitude,
                        Longitude = RestModel.Longitude,
                        Name = RestModel.Name,
                        Menu = RestModel.Menu,
                        TableCount = RestModel.TableCount
                    };

                    if (rest.Id == Guid.Empty)
                        service.CreateRestaurant(rest);
                    else
                        service.UpdateRestaurant(rest);

                    return Json(new { isAuthorized = true, isSuccess = true });
                }
                else
                    return Json(new { isAuthorized = true, isSuccess = false});

            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }
    }
}