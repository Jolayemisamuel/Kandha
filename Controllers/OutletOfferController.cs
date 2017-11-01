using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Repository;
namespace NibsMVC.Controllers
{
    [Authorize(Roles = "Outlet,Operator")]
    public class OutletOfferController : Controller
    {
        //
        // GET: /OutletOffer/
        OutletRecievedOfferRepository offer = new OutletRecievedOfferRepository();
        public ActionResult Index()
        {
            return View(offer.List());
        }

    }
}
