using NibsMVC.EDMX;
using NibsMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using NibsMVC.Repository;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
namespace NibsMVC.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        NIBSEntities db = new NIBSEntities();
        BillSearchReportRepository bill = new BillSearchReportRepository();
        [Authorize(Roles = "Manager,Outlet,Store")]
        public ActionResult Index()
        {
            return View(bill.getModel());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(BillSearchReportModel model)
        {

            return View(bill.getSearchModel(model));
        }
        public ActionResult ExportToexcel(BillSearchReportModel m)
        {
            DataSet ds = new DataSet();
            string datefrom;
            string dateto;
            if (m.DateTo.HasValue)
            {
                dateto = Convert.ToDateTime(m.DateTo).AddDays(1).ToShortDateString();
            }
            else
            {
                dateto = string.Empty;
            }
            if (m.DateFrom.HasValue)
            {
                datefrom = Convert.ToDateTime(m.DateFrom).ToShortDateString();
            }
            else
            {
                datefrom = string.Empty;
            }
            ds = bill.excuteSp(m.OrderType == null ? string.Empty : m.OrderType, m.PaymentType == null ? string.Empty : m.PaymentType, m.BillNo == null ? 0 : Convert.ToInt32(m.BillNo), datefrom, dateto);

            GridView gv = new GridView();
            DataTable tb = new DataTable();
            if (ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=NibsReport.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                HttpContext.Response.Write("<Table border='1' bgColor='#ffffff' " +
                   "borderColor='#000000' cellSpacing='0' cellPadding='0' " +
                   "style='font-size:10.0pt; font-family:Calibri; background:white;'>");
                HttpContext.Response.Write("<TR>");
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    HttpContext.Response.Write("<TH>" + dt.Columns[i].ColumnName + "</TH>");
                }
                HttpContext.Response.Write("</TR>");
                foreach (DataRow item in dt.Rows)
                {
                    HttpContext.Response.Write("<TR>");
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        HttpContext.Response.Write("<TD>" + item[i].ToString() + "</TD>");
                    }

                    HttpContext.Response.Write("</TR>");
                }
                HttpContext.Response.Write("<TR>");
                HttpContext.Response.Write("<TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD><TD></TD>");
                HttpContext.Response.Write("</TR>");
                HttpContext.Response.Write("<TR>");
                HttpContext.Response.Write("<TD><b>Sum</b></TD><TD></TD><TD></TD><TD></TD><TD></TD>");

                for (int i = 5; i < ds.Tables[0].Columns.Count; i++)
                {
                    string sum = "";
                    sum = dt.Compute("SUM([" + dt.Columns[i].ToString() + "])", "").ToString();
                    HttpContext.Response.Write("<TD><b>" + sum + "</b></TD>");
                }
                HttpContext.Response.Write("</TR>");


            }
            //gv.DataSource = ds;
            //gv.DataBind();
            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            //Response.ContentType = "application/ms-excel";
            //Response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //gv.RenderControl(htw);
            //Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("Index");
        }
        public ActionResult BillingItem()
        {
            searchBillingItemModel model = new searchBillingItemModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult BillingItem(int BillId = 0)
        {
            searchBillingItemModel model = new searchBillingItemModel();
            model.BillId = BillId;
            model.getItems = bill.getBillItems(BillId);
            return View(model);
        }
        public ActionResult Report()
        {
            return View();
        }
        public ActionResult STakeAwayBill()
        {
            return View();
        }
        public int getOutletId()
        {
            string[] roles = Roles.GetRolesForUser();
            var oulte = 0;
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    oulte = WebSecurity.CurrentUserId;
                }
                else
                {
                    oulte = Convert.ToInt32((from n in db.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }
        public ActionResult ShomedelvrBill()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ShomedelvrBill(SearchReportsModel model)
        {
            try
            {
                var from = Convert.ToDateTime(model.FromDate);
                DateTime to = Convert.ToDateTime(model.ToDate);
                int OutletId = getOutletId();
                SearchReportsModel m = new SearchReportsModel();
                List<tblBillMaster> list = new List<tblBillMaster>();
                if (model.TokenNo == null)
                {
                    list = db.tblBillMasters.Where(a => System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) >= from.Date && System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) <= to.Date && a.OutletId == OutletId && a.BillingType == "Door Delivery Hall").ToList();
                }
                else
                {
                    list = db.tblBillMasters.Where(a => System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) >= from.Date
                        && System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) <= to.Date
                        && a.OutletId == OutletId
                        && a.TokenNo == model.TokenNo
                        && a.BillingType == "Door Delivery Hall").ToList();
                }

                m.getbills = list;
                return View(m);
            }
            catch
            {
                ModelState.AddModelError("", "Error Occured");
            }
            return View();
        }
        [HttpPost]
        public ActionResult STakeAwayBill(SearchReportsModel model)
        {

            try
            {
                var from = Convert.ToDateTime(model.FromDate);
                DateTime to = Convert.ToDateTime(model.ToDate);
                int OutletId = getOutletId();
                SearchReportsModel m = new SearchReportsModel();
                List<tblBillMaster> list = new List<tblBillMaster>();
                if (model.TokenNo == null)
                {
                    list = db.tblBillMasters.Where(a => EntityFunctions.TruncateTime(a.BillDate) >= from.Date && EntityFunctions.TruncateTime(a.BillDate) <= to.Date && a.OutletId == OutletId && a.BillingType == "Take Away Hall").ToList();
                }
                else
                {
                    list = db.tblBillMasters.Where(a => EntityFunctions.TruncateTime(a.BillDate) >= from.Date
                        && EntityFunctions.TruncateTime(a.BillDate) <= to.Date
                        && a.OutletId == OutletId
                        && a.TokenNo == model.TokenNo
                        && a.BillingType == "Take Away Hall").ToList();
                }

                m.getbills = list;
                return View(m);
            }
            catch
            {
                ModelState.AddModelError("", "Error Occured");
            }

            return View();
        }
        [HttpPost]
        public ActionResult Report(SearchReportsModel model)
        {
            try
            {
                var from = Convert.ToDateTime(model.FromDate);
                DateTime to = Convert.ToDateTime(model.ToDate);
                //  var abc = model.FromDate.Date;
                var loginname = WebSecurity.CurrentUserName;
                var Outletids = WebSecurity.CurrentUserId;
                //var Billdata = (from p in db.tblBillMasters where p.BillDate.Date >= model.FromDate && p.BillDate.Date <= model.ToDate && p.OutletId == Outletids && p.BillingType == "R" select p).ToList();
                //var Billdata = db.tblBillMasters.Where(a => EntityFunctions.TruncateTime(a.BillDate) >= from.Date && EntityFunctions.TruncateTime(a.BillDate) <= to.Date && a.OutletId == Outletids && a.BillingType == "R").ToList();
                SearchReportsModel m = new SearchReportsModel();
                List<tblBillMaster> list = db.tblBillMasters.Where(a => System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) >= from.Date && System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) <= to.Date && a.OutletId == Outletids && (a.BillingType == "Ac Hall" || a.BillingType == "Dine In Hall")).ToList();

                m.getbills = list;
                return View(m);
            }
            catch
            {
                ModelState.AddModelError("", "Error Occured");
            }
            return View();
        }

        public ActionResult Delete(int id = 0)
        {
            try
            {
                var deletebilldata = (from p in db.tblBillDetails where p.BillId.Equals(id) select p).ToList();
                foreach (var item in deletebilldata)
                {
                    db.tblBillDetails.Remove(item);
                    db.SaveChanges();
                }
                var deletemaindata = (from p in db.tblBillMasters where p.BillId.Equals(id) select p).SingleOrDefault();
                db.tblBillMasters.Remove(deletemaindata);
                db.SaveChanges();
                TempData["error"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction("ShomedelvrBill", "Reports");
        }
        public ActionResult CategoryWise()
        {
            return View();
        }
        public ActionResult ViewBillDetail(int BillId = 0)
        {
            tblBillMaster model = new tblBillMaster();
            model = db.tblBillMasters.Find(BillId);

            return View(model);
        }
        public ActionResult PrintAgainBill(int BillId = 0)
        {
            var data = db.tblBillMasters.Find(BillId);
            PrintBillModel model = new PrintBillModel();
            model.CustomerAddress = data.Address;
            model.BillId = data.BillId;
            model.CustomerName = data.CustomerName;
            model.Address = data.tblOutlet.Address;
            model.ContactA = data.tblOutlet.ContactA;
            model.CustomerContactNo = data.ContactNo;
            model.discount = data.Discount.Value;
            model.DiscountAmount = data.DiscountAmount;
            model.NetAmount = data.NetAmountWithoutDiscount.Value;
            model.NetAmountAfterDiscount = data.NetAmount;
            model.PackingCharge = data.PackingCharges.Value;
            model.ServicesCharge = data.ServicChargesAmount;
            model.ServiceTax = data.ServiceTax.Value;
            model.ServiceTaxNo = data.tblOutlet.ServiceTaxNo;
            model.TableNo = data.TableNo.ToString();
            model.TinNo = data.tblOutlet.TinNo;
            model.TotalAmount = data.TotalAmount;
            model.BillDate = data.BillDate;
            List<PrintItemModel> lst = new List<PrintItemModel>();
            List<PrintVatModel> VatList = new List<PrintVatModel>();
            foreach (var item in data.tblBillDetails)
            {
                PrintItemModel pm = new PrintItemModel();
                pm.ItemName = item.tblItem.Name;
                pm.Amount = item.Amount;
                pm.FullQty = item.FullQty.ToString();
                pm.BasicPrice = item.tblItem.tblBasePriceItems.Where(a => a.ItemId == item.ItemId).Select(a => a.FullPrice).FirstOrDefault();
                lst.Add(pm);
            }
            model.getAllItem = lst;
            foreach (var item in data.tblBillDetails.GroupBy(a=>a.Vat))
            {
                PrintVatModel pm = new PrintVatModel();
                pm.Vat = Convert.ToDecimal(item.Key);
                pm.amtCharges = item.Sum(a => a.VatAmount.Value);
                VatList.Add(pm);
            }
            model.getAllVat = VatList;
            return PartialView(model);
        }
    }
}
