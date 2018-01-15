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
    public class DispatchController : Controller
    {
        [HttpGet]
        public ActionResult AllDispatches()
        {
            var regService = ServiceCreator.GetRegistrationService();
            var account = regService.FindAccount(User.Identity.Name);

            if(account != null)
            {
                var service = ServiceCreator.GetDispatchesService();
                var activeDispatches = service.GetActiveDispatches(account.Id);
                var oldDispatches = service.GetInActiveDispatches(account.Id);

                var model = new DispatchListViewModel();
                model.ActiveDispatches = Mapper.Map<List<DispatchViewModel>>(activeDispatches);
                model.InActiveDispatches = Mapper.Map<List<DispatchViewModel>>(oldDispatches);

                return View("DispatchCardList", model);
            }
            else
            {
                var model = new DispatchListViewModel();
                model.ActiveDispatches = new List<DispatchViewModel>();
                model.InActiveDispatches = new List<DispatchViewModel>();

                return View("DispatchCardList", model);
            }
        }

        //[HttpGet]
        //public ActionResult EditDish(Guid dishId)
        //{
        //    try
        //    {
        //        var service = ServiceCreator.GetManagerService(User.Identity.Name);

        //        var dish = service.GetDish(dishId);
        //        if (dish != null)
        //        {
        //            var model = Mapper.Map<DishViewModel>(dish);

        //            var allDishes = service.GetAllDishes();
        //            if (allDishes != null && allDishes.Any())
        //            {
        //                model.AvailableCategories = allDishes.Where(o => !string.IsNullOrEmpty(o.Category)).Select(o => o.Category).Distinct().ToList();
        //            }

        //            return View("DishCardEdditable", model);
        //        }
        //        else
        //            throw new Exception("Dish not found!");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDisp(DispatchViewModel dispatchModel)
        {
            try
            {
                var regService = new RegistrationService() ;
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var dispService = ServiceCreator.GetDispatchesService();

                var config = regService.FindConfiguration(User.Identity.Name);
                var dispId = Guid.NewGuid();
                var disp = new Dispatch { Host = config.TelegramBotLocation, Id = dispId, Message = dispatchModel.Message, Name = dispatchModel.Name, AccountId = config.AccountId, Done = false };
                dispService.CreateDispatch(disp);

                var allTables = service.GetInActiveTables();
                var allChats = new List<long>();
                allTables.ForEach(o => { if (!allChats.Contains(o.ChatId)) { allChats.Add(o.ChatId); } });
                foreach (var chat in allChats)
                {
                    var dispMes = new DispatchMessage { DispatchId = dispId, ChatId = chat, Id = Guid.NewGuid(), Send = false };
                    dispService.CreateDispatchMessage(dispMes);
                }

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }
    }
}