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
    public class FinanceDetailsController : Controller
    {
        // GET: FinanceDetails
        NIBSEntities db = new NIBSEntities();
        public ActionResult ReceiptDetails()
        {
            List<ReceiptDetailsModel> list = new List<ReceiptDetailsModel>();

            var items = (from t1 in db.Voucher_Entry_Credit                         
                         where t1.voucher_type == "BankReceipt" && t1.voucher_sno == 1
                         select new
                         {
                             id = t1.voucher_entry_id,
                             voucheerno = t1.voucher_no,
                             voucherdate = t1.Voucher_date,
                             
                             creditname = t1.tblLedgerMaster.LedgerName,
                            
                             //debitname = t2.tblLedgerMaster.LedgerName,
                            
                             recordno = t1.record_no

                         }).ToList();


            foreach(var item in items)
            {
                ReceiptDetailsModel model = new ReceiptDetailsModel();

                model.voucherentryid = Convert.ToInt32(item.id);
                model.voucherno = Convert.ToInt32(item.voucheerno);
                model.voucherdate = item.voucherdate;
               
                model.ledgername = item.creditname;
                model.recordno = Convert.ToInt32(item.recordno);
                
                //model.debitledgername = item.debitname;
               
                list.Add(model);
            }

            return View(list);
        }
        public ActionResult CashReceiptDetails()
        {

            List<CashReceiptDetailsModel> list = new List<CashReceiptDetailsModel>();

            var items = (from t1 in db.Voucher_Entry_Credit
                         where t1.voucher_type == "CashReceipt" && t1.voucher_sno == 1
                         select new
                         {
                             id = t1.voucher_entry_id,
                             voucheerno = t1.voucher_no,
                             voucherdate = t1.Voucher_date,

                             creditname = t1.tblLedgerMaster.LedgerName,

                             //debitname = t2.tblLedgerMaster.LedgerName,

                             recordno = t1.record_no

                         }).SingleOrDefault();


            if (items != null)
            {
                CashReceiptDetailsModel model = new CashReceiptDetailsModel();

                model.voucherentryid = Convert.ToInt32(items.id);
                model.voucherno = Convert.ToInt32(items.voucheerno);
                model.voucherdate = items.voucherdate;

                model.ledgername = items.creditname;
                model.recordno = Convert.ToInt32(items.recordno);

                //model.debitledgername = item.debitname;

                list.Add(model);
            }

            return View(list);
        }
        public ActionResult PaymentDetails()
        {
            List<PaymentDetailsModel> list = new List<PaymentDetailsModel>();

            var items = (from t2 in db.Voucher_Entry_Debit
                         where t2.voucher_type == "Payment" && t2.voucher_sno == 1
                         select new
                         {
                             id = t2.voucher_entry_id,
                             voucheerno = t2.voucher_no,
                             voucherdate = t2.voucher_date,

                             debitname = t2.tblLedgerMaster.LedgerName,

                             recordno = t2.record_no

                         }).SingleOrDefault();

            if (items != null)
            {

                PaymentDetailsModel model = new PaymentDetailsModel();

                //model.voucherentryid = Convert.ToInt32(items.id);
                model.voucherno = Convert.ToInt32(items.voucheerno);
                model.voucherdate = items.voucherdate;

                model.recordno = Convert.ToInt32(items.recordno);

                model.debitledgername = items.debitname;

                list.Add(model);

            }
            return View(list);
        }
        public ActionResult CashPaymentDetails()
        {
            List<CashPaymentDetailsModel> list = new List<CashPaymentDetailsModel>();

            var items = (from t2 in db.Voucher_Entry_Debit
                         where t2.voucher_type == "CashPayment" && t2.voucher_sno == 1
                         select new
                         {
                             id = t2.voucher_entry_id,
                             voucheerno = t2.voucher_no,
                             voucherdate = t2.voucher_date,                           
                            
                             debitname = t2.tblLedgerMaster.LedgerName,
                             
                             recordno = t2.record_no

                         }).SingleOrDefault();

            if (items != null)
            {

                CashPaymentDetailsModel model = new CashPaymentDetailsModel();

                //model.voucherentryid = Convert.ToInt32(items.id);
                model.voucherno = Convert.ToInt32(items.voucheerno);
                model.voucherdate = items.voucherdate;

                model.recordno = Convert.ToInt32(items.recordno);

                model.debitledgername = items.debitname;

                list.Add(model);

            }
            return View(list);
        }
        public ActionResult DebitNoteDetails()
        {
            List<DebitNoteDetailsModel> list = new List<DebitNoteDetailsModel>();

            var items = (from t2 in db.Voucher_Entry_Debit
                         where t2.voucher_type == "DebitNote" && t2.voucher_sno == 1
                         select new
                         {
                             id = t2.voucher_entry_id,
                             voucheerno = t2.voucher_no,
                             voucherdate = t2.voucher_date,

                             debitname = t2.tblLedgerMaster.LedgerName,

                             recordno = t2.record_no

                         }).SingleOrDefault();

            if (items != null)
            {

                DebitNoteDetailsModel model = new DebitNoteDetailsModel();

                //model.voucherentryid = Convert.ToInt32(items.id);
                model.voucherno = Convert.ToInt32(items.voucheerno);
                model.voucherdate = items.voucherdate;

                model.recordno = Convert.ToInt32(items.recordno);

                model.debitledgername = items.debitname;

                list.Add(model);

            }
            return View(list);
        }
        public ActionResult CreditNoteDetails()
        {
            List<CreditNoteDetailsModel> list = new List<CreditNoteDetailsModel>();

            var items = (from t1 in db.Voucher_Entry_Credit
                         where t1.voucher_type == "CreditNote" && t1.voucher_sno == 1
                         select new
                         {
                             id = t1.voucher_entry_id,
                             voucheerno = t1.voucher_no,
                             voucherdate = t1.Voucher_date,

                             creditname = t1.tblLedgerMaster.LedgerName,

                             //debitname = t2.tblLedgerMaster.LedgerName,

                             recordno = t1.record_no

                         }).SingleOrDefault();


            if (items != null)
            {
                CreditNoteDetailsModel model = new CreditNoteDetailsModel();

                model.voucherentryid = Convert.ToInt32(items.id);
                model.voucherno = Convert.ToInt32(items.voucheerno);
                model.voucherdate = items.voucherdate;

                model.ledgername = items.creditname;
                model.recordno = Convert.ToInt32(items.recordno);

                //model.debitledgername = item.debitname;

                list.Add(model);
            }

            return View(list);
        }
        public ActionResult ContraDetails()
        {
            List<ContraDetailsModel> list = new List<ContraDetailsModel>();

            var items = (from t2 in db.Voucher_Entry_Debit 
                         where t2.voucher_type == "Contra" && t2.voucher_sno == 1
                         select new
                         {
                             id = t2.voucher_entry_id,
                             voucheerno = t2.voucher_no,
                             voucherdate = t2.voucher_date,
                             
                             //creditname = t1.tblLedgerMaster.LedgerName,
                             
                             debitname = t2.tblLedgerMaster.LedgerName,
                             
                             recordno = t2.record_no,
                             
                         }).SingleOrDefault();


            if(items!=null)
            {
                ContraDetailsModel model = new ContraDetailsModel();

                model.voucherentryid = Convert.ToInt32(items.id);
                model.voucherno = Convert.ToInt32(items.voucheerno);
                model.voucherdate = items.voucherdate;
                
                //model.creditledgername = item.creditname;
                
                model.debitledgername = items.debitname;
                model.recordno = Convert.ToInt32(items.recordno);

                list.Add(model);
            }

            return View(list);
        }
        public ActionResult JournalDetails()
        {
            List<JournalDetailsModel> list = new List<JournalDetailsModel>();

            var items = (from t2 in db.Voucher_Entry_Debit
                         where t2.voucher_type == "Journal" && t2.voucher_sno == 1
                         select new
                         {
                             id = t2.voucher_entry_id,
                             voucheerno = t2.voucher_no,
                             voucherdate = t2.voucher_date,

                             debitname = t2.tblLedgerMaster.LedgerName,

                             recordno = t2.record_no

                         }).SingleOrDefault();

            if (items != null)
            {

                JournalDetailsModel model = new JournalDetailsModel();

                //model.voucherentryid = Convert.ToInt32(items.id);
                model.voucherno = Convert.ToInt32(items.voucheerno);
                model.voucherdate = items.voucherdate;

                model.recordno = Convert.ToInt32(items.recordno);

                model.debitledgername = items.debitname;

                list.Add(model);

            }
            return View(list);
        }
        public ActionResult DeleteReceipt(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "BankReceipt" select p).ToList();
                foreach(var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "BankReceipt" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }
                
                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("ReceiptDetails", "FinanceDetails");
        }
        public ActionResult DeleteCashReceipt(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "CashReceipt" select p).ToList();
                foreach (var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "CashReceipt" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }

                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("CashReceiptDetails", "FinanceDetails");
        }
        public ActionResult DeletePayment(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "Payment" select p).ToList();
                foreach (var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "Payment" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }
                
                
                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("PaymentDetails", "FinanceDetails");
        }
        public ActionResult DeleteCashPayment(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "CashPayment" select p).ToList();
                foreach (var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "CashPayment" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }

                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("CashPaymentDetails", "FinanceDetails");
        }
        public ActionResult DeleteDebitNote(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "DebitNote" select p).ToList();
                foreach (var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "DebitNote" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }
                
                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("DebitNoteDetails", "FinanceDetails");
        }
        public ActionResult DeleteCreditNote(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "CreditNote" select p).ToList();
                foreach (var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "CreditNote" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }

                
                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("CreditNoteDetails", "FinanceDetails");
        }
        public ActionResult DeleteContra(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "Contra" select p).ToList();
                foreach (var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "Contra" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }
                
                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("ContraDetails", "FinanceDetails");
        }
        public ActionResult DeleteJournal(int id = 0)
        {
            try
            {
                var deleteCredit = (from p in db.Voucher_Entry_Credit where p.record_no == id.ToString() && p.voucher_type == "Journal" select p).ToList();
                foreach (var item in deleteCredit)
                {
                    db.Voucher_Entry_Credit.Remove(item);
                    db.SaveChanges();

                }
                var deleteDebit = (from p in db.Voucher_Entry_Debit where p.record_no == id.ToString() && p.voucher_type == "Journal" select p).ToList();
                foreach (var item in deleteDebit)
                {
                    db.Voucher_Entry_Debit.Remove(item);
                    db.SaveChanges();

                }
                
                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("JournalDetails", "FinanceDetails");
        }
    }
}