using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication11.Controllers
{
    public class HomeController : Controller
    {
        private readonly PluggableComponent _component;

        public HomeController(PluggableComponent component)
        {
            _component = component;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}