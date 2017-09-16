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

            var model = Mapper.Map<ConfigViewModel>(config);

            return View(model);
        }

    }
}