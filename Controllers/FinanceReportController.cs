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
using WebMatrix.WebData;
using System.Web.Script.Serialization;

namespace NibsMVC.Controllers
{
    public class FinanceReportController : Controller
    {
        // GET: FinanceReport
        NIBSEntities db = new NIBSEntities();
        public ActionResult LedgerDetailsReport()
        {
            List<FinanceReportModel> list = new List<FinanceReportModel>();

            IEnumerable<SelectListItem> LedgerName = (from n in db.tblLedgerMasters select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.LedgerName, Value = n.LedgerMasterId.ToString() });
            ViewBag.LedgerName = new SelectList(LedgerName, "Value", "Text", "Ledger");

            //var items = (from t1 in db.tblLedgerMasters                         
            //             select new {
            //                 t1.LedgerName,
            //                 t1.LedgerMasterId,
            //                 t1.OPBalance,
            //                 t1.Transfer_Type
            //             }).ToList();


            //foreach (var item in items)
            //{
            //    FinanceReportModel model = new FinanceReportModel();

            //    model.LedgerMasterId = item.LedgerMasterId;
            //    model.LedgerName = item.LedgerName;
            //    model.OpeningBalance = Convert.ToDecimal(item.OPBalance);
            //    model.TransferType = item.Transfer_Type;
               
            //    list.Add(model);
            //}

            return View(list);
        }

      [HttpPost]
        public ActionResult LedgerDetailsReport(string LedgerDetails)
        {
            IEnumerable<SelectListItem> LedgerName = (from n in db.tblLedgerMasters select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.LedgerName, Value = n.LedgerMasterId.ToString() });
            ViewBag.LedgerName = new SelectList(LedgerName, "Value", "Text", "Ledger");

            List<FinanceReportModel> list = new List<FinanceReportModel>();
            int lid = Convert.ToInt32(LedgerDetails);

            if (lid != 0)
            {

                var items = (from t1 in db.tblLedgerMasters
                             where t1.LedgerMasterId == lid
                             select new
                             {
                                 t1.LedgerName,
                                 t1.LedgerMasterId,
                                 t1.OPBalance,
                                 t1.Transfer_Type
                             }).SingleOrDefault();

                FinanceReportModel model = new FinanceReportModel();

                model.LedgerMasterId = items.LedgerMasterId;
                model.LedgerName = items.LedgerName;
                model.OpeningBalance = Convert.ToDecimal(items.OPBalance);
                model.TransferType = items.Transfer_Type;

                list.Add(model);
                ViewBag.name = list;
                return View(list);
            }

            return View(list);
        }
    }
}