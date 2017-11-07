using NibsMVC.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using WebMatrix.WebData;

namespace NibsMVC.Repository
{
    public class CallBuyOneGetOneANDBuyTwoGetOneRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public bool CallOffer(string Id, string Path, string TableNo, string Qty, string KotPath)
        {
            bool status = false;

            int oulte = getOutletId();
            XDocument xd = XDocument.Load(Path);
            var CurrentDay = DateTime.Now.DayOfWeek.ToString();
            int ItemId = Convert.ToInt32(Id);
            var JoinId = (from p in _entities.Nibs_Offer_Buy_Items
                          join q in _entities.Nibs_Offer_Days
                          on p.OfferId equals q.OfferId
                          join f in _entities.NIbs_AssignOffer
                          on p.OfferId equals (f.OfferId)
                          where p.ItemId == ItemId
                          && f.UserId == oulte
                          && q.Days.Equals(CurrentDay)
                          select new { p.OfferId, p.Nibs_Offer.OfferType }).FirstOrDefault();

            if (JoinId == null)
            {
                status = true;
                return status;
            }
            if (JoinId != null)
            {
                if (JoinId.OfferId == 0)
                {
                    return status = true;
                }
                if (JoinId.OfferType != "b1g1" && JoinId.OfferType != "b2g1")
                {
                    return status = true;
                }

                if (JoinId.OfferType == "b1g1")
                {
                    var OfferBuyItemsItems = (from p in _entities.Nibs_Offer_Buy_Items
                                              where p.OfferId == JoinId.OfferId
                                              select new
                                              {
                                                  ItemId = p.ItemId,
                                                  Qty = p.Quantity
                                              }).FirstOrDefault();
                    var ItemsOffer = (from free in xd.Descendants("Items")
                                      where free.Element("UserId").Value == oulte.ToString()
                                      && free.Element("TableNo").Value == TableNo
                                      && free.Element("ItemId").Value == OfferBuyItemsItems.ItemId.ToString()
                                        && Convert.ToInt32(free.Element("FullQty").Value) >= OfferBuyItemsItems.Qty
                                      select new
                                      {
                                          ItemId = free.Element("ItemId").Value,
                                          FullQty = free.Element("FullQty").Value
                                      }).ToList();

                    if (ItemsOffer.Count == 0)
                    {

                        status = true;
                        return status;
                    }
                    else
                    {
                        int ItemOffersBuyQty = ItemsOffer.Sum(a => int.Parse(a.FullQty));
                        if (ItemOffersBuyQty % OfferBuyItemsItems.Qty != 0)
                        {
                            status = true;
                            return status;
                        }
                    }
                    var freeitem = (from p in _entities.Nibs_Offer_Free_Items
                                    where p.OfferId == JoinId.OfferId
                                    select new { ItemId = p.ItemId, ItemName = p.tblItem.Name, FreeQty = p.Quantity, Discount = p.Discount }).FirstOrDefault();
                    if (freeitem != null)
                    {
                        var Prices = _entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).FirstOrDefault();
                        var buyItem = (from item in xd.Descendants("Items")
                                       where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                                       && item.Element("ItemId").Value == Id.ToString()
                                       select item).ToList();
                        int BuyQuantity = buyItem.Sum(a => int.Parse(a.Element("FullQty").Value));
                        var freexmlItem = (from item in xd.Descendants("Items")
                                           where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                                           && item.Element("ItemId").Value == freeitem.ItemId.ToString()
                                           select item).ToList();
                        int FreeQuantity = freexmlItem.Sum(a => int.Parse(a.Element("FullQty").Value));

                        if (freexmlItem.Count() > 0)
                        {
                            int TotalQty = 0;
                            int KotQty = 0;
                            foreach (XElement itemElement in freexmlItem)
                            {

                                int offerQty = Convert.ToInt32(itemElement.Element("OfferQty").Value);
                                TotalQty = offerQty + Convert.ToInt32(itemElement.Element("FullQty").Value);
                                int FullQty = 0;
                                if (TotalQty > BuyQuantity)
                                {
                                    FullQty = TotalQty - BuyQuantity;
                                    offerQty = BuyQuantity / OfferBuyItemsItems.Qty;
                                    KotQty = TotalQty;
                                }
                                else
                                {

                                    offerQty = BuyQuantity / OfferBuyItemsItems.Qty;
                                    if (FreeQuantity > offerQty)
                                    {
                                        FullQty = FreeQuantity - offerQty;
                                    }
                                    else
                                    {
                                        FullQty = 0;
                                    }
                                    KotQty = offerQty;
                                }

                                int RemainingOfferQty = TotalQty - FullQty;
                                decimal TotalAmount = Convert.ToDecimal(itemElement.Element("Amount").Value);
                                decimal FullQtyAmount = (Convert.ToDecimal(itemElement.Element("Fullprice").Value) * FullQty);
                                decimal OfferQtyAmount = Convert.ToDecimal((freeitem.Discount / 100) * FullQtyAmount);
                                decimal TotalAmountAfterOffer = FullQtyAmount - OfferQtyAmount;
                                decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                                decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                                decimal remaingvatamtchrg = ((TotalAmountAfterOffer * vatamt) / 100);
                                itemElement.SetElementValue("FullQty", FullQty);
                                itemElement.SetElementValue("VatAmt", vatamt);
                                itemElement.SetElementValue("Amount", FullQtyAmount);
                                itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                                itemElement.SetElementValue("OfferQty", offerQty);
                            }
                            xd.Save(Path);
                            XDocument kot = XDocument.Load(KotPath);
                            var kotdata = (from p in kot.Descendants("Items")
                                           where p.Element("TableNo").Value == TableNo.ToString()
                                           && p.Element("ItemName").Value == freeitem.ItemName
                                           && p.Element("UserId").Value == oulte.ToString()
                                           select p).ToList();
                            int kotint = kotdata.Sum(a => int.Parse(a.Element("FullQty").Value));
                            if (KotQty > kotint)
                            {
                                if (kotdata.Count > 0)
                                {
                                    foreach (XElement itemElement in kotdata)
                                    {
                                        itemElement.SetElementValue("FullQty", KotQty);
                                    }
                                }
                                else
                                {
                                    var newElement = new XElement("Items",
                                           new XElement("UserId", oulte),
                                            new XElement("TableNo", TableNo),
                                            new XElement("ItemName", freeitem.ItemName),
                                            new XElement("FullQty", Qty),
                                            new XElement("HalfQty", "0"));
                                    kot.Element("Item").Add(newElement);
                                }
                                kot.Save(KotPath);
                            }

                        }
                        else
                        {
                            var Item = _entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).SingleOrDefault();
                            decimal Amount = Item.FullPrice;
                            decimal DiscountAmount = Convert.ToDecimal(((freeitem.Discount / 100) * Amount));
                            decimal RemainingAmount = Amount - DiscountAmount;
                            decimal remaingvatamtchrg = Convert.ToDecimal((RemainingAmount * Convert.ToDecimal(Item.Vat)) / 100);

                            var newElement = new XElement("Items",
                              new XElement("UserId", oulte.ToString()),
                               new XElement("TableNo", TableNo),
                           new XElement("ItemId", freeitem.ItemId),
                           new XElement("ItemName", freeitem.ItemName),
                           new XElement("FullQty", "0"),
                           new XElement("HalfQty", "0"),
                           new XElement("Fullprice", Prices.FullPrice),
                          // new XElement("HalfPrice", Prices.HalfPrice),
                            new XElement("Amount", RemainingAmount),
                           //new XElement("VatAmt", Item.Vat),
                            new XElement("VatAmountCharges", remaingvatamtchrg),
                             new XElement("OfferQty", Qty));
                            xd.Element("Item").Add(newElement);
                            xd.Save(Path);
                            XDocument kot1 = XDocument.Load(KotPath);
                            var kotdata1 = (from p in kot1.Descendants("Items")
                                            where p.Element("TableNo").Value == TableNo.ToString()
                                            && p.Element("ItemName").Value == freeitem.ItemName
                                            && p.Element("UserId").Value == oulte.ToString()
                                            select p).ToList();
                            var newElement1 = new XElement("Items",
                                             new XElement("UserId", oulte),
                                              new XElement("TableNo", TableNo),
                                              new XElement("ItemName", freeitem.ItemName),
                                              new XElement("FullQty", Qty),
                                              new XElement("HalfQty", "0"));
                            kot1.Element("Item").Add(newElement1);
                            kot1.Save(KotPath);
                        }



                    }
                }
                else
                {
                    bool callagainb2g1 = false;
                    int checkagainb2g1 = 0;
                    var OfferBuyItems = (from p in _entities.Nibs_Offer_Buy_Items
                                         where p.OfferId == JoinId.OfferId
                                         select new
                                         {
                                             ItemId = p.ItemId,
                                             Qty = p.Quantity
                                         }).ToList();
                    int checkQtyRatio = 0;
                    foreach (var item in OfferBuyItems)
                    {
                       
                        var ItemsOffer = (from free in xd.Descendants("Items")
                                          where free.Element("UserId").Value == oulte.ToString()
                                          && free.Element("TableNo").Value == TableNo
                                          && free.Element("ItemId").Value == item.ItemId.ToString()
                                            && Convert.ToInt32(free.Element("FullQty").Value) >= item.Qty
                                          select new
                                          {
                                              ItemId = free.Element("ItemId").Value,
                                              FullQty = free.Element("FullQty").Value
                                          }).ToList();
                        if (ItemsOffer.Count == 0)
                        {

                            status = true;
                            return status;
                        }
                        else
                        {
                            if (Convert.ToInt32(ItemsOffer.FirstOrDefault().FullQty) == checkagainb2g1)
                            {
                                callagainb2g1 = true;
                                if (checkQtyRatio > 0)
                                {
                                    if (checkagainb2g1 % checkQtyRatio == 0)
                                    {
                                        callagainb2g1 = true;
                                    }

                                }
                                else
                                {
                                    checkQtyRatio = ItemsOffer.Sum(a => int.Parse(a.FullQty));
                                }
                            }
                            
                            checkagainb2g1 = Convert.ToInt32(ItemsOffer.FirstOrDefault().FullQty);
                            
                           
                            if (checkagainb2g1 % Convert.ToInt32(OfferBuyItems.FirstOrDefault().Qty) != 0)
                            {
                                status = true;
                                break;
                            }
                            
                            //int ItemOffersBuyQty = ItemsOffer.Sum(a => int.Parse(a.FullQty));
                            //if (ItemOffersBuyQty % item.Qty != 0)
                            //{
                            //    status = true;
                            //    return status;
                            //}
                        }
                    }
                    if (!status && callagainb2g1)
                    {
                        var freeitem = (from p in _entities.Nibs_Offer_Free_Items
                                        where p.OfferId == JoinId.OfferId
                                        select new { ItemId = p.ItemId, ItemName = p.tblItem.Name, FreeQty = p.Quantity, Discount = p.Discount }).FirstOrDefault();
                        if (freeitem != null)
                        {
                            var Prices = _entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).FirstOrDefault();
                            var buyItem = (from item in xd.Descendants("Items")
                                           where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                                           && item.Element("ItemId").Value == Id.ToString()
                                           select item).ToList();
                            int BuyQuantity = buyItem.Sum(a => int.Parse(a.Element("FullQty").Value));
                            var freexmlItem = (from item in xd.Descendants("Items")
                                               where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                                               && item.Element("ItemId").Value == freeitem.ItemId.ToString()
                                               select item).ToList();
                            int FreeQuantity = freexmlItem.Sum(a => int.Parse(a.Element("FullQty").Value));
                            if (freexmlItem.Count > 0)
                            {
                                int TotalQty = 0;
                                int KotQty = 0;
                                foreach (XElement itemElement in freexmlItem)
                                {

                                    int offerQty = Convert.ToInt32(itemElement.Element("OfferQty").Value);
                                    TotalQty = offerQty + Convert.ToInt32(itemElement.Element("FullQty").Value);
                                    int FullQty = 0;
                                    if (TotalQty > BuyQuantity)
                                    {
                                        FullQty = TotalQty - BuyQuantity;
                                        offerQty = BuyQuantity;
                                        KotQty = TotalQty;
                                    }
                                    else
                                    {

                                        offerQty = BuyQuantity;
                                        if (FreeQuantity > offerQty)
                                        {
                                            FullQty = FreeQuantity - offerQty;
                                        }
                                        else
                                        {
                                            FullQty = 0;
                                        }
                                        KotQty = offerQty;
                                    }

                                    int RemainingOfferQty = TotalQty - FullQty;
                                    decimal TotalAmount = Convert.ToDecimal(itemElement.Element("Amount").Value);
                                    decimal FullQtyAmount = (Convert.ToDecimal(itemElement.Element("Fullprice").Value) * FullQty);
                                    decimal OfferQtyAmount = Convert.ToDecimal((freeitem.Discount / 100) * FullQtyAmount);
                                    decimal TotalAmountAfterOffer = FullQtyAmount - OfferQtyAmount;
                                    decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                                    decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                                    decimal remaingvatamtchrg = ((TotalAmountAfterOffer * vatamt) / 100);
                                    itemElement.SetElementValue("FullQty", FullQty);
                                    itemElement.SetElementValue("VatAmt", vatamt);
                                    itemElement.SetElementValue("Amount", FullQtyAmount);
                                    itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                                    itemElement.SetElementValue("OfferQty", offerQty);
                                }
                                xd.Save(Path);
                                XDocument kot = XDocument.Load(KotPath);
                                var kotdata = (from p in kot.Descendants("Items")
                                               where p.Element("TableNo").Value == TableNo.ToString()
                                               && p.Element("ItemName").Value == freeitem.ItemName
                                               && p.Element("UserId").Value == oulte.ToString()
                                               select p).ToList();
                                int kotint = kotdata.Sum(a => int.Parse(a.Element("FullQty").Value));
                                if (KotQty > kotint)
                                {
                                    if (kotdata.Count > 0)
                                    {
                                        foreach (XElement itemElement in kotdata)
                                        {
                                            itemElement.SetElementValue("FullQty", KotQty);
                                        }
                                    }
                                    else
                                    {
                                        var newElement = new XElement("Items",
                                               new XElement("UserId", oulte),
                                                new XElement("TableNo", TableNo),
                                                new XElement("ItemName", freeitem.ItemName),
                                                new XElement("FullQty", Qty),
                                                new XElement("HalfQty", "0"));
                                        kot.Element("Item").Add(newElement);
                                    }
                                    kot.Save(KotPath);
                                }
                            }
                            else
                            {


                                var Item = _entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).SingleOrDefault();
                                decimal Amount = Item.FullPrice;
                                decimal DiscountAmount = Convert.ToDecimal(((freeitem.Discount / 100) * Amount));
                                decimal RemainingAmount = Amount - DiscountAmount;
                                decimal remaingvatamtchrg2 = Convert.ToDecimal((RemainingAmount * Convert.ToDecimal(Item.Vat)) / 100);

                                var newElement2 = new XElement("Items",
                                  new XElement("UserId", oulte.ToString()),
                                   new XElement("TableNo", TableNo),
                               new XElement("ItemId", freeitem.ItemId),
                               new XElement("ItemName", freeitem.ItemName),
                               new XElement("FullQty", "0"),
                               new XElement("HalfQty", "0"),
                               new XElement("Fullprice", Prices.FullPrice),
                              // new XElement("HalfPrice", Prices.HalfPrice),
                                new XElement("Amount", RemainingAmount),
                               new XElement("VatAmt", Item.Vat),
                                new XElement("VatAmountCharges", remaingvatamtchrg2),
                                 new XElement("OfferQty", Qty));
                                xd.Element("Item").Add(newElement2);
                                xd.Save(Path);
                                XDocument kot1 = XDocument.Load(KotPath);
                                var kotdata1 = (from p in kot1.Descendants("Items")
                                                where p.Element("TableNo").Value == TableNo.ToString()
                                                && p.Element("ItemName").Value == freeitem.ItemName
                                                && p.Element("UserId").Value == oulte.ToString()
                                                select p).ToList();
                                var newElement1 = new XElement("Items",
                                                 new XElement("UserId", oulte),
                                                  new XElement("TableNo", TableNo),
                                                  new XElement("ItemName", freeitem.ItemName),
                                                  new XElement("FullQty", Qty),
                                                  new XElement("HalfQty", "0"));
                                kot1.Element("Item").Add(newElement1);
                                kot1.Save(KotPath);
                            }
                        }
                    }

                }


            }



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
                    oulte = Convert.ToInt32((from n in _entities.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }
    }
}