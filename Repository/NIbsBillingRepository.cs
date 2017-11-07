using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Xml.Linq;
using System.Web.Security;
using WebMatrix.WebData;

namespace NibsMVC.Repository
{
    public class NIbsBillingRepository
    {
        NIBSEntities entities = new NIBSEntities();
        XMLTablesRepository xml = new XMLTablesRepository();
        public bool CheckBillItem(GetBillItemModel model, string filepath, string KotFilePath)
            {
            XDocument xd = XDocument.Load(filepath);

            int oulte = getOutletId();
            var result = entities.tblBasePriceItems.Where(o => o.ItemId == model.ItemId).FirstOrDefault();
            var QtyValue = Convert.ToInt32(model.Qty);
            decimal finalfullamout = result.FullPrice * QtyValue;
            //decimal finalhalfamout = result.HalfPrice * QtyValue;
            if (model.Type == "Half")
            {
                xml.UpdateKotXmlData(filepath, KotFilePath, model.RunningTable.ToString(), result.tblItem.Name, "0", QtyValue.ToString());
            }
            else
            {
                xml.UpdateKotXmlData(filepath, KotFilePath, model.RunningTable.ToString(), result.tblItem.Name, QtyValue.ToString(), "0");
            }
            var items = from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == model.RunningTable.ToString() && item.Element("ItemId").Value == model.ItemId.ToString()
                        select item;
            if (items.Count() > 0)
            {
                if (model.Type == "Half")
                {

                    foreach (XElement itemElement in items)
                    {
                        var totalamount = Convert.ToDecimal(finalfullamout) + Convert.ToDecimal(itemElement.Element("Amount").Value);//finalhalfamout
                        var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);//
                        itemElement.SetElementValue("UserId", oulte.ToString());
                        itemElement.SetElementValue("TableNo", model.RunningTable.ToString());
                        itemElement.SetElementValue("ItemId", result.ItemId);
                        itemElement.SetElementValue("ItemName", result.tblItem.Name);
                        itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("Fullprice", result.FullPrice);
                        //itemElement.SetElementValue("HalfPrice", result.HalfPrice);
                        itemElement.SetElementValue("VatAmt", result.Vat);
                        itemElement.SetElementValue("HalfQty", Convert.ToInt32(QtyValue) + Convert.ToInt32(itemElement.Element("HalfQty").Value));
                        itemElement.SetElementValue("Amount", totalamount);
                        itemElement.SetElementValue("VatAmountCharges", vatamtchrg);
                        itemElement.SetElementValue("OfferQty", itemElement.Element("OfferQty").Value);
                    }

                }
                else
                {
                    foreach (XElement itemElement in items)
                    {
                        var totalamount = Convert.ToDecimal(finalfullamout) + Convert.ToDecimal(itemElement.Element("Amount").Value);
                        var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100); 
                        itemElement.SetElementValue("UserId", oulte.ToString());
                        itemElement.SetElementValue("TableNo", model.RunningTable.ToString());
                        itemElement.SetElementValue("ItemId", result.ItemId);
                        itemElement.SetElementValue("ItemName", result.tblItem.Name);
                        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                        itemElement.SetElementValue("Fullprice", result.FullPrice);
                        //itemElement.SetElementValue("HalfPrice", result.HalfPrice);
                        itemElement.SetElementValue("VatAmt", result.Vat);
                        itemElement.SetElementValue("FullQty", Convert.ToInt32(QtyValue) + Convert.ToInt32(itemElement.Element("FullQty").Value));
                        itemElement.SetElementValue("Amount", totalamount);
                        itemElement.SetElementValue("VatAmountCharges", vatamtchrg);
                        itemElement.SetElementValue("OfferQty", itemElement.Element("OfferQty").Value);
                    }

                }
                xd.Save(filepath);
            }
            else
            {
                if (model.Type == "Half")
                {
                    //var totalamount = Convert.ToDecimal(finalhalfamout);
                    //var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
                    //var newElement = new XElement("Items",
                    //     new XElement("UserId", oulte.ToString()),
                    //     new XElement("TableNo", model.RunningTable),
                    //     new XElement("ItemId", result.ItemId),
                    //     new XElement("ItemName", result.tblItem.Name),
                    //     new XElement("FullQty", "0"),
                    //     new XElement("HalfQty", QtyValue),
                    //     new XElement("Fullprice", result.FullPrice),
                    //     new XElement("HalfPrice", result.HalfPrice),
                    //     new XElement("Amount", totalamount),
                    //     new XElement("VatAmt", result.Vat),
                    //     new XElement("VatAmountCharges", vatamtchrg),
                    //     new XElement("OfferQty", "0"));
                    //xd.Element("Item").Add(newElement);
                }
                else
                {
                    var totalamount = Convert.ToDecimal(finalfullamout);
                    var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
                    var newElement = new XElement("Items",
                          new XElement("UserId", oulte.ToString()),
                          new XElement("TableNo", model.RunningTable),
                          new XElement("ItemId", result.ItemId),
                          new XElement("ItemName", result.tblItem.Name),
                          new XElement("FullQty", QtyValue),
                          new XElement("HalfQty", "0"),
                          new XElement("Fullprice", result.FullPrice),
                          //new XElement("HalfPrice", result.HalfPrice),
                          new XElement("Amount", totalamount),
                          new XElement("VatAmt", result.Vat),
                          new XElement("VatAmountCharges", vatamtchrg),
                          new XElement("OfferQty", "0"));
                    xd.Element("Item").Add(newElement);
                }


                xd.Save(filepath);

            }
            if (model.Type == "Full")
            {
                SameItemOfferRepository same = new SameItemOfferRepository();
                bool s = same.CallSameBuyOneGetOne(model.ItemId.ToString(), filepath, model.RunningTable.ToString(), model.Qty.ToString(), KotFilePath);
                if (!s)
                {
                    return true;
                }
                AdminCallHappyHoursRepository hh = new AdminCallHappyHoursRepository();;
                bool Happy = hh.CallHappyHoursDaysOffer(model.ItemId.ToString(), model.RunningTable.ToString(), filepath, KotFilePath);
                if (Happy == true)
                {
                    AdminCallOfferRepository offer = new AdminCallOfferRepository();
                    CallBuyOneGetOneANDBuyTwoGetOneRepository buy = new CallBuyOneGetOneANDBuyTwoGetOneRepository();
                    bool B1g1AndB2G1 = buy.CallOffer(model.ItemId.ToString(), filepath, model.RunningTable.ToString(), model.Qty.ToString(), KotFilePath);
                    if (B1g1AndB2G1)
                    {
                        bool Combo = offer.CallComboOffer(model.ItemId.ToString(), model.RunningTable.ToString(), filepath, KotFilePath);
                        if (Combo == true)
                        {
                            offer.CallAmountBasisOffer(model.ItemId.ToString(), model.RunningTable.ToString(), filepath, KotFilePath, model.Qty.ToString());
                        }
                    }
                    //bool Data = offer.CallOffer(model.ItemId.ToString(), filepath, model.RunningTable.ToString(), model.Qty.ToString(), KotFilePath);
                    //if (Data == true)
                    //{
                        
                    //}
                }
            }
            return true;
        }
        public PrintBillModel GetBill(string Path, GetBillingModel m)
        {
            PrintBillModel model = new PrintBillModel();
            List<PrintItemModel> lst = new List<PrintItemModel>();
            List<PrintVatModel> VatList = new List<PrintVatModel>();
            int oulte = getOutletId();
            var outletData = (from p in entities.tblOutlets where p.OutletId == oulte select p).SingleOrDefault();
            XDocument xd = XDocument.Load(Path);
            var result = from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == m.TableNo.ToString()
                         select item;
            var VatDetail = (from p in xd.Descendants("Items")
                             where p.Element("UserId").Value == oulte.ToString()
                             && p.Element("TableNo").Value == m.TableNo.ToString()
                             group p by p.Element("VatAmt").Value into g
                             select new
                             {
                                 Vat = g.Key,
                                 amtCharges = g.Sum(a => Convert.ToDecimal(a.Element("VatAmountCharges").Value))// xd.Descendants("Items").Sum(a => Convert.ToDecimal(a.Element("VatAmountCharges").Value))
                             }).ToList();
            model.BillId = m.BillId;
            model.TinNo = outletData.TinNo;
            model.ServiceTaxNo = outletData.ServiceTaxNo;
            model.Address = outletData.Address;
            model.ContactA = outletData.ContactA;
            model.CustomerAddress = m.CustomerAddress;
            model.TableNo = m.TableNo.ToString();
            model.DiscountAmount = Math.Truncate(m.DiscountAmount*100)/100;
            model.TotalAmount = Math.Truncate( m.TotalAmount*100)/100;;
            model.ServicesCharge =  Math.Truncate(m.ServicesCharge*100)/100;;
            model.ServiceTax = Math.Truncate( m.ServiceTax*100)/100;;
            model.NetAmount =  Math.Truncate(m.NetAmountWithoutDiscount*100)/100;;
            model.CustomerName = m.CustomerName;
            model.PackingCharge =  m.PackingCharges;
            model.NetAmountAfterDiscount = Math.Truncate(m.NetAmount * 100) / 100; 
            model.CustomerContactNo = m.ContactNo;
           // model.BillType=
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
                    var Name = entities.tblItems.Where(o => o.ItemId == Itemid).Select(x => x.Name).SingleOrDefault();
                    pm.ItemName = Name;
                }
                var amount = item.Element("Amount").Value;
                decimal amt = Convert.ToDecimal(amount);
                pm.Amount = Math.Round(amt,2);
                pm.FullQty = item.Element("FullQty").Value;
                pm.HalfQty = item.Element("HalfQty").Value;
                pm.BasicPrice = Math.Round(Convert.ToDecimal(item.Element("Fullprice").Value),2);
                lst.Add(pm);
            }
            model.getAllItem = lst;

            foreach (var item in VatDetail)
            {
                PrintVatModel pm = new PrintVatModel();
                decimal discountvat = 0;
                if (m.Discount>0)
                {
                    decimal discount = (m.Discount / 100) * item.amtCharges;
                    discountvat = item.amtCharges - discount;
                }
                else
                {
                    discountvat = item.amtCharges;
                }
                pm.amtCharges = discountvat;
                pm.Vat = Convert.ToDecimal(item.Vat);
                VatList.Add(pm);
            }
            model.getAllVat = VatList;
            return model;
        }
        public bool ShiftTable(string shiftfrom, string shiftto, int oulte, string shiftfromT, string shiftToT)
        {
            bool status = false;
            XDocument xd = XDocument.Load(shiftfrom);
            var items = (from p in xd.Descendants("Items")
                         where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == shiftfromT.ToString()
                         select p).ToList();

            XDocument xd1 = XDocument.Load(shiftto);
            foreach (XElement itemElement in items)
            {
                var newElement = new XElement("Items",
                        new XElement("UserId", oulte.ToString()),
                        new XElement("TableNo", shiftToT),
                        new XElement("ItemId", itemElement.Element("ItemId").Value),
                        new XElement("ItemName", itemElement.Element("ItemName").Value),
                        new XElement("FullQty", itemElement.Element("FullQty").Value),
                        new XElement("HalfQty", itemElement.Element("HalfQty").Value),
                        new XElement("Fullprice", itemElement.Element("Fullprice").Value),
                        new XElement("HalfPrice", itemElement.Element("HalfPrice").Value),
                        new XElement("Amount", itemElement.Element("Amount").Value),
                        new XElement("VatAmt", itemElement.Element("VatAmt").Value),
                        new XElement("VatAmountCharges", itemElement.Element("VatAmountCharges").Value),
                        new XElement("OfferQty", itemElement.Element("OfferQty").Value));
                xd1.Element("Item").Add(newElement);
                xd1.Save(shiftto);
            }
            items.Remove();
            xd.Save(shiftfrom);
            status = true;
            return status;
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
                    oulte = Convert.ToInt32((from n in entities.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }
        public bool _TransferToReturnXML(string TablePath, string ReturnxmlPath, int TableNo = 0, int ItemId = 0)
        {
            bool status = false;
            int OutletId = getOutletId();
            XDocument xd = XDocument.Load(TablePath);
            var items = (from p in xd.Descendants("Items")
                         where p.Element("UserId").Value == OutletId.ToString() && p.Element("TableNo").Value == TableNo.ToString()
                         && p.Element("ItemId").Value == ItemId.ToString()
                         select p).ToList();
            XDocument xdreturn = XDocument.Load(ReturnxmlPath);
            var returnitems = (from p in xdreturn.Descendants("Items")
                               where p.Element("UserId").Value == OutletId.ToString() && p.Element("TableNo").Value == TableNo.ToString()
                               && p.Element("ItemId").Value == ItemId.ToString()
                               select p).ToList();
            if (returnitems.Count > 0)
            {
                foreach (XElement itemElement in items)
                {

                    itemElement.SetElementValue("UserId", OutletId.ToString());
                    itemElement.SetElementValue("TableNo", TableNo.ToString());
                    itemElement.SetElementValue("ItemId", itemElement.Element("ItemId").Value);
                    itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                    itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                    itemElement.SetElementValue("Fullprice", itemElement.Element("Fullprice").Value);
                    itemElement.SetElementValue("HalfPrice", itemElement.Element("HalfPrice").Value);
                    itemElement.SetElementValue("VatAmt", itemElement.Element("VatAmt").Value);
                    itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                    itemElement.SetElementValue("Amount", itemElement.Element("Amount").Value);
                    itemElement.SetElementValue("VatAmountCharges", itemElement.Element("VatAmountCharges").Value);
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
                            new XElement("TableNo", TableNo),
                            new XElement("ItemId", itemElement.Element("ItemId").Value),
                            new XElement("ItemName", itemElement.Element("ItemName").Value),
                            new XElement("FullQty", itemElement.Element("FullQty").Value),
                            new XElement("HalfQty", itemElement.Element("HalfQty").Value),
                            new XElement("Fullprice", itemElement.Element("Fullprice").Value),
                            new XElement("HalfPrice", itemElement.Element("HalfPrice").Value),
                            new XElement("Amount", itemElement.Element("Amount").Value),
                            new XElement("VatAmt", itemElement.Element("VatAmt").Value),
                            new XElement("VatAmountCharges", itemElement.Element("VatAmountCharges").Value),
                            new XElement("OfferQty", itemElement.Element("OfferQty").Value));
                    xdreturn.Element("Item").Add(newElement);
                    xdreturn.Save(ReturnxmlPath);
                }
            }
            items.Remove();
            xd.Save(TablePath);
            status = true;
            return status;
        }
        public bool SaveReturnItem(string Path)
        {
            int OutletId = getOutletId();
            XDocument xd = XDocument.Load(Path);
            var items = (from p in xd.Descendants("Items")
                         where p.Element("UserId").Value == OutletId.ToString()
                         select p).ToList();
            if (items.Count > 0)
            {
                decimal AmountSum = Math.Round(items.Sum(a => Convert.ToDecimal(a.Element("Amount").Value)), 2);
                tbl_OutletReturnItem tb = new tbl_OutletReturnItem();
                var check = (from p in entities.tbl_OutletReturnItem where p.OutletId == OutletId select p).FirstOrDefault();
                if (check!=null)
                {
                    tb.OutletId = OutletId;
                tb.ReturnAmount =check.ReturnAmount+ AmountSum;
                entities.SaveChanges();
                }
                else
                {
                    tb.OutletId = OutletId;
                    tb.ReturnAmount = AmountSum;
                    entities.tbl_OutletReturnItem.Add(tb);
                    entities.SaveChanges();
                }
                items.Remove();
                xd.Save(Path);
            }
            return true;

        }
        public void SaveXmlOpenFood(BillOpenFoodModel model,string Path,string KotFilePath)
        {
            int oulte = getOutletId();
            XDocument xd = XDocument.Load(Path);
            var items = from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == model.TableNo.ToString() && item.Element("ItemName").Value == model.OpenFoodName.ToString()
                        select item;
            xml.UpdateKotXmlData(Path, KotFilePath, model.TableNo.ToString(), model.OpenFoodName, model.OpenFoodQuantity.ToString(), "0");
            if (items.Count() > 0)
            {
                foreach (XElement itemElement in items)
                {
                    var totalamount = (Convert.ToDecimal(model.OpenFoodPrice))*Convert.ToDecimal(model.OpenFoodQuantity) + Convert.ToDecimal(itemElement.Element("Amount").Value);
                    var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(model.OpenFoodVat)) / 100);
                    itemElement.SetElementValue("UserId", oulte.ToString());
                    itemElement.SetElementValue("TableNo", model.TableNo.ToString());
                    itemElement.SetElementValue("ItemId", "0");
                    itemElement.SetElementValue("ItemName", model.OpenFoodName);
                    itemElement.SetElementValue("HalfQty", "0");
                    itemElement.SetElementValue("Fullprice", model.OpenFoodPrice);
                    itemElement.SetElementValue("HalfPrice", "0");
                    itemElement.SetElementValue("VatAmt", model.OpenFoodVat);
                    itemElement.SetElementValue("FullQty", Convert.ToInt32(model.OpenFoodQuantity) + Convert.ToInt32(itemElement.Element("FullQty").Value));
                    itemElement.SetElementValue("Amount", totalamount);
                    itemElement.SetElementValue("VatAmountCharges", vatamtchrg);
                    itemElement.SetElementValue("OfferQty", "0");
                }
                xd.Save(Path);
            }
            else
            {
                var totalamount = (Convert.ToDecimal(model.OpenFoodPrice))*Convert.ToDecimal(model.OpenFoodQuantity);
                var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(model.OpenFoodVat)) / 100);
                var newElement = new XElement("Items",
                      new XElement("UserId", oulte.ToString()),
                      new XElement("TableNo", model.TableNo),
                      new XElement("ItemId", "0"),
                      new XElement("ItemName", model.OpenFoodName),
                      new XElement("FullQty", model.OpenFoodQuantity),
                      new XElement("HalfQty", "0"),
                      new XElement("Fullprice", model.OpenFoodPrice),
                      new XElement("HalfPrice", "0"),
                      new XElement("Amount", totalamount),
                      new XElement("VatAmt", model.OpenFoodVat),
                      new XElement("VatAmountCharges", vatamtchrg),
                      new XElement("OfferQty", "0"));
                xd.Element("Item").Add(newElement);
                xd.Save(Path);
            }
        }
    }
}