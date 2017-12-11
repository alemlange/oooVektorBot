using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiteDbService.Helpers;
using ManagerDesk.ViewModels;
using ManagerDesk.ViewModels.Enums;
using AutoMapper;
using ManagerDesk.Services;
using DataModels;
using System.IO;

namespace ManagerDesk.Controllers
{
    [Authorize]
    public class ConfigurationController : Controller
    {
        [HttpGet]
        public ActionResult Configuration()
        {
            var regService = new RegistrationService();
            var config = regService.FindConfiguration(User.Identity.Name);

            var model = new ConfigViewModel { Config = config };

            return View(model);
        }

        [HttpPost]
        public ActionResult Configuration(ConfigViewModel model)
        {
            var regService = new RegistrationService();

            if (model.Image != null && model.Image.ContentLength > 0)
            {
                var fileName = Path.GetFileName(model.Image.FileName);
                var path = Path.Combine(Server.MapPath("~/Assets/Imgs/UserPics/"), fileName);
                model.Image.SaveAs(path);

                model.Config.ProfilePicturePath = "~/Assets/Imgs/UserPics/" + fileName;
            }

            regService.UpdateConfiguration(model.Config);

            return RedirectToAction("Index", "Manager");
        }
    }
}