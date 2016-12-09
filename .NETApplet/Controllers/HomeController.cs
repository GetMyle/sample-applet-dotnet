using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;


namespace NETApplet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var api = new Api(Request.QueryString.AllKeys.Contains("sandbox"));
            var ticket = api.getTicket(Request.QueryString["session"]);

            var query = JObject.Parse(@"{
                sets: ['master'],
                select: ['phrase', 'time'],
                orderBy: [{ $desc: 'time' }]
            }");
            var records = api.Query(ticket, query);

            return View(records);
        }
    }
}
