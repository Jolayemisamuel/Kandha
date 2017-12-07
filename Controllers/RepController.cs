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
    public class RepController : Controller
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

            return RedirectToAction("ShomedelvrBill", "Rep");
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
                //s45.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                //s45.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                //s45.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                //s45.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
                s45.NumberFormat = "0";
                s45.Alignment.Horizontal = StyleHorizontalAlignment.Right;


                // -----------------------------------------------
                //  s46
                // -----------------------------------------------
                WorksheetStyle s46 = styles.Add("s46");
                s46.Font.FontName = "Verdana";
                s46.Font.Color = "#000000";
                s46.Alignment.Horizontal = StyleHorizontalAlignment.Left;
                s46.Alignment.Vertical = StyleVerticalAlignment.Bottom;
                //s46.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                //s46.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                //s46.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                //s46.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
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
                //s55.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                //s55.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                //s55.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                //s55.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
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
                //s56.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
                //s56.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
                //s56.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
                //s56.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            }
            catch (Exception ex)
            {

            }
        }
        public ActionResult exportMovementAnalysis(MovementAnalysisReport Report)
        {

            string filename= GenerateExcelReport(Report);
            var f=  File(Path.Combine( Server.MapPath("~/Report/"), filename), "application/vnd.ms-excel");
            f.FileDownloadName = "MovementAnalysis.xls";
            return f;

            //string ext = System.IO.Path.GetExtension(filename).ToLower();
            //Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            //string mimeType = "";
            //if (regKey != null && regKey.GetValue("Content Type") != null)
            //    mimeType = regKey.GetValue("Content Type").ToString();

            // return RedirectToAction("MovementAnalysisReport", "Reports");
        }

        public string GenerateExcelReport(MovementAnalysisReport Report)
        {

            Workbook book = new Workbook();

            string str_excelfilename = "MovementReport" + ".xls";
            string str_excelpath = Server.MapPath("~/Report/") + "\\" + str_excelfilename;


            book.Properties.Author = "KS";
            book.Properties.LastAuthor = "Admin";
            book.ExcelWorkbook.WindowHeight = 4815;
            book.ExcelWorkbook.WindowWidth = 11295;
            book.ExcelWorkbook.WindowTopX = 120;
            book.ExcelWorkbook.WindowTopY = 105;

            GenerateStyles(book.Styles);




            GenerateWorksheet_printregister(book.Worksheets, Report);


            book.Save(str_excelpath);

            return str_excelfilename;


        }
        

        private void GenerateWorksheet_printregister(WorksheetCollection sheets, MovementAnalysisReport Report)
        {
            var dataListStore = new List<MovementAnalysisStore_Result>();
            var dataListKitchen = new List<MovementAnalysisKitchen_Result>();
            if (Report.Type == "Store")
                dataListStore = db.MovementAnalysisStore(Report.DateFrom, Report.DateTo, Report.RawMaterialId).ToList();
            else
                dataListKitchen = db.MovementAnalysisKitchen(Report.DateFrom, Report.DateTo, Report.RawMaterialId).ToList();



            Worksheet sheet = sheets.Add("MA");




            sheet.Table.Columns.Add(150);
            sheet.Table.Columns.Add(200);
            sheet.Table.Columns.Add(80);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(80);
            sheet.Table.Columns.Add(200);

            WorksheetRow Row = sheet.Table.Rows.Add();

            Row.Cells.Add("Date", DataType.String, "s56");
            Row.Cells.Add("Name", DataType.String, "s56");
            Row.Cells.Add("Op Stock", DataType.String, "s56");
            Row.Cells.Add("Op Value", DataType.String, "s56");
            Row.Cells.Add("In Qty", DataType.String, "s56");
            Row.Cells.Add("In Value", DataType.String, "s56");
            Row.Cells.Add("Out Qty", DataType.String, "s56");
            Row.Cells.Add("Out value", DataType.String, "s56");
            Row.Cells.Add("Cls Stock", DataType.String, "s56");
            Row.Cells.Add("Cls Value", DataType.String, "s56");
            Row.Cells.Add("Remarks", DataType.String, "s56");

            DateTime frDate = Convert.ToDateTime(Report.DateFrom);
            decimal opStck = 0, clsStck = 0;
            decimal opStckVal = 0, clsStckVal = 0;
            int chkRawMatId = 0;

            if (dataListStore.Count > 0)
            {

                foreach (var li in dataListStore)
                {
                    if(chkRawMatId==0 || chkRawMatId!=li.MaterialId)
                    {
                        if (db.vwStockTransactions.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Count() > 0)
                        {
                            opStck = Convert.ToDecimal(db.vwStockTransactions.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.inQty)) - Convert.ToDecimal(db.vwStockTransactions.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.outQty));
                            opStckVal = Convert.ToDecimal(db.vwStockTransactions.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.inVal)) - Convert.ToDecimal(db.vwStockTransactions.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.outVal));
                        }
                        else
                        {
                            opStck = 0;
                            opStckVal = 0;
                        }

                        chkRawMatId = li.MaterialId;
                    }

                    Row = sheet.Table.Rows.Add();
                    Row.Cells.Add(li.Date, DataType.String, "s46");
                    Row.Cells.Add(li.Name, DataType.String, "s46");
                    Row.Cells.Add(opStck.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(opStckVal.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(li.inQty.ToString(), DataType.String, "s45");
                    Row.Cells.Add(li.inVal.ToString(), DataType.String, "s45");
                    Row.Cells.Add(li.outQty.ToString(), DataType.String, "s45");
                    Row.Cells.Add(li.outVal.ToString(), DataType.String, "s45");

                    clsStck = Convert.ToDecimal(opStck + li.inQty - li.outQty);
                    opStck = clsStck;
                    clsStckVal = Convert.ToDecimal(opStckVal + li.inVal - li.outVal);
                    opStckVal = clsStckVal;

                    Row.Cells.Add(clsStck.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(clsStckVal.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(li.Remarks.ToString(), DataType.String, "s46");

                }

            }
            if (dataListKitchen.Count > 0)
            {
                foreach (var li in dataListKitchen)
                {
                    if (chkRawMatId == 0 || chkRawMatId != li.MaterialId)
                    {
                        if (db.vwStockTransactions.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Count() > 0)
                        {
                            opStck = Convert.ToDecimal(db.vwStockTransactionsKitchens.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.inQty)) - Convert.ToDecimal(db.vwStockTransactionsKitchens.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.outQty));
                            opStckVal = Convert.ToDecimal(db.vwStockTransactionsKitchens.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.inVal)) - Convert.ToDecimal(db.vwStockTransactionsKitchens.Where(p => p.Date < frDate && p.MaterialId == li.MaterialId).Sum(p => p.outVal));
                        }
                        else
                        {
                            opStck = 0;
                            opStckVal = 0;
                        }

                        chkRawMatId = li.MaterialId;
                    }

                    Row = sheet.Table.Rows.Add();
                    Row.Cells.Add(li.Date, DataType.String, "s46");
                    Row.Cells.Add(li.Name, DataType.String, "s46");
                    Row.Cells.Add(opStck.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(opStckVal.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(li.inQty.ToString(), DataType.String, "s45");
                    Row.Cells.Add(li.inVal.ToString(), DataType.String, "s45");
                    Row.Cells.Add(li.outQty.ToString(), DataType.String, "s45");
                    Row.Cells.Add(li.outVal.ToString(), DataType.String, "s45");

                    clsStck = Convert.ToDecimal(opStck + li.inQty - li.outQty);
                    opStck = clsStck;
                    clsStckVal = Convert.ToDecimal(opStckVal + li.inVal - li.outVal);
                    opStckVal = clsStckVal;

                    Row.Cells.Add(clsStck.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(clsStckVal.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(li.Remarks.ToString(), DataType.String, "s46");

                }

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

          

            

        }


        public ActionResult IngrediantReport()
        {
            IEnumerable<SelectListItem> RawId = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.RawMaterialId.ToString() });
            ViewBag.RawId = new SelectList(RawId, "Value", "Text", "RawId");

            IEnumerable<SelectListItem> RawCategoryId = (from n in db.RawCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.RawCategoryID.ToString() });
            ViewBag.RawCategoryId = new SelectList(RawCategoryId, "Value", "Text", "RawCategoryId");

            IEnumerable<SelectListItem> MenuCategoryId = (from n in db.tblCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.CategoryId.ToString() });
            ViewBag.MenuCategoryId = new SelectList(MenuCategoryId, "Value", "Text", "MenuCategoryId");

            IEnumerable<SelectListItem> MenuItemId = (from n in db.tblItems select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.ItemId.ToString() });
            ViewBag.MenuItemId = new SelectList(MenuItemId, "Value", "Text", "MenuItemId");

            IEnumerable<SelectListItem> SubItemId = (from n in db.tblSubItems select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.SubItemId.ToString() });
            ViewBag.SubItemId = new SelectList(SubItemId, "Value", "Text", "SubItemId");

            return View();
        }
        public ActionResult exportIngrediants(ingrediantReport Report)
        {

            string filename = GenerateExcelReport(Report);
            var f = File(Path.Combine(Server.MapPath("~/Report/"), filename), "application/vnd.ms-excel");
            f.FileDownloadName = "Ingrediants.xls";
            return f;

            //string ext = System.IO.Path.GetExtension(filename).ToLower();
            //Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            //string mimeType = "";
            //if (regKey != null && regKey.GetValue("Content Type") != null)
            //    mimeType = regKey.GetValue("Content Type").ToString();

            // return RedirectToAction("MovementAnalysisReport", "Reports");
        }
        public string GenerateExcelReport(ingrediantReport Report)
        {

            Workbook book = new Workbook();

            string str_excelfilename = "IngerediantReport" + ".xls";
            string str_excelpath = Server.MapPath("~/Report/") + "\\" + str_excelfilename;


            book.Properties.Author = "KS";
            book.Properties.LastAuthor = "Admin";
            book.ExcelWorkbook.WindowHeight = 4815;
            book.ExcelWorkbook.WindowWidth = 11295;
            book.ExcelWorkbook.WindowTopX = 120;
            book.ExcelWorkbook.WindowTopY = 105;

            GenerateStyles(book.Styles);




            GenerateWorksheet_printregister(book.Worksheets, Report);


            book.Save(str_excelpath);

            return str_excelfilename;


        }


        private void GenerateWorksheet_printregister(WorksheetCollection sheets, ingrediantReport Report)
        {
            var dataListMain = new List<ItemIndentDetails_Result>();
            var dataListSub = new List<SubItemIndentDetails_Result>();
            if (Report.Type == "Menu Items")
                dataListMain = db.ItemIndentDetails(Report.MenuCategoryId, Report.MenuItemId,Report.RawCategoryId, Report.RawMaterialId).ToList();
            else
                dataListSub = db.SubItemIndentDetails(Report.MenuCategoryId, Report.MenuItemId,Report.SubItemId, Report.RawCategoryId, Report.RawMaterialId).ToList();



            Worksheet sheet = sheets.Add("ing1");

            WorksheetRow Row;

            if (Report.Type == "Menu Items")
            {
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(100);
                sheet.Table.Columns.Add(100);
             

                Row = sheet.Table.Rows.Add();

                Row.Cells.Add("Menu Category", DataType.String, "s56");
                Row.Cells.Add("Menu Item", DataType.String, "s56");
                Row.Cells.Add("Portion", DataType.String, "s56");
                Row.Cells.Add("Raw Category", DataType.String, "s56");
                Row.Cells.Add("Raw Material", DataType.String, "s56");
                Row.Cells.Add("Quantity", DataType.String, "s56");
                Row.Cells.Add("Unit", DataType.String, "s56");
               
            }
            else
            {
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(80);
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(200);
                sheet.Table.Columns.Add(100);
                sheet.Table.Columns.Add(100);


                Row = sheet.Table.Rows.Add();

                Row.Cells.Add("Menu Category", DataType.String, "s56");
                Row.Cells.Add("Menu Item", DataType.String, "s56");
                Row.Cells.Add("Sub Item", DataType.String, "s56");
                Row.Cells.Add("Portion", DataType.String, "s56");
                Row.Cells.Add("Raw Category", DataType.String, "s56");
                Row.Cells.Add("Raw Material", DataType.String, "s56");
                Row.Cells.Add("Quantity", DataType.String, "s56");
                Row.Cells.Add("Unit", DataType.String, "s56");

            }

           
            int chkItemId = 0;

            if (dataListMain.Count > 0)
            {

                foreach (var li in dataListMain)
                {
                    Row = sheet.Table.Rows.Add();
                    if (chkItemId == 0 || chkItemId != li.ItemId)
                    {
                        Row.Cells.Add(li.MenuCategory, DataType.String, "s46");
                        Row.Cells.Add(li.Item, DataType.String, "s46");
                        Row.Cells.Add(li.Portion.ToString(), DataType.String, "s45");
                        chkItemId = li.ItemId;
                    }
                    else
                    {
                        Row.Cells.Add("", DataType.String, "s46");
                        Row.Cells.Add("", DataType.String, "s46");
                        Row.Cells.Add("", DataType.String, "s45");
                    }

                    //Row.Cells.Add(li.MenuCategory, DataType.String, "s46");
                    //Row.Cells.Add(li.Item, DataType.String, "s46");
                    //Row.Cells.Add(li.Portion.ToString(), DataType.String, "s45");
                    Row.Cells.Add(li.RawCategory, DataType.String, "s46");
                    Row.Cells.Add(li.RawMaterial, DataType.String, "s46");
                    Row.Cells.Add(li.Quantity.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(li.Unit, DataType.String, "s46");
                    
                }

            }
            int chkSubItemId = 0;
            if (dataListSub.Count > 0)
            {
                foreach (var li in dataListSub)
                {
                    Row = sheet.Table.Rows.Add();

                    if (chkSubItemId == 0 || chkSubItemId != li.subitemid)
                    {
                        Row.Cells.Add(li.MenuCategory, DataType.String, "s46");
                        Row.Cells.Add(li.Item, DataType.String, "s46");
                        Row.Cells.Add(li.SubItem, DataType.String, "s46");
                        Row.Cells.Add(li.Portion.ToString(), DataType.String, "s45");
                        chkSubItemId = li.subitemid;
                    }
                    else
                    {
                        Row.Cells.Add("", DataType.String, "s46");
                        Row.Cells.Add("", DataType.String, "s46");
                        Row.Cells.Add("", DataType.String, "s46");
                        Row.Cells.Add("", DataType.String, "s45");
                    }


                    //Row.Cells.Add(li.MenuCategory, DataType.String, "s46");
                    //Row.Cells.Add(li.Item, DataType.String, "s46");
                    //Row.Cells.Add(li.SubItem, DataType.String, "s46");
                    //Row.Cells.Add(li.Portion.ToString("0"), DataType.String, "s45");
                    Row.Cells.Add(li.RawCategory, DataType.String, "s46");
                    Row.Cells.Add(li.RawMaterial, DataType.String, "s46");
                    Row.Cells.Add(li.Qty.ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add(li.Unit, DataType.String, "s46");

                }

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





        }
    }

}
