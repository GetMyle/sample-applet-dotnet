using System;
using System.Linq;
using System.Web.Mvc;


namespace NETApplet.Controllers
{
    public class HookController : Controller
    {
        public ActionResult Index(Record record)
        {
            var api = new Api(Request.QueryString.AllKeys.Contains("sandbox"));
            var ticket = api.getTicket(Request.QueryString["session"]);

            return null;
        }
    }
}
