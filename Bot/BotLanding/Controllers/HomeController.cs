using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BotLanding.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewRequest(string orgName, string email, string comment)
        {

            return Json(new { isAuthorized = true, isSuccess = true });
        }
    }
}