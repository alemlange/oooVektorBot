﻿using System;
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

        [HttpPost]
        public ActionResult CloseBooking(Guid bookId)
        {
            try
            {
                if (bookId != Guid.Empty)
                {
                    var service = ServiceCreator.GetManagerService(User.Identity.Name);
                    service.CloseBooking(bookId);
                    return Json(new { isAuthorized = true, isSuccess = false });

                }
                else
                    return Json(new { isAuthorized = true, isSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message });
            }
        }
    }
}