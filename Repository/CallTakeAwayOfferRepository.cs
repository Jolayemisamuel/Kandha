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
    public class CallTakeAwayOfferRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        AdminCallHappyHoursRepository hh = new AdminCallHappyHoursRepository();
        int i = 0;
        public bool CallOffer(string Id, string Path, string TableNo, string Qty,string KotPath,string ItemName)
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
            foreach (var item in OfferfreeItems)
            {
                if (item.Qty != 0)
                {
                    var ItemsOffer = (from free in xd.Descendants("Items")
                                      where free.Element("UserId").Value == oulte.ToString()
                                      && free.Element("ItemId").Value == item.ItemId.ToString()
                                        && Convert.ToInt32(free.Element("FullQty").Value) >= item.Qty
                                      select new
                                      {
                                          ItemId = free.Element("ItemId").Value,
                                          FullQty = free.Element("FullQty").Value
                                      }).ToList();
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
            int freeItemKotId = 0;
            string FreeKotItemName = string.Empty;
            if (flag1 == false)
            {
                var freeitem = (from p in _entities.Nibs_Offer_Free_Items
                                where p.OfferId == JoinId
                                select new { ItemId = p.ItemId, ItemName = p.tblItem.Name, FreeQty = p.Quantity, Discount = p.Discount }).FirstOrDefault();
                freeItemKotId = freeitem.ItemId;
                FreeKotItemName = freeitem.ItemName;
                if (freeitem != null)
                {
                    var Prices = _entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).FirstOrDefault();
                    var freeitems = (from item in xd.Descendants("Items")
                                     where item.Element("UserId").Value == oulte.ToString() 
                                     && item.Element("ItemId").Value == freeitem.ItemId.ToString()
                                     select item).ToList();
                    int FullFreeQty = 0;

                    if (freeitems.Count() > 0)
                    {
                        foreach (XElement itemElement in freeitems)
                        {
                            FullFreeQty = Convert.ToInt32(itemElement.Element("FullQty").Value);
                            var remianing = 0;
                            int remainingFreeQty = (Convert.ToInt32(Qty) - OfferfreeItems.FirstOrDefault().Qty);
                            if (FullFreeQty > 1)
                            {
                                remianing = FullFreeQty - remainingFreeQty;
                            }
                            else
                            {
                                remianing = FullFreeQty;
                            }
                            decimal amt = Convert.ToDecimal(itemElement.Element("TotalAmount").Value);
                            decimal remainingamt = 0;
                            if (amt != 0)
                            {
                                decimal dis = Convert.ToDecimal(freeitem.Discount);
                                decimal a = (Convert.ToDecimal(itemElement.Element("FullPrice").Value) * OfferfreeItems.FirstOrDefault().Qty);
                                decimal amount = ((dis / 100) * a);
                                remainingamt = amt - amount;
                            }

                            decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                            decimal vatamt = Convert.ToDecimal(itemElement.Element("Vat").Value);
                            decimal remaingvatamtchrg = ((remainingamt * vatamt) / 100);
                            itemElement.SetElementValue("FullQty", remainingFreeQty);
                            itemElement.SetElementValue("Vat", vatamt);
                            itemElement.SetElementValue("TotalAmount", remainingamt);
                            itemElement.SetElementValue("VatAmt", remaingvatamtchrg);
                            itemElement.SetElementValue("OfferQty", remianing);
                        }

                    }
                    else
                    {
                        int freeqty = (Convert.ToInt32(Qty) / OfferfreeItems.FirstOrDefault().Qty);
                        int Free;
                        if (freeqty == 1)
                        {
                            Free = 0;
                        }
                        else
                        {
                            Free = freeqty;
                        }
                        if (freeitem.Discount == 100)
                        {
                            var newElement = new XElement("Items",
                            new XElement("UserId", oulte.ToString()),
                         new XElement("ItemId", freeitem.ItemId),
                         new XElement("ItemName", freeitem.ItemName),
                         new XElement("FullQty", Free),
                         new XElement("HalfQty", "0"),
                         new XElement("FullPrice", Prices.FullPrice),
                         new XElement("HalfPrice", Prices.HalfPrice),
                          new XElement("TotalAmount", "0"),
                         new XElement("Vat", "0"),
                          new XElement("VatAmt", "0"),
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
                         new XElement("ItemId", freeitem.ItemId),
                         new XElement("ItemName", freeitem.ItemName),
                         new XElement("FullQty", Free),
                         new XElement("HalfQty", "0"),
                         new XElement("FullPrice", Prices.FullPrice),
                         new XElement("HalfPrice", Prices.HalfPrice),
                          new XElement("TotalAmount", RemainingAmount),
                         new XElement("Vat", Item.Vat),
                          new XElement("VatAmt", remaingvatamtchrg),
                           new XElement("OfferQty", freeqty));
                            xd.Element("Item").Add(newElement);
                        }

                    }
                }

                xd.Save(Path);
                XDocument kot = XDocument.Load(KotPath);
                var kotdata = (from p in kot.Descendants("Items")
                            where p.Element("TokenNo").Value == TableNo.ToString() 
                            && p.Element("ItemName").Value == FreeKotItemName
                            && p.Element("UserId").Value == oulte.ToString()
                            select p).ToList();
                if (kotdata.Count() > 0)
                {
                    foreach (XElement itemElement in kotdata)
                    {
                        int Full = Convert.ToInt32(Qty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
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
                         new XElement("TokenNo", TableNo),
                         new XElement("ItemName", FreeKotItemName),
                         new XElement("FullQty", Qty),
                         new XElement("HalfQty", "0"));
                    kot.Element("Item").Add(newElement);
                }
                kot.Save(KotPath);
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
            foreach (var item in OfferfreeItems)
            {

                var ItemsOffer = (from free in xd.Descendants("Items")
                                  where free.Element("UserId").Value == oulte.ToString()
                                 
                                  && free.Element("ItemId").Value == item.ItemId.ToString()
                                  select new
                                  {
                                      Vat = free.Element("Vat").Value,
                                      Amount = free.Element("TotalAmount").Value,
                                      VatAmountCharges = free.Element("VatAmt").Value
                                  }).ToList();

                if (ItemsOffer.Count() == 0)
                {

                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                var BaseAmount = _entities.NIbs_ComboOffer.Where(a => a.OfferId == JoinId).Select(a => a.BaseAmount).SingleOrDefault();
                var offeramount = (Convert.ToDecimal(BaseAmount)) / OfferfreeItems.Count();
                foreach (var item in OfferfreeItems)
                {
                    var ItemsOffer = (from free in xd.Descendants("Items")
                                      where free.Element("UserId").Value == oulte.ToString()
                                      
                                      && free.Element("ItemId").Value == item.ItemId.ToString()
                                      select free).ToList();
                    foreach (XElement itemElement in ItemsOffer)
                    {
                        var vatamtchrg = (offeramount * Convert.ToDecimal(Convert.ToDecimal(itemElement.Element("Vat").Value)) / 100);
                        itemElement.SetElementValue("TotalAmount", offeramount);
                        itemElement.SetElementValue("VatAmouVatAmtntCharges", vatamtchrg);
                        xd.Save(Path);
                    }
                }
                //XDocument kot = XDocument.Load(KotPath);
                //var kotdata = (from p in kot.Descendants("Items")
                //               where p.Element("TokenNo").Value == TableNo.ToString()
                //               && p.Element("ItemName").Value == FreeKotItemName
                //               && p.Element("UserId").Value == oulte.ToString()
                //               select p).ToList();
                //if (kotdata.Count() > 0)
                //{
                //    foreach (XElement itemElement in kotdata)
                //    {
                //        int Full = Convert.ToInt32(Qty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                //        itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                //        itemElement.SetElementValue("TokenNo", itemElement.Element("TokenNo").Value);
                //        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                //        itemElement.SetElementValue("FullQty", Full);
                //        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                //    }
                //}
                //else
                //{
                //    var newElement = new XElement("Items",
                //        new XElement("UserId", oulte),
                //         new XElement("TokenNo", TableNo),
                //         new XElement("ItemName", FreeKotItemName),
                //         new XElement("FullQty", Qty),
                //         new XElement("HalfQty", "0"));
                //    kot.Element("Item").Add(newElement);
                //}
                //kot.Save(KotPath);
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
                              
                              select new
                              {
                                  Amount = free.Element("TotalAmount").Value
                              }).ToList();
            foreach (var item in ItemsOffer)
            {
                amout += Convert.ToDecimal(item.Amount);

            }
            var data = _entities.Nibs_Offer_Amount.Where(a => a.OfferId == AmtOfferId).SingleOrDefault();
            decimal FixAmount = data.Amount;
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
                                 }).SingleOrDefault();
                    var freeitem = (from item in xd.Descendants("Items")
                                    where item.Element("UserId").Value == oulte.ToString() 
                                    && item.Element("ItemId").Value == freeItemId.ItemID.ToString()
                                    select item).ToList();
                    int FullFreeQty = 0;

                    if (freeitem.Count() > 0)
                    {
                        foreach (XElement itemElement in freeitem)
                        {
                            FullFreeQty = Convert.ToInt32(itemElement.Element("FullQty").Value);
                            var remianing = 0;
                            int remainingFreeQty = Convert.ToInt32(freeItemId.ItemQty);
                            if (Convert.ToInt32(Qty) == Convert.ToInt32(freeItemId.ItemQty))
                            {
                                remianing = Convert.ToInt32(freeItemId.ItemQty);
                            }
                            else if (FullFreeQty > 1)
                            {
                                remianing = FullFreeQty - remainingFreeQty;
                            }
                            else
                            {
                                remianing = FullFreeQty;
                            }
                            decimal amt = Convert.ToDecimal(itemElement.Element("TotalAmount").Value);
                            decimal remainingamt = 0;
                            if (amt != 0)
                            {

                                decimal dis = Convert.ToDecimal(freeItemId.Discount);
                                decimal a = Convert.ToDecimal(Convert.ToDecimal(itemElement.Element("FullPrice").Value) * freeItemId.ItemQty);
                                decimal amount = ((dis / 100) * a);
                                remainingamt = amt - amount;
                            }

                            decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                            decimal vatamt = Convert.ToDecimal(itemElement.Element("Vat").Value);
                            decimal remaingvatamtchrg = ((remainingamt * vatamt) / 100);
                            itemElement.SetElementValue("FullQty", remianing);
                            itemElement.SetElementValue("Vat", vatamt);
                            itemElement.SetElementValue("TotalAmount", remainingamt);
                            itemElement.SetElementValue("VatAmt", remaingvatamtchrg);
                            itemElement.SetElementValue("OfferQty", remainingFreeQty);
                        }
                    }
                    else
                    {
                        decimal Amount = 0;
                        decimal RemainingAmount = 0;
                        decimal remaingvatamtchrg = 0;
                        var Item = _entities.tblBasePriceItems.Where(a => a.ItemId == freeItemId.ItemID).SingleOrDefault();
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
                         new XElement("FullPrice", Price.FullPrice),
                         new XElement("HalfPrice", Price.HalfPrice),
                          new XElement("TotalAmount", RemainingAmount),
                         new XElement("Vat", Item.Vat),
                          new XElement("VatAmt", remaingvatamtchrg),
                           new XElement("OfferQty", freeItemId.ItemQty));
                        xd.Element("Item").Add(newElement);
                    }
                    xd.Save(Path);
                    XDocument kot = XDocument.Load(KotPath);
                    var kotdata = (from p in kot.Descendants("Items")
                                   where p.Element("TokenNo").Value == TableNo.ToString()
                                   && p.Element("ItemName").Value == freeItemId.ItemName
                                   && p.Element("UserId").Value == oulte.ToString()
                                   select p).ToList();
                    if (kotdata.Count() > 0)
                    {
                        foreach (XElement itemElement in kotdata)
                        {
                            int Full = Convert.ToInt32(freeItemId.ItemQty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
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
                             new XElement("TokenNo", TableNo),
                             new XElement("ItemName", freeItemId.ItemName),
                             new XElement("FullQty", freeItemId.ItemQty),
                             new XElement("HalfQty", "0"));
                        kot.Element("Item").Add(newElement);
                    }
                    kot.Save(KotPath);
                }
            }
            return flag;
        }
        //public string RemoveOffer(string Id, string Path, string TableNo)
        //{
        //    int oulte = getOutletId();
        //    int ItemId = Convert.ToInt32(Id);
        //    XDocument xd = XDocument.Load(Path);
        //    var CurrentDay = DateTime.Now.DayOfWeek.ToString();
        //    var offerId = _entities.Nibs_Offer_Buy_Items.Where(z => z.ItemId == ItemId).Select(a => a.OfferId).SingleOrDefault();
        //    var deleteFreeItemId = _entities.Nibs_Offer_Free_Items.Where(a => a.OfferId == offerId).Select(s => s.ItemId).SingleOrDefault();
        //    var ItemsOffer = (from free in xd.Descendants("Items")
        //                      where free.Element("UserId").Value == oulte.ToString()
        //                      && free.Element("TableNo").Value == TableNo
        //                      && free.Element("ItemId").Value == deleteFreeItemId.ToString()
        //                      select free).SingleOrDefault();

        //    if (ItemsOffer != null)
        //    {
        //        int full = Convert.ToInt32(ItemsOffer.Element("FullQty").Value);
        //        int freeItem = Convert.ToInt32(ItemsOffer.Element("OfferQty").Value);
        //        int ReturnItem = full + freeItem;
        //        if (full > 0)
        //        {
        //            ItemsOffer.Element("FullQty").Value = (ReturnItem).ToString();
        //            ItemsOffer.Element("OfferQty").Value = "0";
        //        }
        //        else
        //        {
        //            ItemsOffer.Remove();

        //        }
        //        xd.Save(Path);
        //    }

        //    return "";
        //}

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