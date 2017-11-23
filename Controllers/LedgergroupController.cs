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
    public class LedgergroupController : Controller
    {
        // GET: Ledgergroup
        NIBSEntities db = new NIBSEntities();
        LedgerRepository obj = new LedgerRepository();
        public ActionResult Index()
        {
            return View(obj.ShowAllLedgerGroupList());
        }
        [HttpGet]
        public ActionResult AddLegderGroup(int id = 0)
        {

            IEnumerable<SelectListItem> LedgerCat = (from z in db.tblLedgerCategories select z).AsEnumerable().Select(z => new SelectListItem() { Text = z.LedgerCategoryName, Value = z.id.ToString() });

            ViewBag.LedgerCat = new SelectList(LedgerCat, "Value", "Text");

            return View(obj.EditLedgerGroupModel(id));

        }
        [HttpPost]
        public ActionResult AddLegderGroup(Ledgergroupmodel model)
        {
            var Data = obj.SaveLedgerGroup(model);
            TempData["Error"] = Data;
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Id = 0)
        {
            var data = obj.DeleteLedgerGroup(Id);
            TempData["Error"] = data;
            return RedirectToAction("Index");
        }
    }
}