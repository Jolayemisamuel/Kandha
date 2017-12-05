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

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("Bank Receipt"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "Bank Receipt") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

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

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("CashReceipt"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "CashReceipt") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }
        public ActionResult Payment()
        {
            Payment model = new Payment();

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("Payment"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "Payment") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }
        public ActionResult CashPayment()
        {
            CashPayement model = new CashPayement();

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("Cash Payment"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "Cash Payment") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }

        public ActionResult DebitNote()
        {
            DebitNote model = new DebitNote();

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("DebitNote"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "DebitNote") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }
        public ActionResult CreditNote()
        {
            CreditNote model = new CreditNote();

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("CreditNote"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "CreditNote") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }

        public ActionResult Contra()
        {
            Contra model = new Contra();

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("Contra"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "Contra") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

                model.RecordNo = recno.ToString();
            }
            else
            {
                model.RecordNo = "1";
            }

            return View(model);
        }

        public ActionResult Journal()
        {
            Journal model = new Journal();

            bool Exists = db.Voucher_Entry_Credit.Any(c => c.voucher_type.Equals("Journal"));

            if (Exists == true && db.Voucher_Entry_Credit.Select(p => p.record_no).Count() > 0)
            {
                var id = (from n in db.Voucher_Entry_Credit.Where(x => x.voucher_type == "Journal") select n.record_no).Max();

                int recno = Convert.ToInt32(id) + 1;

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
                TempData["Perror"] = "Please Enter the Details Correctlly !";
                return RedirectToAction("Receipt");

                //return View(e);
            }



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

                    tb.from_form_id = 0;
                    tb1.from_form_id = 0;
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

            catch (Exception ex)
            {
                //return View(ex);

                TempData["Perror"] = "Please Enter the Details Correctlly !";
                return RedirectToAction("CashReceipt");
            }
        }
        [HttpPost]
        public ActionResult Payment(Payment model)
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


                for (int i = 0; i < model.ByAccount.Length; i++)
                {

                    Voucher_Entry_Debit tb1 = new Voucher_Entry_Debit();

                    tb1.voucher_no = model.VoucherNo;
                    tb1.voucher_date = model.Date;

                    tb1.record_no = model.RecordNo.ToString();

                    tb1.record_date = model.Date;

                    tb1.voucher_sno = i + 1;

                    tb1.voucher_tb = "By";

                    tb1.voucher_type = "Payment";

                    tb1.voucher_year = FinYear;

                    tb1.from_form_name = "Payment";

                    tb1.userid = WebSecurity.CurrentUserId;

                    tb1.account_type = "Payment";


                    tb1.from_form_id = 0;

                    tb1.check_no = model.ChequeNo[i];
                    tb1.check_date = model.ChequeDate[i];


                    string accountname = model.ByAccount[i];
                    var idl = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == accountname) select n.LedgerMasterId).SingleOrDefault();

                    tb1.voucher_ledger_accout_id = Convert.ToInt32(idl);
                    tb1.voucher_dbt_amount = model.DrAmount[i];
                    tb1.voucher_narration = model.DrNarration[i];
                    tb1.purchase_id = model.PurchaseId[i];
                    tb1.create_date = DateTime.Now;

                    db.Voucher_Entry_Debit.Add(tb1);
                    db.SaveChanges();

                }
                for (int j = 0; j < model.LedgerAccId.Length; j++)
                {
                    Voucher_Entry_Credit tb = new Voucher_Entry_Credit();

                    tb.voucher_no = model.VoucherNo;
                    tb.Voucher_date = model.Date;
                    tb.record_no = model.RecordNo.ToString();
                    tb.record_date = model.Date;
                    tb.voucher_sno = j + 1;
                    tb.voucher_tb = "To";
                    tb.voucher_type = "Payment";
                    tb.voucher_year = FinYear;
                    tb.from_form_name = "Payment";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "Payment";
                    tb.from_form_id = 0;
                    string name = model.LedgerAccId[j];
                    var id = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == name) select n.LedgerMasterId).SingleOrDefault();
                    tb.voucher_ledger_accout_id = Convert.ToInt32(id);
                    tb.voucher_cr_amount = model.CreditAmount[j];
                    tb.voucher_narration = model.CrNarration[j];
                    tb.purchase_id = model.PurchaseId[j];
                    tb.create_date = DateTime.Now;

                    db.Voucher_Entry_Credit.Add(tb);
                    db.SaveChanges();
                }

                TempData["Perror"] = "Inserted Successfully !";
                return View("Payment");

            }
            catch (Exception e)
            {
                TempData["Perror"] = "Please Enter the Details Correctlly !";
                return RedirectToAction("Payment");

                //return View(e);
            }



        }

        public ActionResult CashPaymentBill(CashPayement model)
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
                    tb.voucher_type = "Cash Payment";
                    tb1.voucher_type = "Cash Payment";
                    tb.voucher_year = FinYear;
                    tb1.voucher_year = FinYear;
                    tb.from_form_name = "Cash Payment";
                    tb1.from_form_name = "Cash Payment";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb1.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "Cash Payment";
                    tb1.account_type = "Cash Payment";

                    string name = model.LedgerAccId[i];
                    var id = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == name) select n.LedgerMasterId).SingleOrDefault();

                    tb.from_form_id = 0;
                    tb1.from_form_id = 0;

                    tb.voucher_ledger_accout_id = Convert.ToInt32(id);
                    tb.voucher_cr_amount = model.CreditAmount[i];
                    tb.purchase_id = model.PurchaseId[i];

                    tb.voucher_narration = model.CrNarration[i];

                    string accountname = model.ByAccount[i];
                    var idl = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == accountname) select n.LedgerMasterId).SingleOrDefault();

                    tb1.voucher_ledger_accout_id = Convert.ToInt32(idl);
                    tb1.voucher_dbt_amount = model.DrAmount[i];
                    tb1.voucher_narration = model.DrNarration[i];
                    tb.create_date = DateTime.Now;
                    tb1.create_date = DateTime.Now;
                    tb1.purchase_id = model.PurchaseId[i];
                    db.Voucher_Entry_Credit.Add(tb);
                    db.Voucher_Entry_Debit.Add(tb1);
                    db.SaveChanges();

                }

                TempData["Perror"] = "Inserted Successfully !";
                return View("CashPayment");
            }

            catch (Exception ex)
            {
                //return View(ex);

                TempData["Perror"] = "Please Enter the Details Correctlly !";
                return RedirectToAction("CashPayment");
            }
        }

        public ActionResult DebitNoteBill(DebitNote model)
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


                for (int i = 0; i < model.ByAccount.Length; i++)
                {

                    Voucher_Entry_Debit tb1 = new Voucher_Entry_Debit();

                    tb1.voucher_no = model.VoucherNo;
                    tb1.voucher_date = model.Date;

                    tb1.record_no = model.RecordNo.ToString();

                    tb1.record_date = model.Date;

                    tb1.voucher_sno = i + 1;

                    tb1.voucher_tb = "By";

                    tb1.voucher_type = "DebitNote";

                    tb1.voucher_year = FinYear;

                    tb1.from_form_name = "DebitNote";

                    tb1.userid = WebSecurity.CurrentUserId;

                    tb1.account_type = "DebitNote";


                    tb1.from_form_id = 0;

                    tb1.check_no = model.ChequeNo[i];
                    tb1.check_date = model.ChequeDate[i];


                    string accountname = model.ByAccount[i];
                    var idl = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == accountname) select n.LedgerMasterId).SingleOrDefault();

                    tb1.voucher_ledger_accout_id = Convert.ToInt32(idl);
                    tb1.voucher_dbt_amount = model.DrAmount[i];
                    tb1.voucher_narration = model.DrNarration[i];
                    
                    tb1.create_date = DateTime.Now;

                    db.Voucher_Entry_Debit.Add(tb1);
                    db.SaveChanges();

                }
                for (int j = 0; j < model.LedgerAccId.Length; j++)
                {
                    Voucher_Entry_Credit tb = new Voucher_Entry_Credit();

                    tb.voucher_no = model.VoucherNo;
                    tb.Voucher_date = model.Date;
                    tb.record_no = model.RecordNo.ToString();
                    tb.record_date = model.Date;
                    tb.voucher_sno = j + 1;
                    tb.voucher_tb = "To";
                    tb.voucher_type = "DebitNote";
                    tb.voucher_year = FinYear;
                    tb.from_form_name = "DebitNote";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "DebitNote";
                    tb.from_form_id = 0;
                    string name = model.LedgerAccId[j];
                    var id = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == name) select n.LedgerMasterId).SingleOrDefault();
                    tb.voucher_ledger_accout_id = Convert.ToInt32(id);
                    tb.voucher_cr_amount = model.CreditAmount[j];
                    tb.voucher_narration = model.CrNarration[j];
                   
                    tb.create_date = DateTime.Now;

                    db.Voucher_Entry_Credit.Add(tb);
                    db.SaveChanges();
                }

                TempData["Perror"] = "Inserted Successfully !";
                return View("DebitNote");

            }
            catch (Exception e)
            {
                TempData["Perror"] = "Please Enter the Details Correctlly !";
                return RedirectToAction("DebitNote");

                //return View(e);
            }
            
        }

        public ActionResult CreditNoteBill(CreditNote model)
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
                    tb.voucher_type = "CreditNote";
                    tb1.voucher_type = "CreditNote";
                    tb.voucher_year = FinYear;
                    tb1.voucher_year = FinYear;
                    tb.from_form_name = "CreditNote";
                    tb1.from_form_name = "CreditNote";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb1.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "CreditNote";
                    tb1.account_type = "CreditNote";

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
                return View("CreditNote");
            }

            catch (Exception e)
            {
                TempData["Perror"] = "Please Enter the Details Correctly !";
                return RedirectToAction("CreditNote");

                //return View(e);
            }
            
        }

        public ActionResult ContraBill(Contra model)
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
                    tb.voucher_type = "Contra";
                    tb1.voucher_type = "Contra";
                    tb.voucher_year = FinYear;
                    tb1.voucher_year = FinYear;
                    tb.from_form_name = "Contra";
                    tb1.from_form_name = "Contra";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb1.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "Contra";
                    tb1.account_type = "Contra";
                    tb.Refrenceno = model.Refrenceno[i];
                    tb1.Refrenceno = model.Refrenceno[i];

                    string name = model.LedgerAccId[i];
                    var id = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == name) select n.LedgerMasterId).SingleOrDefault();

                    tb.from_form_id = 0;
                    tb1.from_form_id = 0;

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
                return View("Contra");
            }

            catch (Exception ex)
            {
                //return View(ex);

                TempData["Perror"] = "Please Enter the Details Correctlly !";
                return RedirectToAction("Contra");
            }
        }

        public ActionResult JournalBill(Journal model)
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


                for (int i = 0; i < model.ByAccount.Length; i++)
                {

                    Voucher_Entry_Debit tb1 = new Voucher_Entry_Debit();

                    tb1.voucher_no = model.VoucherNo;
                    tb1.voucher_date = model.Date;

                    tb1.record_no = model.RecordNo.ToString();

                    tb1.record_date = model.Date;

                    tb1.voucher_sno = i + 1;

                    tb1.voucher_tb = "By";

                    tb1.voucher_type = "Journal";

                    tb1.voucher_year = FinYear;

                    tb1.from_form_name = "Journal";

                    tb1.userid = WebSecurity.CurrentUserId;

                    tb1.account_type = "Journal";


                    tb1.from_form_id = 0;

                    tb1.check_no = model.ChequeNo[i];
                    tb1.check_date = model.ChequeDate[i];


                    string accountname = model.ByAccount[i];
                    var idl = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == accountname) select n.LedgerMasterId).SingleOrDefault();

                    tb1.voucher_ledger_accout_id = Convert.ToInt32(idl);
                    tb1.voucher_dbt_amount = model.DrAmount[i];
                    tb1.voucher_narration = model.DrNarration[i];

                    tb1.create_date = DateTime.Now;

                    db.Voucher_Entry_Debit.Add(tb1);
                    db.SaveChanges();

                }
                for (int j = 0; j < model.LedgerAccId.Length; j++)
                {
                    Voucher_Entry_Credit tb = new Voucher_Entry_Credit();

                    tb.voucher_no = model.VoucherNo;
                    tb.Voucher_date = model.Date;
                    tb.record_no = model.RecordNo.ToString();
                    tb.record_date = model.Date;
                    tb.voucher_sno = j + 1;
                    tb.voucher_tb = "To";
                    tb.voucher_type = "Journal";
                    tb.voucher_year = FinYear;
                    tb.from_form_name = "Journal";
                    tb.userid = WebSecurity.CurrentUserId;
                    tb.account_type = "Journal";
                    tb.from_form_id = 0;
                    string name = model.LedgerAccId[j];
                    var id = (from n in db.tblLedgerMasters.Where(x => x.LedgerName == name) select n.LedgerMasterId).SingleOrDefault();
                    tb.voucher_ledger_accout_id = Convert.ToInt32(id);
                    tb.voucher_cr_amount = model.CreditAmount[j];
                    tb.voucher_narration = model.CrNarration[j];

                    tb.create_date = DateTime.Now;

                    db.Voucher_Entry_Credit.Add(tb);
                    db.SaveChanges();
                }

                TempData["Perror"] = "Inserted Successfully !";
                return View("Journal");

            }
            catch (Exception e)
            {
                TempData["Perror"] = "Please Enter the Details Correctlly !";
                return RedirectToAction("Journal");

                //return View(e);
            }

        }
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
        public string getGRNInvoice(string[] id)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            string json = "";

            int[] PoId = new int[id.Length];


            for (int i = 0; i < id.Length; i++)
            {
                PoId[i] = int.Parse(id[i]);
                var purchaseid = (from t in db.tblPurchaseMasters where PoId.Contains(t.PurchaseId) select t.PurchaseId).FirstOrDefault();
                var Balance = (from q in db.Voucher_Entry_Debit
                               join s in db.Voucher_Entry_Credit
                                     on new { q.voucher_no, q.record_no } equals new { s.voucher_no, s.record_no }
                               join r in db.tblPurchaseMasters on q.purchase_id equals r.PurchaseId
                               where (r.PurchaseId == purchaseid)
                               select r.NetAmount - q.voucher_dbt_amount).Sum() ?? 0;

                if (Balance != 0)
                {

                    var result = (from p in db.tblPurchaseMasters
                                  where (PoId.Contains(p.PurchaseId))
                                  select
                                  new
                                  {
                                      p.tblVendor.Name,

                                      Total = Balance,

                                       p.PurchaseId


                                  }
                     ).ToList();
                    json = jsonSerialiser.Serialize(result);
                }
                else
                {
                    var result = (from p in db.tblPurchaseMasters
                                  where (PoId.Contains(p.PurchaseId))
                                  select
                                  new
                                  {
                                      p.tblVendor.Name,

                                      Total = p.NetAmount,

                                      p.PurchaseId
                                  }
                     ).ToList();
                    json = jsonSerialiser.Serialize(result);

                }
                purchaseid = 0;
            }

            return json;

        }
       
    }
}