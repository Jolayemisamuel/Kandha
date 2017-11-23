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
using CarlosAg.ExcelXmlWriter;
using CarlosAg.Utils;
using HtmlAgilityPack;
using itextsharp;
using iTextSharp;
using itextsharp.pdfa;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace NibsMVC.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        NIBSEntities db = new NIBSEntities();
        BillSearchReportRepository bill = new BillSearchReportRepository();
        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        SqlConnection con;
        SqlCommand cmd;
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
            ds = bill.excuteSp(m.OrderType == string.Empty ? m.OrderType : m.OrderType, m.PaymentType == string.Empty ? m.PaymentType : m.PaymentType, m.BillNo == null ? 0 : Convert.ToInt32(m.BillNo), datefrom, dateto);

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
            foreach (var item in data.tblBillDetails.GroupBy(a => a.Vat))
            {
                PrintVatModel pm = new PrintVatModel();
                pm.Vat = Convert.ToDecimal(item.Key);
                pm.amtCharges = item.Sum(a => a.VatAmount.Value);
                VatList.Add(pm);
            }
            model.getAllVat = VatList;
            return PartialView(model);
        }
        public ActionResult MovementAnalysisReport()
        {
            IEnumerable<SelectListItem> RawId = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.RawMaterialId.ToString() });
            ViewBag.RawId = new SelectList(RawId, "Value", "Text", "RawId");

            IEnumerable<SelectListItem> Deprtmnt = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.DepartmentID.ToString() });
            ViewBag.Deprtmnt = new SelectList(Deprtmnt, "Value", "Text", "Deprtmnt");


            return View();
        }

        public ActionResult printexcel(MovementAnalysisReport Report)
        {

            GenerateExcelReport(Report);
            return RedirectToAction("MovementAnalysisReport", "Reports");
        }

        public void GenerateExcelReport(MovementAnalysisReport Report)
        {

            Workbook book = new Workbook();

            string str_excelfilename = "MovementReport" + ".xls";
            string str_excelpath = Server.MapPath("~/Reports/") + "\\" + str_excelfilename;


            book.Properties.Author = "KS";
            book.Properties.LastAuthor = "Admin";
            book.ExcelWorkbook.WindowHeight = 4815;
            book.ExcelWorkbook.WindowWidth = 11295;
            book.ExcelWorkbook.WindowTopX = 120;
            book.ExcelWorkbook.WindowTopY = 105;

            GenerateStyles(book.Styles);




            GenerateWorksheet_printregister(book.Worksheets, Report);


            book.Save(str_excelpath);

        }
        private void GenerateStyles(WorksheetStyleCollection styles)
        {
            try
            {
                // -----------------------------------------------
                //  Default
                // -----------------------------------------------
                WorksheetStyle Default = styles.Add("Default");
                Default.Name = "Normal";
                Default.Font.FontName = "Calibri";
                Default.Font.Size = 11;
                Default.Font.Color = "#000000";
                Default.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                // -----------------------------------------------
                //  s21
                // -----------------------------------------------
                WorksheetStyle s21 = styles.Add("s21");
                s21.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s21.Alignment.Vertical = StyleVerticalAlignment.Bottom;

                // -----------------------------------------------
                //  s22
                // -----------------------------------------------
                WorksheetStyle s22 = styles.Add("s22");
                s22.Alignment.Horizontal = StyleHorizontalAlignment.Right;
                s22.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                // -----------------------------------------------
                //  s34
                // -----------------------------------------------
                WorksheetStyle s34 = styles.Add("s34");
                s34.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s34.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s34.NumberFormat = "0";

                // -----------------------------------------------
                //  s44
                // -----------------------------------------------
                WorksheetStyle s44 = styles.Add("s44");
                s44.Font.FontName = "Verdana";
                s44.Font.Color = "#000000";
                s44.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s44.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s44.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s44.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s44.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s44.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                // -----------------------------------------------
                //  s45
                // -----------------------------------------------
                WorksheetStyle s45 = styles.Add("s45");
                s45.Font.FontName = "Verdana";
                s45.Font.Color = "#000000";
                s45.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s45.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s45.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s45.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                s45.NumberFormat = "0";
                s45.Alignment.Horizontal = StyleHorizontalAlignment.Right;


                // -----------------------------------------------
                //  s46
                // -----------------------------------------------
                WorksheetStyle s46 = styles.Add("s46");
                s46.Font.FontName = "Verdana";
                s46.Font.Color = "#000000";
                s46.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s46.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s46.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s46.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s46.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s46.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                s46.NumberFormat = "0";
                // -----------------------------------------------
                //  s47
                // -----------------------------------------------
                WorksheetStyle s47 = styles.Add("s47");
                s47.Font.FontName = "Verdana";
                s47.Font.Color = "#000000";
                s47.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s47.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s47.NumberFormat = "0";
                // -----------------------------------------------
                //  s48
                // -----------------------------------------------
                WorksheetStyle s48 = styles.Add("s48");
                s48.Font.FontName = "Verdana";
                s48.Interior.Color = "#FFFFFF";
                s48.Interior.Pattern = StyleInteriorPattern.Solid;
                s48.Alignment.Horizontal = StyleHorizontalAlignment.Right;
                s48.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s48.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s48.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s48.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s48.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                // -----------------------------------------------
                //  s49
                // -----------------------------------------------
                WorksheetStyle s49 = styles.Add("s49");
                s49.Font.FontName = "Verdana";
                s49.Font.Color = "#000000";
                s49.Alignment.Horizontal = StyleHorizontalAlignment.Right;
                s49.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s49.NumberFormat = "0";
                // -----------------------------------------------
                //  s50
                // -----------------------------------------------
                WorksheetStyle s50 = styles.Add("s50");
                s50.Font.FontName = "Verdana";
                s50.Font.Color = "#000000";
                s50.NumberFormat = "0";
                // -----------------------------------------------
                //  s51
                // -----------------------------------------------
                WorksheetStyle s51 = styles.Add("s51");
                s51.Font.FontName = "Verdana";
                s51.Font.Color = "RED";
                s51.Alignment.Horizontal = StyleHorizontalAlignment.Right;
                s51.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s51.NumberFormat = "Fixed";
                // -----------------------------------------------
                //  s52
                // -----------------------------------------------
                WorksheetStyle s52 = styles.Add("s52");
                s52.Font.FontName = "Verdana";
                s52.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s52.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s52.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s52.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s52.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s52.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                // -----------------------------------------------
                //  s54
                // -----------------------------------------------
                WorksheetStyle s54 = styles.Add("s54");
                s54.Font.Bold = true;
                s54.Font.FontName = "Verdana";
                s54.Font.Size = 11;
                s54.Font.Color = "#000000";
                s54.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s54.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s54.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s54.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s54.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s54.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                // -----------------------------------------------
                //  s55
                // -----------------------------------------------
                WorksheetStyle s55 = styles.Add("s55");
                s55.Font.Bold = true;
                s55.Font.FontName = "Verdana";
                s55.Font.Size = 10;
                s55.Font.Color = "#000000";
                s55.Alignment.Horizontal = StyleHorizontalAlignment.Left;
                s55.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s55.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s55.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s55.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s55.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                // -----------------------------------------------
                //  s56
                // -----------------------------------------------
                WorksheetStyle s56 = styles.Add("s56");
                s56.Font.Bold = true;
                s56.Font.FontName = "Verdana";
                s56.Font.Size = 10;
                s56.Font.Color = "#000000";
                s56.Alignment.WrapText = true;
                s56.Alignment.Horizontal = StyleHorizontalAlignment.Center;
                s56.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                s56.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                s56.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                s56.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                s56.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            }
            catch (Exception ex)
            {

            }
        }

        private void GenerateWorksheet_printregister(WorksheetCollection sheets, MovementAnalysisReport Report)
        {

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();
            con.Open();

            var RawName = (from p in db.tbl_RawMaterials where p.RawMaterialId.Equals(Report.RawMaterialId) select p).ToList();
            //var DepartmentName = (from p in db.tbl_Department where p.DepartmentID.Equals(Report.DepartmentId) select p).ToList();

            Report.RawMaterialName = RawName.FirstOrDefault().Name;
            //Report.DepartmentName = DepartmentName.FirstOrDefault().Department;

            Worksheet sheet = sheets.Add("EFiling");



            //sheet.Table.Columns.Add(430);
            //sheet.Table.Columns.Add(300);
            //sheet.Table.Columns.Add(300);

            //WorksheetRow Row1 = sheet.Table.Rows.Add();

            //Row1.Cells.Add("", DataType.String, "s56");
            //Row1.Cells.Add("STORES", DataType.String, "s56");
            //Row1.Cells.Add("KITCHEN", DataType.String, "s56");

            sheet.Table.Columns.Add(150);
            sheet.Table.Columns.Add(200);
            sheet.Table.Columns.Add(80);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            //sheet.Table.Columns.Add(150);
            //sheet.Table.Columns.Add(100);

            WorksheetRow Row = sheet.Table.Rows.Add();

            //Int64 serial_no = 0;
            Row.Cells.Add("InvoiceDate", DataType.String, "s56");
            Row.Cells.Add("Item Name", DataType.String, "s56");
            Row.Cells.Add("InvoiceNo", DataType.String, "s56");
            Row.Cells.Add("Store Opening Stock", DataType.String, "s56");
            Row.Cells.Add("Store Inward", DataType.String, "s56");
            Row.Cells.Add("Store Outward", DataType.String, "s56");
            Row.Cells.Add("Store Closing_Stock", DataType.String, "s56");
            Row.Cells.Add("kitchen Opening Stock", DataType.String, "s56");
            Row.Cells.Add("kitchen Inward", DataType.String, "s56");



            if (Report.DateFrom != null && Report.DateTo != null)
            {
                DateTime from;

                for (from = Report.DateFrom; from <= Report.DateTo; from = from.AddDays(1.0))
                {
                    sb.Append("with tempMoveAnal   as (  select  m.InvoiceDate ,r.Name,m.InvoiceNo,");
                    sb.Append("Opening_Stock= (select ((op.Qty-op.IssQty)+(gs.Qty-gs.IssQty)) from tbl_RawMaterials rm inner join tblGRNStock gs on ");
                    sb.Append("gs.MaterialId=rm.RawMaterialId inner join tblOpStckRate op on op.MaterialId=rm.RawMaterialId ");
                    sb.Append("where rm.RawMaterialId='" + Report.RawMaterialId + "' and gs.Date='" + from.ToString("yyyy-MM-dd") + "'),");
                    sb.Append("Inward=(select Qty from tblGRNStock where MaterialId='" + Report.RawMaterialId + "' and Date='" + from.ToString("yyyy-MM-dd") + "'),");
                    sb.Append("outward=(select sum(TransferQuantity) from tblTransfer where RawMaterialId='" + Report.RawMaterialId + "' and TransferDate='" + from.ToString("yyyy-MM-dd") + "'),");
                    sb.Append("kitchen_op=(select sum(TransferQuantity) from tblTransfer where RawMaterialId='" + Report.RawMaterialId + "' and TransferDate='" + from.ToString("yyyy-MM-dd") + "'),");
                    sb.Append("kitchen_In=(select sum(TransferQuantity) from tblTransfer where RawMaterialId='" + Report.RawMaterialId + "' and TransferDate='" + from.ToString("yyyy-MM-dd") + "')");
                    sb.Append("from   tblPurchaseMaster m inner join tblPurchasedItem c on c.PurchaseId = m.PurchaseId inner join tbl_RawMaterials r on c.RawMaterialId=r.RawMaterialId  ");
                    sb.Append("where r.RawMaterialId='" + Report.RawMaterialId + "' and m.InvoiceDate='" + from.ToString("yyyy-MM-dd") + "')  ");
                    sb.Append("select InvoiceDate,name ,InvoiceNo,Opening_Stock,Inward,outward,closing_stock=Opening_Stock-(Inward+outward),kitchen_op,kitchen_In from tempMoveAnal ");


                    cmd = new SqlCommand(sb.ToString(), con);
                    cmd.CommandType = CommandType.Text;
                    DataTable dt = new DataTable();

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    var data = sda.Fill(dt);
                    List<DataRow> list = dt.AsEnumerable().ToList();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Row = sheet.Table.Rows.Add();
                        //serial_no = serial_no + 1;
                        //Row.Cells.Add(serial_no.ToString(), DataType.String, "s44");
                        Row.Cells.Add(Convert.ToString(from), DataType.String, "s45");
                        Row.Cells.Add(Report.RawMaterialName, DataType.String, "s45");
                        Row.Cells.Add(dr["InvoiceNo"].ToString(), DataType.String, "s49");
                        Row.Cells.Add(dr["Opening_Stock"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["Inward"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["outward"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["closing_stock"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["kitchen_op"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["kitchen_In"].ToString(), DataType.String, "s45");
                        //Row.Cells.Add(dr["Inward"].ToString(), DataType.String, "s45");
                        //Row.Cells.Add(dr["Inward"].ToString(), DataType.String, "s45");
                    }
                    dt = null;
                }
                sheet.Options.Selected = true;
                sheet.Options.ProtectObjects = false;
                sheet.Options.ProtectScenarios = false;
                sheet.Options.PageSetup.Layout.Orientation = CarlosAg.ExcelXmlWriter.Orientation.Landscape;
                sheet.Options.PageSetup.PageMargins.Bottom = 1F;
                sheet.Options.PageSetup.PageMargins.Left = 1F;
                sheet.Options.PageSetup.PageMargins.Right = 1F;
                sheet.Options.PageSetup.PageMargins.Top = 1F;
                sheet.Options.Print.PaperSizeIndex = 9;
                sheet.Options.Print.HorizontalResolution = 120;
                sheet.Options.Print.VerticalResolution = 72;
                sheet.Options.Print.ValidPrinterInfo = true;

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=NibsReport.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                Response.Flush();
                Response.Write(sheet);
                //Response.Write(Row1);
                Response.End();

                con.Close();

            }


            //Row.Cells.Add("Total Taxable Value", DataType.String, "s56");
            //Row.Cells.Add("Total Cess", DataType.String, "s56");



        }
    }
}
