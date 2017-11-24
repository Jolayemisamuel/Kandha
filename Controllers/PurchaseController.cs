using NibsMVC.EDMX;
using NibsMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Web.Security;
using NibsMVC.Repository;
using System.Web.Script.Serialization;
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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace NibsMVC.Controllers
{
    public class PurchaseController : Controller
    {
        //
        // GET: /Purchase/
        NIBSEntities db = new NIBSEntities();
        XMLTablesRepository obj = new XMLTablesRepository();
        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        SqlConnection con;
        SqlCommand cmd;


        public ActionResult Index()
        {

            var data = (from p in db.tblPurchaseMasters select p).ToList();//where p.OutletId == WebSecurity.CurrentUserId
            List<PurchaseModel> list = new List<PurchaseModel>();
            List<purchaseItemDetailsModel> itemList = new List<purchaseItemDetailsModel>();

            IEnumerable<SelectListItem> vend = (from n in db.tblVendors select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.VendorId.ToString() });
            ViewBag.vend = new SelectList(vend, "Value", "Text", "Vendor");


            foreach (var item in data)
            {
                PurchaseModel model = new PurchaseModel();

                var items = db.tblPurchasedItems.Where(a => a.PurchaseId == item.PurchaseId).ToList();
                foreach (var i in items)
                {
                    purchaseItemDetailsModel m = new purchaseItemDetailsModel();
                    m.Amount = Math.Round(i.Amount, 2);
                    m.Name = i.tbl_RawMaterials.Name;
                    m.PurchaseDetailId = i.PurchaseDetailId;
                    m.Quantity = ReturnConvertValues(i.Unit, i.Quantity);
                    m.Unit = ReturnType(i.Unit);
                    itemList.Add(m);
                }
                model.getAllPurchaseItems = itemList;
                model.InvoiceNo = item.InvoiceNo;
                model.InvoiceDate = item.InvoiceDate.Date;
                model.TotalAmount = Math.Round(item.TotalAmount, 2);
                model.Tax = item.Tax;
                model.DepositeAmount = Math.Round(Convert.ToDecimal(item.DepositeAmount.Value), 2);
                model.RemainingAmount = Math.Round(Convert.ToDecimal(item.RemainingAmount.Value), 2);
                model.NetAmount = Math.Round(Convert.ToDecimal(item.NetAmount), 2);
                model.ExtraCharge = Math.Round(Convert.ToDecimal(item.ExtraCharge), 2);
                model.ExtraChargeDetail = item.ExtraChargeDetails;
                model.PaymentMode = item.PaymenyMode;
                //model.VendorId = item.VendorId;
                model.OutletId = item.OutletId;
                model.PurchaseId = item.PurchaseId;
                model.Remarks = item.Remarks;
                model.ChequeNo = item.ChequeNo;
                list.Add(model);
            }
            return View(list);
        }


        public ActionResult Deletepurchase(int id = 0, int purchaseorderid = 0)
        {

            try
            {
                var deletepurchasedata = (from p in db.tblPurchasedItems where p.PurchaseId.Equals(id) select p).ToList();
                var deletepurchasemain = (from p in db.tblPurchaseMasters where p.PurchaseId.Equals(id) select p).FirstOrDefault();
                foreach (var item in deletepurchasedata)
                {
                    db.tblPurchasedItems.Remove(item);
                    db.SaveChanges();
                    int OutletId = obj.getOutletId();
                    foreach (var items in db.tbl_KitchenStock.Where(a => a.OutletId == OutletId && a.RawMaterialId == item.RawMaterialId).ToList())
                    {

                        string qry = "update tbl_KitchenStock set Quantity  = Quantity - " + item.Quantity + " where  RawMaterialId=" + item.RawMaterialId;
                        qry = qry + " delete from  tblGRNStock where qty  = " + item.Quantity + " and  MaterialId=" + item.RawMaterialId + " and date='" + deletepurchasemain.InvoiceDate.ToString("dd-MMM-yyyy") + "'";

                        con = new SqlConnection(webconnection);
                        cmd = new SqlCommand(qry, con);
                        cmd.CommandType = CommandType.Text;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        //db.tbl_KitchenStock.Remove(items);
                        //db.SaveChanges();
                    }
                }


                db.tblPurchaseMasters.Remove(deletepurchasemain);
                db.SaveChanges();
                TempData["Perror"] = "Delete Successfully !!";
            }


            catch (Exception ex)
            {
                TempData["Perror"] = ex.Message;
            }

            return RedirectToAction("Report", "Purchase");
        }
        // Update purchase stock after stock return it update master stock in purchase and insert data into purchase return table//

        public ActionResult Delete(int id = 0)
        {
            try
            {
                var deletedata = (from p in db.tblPurchaseReturns where p.PurchaseReturnId == id select p).SingleOrDefault();
                db.tblPurchaseReturns.Remove(deletedata);
                db.SaveChanges();
                TempData["Prerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Prerror"] = ex.Message;
            }

            return RedirectToAction("ReturnPurchaseReport", "Purchase");
        }

        public ActionResult ReturnPurchaseReport()
        {
            var name = obj.getOutletId();
            var returndata = (from p in db.tblPurchaseReturns where p.OutletId == name select p).ToList();
            List<PurchaseReturnModel> list = new List<PurchaseReturnModel>();
            foreach (var item in returndata)
            {
                PurchaseReturnModel model = new PurchaseReturnModel();
                model.VendorId = item.vendorId;
                model.ItemName = item.tbl_RawMaterials.Name;
                model.RawMaterialId = item.RawMaterialId;
                model.ReturnDate = item.ReturnDate;
                model.ReturnDescription = item.ReturnDescription;
                model.ReturnQuantity = item.ReturnQuantity;
                model.Unit = item.tbl_RawMaterials.units;
                model.ReturnStatuss = item.ReturnStatuss;
                model.PurchaseReturnId = item.PurchaseReturnId;
                list.Add(model);
            }
            return View(list);
        }

        public JsonResult SetPurchaseQuantity(string Id)
        {
            var name = WebSecurity.CurrentUserName;
            var outletids = (from p in db.tblOperators where p.Name.Equals(name) select p.OutletId).FirstOrDefault();
            if (Id != null && Id != "")
            {
                string[] str = Id.Split(';');
                for (var i = 0; i < str.Length; i++)
                {
                    string[] arr = str[i].Split('^');
                    int purchasedetailid = Convert.ToInt32(arr[1]);
                    tblPurchasedItem tbl = db.tblPurchasedItems.Where(a => a.PurchaseDetailId == purchasedetailid).FirstOrDefault();
                    tbl.Quantity = Convert.ToInt32(arr[2]);
                    tblPurchaseReturn obj = new tblPurchaseReturn();
                    obj.RawMaterialId = Convert.ToInt32(arr[3]);
                    obj.vendorId = Convert.ToInt32(arr[0]);
                    obj.ReturnQuantity = Convert.ToInt32(arr[4]);
                    obj.ReturnDate = DateTime.Now.Date;
                    obj.ReturnStatuss = "confirm";
                    obj.ReturnDescription = arr[5];
                    obj.OutletId = Convert.ToInt32(outletids);
                    db.tblPurchaseReturns.Add(obj);
                    db.SaveChanges();
                }

                return Json("Update Succefully", JsonRequestBehavior.AllowGet);
            }

            return Json("Not Update", JsonRequestBehavior.AllowGet);
        }


        public ActionResult PurchaseReturn(int purchaseid = 0, int itemid = 0, int purchasdetailId = 0)
        {
           
            var ReturnData = (from p in db.tblPurchasedItems
                              where p.PurchaseDetailId == purchasdetailId
                              select p).SingleOrDefault();

            decimal Returned = (from q in db.tblPurchaseReturns
                                where (q.vendorId == purchasdetailId && q.RawMaterialId == ReturnData.RawMaterialId)
                                select (decimal?)q.ReturnQuantity).Sum() ?? 0;

            OutletPurchageReturnModel model = new OutletPurchageReturnModel();
            model.Amount = ReturnData.Amount;
            model.Purchasedetailid = ReturnData.PurchaseDetailId;
            model.Purchaseid = ReturnData.PurchaseId;
            model.OutletId = obj.getOutletId();
            model.Quantity = ReturnData.Quantity- Returned;
            model.Type = ReturnType(ReturnData.Unit);
            model.VendorId = purchasdetailId;
            model.RowMaterialId = ReturnData.RawMaterialId;
            //model.VenderName = ReturnData.tblPurchaseMaster.tblVendor.Name;
            model.RowMaterialName = ReturnData.tbl_RawMaterials.Name;

            return View(model);
        }
        [HttpPost]
        public ActionResult PurchaseReturn(OutletPurchageReturnModel model)
        {
            if (model.Quantity >= model.ReturnQuantity)
            {
                var Data = (from p in db.tblPurchasedItems
                            where p.PurchaseDetailId == model.Purchasedetailid
                            select p).SingleOrDefault();
                decimal perticularAmount = Data.Amount / Data.Quantity;
                decimal returnAmount = perticularAmount * model.ReturnQuantity;
                tblPurchaseReturn tb = new tblPurchaseReturn();
                tb.OutletId = obj.getOutletId();
                tb.RawMaterialId = model.RowMaterialId;
                tb.ReturnDate = DateTime.Now.Date;
                tb.ReturnDescription = model.Reasion;
                tb.ReturnQuantity = model.ReturnQuantity;
                tb.vendorId = model.Purchasedetailid;
                tb.ReturnStatuss = "Success";
                db.tblPurchaseReturns.Add(tb);
                //db.SaveChanges();
                //remaining stock
                int Outlet = obj.getOutletId();
                decimal remaingquantity = model.Quantity - model.ReturnQuantity;

                tbl_KitchenStock DataStock = (from p in db.tbl_KitchenStock where p.RawMaterialId == model.RowMaterialId && p.OutletId == Outlet select p).SingleOrDefault();
                //tblPurchasedItem datastockpurchase = (from q in db.tblPurchasedItems where q.PurchaseDetailId == model.Purchasedetailid select q).SingleOrDefault();
                //tblPurchaseMaster datastockpurchaseMst = (from q in db.tblPurchaseMasters where q.PurchaseId == model.Purchaseid select q).SingleOrDefault();
                //DateTime grnStckdate = Convert.ToDateTime(datastockpurchaseMst.Date.ToShortDateString());
                //tblGRNStock datastockgrn = (from r in db.tblGRNStocks where r.MaterialId == model.RowMaterialId && r.Date== grnStckdate && r.Qty== datastockpurchase.Quantity select r).SingleOrDefault();
                //tblPurchaseMaster datastockmaster = (from s in db.tblPurchaseMasters where s.PurchaseId == model.Purchaseid select s).SingleOrDefault();
                DataStock.Quantity = DataStock.Quantity - model.ReturnQuantity;
                // datastockpurchase.Quantity = remaingquantity;
                // datastockpurchase.Amount = datastockpurchase.Amount - returnAmount;
                // datastockmaster.TotalAmount = datastockmaster.TotalAmount - returnAmount;
                // decimal taxrate = returnAmount / datastockpurchase.TaxPer;
                // datastockmaster.Tax = datastockmaster.Tax - taxrate;
                // datastockmaster.NetAmount = datastockmaster.TotalAmount + datastockmaster.Tax;

                //datastockgrn.Qty = remaingquantity;
                db.SaveChanges();
                TempData["Prerror"] = "Returned Successfully";
            }

            else
                TempData["Prerror"] = "UnSuccessful ! Return Qty Exceeds";

            return RedirectToAction("ReturnPurchaseReport");
        }
        [Authorize]
        public ActionResult Create()
        {
            var name = obj.getOutletId();
            IEnumerable<SelectListItem> venderList = (from m in db.tblVendors where m.Active == true && m.OutletId == name select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.VendorId.ToString() });
            ViewBag.venderLists = new SelectList(venderList, "Value", "Text", "venderID");

            //IEnumerable<SelectListItem> itemList = (from n in db.tblItems select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });// where (from p in db.tblMenuOutlets where p.OutletId == name select p.ItemId).Contains(n.ItemId)
            //ViewBag.itemLists = new SelectList(itemList, "Value", "Text", "ItemId");

            IEnumerable<SelectListItem> poList = (from p in db.tblPurchaseOrderMasters where !db.tblPurchaseMasters.Any(PurchaseId => PurchaseId.PurchaseOrderId == p.PurchaseOrderId) select p).AsEnumerable().Select(p => new SelectListItem() { Text = p.PONo, Value = p.PurchaseOrderId.ToString() });
            //IEnumerable<SelectListItem> polist1 = (from p in db.tblPurchaseOrderMasters where (db.tblPurchaseMasters.Any(PurchaseId => PurchaseId.PurchaseOrderId != p.PurchaseOrderId)) select p)

            ViewBag.poLists = new SelectList(poList, "Value", "Text", "PurchaseOrderId");
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
        public JsonResult FillDropDownKeysByCategory(String CategoryId)
        {
            var name = obj.getOutletId();
            var catid = Convert.ToInt32(CategoryId);

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
                        where p.rawcategoryId == catid
                        select new
                        {
                            Key = p.RawMaterialId,
                            Name = p.Name
                        }).GroupBy(a => a.Key).ToList();
            strbuild.Append("<option value='--Select Item--'> --Select Item-- </option>");
            foreach (var key in Data)
            {
                strbuild.Append("<option value='" + key.Key + "'>" + key.FirstOrDefault().Name + "</option>");
            }
            return Json(strbuild.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillVendorDropDown()
        {
            var name = obj.getOutletId();

            StringBuilder strbuild = new StringBuilder();

            var Data = (from p in db.tblVendors
                        select new
                        {
                            Key = p.VendorId,
                            Name = p.Name
                        }).GroupBy(a => a.Key).ToList();

            strbuild.Append("<option value='--Select Category--'> --Select Category-- </option>");
            foreach (var key in Data)
            {

                strbuild.Append("<option value='" + key.Key + "'>" + key.FirstOrDefault().Name + "</option>");
            }
            return Json(strbuild.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult FillDropDownCategories(string VendorId)
        {

            var name = obj.getOutletId();
            if (VendorId == "")
                VendorId = "0";
            int venid = Convert.ToInt32(VendorId);


            var Data = (from p in db.RawCategories
                        join
                            q in db.AddCategoryVendors on p.RawCategoryID equals q.RawCategoryId
                        where q.VendorId == venid
                        select new
                        {
                            Key = q.RawCategoryId,
                            Name = p.Name
                        }).GroupBy(a => a.Key).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            //strbuild.Append("<option value='--Select Category--'> --Select Category-- </option>");
            foreach (var key in Data)
            {
                list.Add(new SelectListItem { Text = Convert.ToString(key.FirstOrDefault().Name), Value = Convert.ToString(key.Key) });

            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        [HttpPost]
        public ActionResult Create(OutletPurchageModel model)
        {
            try
            {
                if (model.InvoiceNo == 0)
                {
                    TempData["Perror"] = "Enter Invoice No !";
                    //return View(model);
                    return RedirectToAction("Create");
                }
                else
                {
                    tblPurchaseMaster tb = new tblPurchaseMaster();

                    tb.Date = DateTime.Now;
                    tb.PurchaseId = model.PurchaseId;
                    tb.ChequeNo = model.ChequeNo.ToString();
                    tb.DepositeAmount = model.DepositeAmount;
                    tb.ExtraCharge = model.ExtraCharge;
                    tb.ExtraChargeDetails = model.ExtraChargeDetail;
                    tb.InvoiceDate = model.InvoiceDate;
                    tb.InvoiceNo = model.InvoiceNo;
                    tb.NetAmount = model.NetAmount;
                    tb.OutletId = obj.getOutletId(); ;
                    tb.TotalAmount = model.TotalAmount;
                    tb.PaymenyMode = model.PaymentMode;
                    tb.RemainingAmount = model.RemainingAmount;
                    tb.Tax = model.Tax;
                    tb.VendorId = model.VendorId;
                    ;
                    tb.PurchaseOrderId = model.PurchaseOrderId;
                    tb.Remarks = model.Remarks;
                    db.tblPurchaseMasters.Add(tb);
                    db.SaveChanges();
                    int OutletId = obj.getOutletId();
                    int Pid = (from p in db.tblPurchaseMasters where p.OutletId == OutletId select p.PurchaseId).Max();
                    for (int i = 0; i < model.Type.Length; i++)
                    {
                        tblPurchasedItem tbl = new tblPurchasedItem();
                        tblGRNStock tblGrn = new tblGRNStock();

                        tblGrn.Date = model.InvoiceDate;
                        tbl.RawMaterialId = model.RowMaterialId[i];
                        tblGrn.MaterialId = model.RowMaterialId[i];

                        string unit = getMaterialUnit(model.RowMaterialId[i].ToString());

                        if (unit != model.Type[i].ToString())
                        {
                            tbl.Quantity = ConvertValues(model.Type[i], model.Quantity[i]);
                            tblGrn.Qty = ConvertValues(model.Type[i], model.Quantity[i]);
                            tblGrn.IssQty = 0;
                            tbl.Unit = Type(model.Type[i]);
                        }
                        else
                        {
                            tbl.Quantity = model.Quantity[i];
                            tblGrn.Qty = model.Quantity[i];
                            tblGrn.IssQty = 0;
                            tbl.Unit = model.Type[i];
                        }


                        tbl.Amount = model.txt_Amount[i];
                        tbl.PurchaseId = Pid;
                        tbl.TaxPer = model.TaxPer[i];

                        tblGrn.Rate = (tbl.Amount + (tbl.Amount * (tbl.TaxPer / 100))) / tbl.Quantity;

                        db.tblGRNStocks.Add(tblGrn);
                        db.tblPurchasedItems.Add(tbl);
                        db.SaveChanges();
                        tbl_KitchenStock stock = new tbl_KitchenStock();
                        var stockdata = (from l in db.tbl_KitchenStock where l.OutletId == OutletId select new { l.RawMaterialId, l.Quantity }).ToList();
                        int flag = 0;
                        foreach (var item in stockdata)
                        {
                            if (item.RawMaterialId == Convert.ToInt32(model.RowMaterialId[i]))
                            {
                                flag = 1;
                            }

                        }
                        if (flag == 0)
                        {
                            stock.OutletId = obj.getOutletId();
                            stock.RawMaterialId = Convert.ToInt32(model.RowMaterialId[i]);
                            // string unit = getMaterialUnit(model.RowMaterialId[i].ToString());

                            if (unit != model.Type[i].ToString())
                            {
                                tbl.Quantity = ConvertValues(model.Type[i], model.Quantity[i]);
                                tbl.Unit = Type(model.Type[i]);
                            }
                            else
                            {
                                stock.Quantity = model.Quantity[i];
                                stock.Unit = model.Type[i];
                            }

                            //stock.Unit = Type(model.Type[i]);
                            //stock.Quantity = ConvertValues(model.Type[i], model.Quantity[i]);
                            db.tbl_KitchenStock.Add(stock);
                            db.SaveChanges();
                        }
                        else if (flag == 1)
                        {

                            int rawmatId = Convert.ToInt32(model.RowMaterialId[i]);
                            var stockhavingdata = (from p in db.tbl_KitchenStock where p.RawMaterialId == rawmatId select p).SingleOrDefault(); //&& p.OutletId == WebSecurity.CurrentUserId
                            if (unit != model.Type[i].ToString())
                            {
                                stockhavingdata.Quantity = Convert.ToDecimal(stockhavingdata.Quantity + ConvertValues(model.Type[i], model.Quantity[i]));
                            }
                            else
                            {
                                stockhavingdata.Quantity = Convert.ToDecimal(stockhavingdata.Quantity + model.Quantity[i]);
                            }

                            db.SaveChanges();
                        }
                    }

                    TempData["Perror"] = "Inserted Successfully !";
                    return RedirectToAction("Report");
                }
            }
            catch
            {
                TempData["Perror"] = "Try Again !";
                return RedirectToAction("Create");
            }
        }
        public string getMaterialUnit(string MaterialId)
        {
            int MatId = Convert.ToInt32(MaterialId);
            string result = (from p in db.tbl_RawMaterials
                             where p.RawMaterialId == MatId
                             select p.units).SingleOrDefault();
            return result.ToString();
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

        // retrun convert values
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


        public string getVendor(string Id = "0")
        {
            int PoId = Convert.ToInt32(Id);
            string result = (from p in db.tblPurchaseOrderMasters where p.PurchaseOrderId == PoId select p.VendorId == null ? 0 : (p.VendorId)).SingleOrDefault().ToString();
            return result;

        }

        public string getPoItems(string Id = "0")
        {
            var jsonSerialiser = new JavaScriptSerializer();


            int PoId = Convert.ToInt32(Id);
            var result = (from p in db.tblPurchaseOrderItems
                          where p.PurchaseOrderId == PoId
                          select
                          new
                          {
                              p.PurchaseOrderId,
                              p.RawMaterialId,
                              Quantity = (p.Unit == "Gms" || p.Unit == "ML") ? p.Quantity / 1000 : p.Quantity,
                              Unit = p.Unit == "Gms" ? "Kgs" : p.Unit == "ML" ? "Ltr" : p.Unit,
                              p.tbl_RawMaterials.Name
                          }
                ).ToList();
            string json = jsonSerialiser.Serialize(result);
            return json;

        }

        [HttpGet]
        public ActionResult Report()
        {

            PurchaseModel model = new PurchaseModel();


            IEnumerable<SelectListItem> vend = (from n in db.tblVendors select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.VendorId.ToString() });
            ViewBag.vend = new SelectList(vend, "Value", "Text", "Vendor");


            StringBuilder sb = new StringBuilder();


            sb.Append("select pm.PurchaseId,pm.InvoiceNo,pm.InvoiceDate,v.Name,pm.TotalAmount,pm.Tax,pm.ExtraCharge,pm.ExtraChargeDetails,pm.NetAmount,pm.DepositeAmount,pm.RemainingAmount,pm.Remarks,pm.PaymenyMode,pm.ChequeNo,pm.PurchaseOrderId from tblPurchaseMaster pm inner join tblVendor v on v.VendorId=pm.VendorId");

            con = new SqlConnection(webconnection);
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
        public ActionResult Report(PurchaseModel model)
        {



            IEnumerable<SelectListItem> vend = (from n in db.tblVendors select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.VendorId.ToString() });
            ViewBag.vend = new SelectList(vend, "Value", "Text", "Vendor");


            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();


            sb.Append("select pm.PurchaseId,pm.InvoiceNo,pm.InvoiceDate,v.Name,pm.TotalAmount,pm.Tax,pm.ExtraCharge,pm.ExtraChargeDetails,pm.NetAmount,pm.DepositeAmount,pm.RemainingAmount,pm.Remarks,pm.PaymenyMode,pm.ChequeNo,pm.PurchaseOrderId from tblPurchaseMaster pm inner join tblVendor v on v.VendorId=pm.VendorId");
            if (model.Vendorname != "" && model.Vendorname != null)
            {
                sb.Append(" Where v.VendorId= '" + model.Vendorname + "'");
            }
            else if (model.FromDate != null && model.ToDate != null)
            {
                sb.Append(" where pm.InvoiceDate between '" + model.FromDate + "' and '" + model.ToDate + "'");
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
        public ActionResult VendorDetails(VendorItemDetails vend, int purchaseid = 0)
        {
            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();

            sb.Append("select i.RawMaterialId,r.Name,i.Unit,i.Quantity,i.Amount,i.TaxPer,m.NetAmount,v.Name,m.PurchaseId,m.InvoiceDate,m.InvoiceNo from tblPurchasedItem i inner join tblPurchaseMaster m on m.PurchaseId=i.PurchaseId inner join tbl_RawMaterials r on r.RawMaterialId=i.RawMaterialId inner join tblVendor v on v.VendorId=m.VendorId where i.PurchaseId='" + purchaseid + "'");

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.model = list;
            return View(vend);

        }

        protected ActionResult printexcel()
        {

            GenerateExcelReport();
            //string str_excelfilename = hidprint.Value.Trim();
            //string script = string.Format(@"showDialogfile('{0}')", str_excelfilename);
            //ScriptManager.RegisterClientScriptBlock(this, typeof(Page), UniqueID, script, true);
            //btnprint.Text = "View";            
            return View();
            //return Json ("Print".ToString(), JsonRequestBehavior.AllowGet);
        }

        public void GenerateExcelReport()
        {

            Workbook book = new Workbook();

            string str_excelfilename = "EFiling" + ".xls";
            string str_excelpath = Server.MapPath("") + "\\" + str_excelfilename;

            //if (File.Exists(str_excelpath) == true)
            //{
            //    File.Delete(str_excelpath);
            //}

            book.Properties.Author = "KS";
            book.Properties.LastAuthor = "Admin";
            book.ExcelWorkbook.WindowHeight = 4815;
            book.ExcelWorkbook.WindowWidth = 11295;
            book.ExcelWorkbook.WindowTopX = 120;
            book.ExcelWorkbook.WindowTopY = 105;

            GenerateStyles(book.Styles);

            GenerateWorksheet_printregister(book.Worksheets);

            book.Save(str_excelpath);

            //hidprint.Value = str_excelfilename;

        }

        private void GenerateStyles(WorksheetStyleCollection styles)
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
        private void GenerateWorksheet_printregister(WorksheetCollection sheets)
        {

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
            DataTable dt = new DataTable();
            //DataTable dt = Vechile_Report_B_Class.Vechile_Report_B_Class.eFilingReportSum(txtfromdate.Text.ToString(), txttodate.Text.ToString());
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
                Row = sheet.Table.Rows.Add();
                //Int64 serial_no = 0;
                Row.Cells.Add("GSTIN/UIN of Recipient", DataType.String, "s56");
                Row.Cells.Add("Invoice Number", DataType.String, "s56");
                Row.Cells.Add("Invoice date", DataType.String, "s56");
                Row.Cells.Add("Invoice Value", DataType.String, "s56");
                Row.Cells.Add("Place Of Supply", DataType.String, "s56");
                Row.Cells.Add("Reverse Charge", DataType.String, "s56");
                Row.Cells.Add("Invoice Type", DataType.String, "s56");
                Row.Cells.Add("E-Commerce GSTIN", DataType.String, "s56");
                Row.Cells.Add("Rate", DataType.String, "s56");
                Row.Cells.Add("Taxable Value", DataType.String, "s56");
                Row.Cells.Add("Cess Amount", DataType.String, "s56");

                //dt = Vechile_Report_B_Class.Vechile_Report_B_Class.eFilingReport(txtfromdate.Text.ToString(), txttodate.Text.ToString());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        Row = sheet.Table.Rows.Add();
                        //serial_no = serial_no + 1;
                        //Row.Cells.Add(serial_no.ToString(), DataType.String, "s44");
                        Row.Cells.Add(dr["GSTIN"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["BillNo"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["Date"].ToString(), DataType.String, "s49");
                        Row.Cells.Add(Convert.ToDouble(dr["grand_total"]).ToString("0.00"), DataType.String, "s45");
                        Row.Cells.Add(dr["placeofsupply"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["ReverseCharge"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["InvoiceType"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["ECommerceGSTIN"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(dr["Rate"].ToString(), DataType.String, "s45");
                        Row.Cells.Add(Convert.ToDouble(dr["total_amount"]).ToString("0.00"), DataType.String, "s45");
                        Row.Cells.Add("", DataType.String, "s45");

                    }
                }

            }

            else
            {
                //btnprint.Text = "Print Register";

                return;
            }
            // -----------------------------------------------
            //  Options
            // -----------------------------------------------
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

        public ActionResult printpdf(VendorItemDetails vendoritem, int PurchaseId)
        {

            GenerateReport1(PurchaseId);
            return RedirectToAction("Report", "Purchase");

        }
        protected void GenerateReport1(int po)
        {
            List<VendorItemDetails> List = new List<VendorItemDetails>();
            VendorItemDetails model = new VendorItemDetails();
            decimal total = 0;
            decimal total1 = 0;
            decimal tax = 0;
            //var data = db.tblPurchaseOrderItems.ToList();
            var Itemdetails = (from t1 in db.tblPurchasedItems
                               join t2 in db.tblPurchaseMasters on t1.PurchaseId equals t2.PurchaseId
                               join t3 in db.tblPurchaseOrderMasters on t2.PurchaseOrderId equals
                 t3.PurchaseOrderId
                               join t4 in db.tblPurchaseOrderItems on new { t3.PurchaseOrderId, t1.RawMaterialId } equals new
                               { t4.PurchaseOrderId, t4.RawMaterialId }
                               where t1.PurchaseId == po
                               select new
                               {
                                   t1.PurchaseId,
                                   t1.RawMaterialId,
                                   t1.tbl_RawMaterials.Name,
                                   t1.tbl_RawMaterials.units,
                                   t1.TaxPer,
                                   t1.Quantity,
                                   t2.InvoiceDate,
                                   t2.InvoiceNo,
                                   Vendorname = t2.tblVendor.Name,
                                   t2.tblVendor.Address,
                                   t2.tblVendor.ContactA,
                                   t2.tblVendor.GSTin,
                                   t2.tblVendor.TinNo,
                                   PurchaseQuantity = t4.Quantity,
                                   rate = t1.Amount / t1.Quantity,
                                   t1.Amount,
                                   t2.NetAmount,
                                   Value = t1.Amount + ((t1.Amount * t1.TaxPer) / 100),
                                   t2.tblVendor.AccountNumber,
                                   t2.tblVendor.Bank,
                                   t2.tblVendor.Branch,
                                   t2.tblVendor.IfscCode
                               }).ToList();

            foreach (var item in Itemdetails)
            {
                model.PurchaseId = item.PurchaseId;
                model.RawMaterialId = item.RawMaterialId;
                model.Rawname = item.Name;
                model.quantity = item.Quantity;
                model.unit = item.units;
                model.InvoiceDate = item.InvoiceDate;
                model.InvoiceNo = item.InvoiceNo;
                model.VendorName = item.Vendorname;
                model.tax = item.TaxPer;
                model.Address = item.Address;
                model.Contact = item.ContactA;
                model.GSTNO = item.GSTin;
                model.Purchasequantity = item.PurchaseQuantity;
                model.rate = item.rate;
                model.amount = item.Amount;
                model.totalamount = item.Value;
                model.netamount = item.NetAmount;
                total = total + item.Amount;
                total1 = total1 + item.Value;
                model.Accountnumber = item.AccountNumber;
                model.bank = item.Bank;
                model.branch = item.Branch;
                model.ifsc = item.IfscCode;
            }
            tax = total1 - total;
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
            iTextSharp.text.Font Font15 = FontFactory.GetFont("Verdana", 12,Font.BOLDITALIC);
            iTextSharp.text.Font Font15A = FontFactory.GetFont("Verdana", 10, Font.BOLD);

            string str_pdffilename = DateTime.Now.ToString().Replace("/","_").Replace(":", "_").Replace(" ", "_") + " GRNOrder.pdf";
            string str_pdfpath = Server.MapPath("~/Reports/") +  str_pdffilename; //"D:\\CDS\\MainProject\\NibsMVC\\Reports" + "\\" + str_pdffilename;//
            //System.IO.FileStream fs = new FileStream(str_pdfpath, FileMode.OpenOrCreate,FileAccess.Read);
            Document doc = new Document(PageSize.A4, 80, 80, 55, 25);


            PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
            doc.Open();

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(12);
            table.WidthPercentage = 100;
            float[] intwidth = new float[12] { 0.5f, 1, 1, 1, 1, 0.8f, 1, 1.2f, 0.8f, 1.2f, 1, 1 };
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Goods Reciept Note", Font14A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 12;
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Supplier Details: " , Font15));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            string imageURL = Server.MapPath("~/Content\\Logo\\Kandha-Logo-100x44.png");/*"D:\\CDS\\MainProject\\NibsMVC\\Content\\Logo\\Kandha-Logo-100x44.png"*/
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
            //cellPdf = new iTextSharp.text.Image.GetInstance(imageURL);
            cellPdf = new iTextSharp.text.pdf.PdfPCell((Image.GetInstance(jpg)));
            jpg.ScaleToFit(3F, 1.5F);
            jpg.SpacingBefore = 2F;
            jpg.Alignment = Element.ALIGN_RIGHT;
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 4;
            cellPdf.BorderWidthTop = 1F;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase( model.VendorName , Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 3;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("435 & 434 Trichy Road,Opp.Vasantha Mills,Singanallur,Coimbatore-641005.", Font15A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 4;
            
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase( model.Address , Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Contact: "+model.Contact+"", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Phone:0422-4213134", Font15A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Supplier Bank Details: ", Font15));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("ACC-No:"+model.Accountnumber+"", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Bank Name:" + model.bank + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Branch:" + model.branch + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(""));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("IFSC Code:" + model.ifsc + "", Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Invoice No: "+model.InvoiceNo+"", Font15A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf);
            
            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("GSTIN: "+model.GSTNO+"",Font8));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            cellPdf.Rowspan = 2;
            cellPdf.SetLeading(1.5F, 1.5F);
            cellPdf.BorderWidthBottom = 0;
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Invoice-Date:" + model.InvoiceDate.ToString("dd-MM-yyyy") + "", Font15A));
            cellPdf.Border = 0;
            cellPdf.Colspan = 6;
            //cellPdf.Rowspan = 4;
            cellPdf.BorderWidthBottom = 0F;
            cellPdf.BorderWidthLeft = 0F;
            cellPdf.BorderWidthRight = 0F;
            cellPdf.BorderWidthTop = 0F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellPdf.VerticalAlignment = Element.ALIGN_RIGHT;
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
            cellPdf.Colspan = 4;
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Order Qty", Font9B));
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Received Qty", Font9B));
            cellPdf.Border = 0;
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Rate", Font9B));
            cellPdf.Border = 0;
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Amount", Font9B));
            cellPdf.Border = 0;
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("GSTIN", Font9B));
            cellPdf.Border = 0;
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

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Value", Font9B));
            cellPdf.Border = 0;
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
                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(serno.ToString()+".", Font8));
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
                    cellPdf.Colspan = 4;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);
                    //3--------------------------------------------------------------------------------------------------------------------
                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.units.ToString(), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.PurchaseQuantity.ToString(), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.Quantity.ToString(), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.rate.ToString("0.00"), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.Amount.ToString("0.00"), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.TaxPer.ToString()+"%", Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
                    cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    table.AddCell(cellPdf);

                    cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr.Value.ToString("0.00"), Font8));
                    cellPdf.Border = 1;
                    cellPdf.Colspan = 1;
                    //cellPdf.Rowspan = 2;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 1F;
                    //cellPdf.BorderWidthTop = 0.5F;
                    cellPdf.SetLeading(1.5F, 1.5F);
                    cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cellPdf);
                    //19--------------------------------------------------------------------------------------------------------------------
                    //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr["Service_Tax"].ToString(), Font8));
                    //cellPdf.Border = 0;
                    //cellPdf.Colspan = 1;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    ////cellPdf.BorderWidthRight = 0.5F;
                    ////cellPdf.BorderWidthTop = 0.5F;
                    //cellPdf.SetLeading(1.5F, 1.5F);
                    //cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    //table.AddCell(cellPdf);
                    ////20--------------------------------------------------------------------------------------------------------------------
                    //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(dr["total_amount"].ToString(), Font8));
                    //cellPdf.Border = 0;
                    //cellPdf.Colspan = 1;
                    //cellPdf.BorderWidthBottom = 0.5F;
                    //cellPdf.BorderWidthLeft = 0.5F;
                    //cellPdf.BorderWidthRight = 0.5F;
                    ////cellPdf.BorderWidthTop = 0.5F;
                    //cellPdf.SetLeading(1.5F, 1.5F);
                    //cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                    //table.AddCell(cellPdf);

                    //temptotal = Convert.ToDecimal(dr["total_amount"]);
                    //total += temptotal;
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
                cellPdf.Colspan = 6;
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

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Total Amount: ", Font8));
                cellPdf.Border = 0;
                cellPdf.Colspan = 10;
                cellPdf.BorderWidthBottom = 0;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(1, 1);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(total.ToString("0.00"), Font8));
                cellPdf.Border = 0;
                cellPdf.Colspan = 2;
                cellPdf.BorderWidthBottom = 0;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(1, 1);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("GSTIN: ", Font8));
                cellPdf.Border = 0;
                cellPdf.Colspan = 10;
                cellPdf.BorderWidthBottom = 0;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(1, 1);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(tax.ToString("0.00"), Font8));
                cellPdf.Border = 0;
                cellPdf.Colspan = 2;
                cellPdf.BorderWidthBottom = 0;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(1, 1);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Net Amount: ", Font15A));
                cellPdf.Border = 0;
                cellPdf.Colspan = 10;
                cellPdf.BorderWidthBottom = 0.5F;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(2.5F, 2.5F);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);

                cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(total1.ToString("0.00"), Font15A));
                cellPdf.Border = 0;
                cellPdf.Colspan = 2;
                cellPdf.BorderWidthBottom = 0.5F;
                cellPdf.BorderWidthLeft = 0;
                cellPdf.BorderWidthRight = 0;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(2.5F,2.5F);
                cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell(cellPdf);
                //total = 0;
                //temptotal = 0;

                //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("TOTAL AMOUNT:", Font9Bc));
                //cellPdf.Border = 0;
                //cellPdf.Colspan = 5;
                //cellPdf.BorderWidthBottom = 0.5F;
                //cellPdf.BorderWidthLeft = 0.5F;
                ////cellPdf.BorderWidthRight = 0.5F;
                //cellPdf.BorderWidthTop = 0.5F;
                //cellPdf.SetLeading(1, 1);
                //cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                //table.AddCell(cellPdf);

                //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(A.ToString("0.00"), Font9Bc));
                //cellPdf.Border = 0;
                //cellPdf.Colspan = 1;
                //cellPdf.BorderWidthBottom = 0.5F;
                //cellPdf.BorderWidthLeft = 0.5F;
                //cellPdf.BorderWidthRight = 0.5F;
                //cellPdf.BorderWidthTop = 0.5F;
                //cellPdf.SetLeading(1, 1);
                //cellPdf.HorizontalAlignment = Element.ALIGN_RIGHT;
                //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
                //table.AddCell(cellPdf);
            }

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
            cellPdf.Border = 0;
            cellPdf.Colspan = 4;
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
            cellPdf.Colspan = 4;
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
            cellPdf.Colspan = 4;
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
            cellPdf.Colspan = 4;
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
            cellPdf.Colspan = 4;
            //cellPdf.Rowspan = 4;
            cellPdf.BorderWidthBottom = 0;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 0F;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cellPdf);

            cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase("Approved By:", Font9Bc));
            cellPdf.Border = 0;
            cellPdf.Colspan = 4;
            //cellPdf.Rowspan = 4;
            cellPdf.BorderWidthBottom = 0F;
            cellPdf.BorderWidthLeft = 0;
            cellPdf.BorderWidthRight = 0;
            cellPdf.BorderWidthTop = 0;
            cellPdf.SetLeading(1, 1);
            cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            table.AddCell(cellPdf);

            //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
            //cellPdf.Border = 0;
            //cellPdf.Colspan = 4;
            ////cellPdf.Rowspan = 2;
            //cellPdf.BorderWidthBottom = 1F;
            //cellPdf.BorderWidthLeft = 1F;
            //cellPdf.BorderWidthRight = 0.5F;
            //cellPdf.BorderWidthTop = 0F;
            //cellPdf.SetLeading(1, 1);
            //cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            //table.AddCell(cellPdf);

            //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
            //cellPdf.Border = 0;
            //cellPdf.Colspan = 4;
            ////cellPdf.Rowspan = 2;
            //cellPdf.BorderWidthBottom = 1F;
            //cellPdf.BorderWidthLeft = 0.5F;
            //cellPdf.BorderWidthRight = 0.5F;
            //cellPdf.BorderWidthTop = 0F;
            //cellPdf.SetLeading(1, 1);
            //cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            //table.AddCell(cellPdf);

            //cellPdf = new iTextSharp.text.pdf.PdfPCell(new Phrase(" "));
            //cellPdf.Border = 0;
            //cellPdf.Colspan = 4;
            ////cellPdf.Rowspan = 2;
            //cellPdf.BorderWidthBottom = 1F;
            //cellPdf.BorderWidthLeft = 0.5F;
            //cellPdf.BorderWidthRight = 1F;
            //cellPdf.BorderWidthTop = 0F;
            //cellPdf.SetLeading(1, 1);
            //cellPdf.HorizontalAlignment = Element.ALIGN_LEFT;
            //cellPdf.VerticalAlignment = Element.ALIGN_BOTTOM;
            //table.AddCell(cellPdf);

            doc.Add(table);
            
            writer.CloseStream = false;
            doc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename="+model.VendorName+" - GRNReport.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(doc);
            Response.End();
            string script = string.Format(@"showDialogfile('{0}')", str_pdffilename);
            //ScriptManager.RegisterClientScriptBlock(this, typeof(Page), UniqueID, script, true);
        }
    }
}
