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
    public class FeedbackController : Controller
    {
        [HttpGet]
        public ActionResult AllFeedbacks()
        {
            var service = ServiceCreator.GetManagerService(User.Identity.Name);
            var feedbacks = service.GetAllFeedbacks();

            var model = Mapper.Map<List<FeedbackViewModel>>(feedbacks).ToList();
            return View("FeedbackCardList", model);
        }
    }
}