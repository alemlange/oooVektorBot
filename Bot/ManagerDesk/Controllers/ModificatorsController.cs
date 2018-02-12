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
    public class ModificatorsController : Controller
    {
        [HttpGet]
        public ActionResult AllModificators()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var mods = Mapper.Map<List<ModificatorViewModel>>(service.GetAllModificators());

            return View("ModificatorCardList", mods);
        }

        [HttpGet]
        public ActionResult EditModificator(int modId)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                var mod = service.GetModificator(modId);
                if (mod != null)
                {
                    var model = Mapper.Map<ModificatorViewModel>(mod);

                    return View("ModificatorsCardEdditable", model);
                }
                else
                    throw new Exception("Modificator not found!");
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditModificator(Modificator mod)
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);

                if (mod.Id == 0)
                    service.CreateNewModificator(mod);
                else
                    service.UpdateModificator(mod);

                return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }
    }
}