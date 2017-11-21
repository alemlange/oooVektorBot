using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clients;

namespace BotLanding.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewRequest(string orgName, string email, string comment)
        {
            var body = "Запрос от организации " + orgName + "email: " + email + " комментарий: " + comment;
            var client = new EmailClient();
            client.Send("info@karhouse.org", body, "запрос с лэндинга");

            return Json(new { isAuthorized = true, isSuccess = true });
        }
    }
}