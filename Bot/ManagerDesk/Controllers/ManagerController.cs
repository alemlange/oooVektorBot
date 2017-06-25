using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ManagerDesk.Helpers;
using ManagerDesk.ViewModels;
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

        public ActionResult AllTables()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Table, TableCardViewModel>());

            var service = ServiceCreator.GetManagerService();
            var tables = service.GetAllTables();

            var model = Mapper.Map<List<TableCardViewModel>>(tables);

            model = model.Select((o, i) => { o.TableNumber = i + 1; return o; }).ToList();
            return View("TableCardList", model);
        }
    }
}