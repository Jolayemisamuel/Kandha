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
        public ActionResult CashPaymentDetails()
        {
            return View();
        }
        public ActionResult ReceiptDetails()
        {
            ReceiptDetails model = new ReceiptDetails();

            var items = (from t1 in db.Voucher_Entry_Credit
                         join t2 in db.Voucher_Entry_Debit on
                         t1.voucher_entry_id equals t2.voucher_entry_id
                         where t1.voucher_type == "Bank Receipt"
                         select new
                         {
                             id=t1.voucher_entry_id,
                             voucheerno=t1.voucher_no,
                             voucherdate=t1.Voucher_date,
                             creditid=t1.voucher_ledger_accout_id,
                             creditname=t1.tblLedgerMaster.LedgerName,
                             creditamount=t1.voucher_cr_amount,
                             checkno=t1.check_no,
                             checkdate=t1.check_date,
                             crnarration=t1.voucher_narration,
                             debitid=t2.voucher_ledger_accout_id,
                             debitname=t2.tblLedgerMaster.LedgerName,
                             debitamount=t2.voucher_dbt_amount,
                             drnarration=t2.voucher_narration,
                             recordno=t1.record_no

                         }).ToList();
            List<ReceiptDetails> list = new List<ReceiptDetails>();

            foreach (var item in items)
            {
                model.voucherentryid = Convert.ToInt32(item.id);
                model.voucherno = Convert.ToInt32(item.voucheerno);
                model.voucherdate = item.voucherdate;
                model.ledgerid = Convert.ToInt32(item.creditid);
                model.ledgername = item.creditname;
                model.recordno = Convert.ToInt32(item.recordno);
                model.debitledgerid = Convert.ToInt32(item.debitid);
                model.debitledgername = item.debitname;
                model.checkno = item.checkno;
                model.checkdate = item.checkdate;
                model.crdescription = item.crnarration;
                model.drdescription = item.drnarration;
                model.creditamount = Convert.ToDecimal(item.creditamount);
                model.debitamount = Convert.ToDecimal(item.debitamount);
                list.Add(model);
            }

            return View(list);
        }
    }
}