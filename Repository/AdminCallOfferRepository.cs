using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Xml.Linq;
using WebMatrix.WebData;
using NibsMVC.Repository;
using System.Web.Security;
namespace NibsMVC.Repository
{
    public class AdminCallOfferRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        AdminCallHappyHoursRepository hh = new AdminCallHappyHoursRepository();
        int i = 0;
        public bool CallOffer(string Id, string Path, string TableNo, string Qty, string KotPath)
        {

            bool flag1 = false;


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
                          select p.OfferId).FirstOrDefault();
            if (JoinId == 0)
            {
                return flag1 = true;
            }
            var OfferfreeItems = (from p in _entities.Nibs_Offer_Buy_Items
                                  where p.OfferId == JoinId
                                  select new
                                  {
                                      ItemId = p.ItemId,
                                      Qty = p.Quantity
                                  }).ToList();
            bool callagainb2g1 = false;
            int checkagainb2g1 = 0;
            bool checkkotAdd = false;

            foreach (var item in OfferfreeItems)
            {

                if (item.Qty != 0)
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
                    var checkb2g1 = (from free in xd.Descendants("Items")
                                     where free.Element("UserId").Value == oulte.ToString()
                                     && free.Element("TableNo").Value == TableNo
                                     && free.Element("ItemId").Value == item.ItemId.ToString()
                                       && Convert.ToInt32(free.Element("FullQty").Value) >= item.Qty
                                     select new
                                     {
                                         ItemId = free.Element("ItemId").Value,
                                         FullQty = free.Element("FullQty").Value
                                     }).ToList();

                    if (ItemsOffer.Count() > 0)
                    {
                        if (Convert.ToInt32(ItemsOffer.FirstOrDefault().FullQty) == checkagainb2g1)
                        {
                            callagainb2g1 = true;
                        }
                        checkagainb2g1 = Convert.ToInt32(ItemsOffer.FirstOrDefault().FullQty);

                        if (checkagainb2g1 % Convert.ToInt32(OfferfreeItems.FirstOrDefault().Qty) != 0)
                        {
                            flag1 = true;
                            break;
                        }
                    }

                    if (ItemsOffer.Count() == 0)
                    {

                        flag1 = true;
                        break;
                    }
                }
                if (item.Qty == 0)
                {
                    flag1 = true;
                    break;
                }
            }

            if (flag1 == false)
            {
                var freeitem = (from p in _entities.Nibs_Offer_Free_Items
                                where p.OfferId == JoinId
                                select new { ItemId = p.ItemId, ItemName = p.tblItem.Name, FreeQty = p.Quantity, Discount = p.Discount }).FirstOrDefault();
                int kotFree = 0;
                if (freeitem != null)
                {
                    var Prices = _entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).FirstOrDefault();
                    var freeitems = (from item in xd.Descendants("Items")
                                     where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                                     && item.Element("ItemId").Value == freeitem.ItemId.ToString()
                                     select item).ToList();
                    int FullFreeQty = 0;

                    if (freeitems.Count() > 0)
                    {
                        foreach (XElement itemElement in freeitems)
                        {
                            var offerType = _entities.Nibs_Offer.Where(a => a.OfferId == JoinId).Select(a => a.OfferType).FirstOrDefault();
                            if (offerType == "b1g1")
                            {
                                int AllQty = 0;
                                int OfferQry = 0;
                                int remainingFreeQty = 0;
                                decimal remainingamt = 0;
                                decimal amt = Convert.ToDecimal(itemElement.Element("Amount").Value);
                                int CheckOfferRatio = OfferfreeItems.FirstOrDefault().Qty;
                                if (CheckOfferRatio == 1)
                                {
                                    AllQty = Convert.ToInt32(itemElement.Element("OfferQty").Value) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                                    OfferQry = Convert.ToInt32(itemElement.Element("OfferQty").Value) + (Convert.ToInt32(freeitem.FreeQty) * Convert.ToInt32(Qty));
                                    if ((AllQty - OfferQry) < 0)
                                    {
                                        remainingFreeQty = 0;
                                    }
                                    else
                                    {
                                        remainingFreeQty = AllQty - OfferQry;
                                        if (remainingFreeQty == 0)
                                        {
                                            checkkotAdd = true;
                                        }
                                    }
                                    decimal dis = Convert.ToDecimal(freeitem.Discount);
                                    decimal a = (Convert.ToDecimal(itemElement.Element("Fullprice").Value) * remainingFreeQty);
                                    decimal amount = ((dis / 100) * a);
                                    if (amount > 0)
                                    {
                                        remainingamt = amt - amount;
                                    }
                                    else
                                    {
                                        remainingamt = 0;
                                    }
                                    kotFree = AllQty;
                                }

                                else
                                {
                                    AllQty = Convert.ToInt32(itemElement.Element("OfferQty").Value) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                                    OfferQry = Convert.ToInt32(itemElement.Element("OfferQty").Value) + Convert.ToInt32(freeitem.FreeQty);
                                    if ((AllQty - OfferQry) < 0)
                                    {
                                        remainingFreeQty = 0;
                                    }
                                    else
                                    {
                                        remainingFreeQty = AllQty - OfferQry;
                                        if (remainingFreeQty == 0)
                                        {
                                            checkkotAdd = true;
                                        }
                                    }
                                    //remainingFreeQty = AllQty - OfferQry;
                                    decimal dis = Convert.ToDecimal(freeitem.Discount);
                                    decimal a = (Convert.ToDecimal(itemElement.Element("Fullprice").Value) * remainingFreeQty);
                                    decimal amount = ((dis / 100) * a);
                                    remainingamt = amt - amount;
                                    if (remainingFreeQty == 0)
                                    {
                                        checkkotAdd = true;
                                    }
                                }

                                decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                                decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                                decimal remaingvatamtchrg = ((remainingamt * vatamt) / 100);
                                itemElement.SetElementValue("FullQty", remainingFreeQty);
                                itemElement.SetElementValue("VatAmt", vatamt);
                                itemElement.SetElementValue("Amount", remainingamt);
                                itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                                itemElement.SetElementValue("OfferQty", OfferQry);
                            }
                            else
                            {

                                if (callagainb2g1)
                                {
                                    FullFreeQty = Convert.ToInt32(itemElement.Element("FullQty").Value);
                                    var remianing = 0;
                                    int remainingFreeQty = 0;
                                    if (Convert.ToInt32(itemElement.Element("OfferQty").Value) == 0)
                                    {
                                        remainingFreeQty = (Convert.ToInt32(Qty) - OfferfreeItems.FirstOrDefault().Qty);
                                        if (FullFreeQty > 1)
                                        {
                                            remianing = FullFreeQty - remainingFreeQty;
                                        }
                                        else
                                        {
                                            remianing = FullFreeQty;
                                        }

                                    }
                                    else
                                    {
                                        if (callagainb2g1)
                                        {
                                            remianing = Convert.ToInt32(itemElement.Element("OfferQty").Value) + Convert.ToInt32(Qty);
                                        }
                                        else
                                        {
                                            remianing = Convert.ToInt32(itemElement.Element("OfferQty").Value);
                                        }

                                    }



                                    decimal amt = Convert.ToDecimal(itemElement.Element("Amount").Value);
                                    decimal remainingamt = 0;
                                    if (amt != 0)
                                    {
                                        decimal dis = Convert.ToDecimal(freeitem.Discount);
                                        decimal a = (Convert.ToDecimal(itemElement.Element("Fullprice").Value) * OfferfreeItems.FirstOrDefault().Qty);
                                        decimal amount = ((dis / 100) * a);
                                        remainingamt = amt - amount;
                                    }

                                    decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                                    decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                                    decimal remaingvatamtchrg = ((remainingamt * vatamt) / 100);
                                    itemElement.SetElementValue("FullQty", remainingFreeQty);
                                    itemElement.SetElementValue("VatAmt", vatamt);
                                    itemElement.SetElementValue("Amount", remainingamt);
                                    itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                                    itemElement.SetElementValue("OfferQty", remianing);
                                }
                            }

                        }

                    }
                    else
                    {
                        int Free = 0;
                        int freeqty = 0;
                        var offerType = _entities.Nibs_Offer.Where(a => a.OfferId == JoinId).Select(a => a.OfferType).FirstOrDefault();
                        if (offerType == "b1g1")
                        {
                            freeqty = freeitem.FreeQty.Value * Convert.ToInt32(Qty);
                        }
                        else
                        {
                            freeqty = (Convert.ToInt32(Qty) / OfferfreeItems.FirstOrDefault().Qty);

                            if (freeqty == 1)
                            {
                                Free = 0;
                            }
                            else
                            {
                                Free = freeqty;
                            }
                        }
                        kotFree = freeqty + Free;
                        if (freeitem.Discount == 100)
                        {
                            var newElement = new XElement("Items",
                            new XElement("UserId", oulte.ToString()),
                             new XElement("TableNo", TableNo),
                         new XElement("ItemId", freeitem.ItemId),
                         new XElement("ItemName", freeitem.ItemName),
                         new XElement("FullQty", Free),
                         new XElement("HalfQty", "0"),
                         new XElement("Fullprice", Prices.FullPrice),
                         new XElement("HalfPrice", Prices.HalfPrice),
                          new XElement("Amount", "0"),
                         new XElement("VatAmt", "0"),
                          new XElement("VatAmountCharges", "0"),
                           new XElement("OfferQty", freeqty));
                            xd.Element("Item").Add(newElement);
                        }
                        else
                        {
                            var Item = _entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).SingleOrDefault();
                            decimal Amount = Item.FullPrice;
                            decimal RemainingAmount = Convert.ToDecimal(((freeitem.Discount / 100) * Amount));
                            decimal remaingvatamtchrg = Convert.ToDecimal((RemainingAmount * Convert.ToDecimal(Item.Vat)) / 100);
                            var newElement = new XElement("Items",
                            new XElement("UserId", oulte.ToString()),
                             new XElement("TableNo", TableNo),
                         new XElement("ItemId", freeitem.ItemId),
                         new XElement("ItemName", freeitem.ItemName),
                         new XElement("FullQty", Free),
                         new XElement("HalfQty", "0"),
                         new XElement("Fullprice", Prices.FullPrice),
                         new XElement("HalfPrice", Prices.HalfPrice),
                          new XElement("Amount", RemainingAmount),
                         new XElement("VatAmt", Item.Vat),
                          new XElement("VatAmountCharges", remaingvatamtchrg),
                           new XElement("OfferQty", freeqty));
                            xd.Element("Item").Add(newElement);
                        }

                    }
                }

                xd.Save(Path);
                XDocument kot = XDocument.Load(KotPath);
                if (freeitem != null)
                {
                    var offerType = _entities.Nibs_Offer.Where(a => a.OfferId == JoinId).Select(a => a.OfferType).FirstOrDefault();
                    if (offerType == "b1g1")
                    {
                        var kotdata = (from p in kot.Descendants("Items")
                                       where p.Element("TableNo").Value == TableNo.ToString()
                                       && p.Element("ItemName").Value == freeitem.ItemName
                                       && p.Element("UserId").Value == oulte.ToString()
                                       select p).ToList();
                        if (kotdata.Count() > 0)
                        {
                            foreach (XElement itemElement in kotdata)
                            {
                                //int Full = Convert.ToInt32(itemElement.Element("FullQty").Value)+kotFree;
                                int Full = 0;
                                int CheckOfferRatio = OfferfreeItems.FirstOrDefault().Qty;
                                if (!checkkotAdd)
                                {
                                    if (CheckOfferRatio == 1)
                                    {
                                        if (callagainb2g1)
                                        {
                                            int diff = Convert.ToInt32(Qty) - Convert.ToInt32(itemElement.Element("FullQty").Value);
                                            Full = Convert.ToInt32(itemElement.Element("FullQty").Value) + diff;
                                        }
                                        else
                                        {
                                            Full = Convert.ToInt32(itemElement.Element("FullQty").Value) + Convert.ToInt32(Qty);
                                        }

                                    }
                                    else
                                    {
                                        Full = Convert.ToInt32(itemElement.Element("FullQty").Value);
                                    }
                                }
                                else
                                {

                                    if (CheckOfferRatio == 1)
                                    {
                                        Full = Convert.ToInt32(itemElement.Element("FullQty").Value);
                                    }
                                    else
                                    {
                                        Full = Convert.ToInt32(itemElement.Element("FullQty").Value);
                                    }

                                }
                                itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                                itemElement.SetElementValue("TableNo", itemElement.Element("TableNo").Value);
                                itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                                itemElement.SetElementValue("FullQty", Full);
                                itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
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
                    else
                    {
                        if (callagainb2g1)
                        {
                            var kotdata = (from p in kot.Descendants("Items")
                                           where p.Element("TableNo").Value == TableNo.ToString()
                                           && p.Element("ItemName").Value == freeitem.ItemName
                                           && p.Element("UserId").Value == oulte.ToString()
                                           select p).ToList();
                            if (kotdata.Count() > 0)
                            {
                                foreach (XElement itemElement in kotdata)
                                {
                                    //int Full = Convert.ToInt32(itemElement.Element("FullQty").Value)+kotFree;
                                    int Full = 0;
                                    int CheckOfferRatio = OfferfreeItems.FirstOrDefault().Qty;
                                    if (!checkkotAdd)
                                    {
                                        if (CheckOfferRatio == 1)
                                        {
                                            Full = Convert.ToInt32(Qty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                                           // Full = Convert.ToInt32(itemElement.Element("FullQty").Value) + diff;

                                        }
                                        else
                                        {
                                            Full = Convert.ToInt32(itemElement.Element("FullQty").Value);
                                        }
                                    }
                                    else
                                    {

                                        if (CheckOfferRatio == 1)
                                        {
                                            Full = Convert.ToInt32(itemElement.Element("FullQty").Value);
                                        }
                                        else
                                        {
                                            Full = Convert.ToInt32(itemElement.Element("FullQty").Value);
                                        }

                                    }
                                    itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                                    itemElement.SetElementValue("TableNo", itemElement.Element("TableNo").Value);
                                    itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                                    itemElement.SetElementValue("FullQty", Full);
                                    itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
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

                }
                else
                {
                    flag1 = true;
                }

            }
            return flag1;
        }
        public bool CallComboOffer(string Id, string TableNo, string Path, string KotPath)
        {
            bool flag = false;
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
                          select p.OfferId).FirstOrDefault();
            if (JoinId == 0)
            {
                return flag = true;
            }
            var OfferfreeItems = (from p in _entities.Nibs_Offer_Buy_Items
                                  where p.OfferId == JoinId
                                  select new
                                  {
                                      ItemId = p.ItemId,
                                      Qty = p.Quantity
                                  }).ToList();
            int checkagainb2g1 = 0;
            bool callagainb2g1 = false;
            foreach (var item in OfferfreeItems)
            {

                var ItemsOffer = (from free in xd.Descendants("Items")
                                  where free.Element("UserId").Value == oulte.ToString()
                                  && free.Element("TableNo").Value == TableNo
                                  && free.Element("ItemId").Value == item.ItemId.ToString()
                                  select new
                                  {
                                      Vat = free.Element("VatAmt").Value,
                                      Amount = free.Element("Amount").Value,
                                      FullQty = free.Element("FullQty").Value,
                                      VatAmountCharges = free.Element("VatAmountCharges").Value
                                  }).ToList();
                if (ItemsOffer.Count() > 0)
                {
                    if (Convert.ToInt32(ItemsOffer.FirstOrDefault().FullQty) == checkagainb2g1)
                    {
                        callagainb2g1 = true;
                    }
                    else
                    {
                        callagainb2g1 = false;
                    }
                    checkagainb2g1 = Convert.ToInt32(ItemsOffer.FirstOrDefault().FullQty);
                }

                if (ItemsOffer.Count() == 0)
                {

                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                if (callagainb2g1)
                {
                    var BaseAmount = _entities.NIbs_ComboOffer.Where(a => a.OfferId == JoinId).Select(a => a.BaseAmount).SingleOrDefault();
                    if (BaseAmount > 0)
                    {
                        var offeramount = ((Convert.ToDecimal(BaseAmount)) / OfferfreeItems.Count()) * checkagainb2g1;
                        foreach (var item in OfferfreeItems)
                        {
                            var ItemsOffer = (from free in xd.Descendants("Items")
                                              where free.Element("UserId").Value == oulte.ToString()
                                              && free.Element("TableNo").Value == TableNo
                                              && free.Element("ItemId").Value == item.ItemId.ToString()
                                              select free).ToList();
                            foreach (XElement itemElement in ItemsOffer)
                            {
                                var vatamtchrg = (offeramount * Convert.ToDecimal(Convert.ToDecimal(itemElement.Element("VatAmt").Value)) / 100);
                                itemElement.SetElementValue("Amount", offeramount);
                                itemElement.SetElementValue("VatAmountCharges", vatamtchrg);
                                xd.Save(Path);
                            }
                        }
                    }
                }


            }


            return flag;
        }
        public bool CallAmountBasisOffer(string Id, string TableNo, string Path, string KotPath, string Qty)
        {
            var flag = false;
            int oulte = getOutletId();
            XDocument xd = XDocument.Load(Path);
            var CurrentDay = DateTime.Now.DayOfWeek.ToString();
            var AmtOfferId = (from p in _entities.Nibs_Offer_Days
                              join q in _entities.Nibs_Offer_Amount
                              on p.OfferId equals q.OfferId
                              join f in _entities.NIbs_AssignOffer
                          on p.OfferId equals (f.OfferId)
                              where p.Days.Equals(CurrentDay)
                              && f.UserId == oulte
                              && p.Days.Equals(CurrentDay)
                              select p.OfferId).FirstOrDefault();
            if (AmtOfferId == 0)
            {
                return flag = true;
            }
            decimal amout = 0;
            var ItemsOffer = (from free in xd.Descendants("Items")
                              where free.Element("UserId").Value == oulte.ToString()
                              && free.Element("TableNo").Value == TableNo
                              select new
                              {
                                  Amount = free.Element("Amount").Value
                              }).ToList();
            foreach (var item in ItemsOffer)
            {
                amout += Convert.ToDecimal(item.Amount);

            }
            var data = _entities.Nibs_Offer_Amount.Where(a => a.OfferId == AmtOfferId).SingleOrDefault();
            decimal FixAmount = data.Amount;
            bool kotflag = false;
            if (amout > FixAmount)
            {
                var freeItemId = (from p in _entities.Nibs_Offer_Amount
                                  where p.OfferId == AmtOfferId
                                  select new
                                  {
                                      ItemID = p.tblItem.ItemId,
                                      ItemName = p.tblItem.Name,
                                      ItemQty = p.Quantity,
                                      Discount = p.Discount
                                  }).FirstOrDefault();
                if (freeItemId != null)
                {
                    var Price = (from p in _entities.tblBasePriceItems
                                 where p.ItemId == freeItemId.ItemID
                                 select new
                                 {
                                     HalfPrice = p.HalfPrice,
                                     FullPrice = p.FullPrice
                                 }).FirstOrDefault();
                    var freeitem = (from item in xd.Descendants("Items")
                                    where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                                    && item.Element("ItemId").Value == freeItemId.ItemID.ToString()
                                    select item).ToList();
                    int FullFreeQty = 0;

                    //if (freeitem.Count() > 0)
                    //{
                    //    foreach (XElement itemElement in freeitem)
                    //    {
                    //        int offerQty = Convert.ToInt32(itemElement.Element("OfferQty").Value);
                    //        if (offerQty == 0)
                    //        {
                    //            FullFreeQty = Convert.ToInt32(itemElement.Element("FullQty").Value);
                    //            var remianing = 0;
                    //            int remainingFreeQty = Convert.ToInt32(freeItemId.ItemQty);
                    //            if (Convert.ToInt32(Qty) == Convert.ToInt32(freeItemId.ItemQty))
                    //            {
                    //                remianing = Convert.ToInt32(freeItemId.ItemQty);
                    //            }
                    //            else if (FullFreeQty > 1)
                    //            {
                    //                remianing = FullFreeQty - remainingFreeQty;
                    //            }
                    //            else
                    //            {
                    //                remianing = FullFreeQty;
                    //            }
                    //            decimal amt = Convert.ToDecimal(itemElement.Element("Amount").Value);
                    //            decimal remainingamt = 0;
                    //            if (amt != 0)
                    //            {

                    //                decimal dis = Convert.ToDecimal(freeItemId.Discount);
                    //                decimal a = Convert.ToDecimal(Convert.ToDecimal(itemElement.Element("Fullprice").Value) * freeItemId.ItemQty);
                    //                decimal amount = ((dis / 100) * a);
                    //                remainingamt = amt - amount;
                    //            }

                    //            decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                    //            decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                    //            decimal remaingvatamtchrg = ((remainingamt * vatamt) / 100);
                    //            itemElement.SetElementValue("FullQty", remianing);
                    //            itemElement.SetElementValue("VatAmt", vatamt);
                    //            itemElement.SetElementValue("Amount", remainingamt);
                    //            itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                    //            itemElement.SetElementValue("OfferQty", remainingFreeQty);
                    //        }
                    //        else
                    //        {
                    //            kotflag = true;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    decimal Amount = 0;
                    //    decimal RemainingAmount = 0;
                    //    decimal remaingvatamtchrg = 0;
                    //    var Item = _entities.tblBasePriceItems.Where(a => a.ItemId == freeItemId.ItemID).FirstOrDefault();
                    //    if (freeItemId.Discount != 100)
                    //    {

                    //        Amount = Item.FullPrice;
                    //        RemainingAmount = Convert.ToDecimal(((freeItemId.Discount / 100) * Amount));
                    //        remaingvatamtchrg = Convert.ToDecimal((RemainingAmount * Convert.ToDecimal(Item.Vat)) / 100);
                    //    }
                    //    var newElement = new XElement("Items",
                    //        new XElement("UserId", oulte.ToString()),
                    //         new XElement("TableNo", TableNo),
                    //     new XElement("ItemId", freeItemId.ItemID),
                    //     new XElement("ItemName", freeItemId.ItemName),
                    //     new XElement("FullQty", "0"),
                    //     new XElement("HalfQty", "0"),
                    //     new XElement("Fullprice", Price.FullPrice),
                    //     new XElement("HalfPrice", Price.HalfPrice),
                    //      new XElement("Amount", RemainingAmount),
                    //     new XElement("VatAmt", Item.Vat),
                    //      new XElement("VatAmountCharges", remaingvatamtchrg),
                    //       new XElement("OfferQty", freeItemId.ItemQty));
                    //    xd.Element("Item").Add(newElement);
                    //}
                    decimal Amount = 0;
                    decimal RemainingAmount = 0;
                    decimal remaingvatamtchrg = 0;
                    var Item = _entities.tblBasePriceItems.Where(a => a.ItemId == freeItemId.ItemID).FirstOrDefault();
                    if (freeItemId.Discount != 100)
                    {
                        Amount = Item.FullPrice;
                        RemainingAmount = Convert.ToDecimal(((freeItemId.Discount / 100) * Amount));
                        remaingvatamtchrg = Convert.ToDecimal((RemainingAmount * Convert.ToDecimal(Item.Vat)) / 100);
                    }
                    var newElement = new XElement("Items",
                        new XElement("UserId", oulte.ToString()),
                         new XElement("TableNo", TableNo),
                     new XElement("ItemId", freeItemId.ItemID),
                     new XElement("ItemName", freeItemId.ItemName),
                     new XElement("FullQty", "0"),
                     new XElement("HalfQty", "0"),
                     new XElement("Fullprice", Price.FullPrice),
                     new XElement("HalfPrice", Price.HalfPrice),
                      new XElement("Amount", RemainingAmount),
                     new XElement("VatAmt", Item.Vat),
                      new XElement("VatAmountCharges", remaingvatamtchrg),
                       new XElement("OfferQty", freeItemId.ItemQty));
                    xd.Element("Item").Add(newElement);
                    xd.Save(Path);
                    XDocument kot = XDocument.Load(KotPath);
                    if (!kotflag)
                    {
                        var kotdata = (from p in kot.Descendants("Items")
                                       where p.Element("TableNo").Value == TableNo.ToString()
                                       && p.Element("ItemName").Value == freeItemId.ItemName
                                       && p.Element("UserId").Value == oulte.ToString()
                                       select p).ToList();
                        if (kotdata.Count() > 0)
                        {
                            foreach (XElement itemElement in kotdata)
                            {
                                int Full = Convert.ToInt32(freeItemId.ItemQty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                                itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                                itemElement.SetElementValue("TableNo", itemElement.Element("TableNo").Value);
                                itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                                itemElement.SetElementValue("FullQty", Full);
                                itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                            }
                        }
                        else
                        {
                            var newElement1 = new XElement("Items",
                                new XElement("UserId", oulte),
                                 new XElement("TableNo", TableNo),
                                 new XElement("ItemName", freeItemId.ItemName),
                                 new XElement("FullQty", freeItemId.ItemQty),
                                 new XElement("HalfQty", "0"));
                            kot.Element("Item").Add(newElement1);
                        }
                        kot.Save(KotPath);
                    }

                }
            }
            return flag;
        }
        public string RemoveOffer(string Id, string Path, string TableNo)
        {
            int oulte = getOutletId();
            int ItemId = Convert.ToInt32(Id);

            XDocument xd = XDocument.Load(Path);
            var CurrentDay = DateTime.Now.DayOfWeek.ToString();
            var offerId = _entities.Nibs_Offer_Buy_Items.Where(z => z.ItemId == ItemId).Select(a => a.OfferId).SingleOrDefault();
            var deleteFreeItemId = _entities.Nibs_Offer_Free_Items.Where(a => a.OfferId == offerId).Select(s => s.ItemId).SingleOrDefault();

            if (ItemId > 0)
            {
                var ItemsOffer = (from free in xd.Descendants("Items")
                                  where free.Element("UserId").Value == oulte.ToString()
                                  && free.Element("TableNo").Value == TableNo
                                  && free.Element("ItemId").Value == deleteFreeItemId.ToString()
                                  select free).SingleOrDefault();


                if (ItemsOffer != null)
                {
                    int full = Convert.ToInt32(ItemsOffer.Element("FullQty").Value);
                    int freeItem = Convert.ToInt32(ItemsOffer.Element("OfferQty").Value);
                    int ReturnItem = full + freeItem;
                    if (full > 0)
                    {
                        ItemsOffer.Element("FullQty").Value = (ReturnItem).ToString();
                        ItemsOffer.Element("OfferQty").Value = "0";
                    }
                    else
                    {
                        ItemsOffer.Remove();

                    }
                    xd.Save(Path);
                }

            }
            return "";
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