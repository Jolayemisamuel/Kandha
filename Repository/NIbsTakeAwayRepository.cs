using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Xml.Linq;
using System.Web.Mvc;
namespace NibsMVC.Repository
{
    public class NIbsTakeAwayRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        XMLTablesRepository xml = new XMLTablesRepository();
        public List<GetBillingSubItemModel> GetAllItems(string Id)
        {
            var ID = Convert.ToInt32(Id);
            int oulte = xml.getOutletId();
            var result = _entities.tblMenuOutlets.Where(o => o.OutletId == oulte && o.CategoryId == ID).ToList();
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            // query for when item is false in tbitems
            var dd = (from p in _entities.tblItems
                      join q in _entities.tblMenuOutlets
                          on p.ItemId equals q.ItemId
                      where p.Active == true &&
                      q.OutletId == oulte &&
                      q.CategoryId == ID
                      select new
                      {
                          ItemId = q.ItemId,
                          Name = q.tblItem.Name,
                          Color = p.tblCategory.Color,
                          TextColor = p.tblCategory.TextColor
                      }).ToList();
            foreach (var item in dd)
            {
                GetBillingSubItemModel model = new GetBillingSubItemModel();
                model.Color = item.Color;
                model.ItemId = item.ItemId;
                model.Name = item.Name;
                model.TextColor = item.TextColor;
                lst.Add(model);
            }

            return lst;
        }
        public int GetTokenNo()
        {
            var tokencount = (from q in _entities.tblBillMasters where q.BillingType.Equals("Take Away Hall") select new { BillId = q.BillId, Date = q.BillDate }).ToList();
            var Curentdate = DateTime.Now.Date;
            int no;
            if (tokencount.Count() > 0)
            {
                int tokenMax = _entities.tblBillMasters.Max(a => a.BillId);
                var LastDate = (from p in _entities.tblBillMasters where p.BillId == tokenMax select p.BillDate).SingleOrDefault();
                if (Curentdate == LastDate)
                {
                    no = tokencount.Count;
                    no = no + 1;
                }
                else
                {
                    no = 1;
                }
            }
            else
            {
                no = 1;
            }
            return no;
        }
        public bool GetXmlData(string Id, string Path, string Qty, string Type, string KotFilePath, string TokenNo)
        {
            int oulte = xml.getOutletId();
            var itemid = Convert.ToInt32(Id);
            var result = _entities.tblBasePriceItems.Where(o => o.ItemId == itemid).FirstOrDefault();
            var QtyValue = Convert.ToInt32(result.Vat);
            decimal finalfullamout = result.FullPrice * QtyValue;
            XDocument xd = XDocument.Load(Path);
            var items = (from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString() && item.Element("ItemName").Value == result.tblItem.Name
                        select item).ToList();
            if (Type == "Half")
            {
                //if (items.Count() > 0)
                //{
                //    foreach (XElement itemElement in items)
                //    {
                //        int Half = Convert.ToInt32(Qty) + Convert.ToInt32(itemElement.Element("HalfQty").Value);
                //        decimal TotalAmount = Convert.ToDecimal(itemElement.Element("TotalAmount").Value) + Convert.ToDecimal(result.HalfPrice);
                //        var vatamtchrg = (TotalAmount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
                //        itemElement.SetElementValue("UserId", oulte);
                //        itemElement.SetElementValue("ItemId", result.ItemId);
                //        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                //        itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                //        itemElement.SetElementValue("HalfQty", Half);
                //        itemElement.SetElementValue("HalfPrice", result.HalfPrice);
                //        itemElement.SetElementValue("FullPrice", result.FullPrice);
                //        itemElement.SetElementValue("Vat", result.Vat);
                //        itemElement.SetElementValue("TotalAmount", TotalAmount);
                //        itemElement.SetElementValue("VatAmt", vatamtchrg);
                //        itemElement.SetElementValue("OfferQty", "0");
                //        xd.Save(Path);
                //    }
                //}
                //else
                //{

                //    var newElement = new XElement("Items",
                //         new XElement("UserId", oulte),
                //          new XElement("ItemId", result.ItemId),
                //         new XElement("ItemName", result.tblItem.Name),
                //         new XElement("FullQty", "0"),
                //         new XElement("HalfQty", Qty),
                //          new XElement("HalfPrice", result.HalfPrice),
                //           new XElement("FullPrice", result.FullPrice),
                //            new XElement("Vat", result.Vat),
                //            new XElement("TotalAmount", (result.HalfPrice * (Convert.ToDecimal(Qty)))),
                //              new XElement("VatAmt", Convert.ToDecimal(result.HalfPrice) * Convert.ToDecimal(result.Vat) / 100),
                //              new XElement("OfferQty", "0"));
                //    xd.Element("Item").Add(newElement);
                //    xd.Save(Path);
                //}
                //UpdateKotXmlData(Path, KotFilePath, TokenNo, result.tblItem.Name, "0", Qty);
            }
            else
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {

                        int Full = Convert.ToInt32(Qty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                        decimal TotalAmount = (Convert.ToDecimal(itemElement.Element("TotalAmount").Value) + Convert.ToDecimal(result.FullPrice))*Convert.ToDecimal(Qty);
                        decimal newvatamount = ((TotalAmount * Convert.ToDecimal(result.Vat)) / 100);
                        var vatamtchrg = (TotalAmount * (Convert.ToDecimal(itemElement.Element("Vat").Value)) / 100);
                        itemElement.SetElementValue("UserId", oulte);
                        itemElement.SetElementValue("ItemId", result.ItemId);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", Full);
                        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                        //itemElement.SetElementValue("HalfPrice", result.HalfPrice);
                        itemElement.SetElementValue("FullPrice", result.FullPrice);
                        itemElement.SetElementValue("Vat", result.Vat);
                        itemElement.SetElementValue("TotalAmount", TotalAmount);
                        itemElement.SetElementValue("VatAmt", vatamtchrg);
                        itemElement.SetElementValue("OfferQty", "0");
                        xd.Save(Path);
                    }
                }
                else
                {
                    var vatamount = (Convert.ToDecimal(result.Vat) / 100) * Convert.ToDecimal(result.FullPrice)*Convert.ToDecimal(Qty); 
                     var newElement = new XElement("Items",
                         new XElement("UserId", oulte),
                          new XElement("ItemId", result.ItemId),
                         new XElement("ItemName", result.tblItem.Name),
                         new XElement("FullQty", Qty),
                         new XElement("HalfQty", "0"),
                          //new XElement("HalfPrice", result.HalfPrice),
                           new XElement("FullPrice", result.FullPrice),
                            new XElement("Vat", result.Vat),
                             new XElement("TotalAmount", result.FullPrice * (Convert.ToDecimal(Qty))),
                              new XElement("VatAmt", vatamount),
                              new XElement("OfferQty", "0"));

                    xd.Element("Item").Add(newElement);
                    xd.Save(Path);
                }
                UpdateKotXmlData(Path, KotFilePath, TokenNo, result.tblItem.Name, Qty, "0");
                if (Type == "Full")
                {
                    TakeAwayhappHourOfferRepository hh = new TakeAwayhappHourOfferRepository();
                    bool Happy = hh.CallHappyHoursDaysOffer(Id, TokenNo, Path, KotFilePath, result.tblItem.Name);
                    if (Happy == true)
                    {
                        CallTakeAwayOfferRepository offer = new CallTakeAwayOfferRepository();
                        bool Data = offer.CallOffer(Id, Path, TokenNo, Qty, KotFilePath, result.tblItem.Name);
                        if (Data == true)
                        {
                            bool Combo = offer.CallComboOffer(Id, TokenNo, Path, KotFilePath);
                            if (Combo == true)
                            {
                                offer.CallAmountBasisOffer(Id, TokenNo, Path, KotFilePath, Qty);
                            }
                        }
                    }
                }
            }
            return true;
        }
        public bool UpdateKotXmlData(string Path, string KotPath, string TokenNo, string ItemName, string FullQty, string HalfQty)
        {
            int oulte = xml.getOutletId();
            XDocument xd = XDocument.Load(KotPath);
            var items = from item in xd.Descendants("Items")
                        where item.Element("TokenNo").Value == TokenNo.ToString() && item.Element("ItemName").Value == ItemName && item.Element("UserId").Value == oulte.ToString()
                        select item;
            if (HalfQty == "0")
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {
                        int Full = Convert.ToInt32(FullQty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                        itemElement.SetElementValue("TokenNo", itemElement.Element("TokenNo").Value);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", Full);
                        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                          new XElement("UserId", oulte),
                         new XElement("TokenNo", TokenNo),
                         new XElement("ItemName", ItemName),
                         new XElement("FullQty", FullQty),
                         new XElement("HalfQty", HalfQty));
                    xd.Element("Item").Add(newElement);
                }
            }
            else
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {
                        int half = Convert.ToInt32(HalfQty) + Convert.ToInt32(itemElement.Element("HalfQty").Value);
                        itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                        itemElement.SetElementValue("TokenNo", itemElement.Element("TokenNo").Value);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("HalfQty", half);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                         new XElement("UserId", oulte),
                         new XElement("TokenNo", TokenNo),
                         new XElement("ItemName", ItemName),
                         new XElement("FullQty", FullQty),
                         new XElement("HalfQty", HalfQty));
                    xd.Element("Item").Add(newElement);
                }

            }


            xd.Save(KotPath);
            return true;
        }

        public GetBillingModel GetBillingItem(string Path, string KotPath)
        {
            GetBillingModel model = new GetBillingModel();
            List<GetBillingItemModel> itemslst = new List<GetBillingItemModel>();
            List<GetKotItemModel> kotList = new List<GetKotItemModel>();
            XDocument xd = XDocument.Load(Path);
            int oulte = xml.getOutletId();
            var result = (from item in xd.Descendants("Items")
                          where item.Element("UserId").Value == oulte.ToString() 
                          select item).ToList();
            foreach (var item in result)
            {
                GetBillingItemModel m = new GetBillingItemModel();
                m.ItemName = item.Element("ItemName").Value;
                m.FullQty = Convert.ToInt32(item.Element("FullQty").Value);
                m.FullPrice = Math.Round(Convert.ToDecimal(item.Element("FullPrice").Value),2);
                m.Amount = Math.Round(Convert.ToDecimal(item.Element("TotalAmount").Value),2);
                m.Vat = Convert.ToDecimal(item.Element("Vat").Value);
                m.OfferQty = Convert.ToInt32(item.Element("OfferQty").Value);
                m.ItemId = Convert.ToInt32(item.Element("ItemId").Value);
                itemslst.Add(m);
            }
            model._getbillingItems = itemslst;
            XDocument xdKot = XDocument.Load(KotPath);
            var kotresult = (from k in xdKot.Descendants("Items")
                             where k.Element("UserId").Value == oulte.ToString()
                             select k).ToList();
            foreach (var item in kotresult)
            {
                GetKotItemModel m = new GetKotItemModel();
                m.TNo = Convert.ToInt32(item.Element("TokenNo").Value);
                m.ItemName = item.Element("ItemName").Value;
                m.FullQty = Convert.ToInt32(item.Element("FullQty").Value);
                m.HalfQty = Convert.ToInt32(item.Element("HalfQty").Value);
                kotList.Add(m);
            }
            var serviccharg = (from p in _entities.tbl_ServiceTax select p.ServiceCharge).FirstOrDefault();
            if (serviccharg == 0)
            {
                //serviccharg = Convert.ToDecimal(4.940);
                serviccharg = Convert.ToDecimal(5.6);
            }
            model._getKotitems = kotList;
            model.getPaymentType = getPaymentMode();
            //model.TableNo = ItemId;
            model.VatAmount = Math.Round(result.Sum(a => Convert.ToDecimal(a.Element("VatAmt").Value)), 2);
            model.TotalAmount = Math.Round(result.Sum(a => Convert.ToDecimal(a.Element("TotalAmount").Value)), 2);
            decimal ServiceChargeAmount = (model.TotalAmount * serviccharg) / 100;
            model.ServiceTax = Math.Round(ServiceChargeAmount, 2);
            model.NetAmount = Math.Round(model.TotalAmount + model.VatAmount + model.ServiceTax, 2);
            model.TableNo = GetTokenNo().ToString();
            return model;

        }
        public List<SelectListItem> getPaymentMode()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Cash", Value = "Cash" });
            list.Add(new SelectListItem { Text = "Cheque", Value = "Cheque" });
            list.Add(new SelectListItem { Text = "Card", Value = "Card" });
            list.Add(new SelectListItem { Text = "Cash&Card", Value = "Cash&Card" });
            return list;
        }
        public bool DeleteNode(string Id, string Path, string KotFilePath)
        {

            int oulte = xml.getOutletId();
            string TokenNo = GetTokenNo().ToString();
            //AdminCallOfferRepository offer = new AdminCallOfferRepository();
            //offer.RemoveOffer(Id, Path, TokenNo);
            XDocument xd = XDocument.Load(Path);
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString() && item.Element("ItemId").Value == Id 
                         select item).ToList();
            items.Remove();
            xd.Save(Path);
            int ItemId = Convert.ToInt32(Id);
            var Name = _entities.tblItems.Where(o => o.ItemId == ItemId).Select(o => o.Name).FirstOrDefault();
            XDocument Kot = XDocument.Load(KotFilePath);
            var KotItems = (from item in Kot.Descendants("Items")
                            where item.Element("UserId").Value == oulte.ToString() && item.Element("TokenNo").Value == TokenNo && item.Element("ItemName").Value == Name
                            select item).ToList();
            KotItems.Remove();
            Kot.Save(KotFilePath);
            return true;
            
        }
        public bool ClearKot(string KotPath)
        {

            int oulte = xml.getOutletId();
            XDocument xd = XDocument.Load(KotPath);
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item);
            items.Remove();
            xd.Save(KotPath);
            return true;
            //return FillFromXmlData(Id, Path, KotPath);
        }
        public bool _TransferToReturnXML(string TablePath, string ReturnxmlPath,string KotFilePath, int ItemId = 0)
        {
            bool status = false;
            int OutletId = xml.getOutletId();
            int TokenNo = GetTokenNo();
            XDocument xd = XDocument.Load(TablePath);
            var items = (from p in xd.Descendants("Items")
                         where p.Element("UserId").Value == OutletId.ToString() 
                         && p.Element("ItemId").Value == ItemId.ToString()
                         select p).ToList();

            XDocument xdreturn = XDocument.Load(ReturnxmlPath);
            var returnitems = (from p in xdreturn.Descendants("Items")
                               where p.Element("UserId").Value == OutletId.ToString() && p.Element("TokenNo").Value == TokenNo.ToString()
                               && p.Element("ItemId").Value == ItemId.ToString()
                               select p).ToList();
            if (returnitems.Count > 0)
            {
                foreach (XElement itemElement in items)
                {

                    itemElement.SetElementValue("UserId", OutletId.ToString());
                    itemElement.SetElementValue("ItemId", itemElement.Element("ItemId").Value);
                    itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                    itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                    itemElement.SetElementValue("FullPrice", itemElement.Element("FullPrice").Value);
                    itemElement.SetElementValue("HalfPrice", itemElement.Element("HalfPrice").Value);
                    itemElement.SetElementValue("Vat", itemElement.Element("Vat").Value);
                    itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                    itemElement.SetElementValue("TotalAmount", itemElement.Element("TotalAmount").Value);
                    itemElement.SetElementValue("VatAmt", itemElement.Element("VatAmt").Value);
                    itemElement.SetElementValue("OfferQty", itemElement.Element("OfferQty").Value);
                }
                xdreturn.Save(ReturnxmlPath);
            }
            else
            {
                foreach (XElement itemElement in items)
                {
                    var newElement = new XElement("Items",
                            new XElement("UserId", OutletId.ToString()),
                            new XElement("ItemId", itemElement.Element("ItemId").Value),
                            new XElement("ItemName", itemElement.Element("ItemName").Value),
                            new XElement("FullQty", itemElement.Element("FullQty").Value),
                            new XElement("HalfQty", itemElement.Element("HalfQty").Value),
                            new XElement("FullPrice", itemElement.Element("FullPrice").Value),
                            new XElement("HalfPrice", itemElement.Element("HalfPrice").Value),
                            new XElement("TotalAmount", itemElement.Element("TotalAmount").Value),
                            new XElement("Vat", itemElement.Element("Vat").Value),
                            new XElement("VatAmt", itemElement.Element("VatAmt").Value),
                            new XElement("OfferQty", itemElement.Element("OfferQty").Value));
                    xdreturn.Element("Item").Add(newElement);
                    xdreturn.Save(ReturnxmlPath);
                }
            }
            items.Remove();
            xd.Save(TablePath);
            XDocument Kot = XDocument.Load(KotFilePath);
            var Name = _entities.tblItems.Where(o => o.ItemId == ItemId).Select(o => o.Name).FirstOrDefault();
            var KotItems = (from item in Kot.Descendants("Items")
                            where item.Element("UserId").Value == OutletId.ToString() && item.Element("TokenNo").Value == TokenNo.ToString() && item.Element("ItemName").Value == Name
                            select item).ToList();
            KotItems.Remove();
            Kot.Save(KotFilePath);
            status = true;
            return status;
        }

        public PrintBillModel GetBill(string Path, GetBillingModel m)
        {
            PrintBillModel model = new PrintBillModel();
            List<PrintItemModel> lst = new List<PrintItemModel>();
            List<PrintVatModel> VatList = new List<PrintVatModel>();
            int oulte = xml.getOutletId();
            var outletData = (from p in _entities.tblOutlets where p.OutletId == oulte select p).SingleOrDefault();
            XDocument xd = XDocument.Load(Path);
            var result = from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString() 
                         select item;
            var VatDetail = (from p in xd.Descendants("Items")
                             where p.Element("UserId").Value == oulte.ToString()
                             group p by p.Element("Vat").Value into g
                             select new
                             {
                                 Vat = g.Key,
                                 amtCharges = g.Sum(a => Convert.ToDecimal(a.Element("VatAmt").Value))// xd.Descendants("Items").Sum(a => Convert.ToDecimal(a.Element("VatAmountCharges").Value))
                             }).ToList();
            model.TinNo = outletData.TinNo;
            model.ServiceTaxNo = outletData.ServiceTaxNo;
            model.Address = outletData.Address;
            model.ContactA = outletData.ContactA;
            model.CustomerName = string.Empty;
            model.TableNo = m.TableNo.ToString();
            model.DiscountAmount = m.DiscountAmount;
            model.TotalAmount = m.TotalAmount;
            model.ServicesCharge = m.ServicesCharge;
            model.NetAmount = m.NetAmount;
            model.CustomerName = m.CustomerName;
            foreach (var item in result)
            {
                PrintItemModel pm = new PrintItemModel();
                int Itemid = Convert.ToInt32(item.Element("ItemId").Value);
                if (Itemid == 0)
                {
                    pm.ItemName = item.Element("ItemName").Value;
                }
                else
                {
                    var Name = _entities.tblItems.Where(o => o.ItemId == Itemid).Select(x => x.Name).SingleOrDefault();
                    pm.ItemName = Name;
                }
                var amount = item.Element("TotalAmount").Value;
                decimal amt = Convert.ToDecimal(amount);
                pm.Amount = amt;
                pm.FullQty = item.Element("FullQty").Value;
                pm.HalfQty = item.Element("HalfQty").Value;
                lst.Add(pm);
            }
            model.getAllItem = lst;

            foreach (var item in VatDetail)
            {
                PrintVatModel pm = new PrintVatModel();
                pm.amtCharges = item.amtCharges;
                pm.Vat = Convert.ToDecimal(item.Vat);
                VatList.Add(pm);
            }
            model.getAllVat = VatList;
            return model;
        }
        public bool DispatchOrder(GetBillingModel model, string Path)
        {

            try
            {
                int oulte = xml.getOutletId();
                XDocument xd = XDocument.Load(Path);
                var result = (from item in xd.Descendants("Items")
                              where item.Element("UserId").Value == oulte.ToString()
                              select item).ToList();

                tblBillMaster tb = new tblBillMaster();
                tb.BillDate = DateTime.Now.Date;
                tb.BillingType = "T";
                tb.CustomerName = model.CustomerName;
                tb.DiscountAmount = model.DiscountAmount;
                tb.NetAmount = Math.Round(model.NetAmount, 2);
                tb.ServicChargesAmount = Math.Round(model.ServicesCharge, 2);
                tb.TotalAmount = Math.Round(model.TotalAmount, 2);
                tb.VatAmount = Math.Round(model.VatAmount, 2);
                tb.TokenNo = Convert.ToInt32(model.TableNo);
                tb.PaymentType = model.PaymentType;
                tb.ChequeNo = model.ChequeNo;
                tb.ChequeDate = model.CheckDate;
                tb.OutletId = oulte;
                tb.Discount = model.Discount;
                _entities.tblBillMasters.Add(tb);
                _entities.SaveChanges();
                //var Id = _entities.tblBillMasters.Where(o => o.OutletId == oulte && o.BillingType == "R").Select(x => x.BillId).Max();

                foreach (var item in result)
                {
                      tblBillDetail bill = new tblBillDetail();
                        bill.Amount = Convert.ToDecimal(item.Element("TotalAmount").Value);
                        bill.FullQty = Convert.ToInt32(item.Element("FullQty").Value);
                        bill.HalfQty = Convert.ToInt32(item.Element("HalfQty").Value);
                        bill.ItemId = Convert.ToInt32(item.Element("ItemId").Value);
                        bill.Vat = Convert.ToDecimal(item.Element("Vat").Value);
                        decimal VatAmt = 0;
                        if (model.Discount > 0)
                        {
                            decimal dis = Convert.ToDecimal(item.Element("VatAmt").Value);
                            decimal vat = (model.Discount * dis) / 100;
                            VatAmt = dis - vat;
                        }
                        else
                        {
                            VatAmt = Convert.ToDecimal(item.Element("VatAmt").Value);
                        }
                        bill.VatAmount = VatAmt * Convert.ToInt32(item.Element("FullQty").Value);
                        bill.BillId = tb.BillId;
                        _entities.tblBillDetails.Add(bill);
                        _entities.SaveChanges();
                        // call auto inventory start..
                        AutoInventoryRepository auto = new AutoInventoryRepository();
                        auto.AutoInventory(Convert.ToInt32(item.Element("ItemId").Value), Convert.ToInt32(item.Element("FullQty").Value), Convert.ToInt32(item.Element("HalfQty").Value), oulte);

                   

                }

                var items = (from item in xd.Descendants("Items")
                             where item.Element("UserId").Value == oulte.ToString()
                             select item);

                items.Remove();
                xd.Save(Path);
                CheckStockItemRepository check = new CheckStockItemRepository();
                string chk = check.OutStockItems(tb.BillId);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SaveReturnItem(string Path)
        {
            int OutletId = xml.getOutletId();
            XDocument xd = XDocument.Load(Path);
            var items = (from p in xd.Descendants("Items")
                         where p.Element("UserId").Value == OutletId.ToString()
                         select p).ToList();
            if (items.Count > 0)
            {
                decimal AmountSum = Math.Round(items.Sum(a => Convert.ToDecimal(a.Element("TotalAmount").Value)), 2);
                tbl_OutletReturnItem tb = new tbl_OutletReturnItem();
                var check = (from p in _entities.tbl_OutletReturnItem where p.OutletId == OutletId select p).FirstOrDefault();
                if (check != null)
                {
                    tb.OutletId = OutletId;
                    tb.ReturnAmount = check.ReturnAmount + AmountSum;
                    _entities.SaveChanges();
                }
                else
                {
                    tb.OutletId = OutletId;
                    tb.ReturnAmount = AmountSum;
                    _entities.tbl_OutletReturnItem.Add(tb);
                    _entities.SaveChanges();
                }
                items.Remove();
                xd.Save(Path);
            }
            return true;

        }
       
    }
}