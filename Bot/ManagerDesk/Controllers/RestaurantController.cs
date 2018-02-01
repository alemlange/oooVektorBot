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
                        QueueNumber = RestModel.QueueNumber,
                        Description = RestModel.Description,
                        Id = RestModel.Id,
                        Latitude = RestModel.Latitude,
                        Longitude = RestModel.Longitude,
                        Name = RestModel.Name,
                        Menu = RestModel.Menu,
                        TimeZoneId = RestModel.TimeZoneId
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