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
using DataModels.Enums;
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

            var activeBookings = Mapper.Map<List<BookingViewModel>>(bookings.Where(o => o.State == BookingState.Active));
            var closedBookings = Mapper.Map<List<BookingViewModel>>(bookings.Where(o => o.State == BookingState.Closed));

            var model = new BookingListViewModel { ActiveBookings = activeBookings, ClosedBookings = closedBookings };
            return View("BookingCardList", model);
        }

        [HttpGet]
        public ActionResult UpdateBookings()
        {
            try
            {
                var service = ServiceCreator.GetManagerService(User.Identity.Name);
                var bookings = service.GetAllBookings();

                var activeBookings = bookings.Where(o => o.State == BookingState.Active);
                if (activeBookings.Any())
                    return Json(new { isAuthorized = true, isSuccess = true, newBookings = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { isAuthorized = true, isSuccess = true, newBookings = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { isAuthorized = true, isSuccess = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
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