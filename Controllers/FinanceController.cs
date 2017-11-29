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
//using System.Data.Entity.Validation;

namespace NibsMVC.Controllers
{
    public class FinanceController : Controller
    {
        // GET: Finance
        NIBSEntities db = new NIBSEntities();
        LedgerRepository obj = new LedgerRepository();
        public ActionResult Receipt()
        {
            Receipt model = new Receipt();

            if ( db.Voucher_Entry_Credit.Select(p=>p.voucher_type).ToString()=="Bank Receipt" && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {

                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "Bank Receipt") select n.record_no).SingleOrDefault();

                int recno = Convert.ToInt32(id.Max()) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }
        [HttpGet]
        public ActionResult CashReceipt()
        {
            CashReceipt model = new CashReceipt();

            if (db.Voucher_Entry_Credit.Select(p => p.voucher_type).ToString() == "Cash Receipt" && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "Cash Receipt") select n.record_no).SingleOrDefault();

                int recno = Convert.ToInt32(id.Max()) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }
        public ActionResult UpdateReceipt(Receipt model)
        {

            try
            {

                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                    FinYear = CurYear + "-" + NexYear;
                else
                    FinYear = PreYear + "-" + CurYear;

                //int Pid = (from p in db.tblPurchaseOrderMasters where p.OutletId == OutletId select p.PurchaseOrderId).Max();
                for (int i = 0; i < model.LedgerAccId.Length; i++)
                {
                    Voucher_Entry_Credit tb = new Voucher_Entry_Credit();
                    Voucher_Entry_Debit tb1 = new Voucher_Entry_Debit();
                    tb.voucher_no = model.VoucherNo;
                    tb.Voucher_date = model.Date;
                    tb1.voucher_no = model.VoucherNo;
                    tb1.voucher_date = model.Date;
                    tb.record_no = model.RecordNo.ToString();
                    tb1.record_no = model.RecordNo.ToString();
                    tb.record_date = model.Date;
                    tb1.record_date = model.Date;
                    tb.voucher_sno = i + 1;
                    tb1.voucher_sno = i + 1;
                    tb.voucher_tb = "To";
                    tb1.voucher_tb = "By";
                    tb.voucher_type = "Bank Receipt";
                    tb1.voucher_type = "Bank Receipt";
                    tb.voucher_year = FinYear;
                    tb1.voucher_year = FinYear;
                    tb.from_form_name = "Bank Receipt";
                    tb1.from_form_name = "Bank Receipt";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb1.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "Bank Receipt";
                    tb1.account_type = "Bank Receipt";

                    string name = model.LedgerAccId[i];
                    var id = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == name) select n.LedgerMasterId).SingleOrDefault();

                    tb.from_form_id = 0;
                    tb1.from_form_id = 0;
                    tb.voucher_ledger_accout_id = Convert.ToInt32(id);
                    tb.voucher_cr_amount = model.CreditAmount[i];
                    tb.check_no = model.ChequeNo[i];
                    tb.check_date = model.ChequeDate[i];
                    tb.voucher_narration = model.CrNarration[i];

                    string accountname = model.ByAccount[i];
                    var idl = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == accountname) select n.LedgerMasterId).SingleOrDefault();

                    tb1.voucher_ledger_accout_id = Convert.ToInt32(idl);
                    tb1.voucher_dbt_amount = model.DrAmount[i];
                    tb1.voucher_narration = model.DrNarration[i];
                    tb.create_date = DateTime.Now;
                    tb1.create_date = DateTime.Now;
                    db.Voucher_Entry_Credit.Add(tb);
                    db.Voucher_Entry_Debit.Add(tb1);
                    db.SaveChanges();

                }

                TempData["Perror"] = "Inserted Successfully !";
                return View("Receipt");
            }

            catch (Exception e)
            {

               
                return View(e);
            }
            //TempData["Perror"] = "Please Enter the Details Correctlly !";
            //return RedirectToAction("Receipt");

            
        }
        public ActionResult CashReceiptPost(CashReceipt model)
        {

            try
            {

                int CurrentYear = DateTime.Today.Year;
                int PreviousYear = DateTime.Today.Year - 1;
                int NextYear = DateTime.Today.Year + 1;
                string PreYear = PreviousYear.ToString();
                string NexYear = NextYear.ToString();
                string CurYear = CurrentYear.ToString();
                string FinYear = null;

                if (DateTime.Today.Month > 3)
                    FinYear = CurYear + "-" + NexYear;
                else
                    FinYear = PreYear + "-" + CurYear;

                //int Pid = (from p in db.tblPurchaseOrderMasters where p.OutletId == OutletId select p.PurchaseOrderId).Max();
                for (int i = 0; i < model.LedgerAccId.Length; i++)
                {
                    Voucher_Entry_Credit tb = new Voucher_Entry_Credit();
                    Voucher_Entry_Debit tb1 = new Voucher_Entry_Debit();
                    tb.voucher_no = model.VoucherNo;
                    tb.Voucher_date = model.Date;
                    tb1.voucher_no = model.VoucherNo;
                    tb1.voucher_date = model.Date;
                    tb.record_no = model.RecordNo.ToString();
                    tb1.record_no = model.RecordNo.ToString();
                    tb.record_date = model.Date;
                    tb1.record_date = model.Date;
                    tb.voucher_sno = i + 1;
                    tb1.voucher_sno = i + 1;
                    tb.voucher_tb = "To";
                    tb1.voucher_tb = "By";
                    tb.voucher_type = "CashReceipt";
                    tb1.voucher_type = "CashReceipt";
                    tb.voucher_year = FinYear;
                    tb1.voucher_year = FinYear;
                    tb.from_form_name = "CashReceipt";
                    tb1.from_form_name = "CashReceipt";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb1.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "Cash Receipt";
                    tb1.account_type = "Cash Receipt";

                    string name = model.LedgerAccId[i];
                    var id = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == name) select n.LedgerMasterId).SingleOrDefault();

                    tb.from_form_id = id;
                    tb1.from_form_id = id;
                    tb.voucher_ledger_accout_id = Convert.ToInt32(id);
                    tb.voucher_cr_amount = model.CreditAmount[i];
                    
                    tb.voucher_narration = model.CrNarration[i];

                    string accountname = model.ByAccount[i];
                    var idl = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == accountname) select n.LedgerMasterId).SingleOrDefault();

                    tb1.voucher_ledger_accout_id = Convert.ToInt32(idl);
                    tb1.voucher_dbt_amount = model.DrAmount[i];
                    tb1.voucher_narration = model.DrNarration[i];
                    tb.create_date = DateTime.Now;
                    tb1.create_date = DateTime.Now;
                    db.Voucher_Entry_Credit.Add(tb);
                    db.Voucher_Entry_Debit.Add(tb1);
                    db.SaveChanges();

                }

                TempData["Perror"] = "Inserted Successfully !";
                return View("CashReceipt");
            }

            catch(Exception ex)
            {
                return View(ex);

                //TempData["Perror"] = "Please Enter the Details Correctlly !";
                //return RedirectToAction("CashReceipt");
            }
        }
        //public ActionResult UpdateReceiptDebit(Receipt model)
        //{
        //    try
        //    {
                
        //        for (int i = 0; i < model.ByAccount.Length; i++)
        //        {
        //            Voucher_Entry_Debit tb = new Voucher_Entry_Debit();
        //            tb.voucher_no = model.VoucherNo;
        //            tb.voucher_date = model.Date;
        //            tb.record_no = model.RecordNo.ToString();
        //            tb.voucher_ledger_accout_id = Convert.ToInt32(model.LedgerAccId[i]);
        //            tb.voucher_dbt_amount = model.DrAmount[i];                    
        //            tb.voucher_narration = model.DrNarration[i];

        //            db.Voucher_Entry_Debit.Add(tb);
        //            db.SaveChanges();

        //        }

        //        TempData["Perror"] = "Inserted Successfully !";
                
        //    }
                       
        //    catch
        //    {
        //        TempData["Perror"] = "Try Again !";
               
        //    }
        //    return RedirectToAction("Receipt");
        //}
        public JsonResult getCategories()
        {

            var Data = (from p in db.tblLedgerMasters
                        select new
                        {
                            Key = p.LedgerMasterId,
                            Name = p.LedgerName
                        }).GroupBy(a => a.Name).ToList();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var key in Data)
            {
                list.Add(new SelectListItem { Text = Convert.ToString(key.FirstOrDefault().Name), Value = Convert.ToString(key.Key) });

            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        public JsonResult getByAcc()
        {

            var Data = (from p in db.tblLedgerMasters
                        where p.LedgerGroup == 13
                        select new
                        {
                            Key = p.LedgerMasterId,
                            Name = p.LedgerName
                        }).GroupBy(a => a.Name).ToList();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var key in Data)
            {
                list.Add(new SelectListItem { Text = Convert.ToString(key.FirstOrDefault().Name), Value = Convert.ToString(key.Key) });

            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        public JsonResult getByAccCash()
        {

            var Data = (from p in db.tblLedgerMasters
                        where p.LedgerGroup == 12
                        select new
                        {
                            Key = p.LedgerMasterId,
                            Name = p.LedgerName
                        }).GroupBy(a => a.Name).ToList();
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var key in Data)
            {
                list.Add(new SelectListItem { Text = Convert.ToString(key.FirstOrDefault().Name), Value = Convert.ToString(key.Key) });

            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
    }
}