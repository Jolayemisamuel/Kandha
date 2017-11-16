using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CarlosAg.ExcelXmlWriter;
using CarlosAg.Utils;
using HtmlAgilityPack;
using itextsharp;
using iTextSharp;
using itextsharp.pdfa;
using System.Web.UI;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
namespace NibsMVC.Controllers
{
    public class PurchaseOrderController : Controller
    {
        NIBSEntities db = new NIBSEntities();
        XMLTablesRepository obj = new XMLTablesRepository();
        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        SqlConnection con;
        SqlCommand cmd;
        int uniqueid = 0;

        public string UniqueID { get; private set; }

        // GET: PurchaseOrder
        public ActionResult Index()
        {

            var data = (from p in db.tblPurchaseOrderMasters select p).ToList();// where p.OutletId == WebSecurity.CurrentUserId
            List<PurchaseOrderModel> list = new List<PurchaseOrderModel>();
            List<purchaseOrderItemDetailsModel> itemList = new List<purchaseOrderItemDetailsModel>();

            foreach (var item in data)
            {
                PurchaseOrderModel model = new PurchaseOrderModel();

                var items = db.tblPurchaseOrderItems.Where(a => a.PurchaseOrderId == item.PurchaseOrderId).ToList();
                foreach (var i in items)
                {
                    purchaseOrderItemDetailsModel m = new purchaseOrderItemDetailsModel();

                    m.Name = i.tbl_RawMaterials.Name;
                    m.PurchaseOrderDetailId = i.PurchaseOrderDetailId;
                    m.Quantity = ReturnConvertValues(i.Unit, i.Quantity);
                    m.Unit = ReturnType(i.Unit);
                    itemList.Add(m);
                }
                model.getAllPurchaseOrderItems = itemList;
                model.PONo = item.PONo;
                model.PODate = item.PODate.Date;
                model.VendorId = item.VendorId;
                model.OutletId = item.OutletId;
                model.POId = item.PurchaseOrderId;
                list.Add(model);
            }
            return View(list);
        }

        public decimal ReturnConvertValues(string Unit, decimal Qty)
        {
            decimal Quantity = 0;
            if (Unit == "Gms")
            {
                Quantity = Convert.ToDecimal(Qty) / 1000;

            }
            else if (Unit == "ML")
            {
                Quantity = Convert.ToDecimal(Qty) / 1000;

            }
            else if (Unit == "Kgs")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "Ltr")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "Piece")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else
                Quantity = Convert.ToDecimal(Qty);

            return Quantity;
        }
        public string ReturnType(string Type)
        {
            string val = string.Empty;
            if (Type == "Gms")
            {
                val = "Kgs";
            }
            else if (Type == "ML")
            {
                val = "Ltr";
            }
            else if (Type == "Gms")
            {
                val = "Gms";
            }
            else if (Type == "ML")
            {
                val = "ML";
            }
            else if (Type == "Piece")
            {
                val = "Piece";
            }
            else
                val = Type;
            return val;
        }

        public ActionResult DeletepurchaseOrder(int id = 0)
        {
            try
            {
                con = new SqlConnection(webconnection);
                con.Open();
                string pp = ("select o.PurchaseOrderId from tblPurchaseMaster m inner join tblPurchaseOrderMaster o on o.PurchaseOrderId=m.PurchaseOrderId where o.PurchaseOrderId='" + id + "'");
                cmd = new SqlCommand(pp, con);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                var data = sda.Fill(dt);

                con.Close();
                if (data == 0)
                {
                    var deletepurchaseorderdata = (from p in db.tblPurchaseOrderItems where p.PurchaseOrderId.Equals(id) select p).ToList();
                    foreach (var item in deletepurchaseorderdata)
                    {
                        db.tblPurchaseOrderItems.Remove(item);
                        db.SaveChanges();

                    }

                    var deletepurchaseordermain = (from p in db.tblPurchaseOrderMasters where p.PurchaseOrderId.Equals(id) select p).FirstOrDefault();
                    db.tblPurchaseOrderMasters.Remove(deletepurchaseordermain);
                    db.SaveChanges();
                    TempData["Perror"] = "Delete Successfully !!";
                }
                else
                {
                    TempData["Perror"] = "Please Delete The GRN First !!";
                }
            }

            catch (Exception ex)
            {
                TempData["Perror"] = ex.Message;
            }
            return RedirectToAction("PurchaseReport", "PurchaseOrder");
        }
        // Update purchase stock after stock return it update master stock in purchase and insert data into purchase return table//

        //public ActionResult Delete(int id = 0)
        //{
        //    try
        //    {
        //        var deletedata = (from p in db.tblPurchaseReturns where p.PurchaseReturnId == id select p).SingleOrDefault();
        //        db.tblPurchaseReturns.Remove(deletedata);
        //        db.SaveChanges();
        //        TempData["Prerror"] = "Delete Successfully !!";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Prerror"] = ex.Message;
        //    }

        //    return RedirectToAction("ReturnPurchaseReport", "Purchase");
        //}

        public ActionResult Create(int id = 0)
        {
            var name = obj.getOutletId();
            IEnumerable<SelectListItem> venderList = (from m in db.tblVendors where m.Active == true && m.OutletId == name select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.VendorId.ToString() });
            ViewBag.venderLists = new SelectList(venderList, "Value", "Text", "venderID");
            //string category = "select * from tblVendor";


            //con = new SqlConnection(webconnection);
            //cmd = new SqlCommand(category, con);

            //DataSet ds = new DataSet();
            //List<string> li = new List<string>();
            //DataTable dt = new DataTable();
            //SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //sda.Fill(dt);
            //List<SelectListItem> list = new List<SelectListItem>();
            ////list.Add(new SelectListItem { Text = "--Choose The Item--", Value = "0" }); 
            //foreach (DataRow row in dt.Rows)
            //{

            //    list.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[1]), Value = Convert.ToString(row.ItemArray[0]) });

            //}

            //ViewBag.VendorList = list;


            IEnumerable<SelectListItem> venderDays = (from m in db.tblVendors select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Paymentcycle, Value = m.Name.ToString() });
            ViewBag.venderDays = new SelectList(venderDays, "Value", "Text", "venderID");

            IEnumerable<SelectListItem> itemList = (from n in db.tblItems where (from p in db.tblMenuOutlets where p.OutletId == name select p.ItemId).Contains(n.ItemId) select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.ItemId.ToString() });
            ViewBag.itemLists = new SelectList(itemList, "Value", "Text", "ItemId");

            int count = (from p in db.tblPurchaseOrderMasters orderby p.PurchaseOrderId descending select p).Count();
            string pono = "";
            if (count > 0)
            {
                var PO = (from p in db.tblPurchaseOrderMasters orderby p.PurchaseOrderId descending select p.PONo == null ? "" : (p.PONo)).First();

                string fy = DateTime.Now.ToFinancialYear();
                if (PO == null)
                    pono = "PO-" + "00001" + "/" + fy;
                else
                    pono = "PO-" + (Convert.ToInt32(PO.Substring(3, 5)) + 1).ToString("00000") + "/" + fy;
            }
            else
            {

                string fy = DateTime.Now.ToFinancialYear();
                pono = "PO-" + "00001" + "/" + fy;
            }
            ViewBag.PoNo = pono;
            ViewBag.PoDate = DateTime.Now;

            return View();

        }

        public JsonResult FillDropDownKeys()
        {
            var name = obj.getOutletId();

            StringBuilder strbuild = new StringBuilder();
            //var Data = (from p in db.tblItems where (from q in db.tblMenuOutlets where q.OutletId == otletid select q.ItemId).Contains(p.ItemId) select p).ToList();
            //var Data = (from p in db.tbl_KitchenRawIndent
            //where (from q in db.tblMenuOutlets
            //       where
            //           q.OutletId == name
            //       select q.ItemId).Contains(p.ItemId)
            //select new
            //{
            //    Key = p.RawMaterialId,
            //    Name = p.tbl_RawMaterials.Name
            //}).GroupBy(a => a.Key).ToList();
            var Data = (from p in db.tbl_RawMaterials
                        select new
                        {
                            Key = p.RawMaterialId,
                            Name = p.Name
                        }).GroupBy(a => a.Key).ToList();
            foreach (var key in Data)
            {
                strbuild.Append("<option value='" + key.Key + "'>" + key.FirstOrDefault().Name + "</option>");
            }
            return Json(strbuild.ToString(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Create(OutletPurchaseOrderModel model)
        {
            try
            {
                tblPurchaseOrderMaster tb = new tblPurchaseOrderMaster();

                tb.Date = DateTime.Now;
                tb.PODate = model.PODate;
                tb.PONo = model.PONo;
                tb.OutletId = obj.getOutletId(); ;
                tb.VendorId = model.VendorId;
                db.tblPurchaseOrderMasters.Add(tb);
                db.SaveChanges();
                int OutletId = obj.getOutletId();
                int Pid = (from p in db.tblPurchaseOrderMasters where p.OutletId == OutletId select p.PurchaseOrderId).Max();
                for (int i = 0; i < model.RowMaterialId.Length; i++)
                {
                    tblPurchaseOrderItem tbl = new tblPurchaseOrderItem();
                    tbl.RawMaterialId = model.RowMaterialId[i];
                    tbl.Quantity = model.Quantity[i];
                    tbl.Unit = model.Type[i];

                    tbl.PurchaseOrderId = Pid;
                    db.tblPurchaseOrderItems.Add(tbl);
                    db.SaveChanges();

                }

                TempData["Perror"] = "Inserted Successfully !";
                return RedirectToAction("PurchaseReport");
            }
            //catch (DbEntityValidationException e)
            //{
            //    TempData["Perror"] = e.Message ;
            //    return RedirectToAction("Create");
            //}
            catch
            {
                TempData["Perror"] = "Try Again !";
                return RedirectToAction("Create");
            }
        }
        public decimal ConvertValues(string Unit, decimal Qty)
        {
            decimal Quantity = 0;
            if (Unit == "Kgs")
            {
                Quantity = Convert.ToDecimal(Qty) * 1000;

            }
            else if (Unit == "Ltr")
            {
                Quantity = Convert.ToDecimal(Qty) * 1000;

            }
            else if (Unit == "Gms")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "ML")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else if (Unit == "Piece")
            {
                Quantity = Convert.ToDecimal(Qty);

            }
            else
                Quantity = Convert.ToDecimal(Qty);
            return Quantity;
        }
        public string Type(string Type)
        {
            string val = string.Empty;
            if (Type == "Kgs")
            {
                val = "Gms";
            }
            else if (Type == "Ltr")
            {
                val = "ML";
            }
            else if (Type == "Gms")
            {
                val = "Gms";
            }
            else if (Type == "ML")
            {
                val = "ML";
            }
            else if (Type == "Piece")
            {
                val = "Piece";
            }
            else
                val = Type;

            return val;
        }

        public JsonResult getPCD(string VendorId)
        {
            if (VendorId != null && VendorId != "")
            {
                int VenId = Convert.ToInt32(VendorId);
                string result = (from p in db.tblVendors
                                 where p.VendorId == VenId
                                 select p.Paymentcycle).SingleOrDefault();
                if (result == null)
                    result = "0";

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult PurchaseReport()
        {

            PurchaseOrderModel model = new PurchaseOrderModel();


            IEnumerable<SelectListItem> vend = (from n in db.tblVendors select n).AsEnumerable().Select(n =>
                new SelectListItem() { Text = n.Name, Value = n.VendorId.ToString() });
            ViewBag.vend = new SelectList(vend, "Value", "Text", "Vendor");

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();


            sb.Append("select m.PurchaseOrderId,m.PONo,m.PODate,v.Name from tblPurchaseOrderMaster m inner join tblVendor v on v.VendorId=m.VendorId");

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.List = list;
            return View(model);

        }

        [HttpPost]
        public ActionResult PurchaseReport(PurchaseOrderModel model)
        {
            IEnumerable<SelectListItem> vend = (from n in db.tblVendors select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.VendorId.ToString() });
            ViewBag.vend = new SelectList(vend, "Value", "Text", "Vendor");

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();


            sb.Append("select m.PurchaseOrderId,m.PONo,m.PODate,v.Name from tblPurchaseOrderMaster m inner join tblVendor v on v.VendorId=m.VendorId");
            if (model.Vendorname != "" && model.Vendorname != null)
            {
                sb.Append(" where v.VendorId='" + model.Vendorname + "'");
            }
            else if (model.PODate != null)
            {
                sb.Append(" where m.PODate='" + model.PODate + "'");
            }
            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.List = list;
            return View(model);

        }

        public ActionResult POBillReport(PurchaseItemDetails purchaseitem, int purchaseorderid = 0)
        {
            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();

            sb.Append("select m.PurchaseOrderId,m.PONo,m.PODate,r.Name,i.Quantity,i.Unit from tblPurchaseOrderMaster m inner join tblPurchaseOrderItem i on i.PurchaseOrderId=m.PurchaseOrderId inner join tbl_RawMaterials r on r.RawMaterialId=i.RawMaterialId where i.PurchaseOrderId='" + purchaseorderid + "'");

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.model = list;
            return View(purchaseitem);

        }

        public ActionResult printexcel(PurchaseItemDetails purchaseitem, int PurchaseOrderId)        
        {

            GenerateExcelReport(PurchaseOrderId);
            return RedirectToAction("PurchaseReport", "PurchaseOrder");
        }

        public void GenerateExcelReport(int id)
        {

            Workbook book = new Workbook();

            string str_excelfilename = "EFiling" + ".xls";
            string str_excelpath = Server.MapPath("~/Reports/") + "\\" + str_excelfilename;


            book.Properties.Author = "KS";
            book.Properties.LastAuthor = "Admin";
            book.ExcelWorkbook.WindowHeight = 4815;
            book.ExcelWorkbook.WindowWidth = 11295;
            book.ExcelWorkbook.WindowTopX = 120;
            book.ExcelWorkbook.WindowTopY = 105;

            GenerateStyles(book.Styles);

            GenerateWorksheet_printregister(book.Worksheets,id);

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

        private void GenerateWorksheet_printregister(WorksheetCollection sheets,int id)
        {

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();


            sb.Append("select m.PurchaseOrderId,m.PONo,m.PODate,v.Name from tblPurchaseOrderMaster m inner join tblVendor v on v.VendorId=m.VendorId");

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            


            Worksheet sheet = sheets.Add("EFiling");


            sheet.Table.Columns.Add(150);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(80);
            sheet.Table.Columns.Add(150);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(100);
            sheet.Table.Columns.Add(80);
            sheet.Table.Columns.Add(150);
            sheet.Table.Columns.Add(45);
            sheet.Table.Columns.Add(150);
            sheet.Table.Columns.Add(100);

            WorksheetRow Row = sheet.Table.Rows.Add();
            //Int64 serial_no = 0;
            Row.Cells.Add("No. of Recipients", DataType.String, "s56");
            Row.Cells.Add("No. of Invoices", DataType.String, "s56");
            Row.Cells.Add("", DataType.String, "s56");
            Row.Cells.Add("Total Invoice Value", DataType.String, "s56");
            Row.Cells.Add("", DataType.String, "s56");
            Row.Cells.Add("", DataType.String, "s56");
            Row.Cells.Add("", DataType.String, "s56");
            Row.Cells.Add("", DataType.String, "s56");
            Row.Cells.Add("", DataType.String, "s56");
            Row.Cells.Add("Total Taxable Value", DataType.String, "s56");
            Row.Cells.Add("Total Cess", DataType.String, "s56");


            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Row = sheet.Table.Rows.Add();
                    //serial_no = serial_no + 1;
                    //Row.Cells.Add(serial_no.ToString(), DataType.String, "s44");
                    Row.Cells.Add(dr["GSTIN"].ToString(), DataType.String, "s45");
                    Row.Cells.Add(dr["BillNo"].ToString(), DataType.String, "s45");
                    Row.Cells.Add("", DataType.String, "s49");
                    Row.Cells.Add(Convert.ToDouble(dr["grand_total"]).ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add("", DataType.String, "s45");
                    Row.Cells.Add("", DataType.String, "s45");
                    Row.Cells.Add("", DataType.String, "s45");
                    Row.Cells.Add("", DataType.String, "s45");
                    Row.Cells.Add("", DataType.String, "s45");
                    Row.Cells.Add(Convert.ToDouble(dr["total_amount"]).ToString("0.00"), DataType.String, "s45");
                    Row.Cells.Add("", DataType.String, "s45");
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

                con.Close();
            }
        }
        public ActionResult printpdf(PurchaseItemDetails purchaseitem, int PurchaseOrderId)
        {

            GenerateReport1(PurchaseOrderId);
            return RedirectToAction("PurchaseReport", "PurchaseOrder");

        }
        protected void GenerateReport1(int po)
        {
            List<PurchaseItemDetails> List = new List<PurchaseItemDetails>();
            PurchaseItemDetails model = new PurchaseItemDetails();
            //var data = db.tblPurchaseOrderItems.ToList();
            var Itemdetails = (from t1 in db.tblPurchaseOrderItems
                               join t2 in db.tblPurchaseOrderMasters on t1.PurchaseOrderId equals t2.PurchaseOrderId
                               where t1.PurchaseOrderId == po
                               select new
                               {
                                   t1.PurchaseOrderId,
                                   t1.RawMaterialId,
                                   t1.tbl_RawMaterials.Name,
                                   t1.tbl_RawMaterials.units,
                                   t1.Quantity,
                                   t2.PODate,
                                   t2.PONo,
                                   Vendorname = t2.tblVendor.Name,
                                   t2.tblVendor.Address,
                                   t2.tblVendor.ContactA,
                                   t2.tblVendor.GSTin,
                                   t2.tblVendor.TinNo,
                                   t2.tblVendor.AccountNumber,
                                   t2.tblVendor.Bank,
                                   t2.tblVendor.Branch,
                                   t2.tblVendor.IfscCode
                               }).ToList();

            foreach (var item in Itemdetails)
            {
                model.PurchaseOrderId = item.PurchaseOrderId;
                model.RawMaterialId = item.RawMaterialId;
                model.Name = item.Name;
                model.Quantity = item.Quantity;
                model.Unit = item.units;
                model.PODate = item.PODate;
                model.PONo = item.PONo;
                model.VendorName = item.Vendorname;
                model.Address = item.Address;
                model.Contact = item.ContactA;
                model.GSTNO = item.GSTin;
                model.Accountnumber = item.AccountNumber;
                model.bank = item.Bank;
                model.branch = item.Branch;
                model.ifsc = item.IfscCode;
            }

            iTextSharp.text.Font Font8 = FontFactory.GetFont("Verdana", 8);
            iTextSharp.text.Font Font8B = FontFactory.GetFont("Verdana", 8, 1);
            iTextSharp.text.Font Font9 = FontFactory.GetFont("Verdana", 9);
            iTextSharp.text.Font Font9B = FontFactory.GetFont("Verdana", 9, 1);
            iTextSharp.text.Font Font9Bc = FontFactory.GetFont("Verdana", 9, CMYKColor.RED);
            iTextSharp.text.Font Font10 = FontFactory.GetFont("Verdana", 10);
            iTextSharp.text.Font Font10B = FontFactory.GetFont("Verdana", 10, 1);
            iTextSharp.text.Font Font10U = FontFactory.GetFont("Verdana", 10, 2);
            iTextSharp.text.Font Font11 = FontFactory.GetFont("Verdana", 11);
            iTextSharp.text.Font Font11B = FontFactory.GetFont("Verdana", 11, 1);
            iTextSharp.text.Font Font12 = FontFactory.GetFont("Verdana", 12);
            iTextSharp.text.Font Font12B = FontFactory.GetFont("Verdana", 12, 1);
            iTextSharp.text.Font Font14 = FontFactory.GetFont("Verdana", 14);
            iTextSharp.text.Font Font14A = FontFactory.GetFont("Arial Black", 22, Font.BOLDITALIC);
            iTextSharp.text.Font Font15 = FontFactory.GetFont("Verdana", 12, Font.BOLDITALIC);
            iTextSharp.text.Font Font15A = FontFactory.GetFont("Verdana", 10, Font.BOLD);


            string str_pdffilename = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_").Replace(" ", "_") + " PurchaseOrder.pdf";
            
            
            //if (File.Exists(str_excelpath) == true)
            //{
            //    File.Delete(str_excelpath);
            //}



            string str_pdfpath = Server.MapPath("~/Reports/") + str_pdffilename; //"D:\\CDS\\MainProject\\NibsMVC\\Reports" + "\\" + str_pdffilename;//
            //System.IO.FileStream fs = new FileStream(str_pdfpath, FileMode.OpenOrCreate,FileAccess.Write);
            //System.IO.FileStream fs1 = new FileStream(str_pdfpath, FileMode.Open, FileAccess.Read);
            Document doc = new Document(PageSize.A4, 80, 80, 55, 25);


            PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
            doc.Open();

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(6);
            table.WidthPercentage = 100;
            float[] intwidth = new float[6] { 0.4f, 1, 1, 1, 0.8f, 0.5f };
            table.SetWidths(intwidth);
            table.DefaultCell.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0, 0);
            table.DefaultCell.BorderWidth = 1;

            table.SpacingAfter = 1;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellPdf = new iTextSharp.text.pdf.PdfPCell();

            //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("DAILLY BILLING REPORT", Font12B));
            //cellPdf.Border = 0;
            //cellPdf.Colspan = 6;
            //cellPdf.SetLeading(1.5F, 1.5F);
            //cellPdf.BorderWidthBottom = 0;
            //cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
            //cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            //table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Purchase Order", Font14A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.BorderWidthBottom = 1F;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 0;
            //cellPdf.Rowspan = 2;
            cellPdf.SetLeading(2.5F, 2.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Supplier Details: ", Font15));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);
                                                                                            //D:\CDS\MainProject\NibsMVC\Content\Logo\Kandha - Logo - 100x44.png\
            string imageURL = Server.MapPath("~/Content\\Logo\\Kandha-Logo-100x44.png");/*"D:\\CDS\\MainProject\\NibsMVC\\Content\\Logo\\Kandha-Logo-100x44.png"*/;
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            //cellPdf = new iTextSharp.text.Image.GetInstance(imageURL);
            cellPdf = new iTextSharp.text.pdf.PdfPCell((Image.GetInstance(jpg)));
            jpg.ScaleToFit(3F, 1.5F);
            jpg.SpacingBefore = 2F;
            jpg.Alignment = Element.ALIGN_RIGHT;
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 4;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(model.VendorName, Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 3;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("435 & 434 Trichy Road,Opp.Vasantha Mills,Singanallur,Coimbatore-641005.", Font15A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 4;

            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(model.Address, Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Contact: " + model.Contact + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Phone:0422-4213134", Font15A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Supplier Bank Details: ", Font15));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("ACC-No:" + model.Accountnumber + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Bank Name:" + model.bank + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Branch:" + model.branch + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("IFSC Code:" + model.ifsc + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Purchase ID: " + model.PONo + "", Font15A));

            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("GSTIN: " + model.GSTNO + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Purchase Date:" + model.PODate.ToString("dd-MM-yyyy") + "", Font15A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 3;
            //cellPdf.Rowspan = 4;
            cellPdf.BorderWidthBottom = 0F;
            cellPdf.BorderWidthLeft = 0F;
            cellPdf.BorderWidthRight = 0F;
            cellPdf.BorderWidthTop = 0F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);


            //cellPdf.Border = 0;
            //cellPdf.Colspan = 3;
            //cellPdf.BorderWidthBottom = 0F;
            //cellPdf.BorderWidthLeft = 0F;
            //cellPdf.BorderWidthRight = 0F;
            //cellPdf.BorderWidthTop = 0F;
            //cellPdf.SetLeading(1, 1);
            //cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            //table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("SI.No", Font9B));
            cellPdf.Border = 1;
            cellPdf.Colspan = 1;
            //cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0.5F;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Item Name", Font9B));
            cellPdf.Border = 1;
            cellPdf.Colspan = 3;
            //cellPdf.Rowspan = 2;
            cellPdf.BorderWidthBottom = 0.5F;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Order Quantity", Font9B));
            cellPdf.Border = 1;
            cellPdf.Colspan = 1;
            //cellPdf.Rowspan = 2;
            cellPdf.BorderWidthBottom = 0.5F;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Unit", Font9B));
            cellPdf.Border = 0;
            cellPdf.Colspan = 1;
            //cellPdf.Rowspan = 2;
            cellPdf.BorderWidthBottom = 0.5F;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1.5F, 1F);
            cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Service Tax", Font9B));
            //cellPdf.Border = 0;
            //cellPdf.Colspan = 1;
            //cellPdf.BorderWidthBottom = 0.5F;
            //cellPdf.BorderWidthLeft = 0.5F;
            ////cellPdf.BorderWidthRight = 0.5F;
            //cellPdf.BorderWidthTop = 0.5F;
            //cellPdf.SetLeading(1.5F, 1.5F);
            //cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
            //cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            //table.AddCell(cellPdf);

            //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Bill Amt", Font9B));
            //cellPdf.Border = 0;
            //cellPdf.Colspan = 1;
            //cellPdf.BorderWidthBottom = 0.5F;
            //cellPdf.BorderWidthLeft = 0.5F;
            //cellPdf.BorderWidthRight = 0.5F;
            //cellPdf.BorderWidthTop = 0.5F;
            //cellPdf.SetLeading(1.5F, 1.5F);
            //cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
            //cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            //table.AddCell(cellPdf);


            //1--------------------------------------------------------------------------------------------------------------------
            int serno = 1;
            //string customer = "", billno = "", billtype = "", billby = "", bookingno = "";
            //Decimal temptotal = 0, total = 0, A = 0, B = 0;
            if (Itemdetails.Count > 0)
            {
                foreach (var dr in Itemdetails)
                {
                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(serno.ToString() + ".", Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 1F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.Name, Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 3;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);
                    //3--------------------------------------------------------------------------------------------------------------------
                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.Quantity.ToString(), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.units.ToString(), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_CENTER;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);
                    
                    serno++;
                }
                
                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
                cellPdf.Border = 0;
                cellPdf.Colspan = 2;
                //cellPdf.Rowspan = 2;
                cellPdf.BorderWidthBottom = 0;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 1F;
                cellPdf.SetLeading(1, 1);
                cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
                cellPdf.Border = 0;
                cellPdf.Colspan = 2;
                //cellPdf.Rowspan = 2;
                cellPdf.BorderWidthBottom = 0;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 1F;
                cellPdf.SetLeading(1, 1);
                cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
                cellPdf.Border = 0;
                cellPdf.Colspan = 2;
                //cellPdf.Rowspan = 2;
                cellPdf.BorderWidthBottom = 0;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 1F;
                cellPdf.SetLeading(1, 1);
                cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
                cellPdf.Border = 0;
                cellPdf.Colspan = 5;
                cellPdf.BorderWidthBottom = 0.5F;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(2.5F, 2.5F);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
                cellPdf.Border = 0;
                cellPdf.Colspan = 1;
                cellPdf.BorderWidthBottom = 0.5F;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(2.5F, 2.5F);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);
            }

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
            cellPdf.Border = 0;
            cellPdf.Colspan = 2;
            //cellPdf.Rowspan = 2;
            cellPdf.BorderWidthBottom = 0;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
            cellPdf.Border = 0;
            cellPdf.Colspan = 2;
            //cellPdf.Rowspan = 2;
            cellPdf.BorderWidthBottom = 0;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
            cellPdf.Border = 0;
            cellPdf.Colspan = 2;
            //cellPdf.Rowspan = 2;
            cellPdf.BorderWidthBottom = 0;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cellPdf);


            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Prepared By:", Font9Bc));
            cellPdf.Border = 0;
            cellPdf.Colspan = 2;
            //cellPdf.Rowspan = 4;
            cellPdf.BorderWidthBottom = 0;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 0F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Checked By:", Font9Bc));
            cellPdf.Border = 0;
            cellPdf.Colspan = 2;
            //cellPdf.Rowspan = 4;
            cellPdf.BorderWidthBottom = 0;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 0F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Approved By:", Font9Bc));
            cellPdf.Border = 0;
            cellPdf.Colspan = 2;
            //cellPdf.Rowspan = 4;
            cellPdf.BorderWidthBottom = 0F;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 0;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            doc.Add(table);
            //doc.Close();

            writer.CloseStream = false;
            doc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename="+model.VendorName+" - Purchase Order Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(doc);
            Response.End();
            //System.IO.FileStream fs1 = new FileStream(str_pdfpath, FileMode.Open, FileAccess.Read);
            //Document doc1 = new Document(PageSize.A4, 80, 80, 55, 25);
            //PdfWriter writer1 = PdfWriter.GetInstance(doc1, fs);

            //GetPDF(model.PurchaseOrderId);
            string script = string.Format(@"showDialogfile('{0}')", str_pdffilename);
            //ScriptManager.RegisterClientScriptBlock(this, typeof(Page), UniqueID, script, true);
        }
        //public FileStreamResult GetPDF(int id)
        //{
            
        //    string str_pdffilename = id+ " PurchaseOrder.pdf";
        //    string str_excelpath = Server.MapPath("~/Reports/") + str_pdffilename;
        //    FileStream fs = new FileStream(str_excelpath, FileMode.Open, FileAccess.Read);
           
        //    return File(fs, "application/pdf");
        //}

    }
}