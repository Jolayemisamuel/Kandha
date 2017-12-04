using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using System.Drawing;

namespace NibsMVC.Controllers
{
    public class LedgerController : Controller
    {
        // GET: Ledger
        NIBSEntities db = new NIBSEntities();
        LedgerRepository obj = new LedgerRepository();
        public ActionResult Index()
        {
            return View(obj.ShowAllLedgerList());
        }
        [HttpGet]
        public ActionResult AddLedger(int id = 0)
        {
            LedgerModel model = new LedgerModel();

            IEnumerable<SelectListItem> LedgerCat = (from z in db.tblLedgerGroups select z).AsEnumerable().Select(z => new SelectListItem() { Text = z.LedgerGroupName, Value = z.LedgerGroupId.ToString() });

            ViewBag.LedgerCat = new SelectList(LedgerCat, "Value", "Text");
                      

            return View(obj.EditLedgerModel(id));

        }
        [HttpPost]
        public ActionResult AddLedger(LedgerModel model)
        {
            var Data = obj.SaveLedger(model);
            TempData["Error"] = Data;
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Id = 0)
        {
            var data = obj.DeleteLedger(Id);
            TempData["Error"] = data;
            return RedirectToAction("Index");
        }
    }
}