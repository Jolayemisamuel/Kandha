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



namespace NibsMVC.Controllers
{
    public class PurchaseOrderController : Controller
    {
        NIBSEntities db = new NIBSEntities();
        XMLTablesRepository obj = new XMLTablesRepository();
        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        SqlConnection con;
        SqlCommand cmd;
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
                model.PONo  = item.PONo;
                model.PODate = item.PODate.Date;
                model.VendorId = item.VendorId;
                model.OutletId = item.OutletId;
                model.POId  = item.PurchaseOrderId ;
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

        public ActionResult Create(int id=0)
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
               
                tb.PODate = model.PODate;
                tb.PONo = model.PONo;
                tb.OutletId = obj.getOutletId(); ;
                tb.VendorId = model.VendorId;
                db.tblPurchaseOrderMasters.Add(tb);
                db.SaveChanges();
                int OutletId = obj.getOutletId();
                int Pid = (from p in db.tblPurchaseOrderMasters where p.OutletId == OutletId select p.PurchaseOrderId ).Max();
                for (int i = 0; i < model.RowMaterialId.Length; i++)
                {
                    tblPurchaseOrderItem tbl = new tblPurchaseOrderItem();
                    tbl.RawMaterialId = model.RowMaterialId[i];
                    tbl.Quantity = model.Quantity[i];
                    tbl.Unit = model.Type[i];
                
                    tbl.PurchaseOrderId  = Pid;
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
            if(model.Vendorname!=""&&model.Vendorname!=null)
            {
            sb.Append(" where v.VendorId='"+model.Vendorname+"'");
            }
            else if(model.PODate!=null)
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

    }
}