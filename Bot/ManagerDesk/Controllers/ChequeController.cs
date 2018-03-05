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
    public class ChequeController : Controller
    {
        [HttpGet]
        public ActionResult AllCheques()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var cheques = service.GetAllCheques().OrderByDescending(o => o.Date);

            var model = Mapper.Map<List<ChequeViewModel>>(cheques).ToList();
            return View("ChequeCardList", model);
        }
    }
}