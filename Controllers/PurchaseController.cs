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
            OutletPurchageReturnModel model = new OutletPurchageReturnModel();
            model.Amount = ReturnData.Amount;
            model.Purchasedetailid = ReturnData.PurchaseDetailId;
            model.Purchaseid = ReturnData.PurchaseId;
            model.OutletId = obj.getOutletId();
            model.Quantity = ReturnData.Quantity;
            model.Type = ReturnType(ReturnData.Unit);
            //model.VendorId = ReturnData.tblPurchaseMaster.VendorId;
            model.RowMaterialId = ReturnData.RawMaterialId;
            //model.VenderName = ReturnData.tblPurchaseMaster.tblVendor.Name;
            model.RowMaterialName = ReturnData.tbl_RawMaterials.Name;

            return View(model);
        }
        [HttpPost]
        public ActionResult PurchaseReturn(OutletPurchageReturnModel model)
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
            tb.vendorId = model.VendorId;
            tb.ReturnStatuss = "Success";
            db.tblPurchaseReturns.Add(tb);
            //db.SaveChanges();
            //remaining stock
            int Outlet = obj.getOutletId();
            decimal remaingquantity = model.Quantity - model.ReturnQuantity;

            tbl_KitchenStock DataStock = (from p in db.tbl_KitchenStock where p.RawMaterialId == model.RowMaterialId && p.OutletId == Outlet select p).SingleOrDefault();
            tblPurchasedItem datastockpurchase = (from q in db.tblPurchasedItems where q.PurchaseDetailId == model.Purchasedetailid select q).SingleOrDefault();
            tblGRNStock datastockgrn = (from r in db.tblGRNStocks where r.Id == model.Purchasedetailid select r).SingleOrDefault();
            tblPurchaseMaster datastockmaster = (from s in db.tblPurchaseMasters where s.PurchaseId == model.Purchaseid select s).SingleOrDefault();
            DataStock.Quantity = DataStock.Quantity - model.ReturnQuantity;
            datastockpurchase.Quantity = remaingquantity;
            datastockpurchase.Amount = datastockpurchase.Amount - returnAmount;
            datastockmaster.TotalAmount = datastockmaster.TotalAmount - returnAmount;
            decimal taxrate = returnAmount / datastockpurchase.TaxPer;
            datastockmaster.Tax = datastockmaster.Tax - taxrate;
            datastockmaster.NetAmount = datastockmaster.TotalAmount + datastockmaster.Tax;

            datastockgrn.Qty = remaingquantity;
            db.SaveChanges();
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

            sb.Append("select i.RawMaterialId,r.Name,i.Unit,i.Quantity,i.Amount,i.TaxPer,m.NetAmount,v.Name from tblPurchasedItem i inner join tblPurchaseMaster m on m.PurchaseId=i.PurchaseId inner join tbl_RawMaterials r on r.RawMaterialId=i.RawMaterialId inner join tblVendor v on v.VendorId=m.VendorId where i.PurchaseId='" + purchaseid + "'");

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


    }
}
