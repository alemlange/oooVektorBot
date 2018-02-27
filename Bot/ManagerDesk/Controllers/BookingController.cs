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
    public class BookingController : Controller
    {
        [HttpGet]
        public ActionResult AllBookings()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var bookings = service.GetAllBookings();

            var model = Mapper.Map<List<BookingViewModel>>(bookings).ToList();
            return View("BookingCardList", model);
        }
    }
}