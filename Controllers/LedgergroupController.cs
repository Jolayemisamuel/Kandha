using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Controllers
{
    public class LedgergroupController : Controller
    {
        // GET: Ledgergroup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddLegderGroup()
        {
            return View();
        }
    }
}