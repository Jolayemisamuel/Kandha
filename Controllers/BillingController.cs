using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Xml;
using System.Xml.Linq;
using System.Data;
using System.Web.Security;
using System.IO;

namespace NibsMVC.Controllers
{
    [Authorize(Roles = "Outlet,Operator")]
    public class BillingController : BaseController
    {
        //
        // GET: /Billing/
        NIBSEntities db = new NIBSEntities();
        StringBuilder sb = new StringBuilder();
        XMLTablesRepository xml = new XMLTablesRepository();
        AdminSearchRepository search = new AdminSearchRepository();
        public ActionResult Billing()
        {
            NibsBillingModel nibs = new NibsBillingModel();
            List<BillTableModelList> ItemLIst = new List<BillTableModelList>();

            int oulte = getOutletId();
            var address = (from p in db.tblOutlets where p.OutletId == oulte select p.Address).SingleOrDefault();
            ViewBag.outletaddress = address;
            var category = (from q in db.tblCategories where q.Active == true && (from p in db.tblMenuOutlets where p.OutletId == oulte select p.CategoryId).Contains(q.CategoryId) select q).ToList();
            // var ItemList = db.tblMenuOutlets.Where(o => o.OutletId == WebSecurity.CurrentUserId).ToList();
            foreach (var item in category)
            {
                BillTableModelList model = new BillTableModelList();
                model.CategoryId = item.CategoryId;
                model.CategoryName = item.Name;
                model.Color = item.Color;
                model.TextColor = item.TextColor;
                ItemLIst.Add(model);

            }
            nibs.getAllItem = ItemLIst;
            var result = db.tblTableMasters.Where(o => o.OutletId == oulte).ToList();

            List<BillTableModel> List = new List<BillTableModel>();
            foreach (var item in result)
            {
                BillTableModel mo = new BillTableModel();
                var filepath = Server.MapPath("/xmltables/table" + item.TableNo + ".xml");
                if (System.IO.File.Exists(filepath))
                {
                    XDocument xd = XDocument.Load(filepath);
                    var items = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                                 select p).ToList();
                    if (items.Count > 0)
                    {
                        mo.Current = "current";
                    }
                }
                mo.TableNo = item.TableNo.ToString();
                mo.AcType = item.AcType.ToString();
                List.Add(mo);
            }
            nibs.getAllTables = List;
            var serviccharg = (from p in db.tbl_ServiceTax select p.ServiceCharge).FirstOrDefault();
            if (serviccharg != 0)
            {
                nibs.ServiceCharge = serviccharg;
            }
            else
            {
                //nibs.ServiceCharge = Convert.ToDecimal(4.940);
                nibs.ServiceCharge = Convert.ToDecimal(5.6);

            }
            return View(nibs);
        }
        [HttpGet]
        public bool GetCurrent(string Id)
        {
            bool yes = false;
            var filepath = Server.MapPath("/xmltables/table" + Id + ".xml");
            int oulte = getOutletId();
            if (System.IO.File.Exists(filepath))
            {
                XDocument xd = XDocument.Load(filepath);
                var items = (from item in xd.Descendants("Items")
                             where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == Id.ToString()
                             select item).ToList();
                if (items.Count > 0)
                {
                    yes = true;
                }
            }
            else
            {
                yes = false;
            }
            return yes;
        }
        [HttpPost]
        public string GetAllItems(string Id)
        {
            return xml.GetAllItems(Id).ToString();
        }
        public PartialViewResult _GetAllItemPartial(string Id)
        {
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            lst = xml.GetAllItems(Id);
            return PartialView("_GetAllItemPartial", lst);
        }
        public PartialViewResult _CreatePartial(int Id)
        {

            GetBillingModel model = new GetBillingModel();
            var Path = Server.MapPath("/xmltables/table" + Id + ".xml");
            var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
            if (System.IO.File.Exists(Path))
            {
                model = xml.GetBillingItem(Id.ToString(), Path, KotFilePath);
            }
            else
            {
                int oulte = getOutletId();
                XmlTextWriter writer = new XmlTextWriter(Server.MapPath("/xmltables/table" + Id + ".xml"), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Item");
                xml.createNode(oulte.ToString(), Id.ToString(), "0", " ", "0", "0", "0", "0", "0", "0", "0", "0", writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                // MessageBox.Show("XML File created ! ");
                model = xml.GetBillingItem(Id.ToString(), Path, KotFilePath);
            }

            return PartialView("_CreatePartial", model);
        }
        //public ActionResult _
        public string CreateXml(string Id)
        {
            var Path = Server.MapPath("/xmltables/table" + Id + ".xml");
            var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
            if (System.IO.File.Exists(Path))
            {
                var Data = xml.FillFromXmlData(Id, Path, KotFilePath);
                return Data;
            }
            else
            {
                int oulte = getOutletId();
                XmlTextWriter writer = new XmlTextWriter(Server.MapPath("/xmltables/table" + Id + ".xml"), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Item");
                xml.createNode(oulte.ToString(), Id, "0", " ", "0", "0", "0", "0", "0", "0", "0", "0", writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                // MessageBox.Show("XML File created ! ");
                var Data = xml.FillFromXmlData(Id, Server.MapPath("/xmltables/table" + Id + ".xml"), KotFilePath);
                return Data;
            }
        }
        //public string DeleteNode(string Id, string TableNo)
        //{
        //    //var arr = Id.Split(',');
        //    //var TableId = Convert.ToInt32(arr[1]);
        //    //var ItemId = arr[0];
        //    var Path = Server.MapPath("/xmltables/table" + TableNo + ".xml");
        //    var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
        //    return xml.DeleteNode(Id, TableNo, Path, KotFilePath);
        //}
        [HttpPost]
        public ActionResult UpdateBillingXml(GetBillItemModel model)
        {
            return PartialView();
        }
        //public string UpdateXML(string Id, string TableNo, string Type, string Qty, string Name, string Price, string Quantity, string Vat)
        //{
        //    int oulte = getOutletId();
        //    if (Id != null)
        //    {
        //        var arr = Id.Split('_');
        //        var tableId = TableNo;
        //        var itemId = Convert.ToInt32(Id);
        //        var QtyType = Type;
        //        var QtyValue = Convert.ToInt32(Qty);

        //        var result = db.tblBasePriceItems.Where(o => o.ItemId == itemId).FirstOrDefault();
        //        decimal finalfullamout = result.FullPrice * QtyValue;
        //        decimal finalhalfamout = result.HalfPrice * QtyValue;
        //        var filepath = Server.MapPath("/xmltables/table" + tableId + ".xml");
        //        var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
        //        XDocument xd = XDocument.Load(filepath);

        //        if (QtyType == "Half")
        //        {
        //            xml.UpdateKotXmlData(filepath, KotFilePath, tableId, result.tblItem.Name, "0", QtyValue.ToString());
        //        }
        //        else
        //        {
        //            xml.UpdateKotXmlData(filepath, KotFilePath, tableId, result.tblItem.Name, QtyValue.ToString(), "0");
        //        }
        //        var items = from item in xd.Descendants("Items")
        //                    where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo && item.Element("ItemId").Value == itemId.ToString()
        //                    select item;


        //        if (items.Count() > 0)
        //        {
        //            if (QtyType == "Half")
        //            {

        //                foreach (XElement itemElement in items)
        //                {
        //                    var totalamount = Convert.ToDecimal(finalhalfamout) + Convert.ToDecimal(itemElement.Element("Amount").Value);
        //                    var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
        //                    itemElement.SetElementValue("UserId", oulte.ToString());
        //                    itemElement.SetElementValue("TableNo", TableNo.ToString());
        //                    itemElement.SetElementValue("ItemId", result.ItemId);
        //                    itemElement.SetElementValue("ItemName", result.tblItem.Name);
        //                    itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
        //                    itemElement.SetElementValue("Fullprice", result.FullPrice);
        //                    itemElement.SetElementValue("HalfPrice", result.HalfPrice);
        //                    itemElement.SetElementValue("VatAmt", result.Vat);
        //                    itemElement.SetElementValue("HalfQty", Convert.ToInt32(QtyValue) + Convert.ToInt32(itemElement.Element("HalfQty").Value));
        //                    itemElement.SetElementValue("Amount", totalamount);
        //                    itemElement.SetElementValue("VatAmountCharges", vatamtchrg);
        //                    itemElement.SetElementValue("OfferQty", itemElement.Element("OfferQty").Value);
        //                }

        //            }
        //            else
        //            {
        //                foreach (XElement itemElement in items)
        //                {
        //                    var totalamount = Convert.ToDecimal(finalfullamout) + Convert.ToDecimal(itemElement.Element("Amount").Value);
        //                    var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
        //                    itemElement.SetElementValue("UserId", oulte.ToString());
        //                    itemElement.SetElementValue("TableNo", TableNo.ToString());
        //                    itemElement.SetElementValue("ItemId", result.ItemId);
        //                    itemElement.SetElementValue("ItemName", result.tblItem.Name);
        //                    itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
        //                    itemElement.SetElementValue("Fullprice", result.FullPrice);
        //                    itemElement.SetElementValue("HalfPrice", result.HalfPrice);
        //                    itemElement.SetElementValue("VatAmt", result.Vat);
        //                    itemElement.SetElementValue("FullQty", Convert.ToInt32(QtyValue) + Convert.ToInt32(itemElement.Element("FullQty").Value));
        //                    itemElement.SetElementValue("Amount", totalamount);
        //                    itemElement.SetElementValue("VatAmountCharges", vatamtchrg);
        //                    itemElement.SetElementValue("OfferQty", itemElement.Element("OfferQty").Value);
        //                }

        //            }
        //            xd.Save(filepath);
        //        }
        //        else
        //        {
        //            if (QtyType == "Half")
        //            {
        //                var totalamount = Convert.ToDecimal(finalhalfamout);
        //                var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
        //                var newElement = new XElement("Items",
        //                     new XElement("UserId", oulte.ToString()),
        //                     new XElement("TableNo", TableNo),
        //                     new XElement("ItemId", result.ItemId),
        //                     new XElement("ItemName", result.tblItem.Name),
        //                     new XElement("FullQty", "0"),
        //                     new XElement("HalfQty", QtyValue),
        //                     new XElement("Fullprice", result.FullPrice),
        //                     new XElement("HalfPrice", result.HalfPrice),
        //                     new XElement("Amount", totalamount),
        //                     new XElement("VatAmt", result.Vat),
        //                     new XElement("VatAmountCharges", vatamtchrg),
        //                     new XElement("OfferQty", "0"));
        //                xd.Element("Item").Add(newElement);
        //            }
        //            else
        //            {
        //                var totalamount = Convert.ToDecimal(finalfullamout);
        //                var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
        //                var newElement = new XElement("Items",
        //                      new XElement("UserId", oulte.ToString()),
        //                      new XElement("TableNo", TableNo),
        //                      new XElement("ItemId", result.ItemId),
        //                      new XElement("ItemName", result.tblItem.Name),
        //                      new XElement("FullQty", QtyValue),
        //                      new XElement("HalfQty", "0"),
        //                      new XElement("Fullprice", result.FullPrice),
        //                      new XElement("HalfPrice", result.HalfPrice),
        //                      new XElement("Amount", totalamount),
        //                      new XElement("VatAmt", result.Vat),
        //                      new XElement("VatAmountCharges", vatamtchrg),
        //                      new XElement("OfferQty", "0"));
        //                xd.Element("Item").Add(newElement);
        //            }


        //            xd.Save(filepath);

        //        }
        //        if (QtyType == "Full")
        //        {
        //            AdminCallHappyHoursRepository hh = new AdminCallHappyHoursRepository();
        //            bool Happy = hh.CallHappyHoursDaysOffer(Id, TableNo, filepath, KotFilePath);
        //            if (Happy == true)
        //            {
        //                AdminCallOfferRepository offer = new AdminCallOfferRepository();
        //                bool Data = offer.CallOffer(Id, filepath, TableNo, Qty, KotFilePath);
        //                if (Data == true)
        //                {
        //                    bool Combo = offer.CallComboOffer(Id, TableNo, filepath, KotFilePath);
        //                    if (Combo == true)
        //                    {
        //                        offer.CallAmountBasisOffer(Id, TableNo, filepath, KotFilePath, Qty);
        //                    }
        //                }
        //            }


        //        }

        //        string data = xml.FillFromXmlData(tableId, filepath, KotFilePath);
        //        return data;
        //    }
        //    else
        //    {
        //        var filepath = Server.MapPath("/xmltables/table" + TableNo + ".xml");
        //        var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
        //        OpenFoodRepository open = new OpenFoodRepository();
        //        StringBuilder Data = open.AddOpenFoodToXml(filepath, KotFilePath, TableNo, Name, Price, Quantity, Vat);
        //        string data = xml.FillFromXmlData(TableNo, filepath, KotFilePath);
        //        return data;
        //        //return Data.ToString();
        //    }
        //    // return "";
        //}

        //public string DispatchOrder(OrderDispatchModel model)
        //{
        //    var Path = Server.MapPath("/xmltables/table" + model.TableNo + ".xml");
        //    var data = xml.DispatchOrder(model, Path);
        //    return data;
        //}
        public string PrintOrder(OrderDispatchModel model)
        {
            var Path = Server.MapPath("/xmltables/table" + model.TableNo + ".xml");
            var Data = xml.PrintOrderData(model, Path);
            return Data;

        }
        //public string ClearKot(string Id)
        //{
        //    var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
        //    var Path = Server.MapPath("/xmltables/table" + Id + ".xml");
        //    var Data = xml.ClearKot(Path, KotFilePath, Id);
        //    return Data;
        //}

        public string MergedTable(MergedTableModel model)
        {
            int oulte = getOutletId();
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("/xmltables/table" + model.MasterTable + ".xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("Item");
            xml.createNode(oulte.ToString(), model.MasterTable.ToString(), "0", " ", "0", "0", "0", "0", "0", "0", "0", "0", writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return "Table Successfully Merged";
        }

        //public string CancelOrder(string Id)
        //{
        //    int oulte = getOutletId();
        //    var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
        //    var Path = Server.MapPath("/xmltables/table" + Id + ".xml");
        //    XDocument xd = XDocument.Load(Path);
        //    var items = (from item in xd.Descendants("Items")
        //                 where item.Element("UserId").Value == oulte.ToString()
        //                 select item);
        //    items.Remove();
        //    xd.Save(Path);
        //    var Data = xml.ClearKot(Path, KotFilePath, Id);
        //    return Data;

        //}
        public JsonResult Billingitms(string id)
        {
            AddItemRepository dis = new AddItemRepository();
            List<string> billitesm = new List<string>();
            billitesm = dis.billingitemlist(Convert.ToInt32(id));
            foreach (var item in billitesm)
            {
                string[] arr = item.Split('-');
                sb.Append("<input type='button' name='GraphicButton' value='" + arr[0] + "' id='" + arr[1] + "' class='btnbillgadd btn blue margin-bottom-5 margin-right-5' />");

            }
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult bilprice(string id)
        {
            AddItemRepository dis = new AddItemRepository();
            string bilitems = dis.billscollectin(Convert.ToInt32(id));
            TempData["baserror"] = bilitems;
            return Json(bilitems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintView()
        {
            int id = db.tblBillMasters.Max(a => a.BillId);
            var billdata = (from p in db.tblBillMasters where p.BillId.Equals(id) select p).ToList();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in billdata)
            {
                BillingModel model = new BillingModel();
                model.BillId = item.BillId;
                model.BillDate = item.BillDate;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.TableNo = item.TableNo;
                model.CustomerName = item.CustomerName;
                list.Add(model);
            }

            return View(list);
        }

        public ActionResult SmallPrint(int id = 0)
        {

            var billdata = (from p in db.tblBillMasters where p.BillId.Equals(id) select p).ToList();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in billdata)
            {
                BillingModel model = new BillingModel();
                model.BillId = item.BillId;
                model.BillDate = item.BillDate;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.TableNo = item.TableNo;
                model.CustomerName = item.CustomerName;
                list.Add(model);
            }

            return View(list);
        }
        public ActionResult Index()
        {
            AdminBillReportModel m = new AdminBillReportModel();
            List<BillingModel> list = new List<BillingModel>();
            List<OpenFood> openfood = new List<OpenFood>();
            int OutletId = xml.getOutletId();
            var billdata = (from p in db.tblBillMasters
                            where p.BillingType == "R"
                            && p.OutletId == OutletId
                            select p).Take(30).OrderByDescending(a => a.BillId).ToList();
            foreach (var item in billdata)
            {
                BillingModel model = new BillingModel();
                List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                model.BillId = item.BillId;
                model.BillDate = item.BillDate;
                model.TotalAmount = Math.Round(item.TotalAmount, 2);
                model.VatAmount = Math.Round(item.VatAmount, 2);
                if (item.ServiceTax.HasValue)
                {
                    model.ServiceTax = item.ServiceTax.Value;
                }
                model.ServicChargeAmt = Math.Round(item.ServicChargesAmount, 2);
                model.DiscountAmount = Math.Round(item.DiscountAmount, 2);
                model.NetAmount = Math.Round(item.NetAmount, 2);
                model.TableNo = item.TableNo;
                model.Outletid = item.OutletId;
                model.PaymentType = item.PaymentType;
                model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                foreach (var i in db.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                {
                    AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                    bill.Amount = i.Amount;
                    bill.BilldeatilId = i.BillDetailsId;
                    bill.FullQty = i.FullQty.Value;
                    bill.HalfQty = i.HalfQty.Value;
                    bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                    lstBill.Add(bill);
                }
                model.getBillItemDetails = lstBill;
                list.Add(model);
            }
            foreach (var item in db.OpenFoods.Where(a => a.OutletId == OutletId).ToList())
            {
                OpenFood open = new OpenFood();
                open.ItemName = item.ItemName;
                open.Price = item.Price;
                open.Quantity = item.Quantity;
                open.Vat = item.Vat;
                open.Amount = Math.Round(item.Amount.Value, 2);
                open.Date = item.Date;
                openfood.Add(open);
            }
            m.getAllOpenFood = openfood;
            m.getAllBillReports = list;
            return View(m);
        }
        [HttpPost]
        public ActionResult Index(AdminBillReportModel model)
        {
            string[] roles = Roles.GetRolesForUser();
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    return View(search.getOutletSearchData(model, "R", WebSecurity.CurrentUserId));
                }
                else
                {
                    int Id = getOutletId();
                    return View(search.getOutletSearchData(model, "R", Id));
                }
            }
            return View();
        }
        public ActionResult Create()
        {

            StringBuilder sb = new StringBuilder();
            IEnumerable<SelectListItem> itemlist = (from q in db.tblItems select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.ItemId.ToString() });
            ViewBag.itemlists = new SelectList(itemlist, "Value", "Text");
            StringBuilder od = new StringBuilder();
            int oulte = getOutletId();
            var numbers = (from q in db.tblTableMasters where q.OutletId == oulte select q).ToList();
            List<BillingModel> list = new List<BillingModel>();
            //ViewBag.num = numbers;
            foreach (var item in numbers)
            {
                BillingModel model = new BillingModel();
                model.TableNo = item.TableNo;

                list.Add(model);
            }
            var category = (from q in db.tblCategories where (from p in db.tblMenuOutlets where p.OutletId == oulte select p.CategoryId).Contains(q.CategoryId) select q).ToList();
            ViewBag.catnames = category;

            //var type = (from p in db.tblOperators where p.Name.Equals(name) select p.Type).SingleOrDefault();
            //ViewBag.Otype = type;
            return View(list);
        }


        public ActionResult DeleteBill(DeleteModel model, int id = 0)
        {
            var billtype = (from p in db.tblBillMasters where p.BillId.Equals(id) select p.BillingType).FirstOrDefault();
            try
            {

                var type = (from q in db.tblOperators where q.UserId == WebSecurity.CurrentUserId select q.Type).SingleOrDefault();
                if (type == "Manager")
                {
                    var delete = db.tblBillMasters.Where(a => a.OutletId == WebSecurity.CurrentUserId && a.BillId == id).SingleOrDefault();
                    tblDeleteBillMaster tb = new tblDeleteBillMaster();
                    tb.Address = delete.Address;
                    tb.BillDate = delete.BillDate;
                    tb.BillingType = delete.BillingType;
                    tb.CustomerName = delete.CustomerName;
                    tb.DiscountAmount = delete.DiscountAmount;
                    tb.NetAmount = delete.NetAmount;
                    tb.OutletId = delete.OutletId;
                    tb.ServiceCharAmt = delete.ServicChargesAmount;
                    tb.TableNo = delete.TableNo;
                    tb.TokenNo = delete.TokenNo;
                    tb.TotalAmount = delete.TotalAmount;
                    tb.DeleteDate = DateTime.Now;
                    tb.VatAmount = delete.VatAmount;
                    tb.BillNo = delete.BillId;
                    db.tblDeleteBillMasters.Add(tb);
                    db.SaveChanges();
                    int Pid = (from p in db.tblDeleteBillMasters where p.OutletId == WebSecurity.CurrentUserId select p.DeleteId).Max();
                    var billChildData = db.tblBillDetails.Where(a => a.BillId == id).ToList();

                    foreach (var item in billChildData)
                    {
                        tblDeletedetail deletetb = new tblDeletedetail();
                        deletetb.Amount = item.Amount;
                        deletetb.DeleteId = Pid;
                        deletetb.FullQty = item.FullQty;
                        deletetb.HalfQty = item.HalfQty;
                        deletetb.ItemId = item.ItemId.Value;
                        db.tblDeletedetails.Add(deletetb);
                        db.SaveChanges();
                    }
                    foreach (var item in billChildData)
                    {
                        db.tblBillDetails.Remove(item);
                        db.SaveChanges();
                    }
                    db.tblBillMasters.Remove(delete);
                    db.SaveChanges();
                    TempData["error"] = "Delete Successfully !!";
                }
                if (User.IsInRole("Outlet"))
                {
                    var delete = db.tblBillMasters.Where(a => a.OutletId == WebSecurity.CurrentUserId && a.BillId == id).SingleOrDefault();
                    tblDeleteBillMaster tb = new tblDeleteBillMaster();
                    tb.Address = delete.Address;
                    tb.BillDate = delete.BillDate;
                    tb.BillingType = delete.BillingType;
                    tb.CustomerName = delete.CustomerName;
                    tb.DiscountAmount = delete.DiscountAmount;
                    tb.NetAmount = delete.NetAmount;
                    tb.OutletId = delete.OutletId;
                    tb.ServiceCharAmt = delete.ServicChargesAmount;
                    tb.TableNo = delete.TableNo;
                    tb.TokenNo = delete.TokenNo;
                    tb.TotalAmount = delete.TotalAmount;
                    tb.DeleteDate = DateTime.Now;
                    tb.VatAmount = delete.VatAmount;
                    tb.BillNo = delete.BillId;
                    db.tblDeleteBillMasters.Add(tb);
                    db.SaveChanges();
                    int Pid = (from p in db.tblDeleteBillMasters where p.OutletId == WebSecurity.CurrentUserId select p.DeleteId).Max();
                    var billChildData = db.tblBillDetails.Where(a => a.BillId == id).ToList();

                    foreach (var item in billChildData)
                    {
                        tblDeletedetail deletetb = new tblDeletedetail();
                        deletetb.Amount = item.Amount;
                        deletetb.DeleteId = Pid;
                        deletetb.FullQty = item.FullQty;
                        deletetb.HalfQty = item.HalfQty;
                        deletetb.ItemId = item.ItemId.Value;
                        db.tblDeletedetails.Add(deletetb);
                        db.SaveChanges();
                    }
                    foreach (var item in billChildData)
                    {
                        db.tblBillDetails.Remove(item);
                        db.SaveChanges();
                    }
                    db.tblBillMasters.Remove(delete);
                    db.SaveChanges();
                    TempData["error"] = "Delete Successfully !!";
                }
                else
                {
                    TempData["error"] = "You are not authorised to Delete Bill !!";
                }

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            if (billtype == "R")
            {
                return RedirectToAction("Index", "Billing");
            }
            else if (billtype == "T")
            {
                return RedirectToAction("TakeAwayReport", "Billing");
            }
            else
            {
                return RedirectToAction("HomeDeliveryReport", "Billing");
            }

        }

        public ActionResult TakeAway()
        {
            IEnumerable<SelectListItem> itemlist = (from q in db.tblItems select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.ItemId.ToString() });
            ViewBag.itemlists = new SelectList(itemlist, "Value", "Text");
            int oulte = getOutletId();
            var category = (from q in db.tblCategories where (from p in db.tblMenuOutlets where p.OutletId == oulte select p.CategoryId).Contains(q.CategoryId) select q).ToList();
            ViewBag.catnames = category;
            var token = (from p in db.tblBillMasters where p.BillingType.Equals("T") select p.BillId).Max();
            var tokendatas = (from q in db.tblBillMasters where q.BillId.Equals(token) select q).FirstOrDefault();
            var Curentdate = DateTime.Now.Date;
            int no;
            if (Curentdate == tokendatas.BillDate)
            {
                no = Convert.ToInt32(tokendatas.TokenNo);
                no = no + 1;
            }
            else
            {
                no = 1;
            }
            ViewBag.tokenno = no;

            //var type = (from p in db.tblOperators where p.Name.Equals(name) select p.Type).SingleOrDefault();
            //ViewBag.Otype = type;
            return View();

        }

        [HttpPost]
        public string Create(BillingModel model, string formData)
        {

            try
            {
                int oulte = getOutletId();
                string[] arr = model.masterdata.Split(';');
                string[] mainitems;
                if (arr.ToString() != string.Empty)
                {
                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        if (arr[i] != null && arr[i] != "")
                        {
                            tblBillMaster tb = new tblBillMaster();
                            mainitems = arr[i].Split('^');
                            tb.BillDate = DateTime.Now.Date;
                            tb.TotalAmount = Convert.ToDecimal(mainitems[0]);
                            tb.VatAmount = Convert.ToDecimal(mainitems[1]);
                            tb.ServicChargesAmount = Convert.ToDecimal(mainitems[2]);
                            if (mainitems[3] == "")
                            {
                                tb.DiscountAmount = Convert.ToDecimal(0);
                            }
                            else
                            {
                                tb.DiscountAmount = Convert.ToDecimal(mainitems[3]);
                            }
                            tb.NetAmount = Convert.ToDecimal(mainitems[4]);
                            tb.TableNo = Convert.ToInt32(mainitems[5]);
                            tb.CustomerName = mainitems[6].ToString();
                            tb.OutletId = Convert.ToInt32(oulte);
                            tb.BillingType = "R";
                            db.tblBillMasters.Add(tb);
                            db.SaveChanges();
                        }
                    }
                }
                int bilid = (from p in db.tblBillMasters where p.OutletId == oulte && p.BillingType.Equals("R") select p.BillId).Max();
                string[] arr1 = model.detailsitems.Split(';');
                string[] detaildata;
                if (arr1.ToString() != string.Empty)
                {
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        if (arr1[i] != null && arr1[i] != "")
                        {
                            detaildata = arr1[i].Split('^');
                            tblBillDetail tbl1 = new tblBillDetail();
                            tbl1.BillId = bilid;
                            tbl1.ItemId = Convert.ToInt32(detaildata[0]);
                            tbl1.FullQty = Convert.ToInt32(detaildata[1]);
                            tbl1.HalfQty = Convert.ToInt32(detaildata[2]);
                            tbl1.Amount = Convert.ToDecimal(detaildata[3]);
                            db.tblBillDetails.Add(tbl1);
                            db.SaveChanges();

                        }
                    }
                }

                return bilid.ToString();

            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }

        public ActionResult HomeDeliveryPrint(int id = 0)
        {
            var data = (from p in db.tblBillMasters where p.BillId.Equals(id) && p.BillingType.Equals("H") select p).ToList();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in data)
            {
                BillingModel model = new BillingModel();
                model.BillDate = item.BillDate;
                model.BillId = item.BillId;
                model.CustomerName = item.CustomerName;
                model.Address = item.Address;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.TokenNo = Convert.ToInt32(item.TokenNo);
                list.Add(model);
            }

            return View(list);
        }

        public ActionResult TakeAwayReport()
        {
            AdminBillReportModel m = new AdminBillReportModel();
            List<BillingModel> list = new List<BillingModel>();
            string[] roles = Roles.GetRolesForUser();
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    var billdata = (from p in db.tblBillMasters
                                    where p.BillingType == "T"
                                    && p.OutletId == WebSecurity.CurrentUserId
                                    select p).ToList();
                    foreach (var item in billdata)
                    {
                        BillingModel model = new BillingModel();
                        List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                        model.BillId = item.BillId;
                        model.BillDate = item.BillDate;
                        model.TotalAmount = item.TotalAmount;
                        model.VatAmount = item.VatAmount;
                        model.ServicChargeAmt = item.ServicChargesAmount;
                        if (item.ServiceTax.HasValue)
                        {
                            model.ServiceTax = item.ServiceTax.Value;
                        }

                        model.DiscountAmount = item.DiscountAmount;
                        model.NetAmount = item.NetAmount;
                        model.TableNo = item.TableNo;
                        model.Outletid = item.OutletId;
                        model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                        foreach (var i in db.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                        {
                            AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                            bill.Amount = i.Amount;
                            bill.BilldeatilId = i.BillDetailsId;
                            bill.FullQty = i.FullQty.Value;
                            bill.HalfQty = i.HalfQty.Value;
                            bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                            lstBill.Add(bill);
                        }
                        model.getBillItemDetails = lstBill;
                        list.Add(model);
                    }
                }
                else
                {
                    var outltId = (from q in db.tblOperators where q.UserId == WebSecurity.CurrentUserId select q.OutletId).FirstOrDefault();
                    var billdata = (from p in db.tblBillMasters
                                    where p.BillingType == "R"
                                    && p.OutletId == outltId
                                    select p).ToList();
                    foreach (var item in billdata)
                    {
                        BillingModel model = new BillingModel();
                        List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                        model.BillId = item.BillId;
                        model.BillDate = item.BillDate;
                        model.TotalAmount = item.TotalAmount;
                        model.VatAmount = item.VatAmount;
                        model.ServicChargeAmt = item.ServicChargesAmount;
                        if (item.ServiceTax.HasValue)
                        {
                            model.ServiceTax = item.ServiceTax.Value;
                        }
                        model.DiscountAmount = item.DiscountAmount;
                        model.NetAmount = item.NetAmount;
                        model.TableNo = item.TableNo;
                        model.Outletid = item.OutletId;
                        model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                        foreach (var i in db.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                        {
                            AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                            bill.Amount = i.Amount;
                            bill.BilldeatilId = i.BillDetailsId;
                            bill.FullQty = i.FullQty.Value;
                            bill.HalfQty = i.HalfQty.Value;
                            bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                            lstBill.Add(bill);
                        }
                        model.getBillItemDetails = lstBill;
                        list.Add(model);
                    }
                }
            }
            m.getAllBillReports = list;
            return View(m);
        }
        [HttpPost]
        public ActionResult TakeAwayReport(AdminBillReportModel model)
        {
            string[] roles = Roles.GetRolesForUser();
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    return View(search.getOutletSearchData(model, "T", WebSecurity.CurrentUserId));
                }
                else
                {
                    int Id = getOutletId();
                    return View(search.getOutletSearchData(model, "T", Id));
                }
            }
            return View();
        }
        public ActionResult TakePrint(int id = 0)
        {
            var data = (from p in db.tblBillMasters where p.BillId.Equals(id) && p.BillingType.Equals("T") select p).ToList();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in data)
            {
                BillingModel model = new BillingModel();
                model.BillDate = item.BillDate;
                model.BillId = item.BillId;
                model.CustomerName = item.CustomerName;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.TokenNo = Convert.ToInt32(item.TokenNo);
                list.Add(model);
            }

            return View(list);

        }
        [HttpPost]
        public ActionResult TakeAway(BillingModel model)
        {
            try
            {
                int oulte = getOutletId();
                string[] arr = model.takemasterdata.Split(';');
                string[] mainitems;
                if (arr.ToString() != string.Empty)
                {
                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        if (arr[i] != null && arr[i] != "")
                        {
                            tblBillMaster tb = new tblBillMaster();
                            mainitems = arr[i].Split('^');
                            tb.BillDate = DateTime.Now.Date;
                            tb.TotalAmount = Convert.ToDecimal(mainitems[0]);
                            tb.VatAmount = Convert.ToDecimal(mainitems[1]);
                            tb.ServicChargesAmount = Convert.ToDecimal(mainitems[2]);
                            tb.DiscountAmount = Convert.ToDecimal(mainitems[3]);
                            tb.NetAmount = Convert.ToDecimal(mainitems[4]);
                            tb.TokenNo = Convert.ToInt32(mainitems[5]);
                            tb.OutletId = Convert.ToInt32(oulte);
                            tb.BillingType = "T";
                            db.tblBillMasters.Add(tb);
                            db.SaveChanges();
                        }
                    }
                }
                int bilid = (from p in db.tblBillMasters select p.BillId).Max();
                string[] arr1 = model.takedetailsitems.Split(';');
                string[] detaildata;
                if (arr1.ToString() != string.Empty)
                {
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        if (arr1[i] != null && arr1[i] != "")
                        {
                            detaildata = arr1[i].Split('^');
                            tblBillDetail tbl1 = new tblBillDetail();
                            tbl1.BillId = bilid;
                            tbl1.ItemId = Convert.ToInt32(detaildata[0]);
                            tbl1.FullQty = Convert.ToInt32(detaildata[1]);
                            tbl1.HalfQty = Convert.ToInt32(detaildata[2]);
                            tbl1.Amount = Convert.ToDecimal(detaildata[3]);
                            db.tblBillDetails.Add(tbl1);
                            db.SaveChanges();

                        }
                    }
                }

            }
            catch { }
            var takebillid = (from p in db.tblBillMasters select p.BillId).Max();
            return RedirectToAction("TakePrint", "Billing", new { id = takebillid });
        }

        public ActionResult HomeDelivery()
        {
            IEnumerable<SelectListItem> itemlist = (from q in db.tblItems select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.ItemId.ToString() });
            ViewBag.itemlists = new SelectList(itemlist, "Value", "Text");
            int oulte = getOutletId();
            var category = (from q in db.tblCategories where (from p in db.tblMenuOutlets where p.OutletId == oulte select p.CategoryId).Contains(q.CategoryId) select q).ToList();
            ViewBag.catnames = category;
            var token = (from p in db.tblBillMasters where p.BillingType.Equals("H") select p.BillId).Max();
            var tokendatas = (from q in db.tblBillMasters where q.BillId.Equals(token) select q).FirstOrDefault();
            var Curentdate = DateTime.Now.Date;
            int no;
            if (Curentdate == tokendatas.BillDate)
            {
                no = Convert.ToInt32(tokendatas.TokenNo);
                no = no + 1;
            }
            else
            {
                no = 1;
            }
            ViewBag.tokenno = no;

            //var type = (from p in db.tblOperators where p.Name.Equals(name) select p.Type).SingleOrDefault();
            //ViewBag.Otype = type;
            return View();
        }
        [HttpPost]
        public ActionResult HomeDelivery(BillingModel model)
        {
            int oulte = getOutletId();
            try
            {

                string[] arr = model.homemasterdata.Split(';');
                string[] mainitems;
                if (arr.ToString() != string.Empty)
                {
                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        if (arr[i] != null && arr[i] != "")
                        {
                            tblBillMaster tb = new tblBillMaster();
                            mainitems = arr[i].Split('^');
                            tb.BillDate = DateTime.Now.Date;
                            tb.TotalAmount = Convert.ToDecimal(mainitems[0]);
                            tb.VatAmount = Convert.ToDecimal(mainitems[1]);
                            tb.ServicChargesAmount = Convert.ToDecimal(mainitems[2]);
                            tb.DiscountAmount = Convert.ToDecimal(mainitems[3]);
                            tb.NetAmount = Convert.ToDecimal(mainitems[4]);
                            tb.TokenNo = Convert.ToInt32(mainitems[5]);
                            tb.CustomerName = mainitems[6].ToString();
                            tb.Address = mainitems[7].ToString();
                            tb.OutletId = Convert.ToInt32(oulte);
                            tb.TableNo = 0;
                            tb.BillingType = "H";
                            db.tblBillMasters.Add(tb);
                            db.SaveChanges();
                        }
                    }
                }
                int bilid = (from p in db.tblBillMasters where p.BillingType == "H" select p.BillId).Max();
                string[] arr1 = model.homedetailsitems.Split(';');
                string[] detaildata;
                if (arr1.ToString() != string.Empty)
                {
                    for (int i = 0; i < arr1.Length; i++)
                    {
                        if (arr1[i] != null && arr1[i] != "")
                        {
                            detaildata = arr1[i].Split('^');
                            tblBillDetail tbl1 = new tblBillDetail();
                            tbl1.BillId = bilid;
                            tbl1.ItemId = Convert.ToInt32(detaildata[0]);
                            tbl1.FullQty = Convert.ToInt32(detaildata[1]);
                            tbl1.HalfQty = Convert.ToInt32(detaildata[2]);
                            tbl1.Amount = Convert.ToDecimal(detaildata[3]);
                            db.tblBillDetails.Add(tbl1);
                            db.SaveChanges();

                        }
                    }
                }

            }
            catch { }
            var takebillid = (from p in db.tblBillMasters where p.OutletId == oulte && p.BillingType.Equals("H") select p.BillId).Max();
            return RedirectToAction("HomeDeliveryPrint", "Billing", new { id = takebillid });
        }

        public ActionResult HomeDeliveryReport()
        {
            //int oulte = getOutletId();
            //var billdata = (from p in db.tblBillMasters where p.OutletId == oulte && p.BillingType.Equals("H") select p).ToList();
            //List<BillingModel> list = new List<BillingModel>();
            //foreach (var item in billdata)
            //{
            //    BillingModel model = new BillingModel();
            //    model.BillId = item.BillId;
            //    model.BillDate = item.BillDate;
            //    model.TotalAmount = item.TotalAmount;
            //    model.VatAmount = item.VatAmount;
            //    model.ServicChargeAmt = item.ServicChargesAmount;
            //    model.DiscountAmount = item.DiscountAmount;
            //    model.NetAmount = item.NetAmount;
            //    model.TokenNo = Convert.ToInt32(item.TokenNo);
            //    model.CustomerName = item.CustomerName;
            //    model.Address = item.Address;
            //    list.Add(model);
            //}

            //return View(list);
            AdminBillReportModel m = new AdminBillReportModel();
            List<BillingModel> list = new List<BillingModel>();
            string[] roles = Roles.GetRolesForUser();
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    var billdata = (from p in db.tblBillMasters
                                    where p.BillingType == "H"
                                    && p.OutletId == WebSecurity.CurrentUserId
                                    select p).ToList();
                    foreach (var item in billdata)
                    {
                        BillingModel model = new BillingModel();
                        List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                        model.BillId = item.BillId;
                        model.BillDate = item.BillDate;
                        model.TotalAmount = item.TotalAmount;
                        model.VatAmount = item.VatAmount;
                        model.ServicChargeAmt = item.ServicChargesAmount;
                        if (item.ServiceTax.HasValue)
                        {
                            model.ServiceTax = item.ServiceTax.Value;
                        }
                        model.DiscountAmount = item.DiscountAmount;
                        model.NetAmount = item.NetAmount;
                        model.TableNo = item.TableNo;
                        model.Outletid = item.OutletId;
                        model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                        foreach (var i in db.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                        {
                            AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                            bill.Amount = i.Amount;
                            bill.BilldeatilId = i.BillDetailsId;
                            bill.FullQty = i.FullQty.Value;
                            bill.HalfQty = i.HalfQty.Value;
                            bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                            lstBill.Add(bill);
                        }
                        model.getBillItemDetails = lstBill;
                        list.Add(model);
                    }
                }
                else
                {
                    var outltId = (from q in db.tblOperators where q.UserId == WebSecurity.CurrentUserId select q.OutletId).FirstOrDefault();
                    var billdata = (from p in db.tblBillMasters
                                    where p.BillingType == "R"
                                    && p.OutletId == outltId
                                    select p).ToList();
                    foreach (var item in billdata)
                    {
                        BillingModel model = new BillingModel();
                        List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                        model.BillId = item.BillId;
                        model.BillDate = item.BillDate;
                        model.TotalAmount = item.TotalAmount;
                        model.VatAmount = item.VatAmount;
                        model.ServicChargeAmt = item.ServicChargesAmount;
                        if (item.ServiceTax.HasValue)
                        {
                            model.ServiceTax = item.ServiceTax.Value;
                        }
                        model.DiscountAmount = item.DiscountAmount;
                        model.NetAmount = item.NetAmount;
                        model.TableNo = item.TableNo;
                        model.Outletid = item.OutletId;
                        model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                        foreach (var i in db.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                        {
                            AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                            bill.Amount = i.Amount;
                            bill.BilldeatilId = i.BillDetailsId;
                            bill.FullQty = i.FullQty.Value;
                            bill.HalfQty = i.HalfQty.Value;
                            bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                            lstBill.Add(bill);
                        }
                        model.getBillItemDetails = lstBill;
                        list.Add(model);
                    }
                }
            }
            m.getAllBillReports = list;
            return View(m);
        }
        [HttpPost]
        public ActionResult HomeDeliveryReport(AdminBillReportModel model)
        {
            string[] roles = Roles.GetRolesForUser();
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    return View(search.getOutletSearchData(model, "H", WebSecurity.CurrentUserId));
                }
                else
                {
                    int Id = getOutletId();
                    return View(search.getOutletSearchData(model, "H", Id));
                }
            }
            return View();
        }

        public ActionResult RestroPrint(int id = 0)
        {
            var restrobilldata = (from p in db.tblBillMasters where p.BillId.Equals(id) select p).ToList();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in restrobilldata)
            {
                BillingModel model = new BillingModel();
                model.BillId = item.BillId;
                model.BillDate = item.BillDate;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.TableNo = item.TableNo;
                model.CustomerName = item.CustomerName;
                list.Add(model);
            }

            return View(list);
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
    }
}
