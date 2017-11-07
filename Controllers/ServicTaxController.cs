using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Models;
using NibsMVC.EDMX;


namespace NibsMVC.Controllers
{
    //[Authorize(Roles = "admin")]
    public class ServicTaxController : Controller
    {
        //
        // GET: /ServicTax/
        NIBSEntities db = new NIBSEntities();
        public ActionResult Index()
        {
            List<ServicTaxModel> list = new List<ServicTaxModel>();
            var data = (from p in db.tbl_ServiceTax select p);
            foreach (var item in data)
            {
                ServicTaxModel model = new ServicTaxModel();
                model.ServicTaxId = item.ServiceTaxId;
                model.ServicTax = item.ServiceCharge;
                list.Add(model);
            }
            return View(list);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(ServicTaxModel model)
        {
            tbl_ServiceTax tb = new tbl_ServiceTax();

            try
            {
                var data = (from p in db.tbl_ServiceTax select p).SingleOrDefault();
                if (data == null)
                {
                    tb.ServiceCharge = model.ServicTax;
                    db.tbl_ServiceTax.Add(tb);
                    db.SaveChanges();
                }
                else
                {
                    db.tbl_ServiceTax.Remove(data);
                    db.SaveChanges();
                    tb.ServiceCharge = model.ServicTax;
                    db.tbl_ServiceTax.Add(tb);
                    db.SaveChanges();
                }
                TempData["servicetax"] = "Insert Record Successfully..";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["servicetax"] = ex.Message;
                return RedirectToAction("Create");
            }
        }
    }
}
