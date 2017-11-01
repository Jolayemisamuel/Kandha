using NibsMVC.EDMX;
using NibsMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class ServicChargesController : Controller
    {
        //
        // GET: /ServicCharges/
        NIBSEntities db = new NIBSEntities();
        public ActionResult Index()
        {
            var result = (from p in db.tblServiceCharges select p).ToList();
            List<ServiceChargesModel> list = new List<ServiceChargesModel>();
            foreach (var item in result)
            {
                ServiceChargesModel model = new ServiceChargesModel();
                model.ServiceId = item.ServicId;
                model.ServicName = item.ServiceName;
                model.ServiceCharges = item.Charges;
                list.Add(model);
            }
            return View(list);
        }

        public ActionResult Create(int id = 0)
        {
            if (id > 0)
            {
                var data = (from p in db.tblServiceCharges where p.ServicId == id select p).SingleOrDefault();
                ServiceChargesModel model = new ServiceChargesModel();
                model.ServiceId = data.ServicId;
                model.ServicName = data.ServiceName;
                model.ServiceCharges = data.Charges;
                return View(model);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(ServiceChargesModel model)
        {
            //var result = (from p in db.tblServiceCharges where p.ServiceName == "T" && p.ServiceName == "R" && p.ServiceName == "H" select p).SingleOrDefault();
            //if (result != null)
            //{
            //    try
            //    {
            //        tblServiceCharge tb = new tblServiceCharge();
            //        if (model.ServiceId > 0)
            //        {
            //            tb = (from p in db.tblServiceCharges where p.ServicId == model.ServiceId select p).SingleOrDefault();
            //        }
            //        tb.ServiceName = model.ServicName;
            //        tb.Charges = model.ServiceCharges;
            //        if (model.ServiceId > 0)
            //        {
            //            db.SaveChanges();
            //            TempData["servicecharg"] = "Record Edit Successfully..!";
            //            return RedirectToAction("Index", "ServicCharges");
            //        }
            //        else
            //        {
            //            db.tblServiceCharges.Add(tb);
            //            db.SaveChanges();
            //            TempData["servicecharg"] = "Record Insert Successfully..!";
            //            return RedirectToAction("Index", "ServicCharges");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        TempData["servicecharg"] = ex.Message;
            //        return RedirectToAction("Index", "ServicCharges");
            //    }
            //}
            //else
            //{
            //    TempData["servicCreate"] = "This Charges is already Exist..First Delete From Service Report..";
            //    return RedirectToAction("Create");
            //}
            try
            {
                tblServiceCharge tb = new tblServiceCharge();
                var result = (from p in db.tblServiceCharges where p.ServiceName == model.ServicName select p).SingleOrDefault();
                if (result != null)
                {
                    result.ServiceName = model.ServicName;
                    result.Charges = model.ServiceCharges;
                    db.SaveChanges();
                    TempData["servicecharg"] = "Record Edit Successfully..!";
                    return RedirectToAction("Index", "ServicCharges");
                }
                else
                {
                    tb.ServiceName = model.ServicName;
                    tb.Charges = model.ServiceCharges;
                    db.tblServiceCharges.Add(tb);
                    db.SaveChanges();
                    TempData["servicecharg"] = "Record Insert Successfully..!";
                    return RedirectToAction("Index", "ServicCharges");
                }
            }
            catch (Exception ex)
            {
                TempData["servicCreate"] = ex.Message;
                return RedirectToAction("Create");
            }
        }

        public ActionResult DeleteService(int id = 0)
        {
            try
            {
                var result = (from p in db.tblServiceCharges where p.ServicId == id select p).FirstOrDefault();
                db.tblServiceCharges.Remove(result);
                db.SaveChanges();
                TempData["servicecharg"] = "Record Delete Successfully..!";
            }
            catch (Exception ex)
            {
                TempData["servicecharg"] = ex.Message;
            }
            return RedirectToAction("Index", "ServicCharges");
        }

    }
}
