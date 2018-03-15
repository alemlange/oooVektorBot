using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Clients;

namespace KarHouseLanding.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewRequest(string orgName, string email, string comment, string type)
        {
            var body = "Запрос от организации " + orgName + ", email: " + email + ", комментарий: " + comment + ", кнопка" + type;
            var client = new EmailClient();
            client.Send("info@karhouse.org", body, "Запрос с кархаус-лэндинга " + DateTime.Now.ToString());

            return Json(new { isAuthorized = true, isSuccess = true });
        }
    }
}