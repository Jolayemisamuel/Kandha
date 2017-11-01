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
    public class SameItemOfferRepository
    {
        NIBSEntities entities = new NIBSEntities();
        public bool CallSameBuyOneGetOne(string Id, string Path, string TableNo, string Qty, string KotPath)
        {
             bool status = false;
            int oulte = getOutletId();
            XDocument xd = XDocument.Load(Path);
            var CurrentDay = DateTime.Now.DayOfWeek.ToString();
            int ItemId=Convert.ToInt32(Id);
            var buyitems = entities.Nibs_Offer_Buy_Items.Where(a => a.ItemId == ItemId).Select(a => a.OfferId).ToList();
            var freeItems = entities.Nibs_Offer_Free_Items.Where(a => a.ItemId == ItemId).Select(a => a.OfferId).ToList();
            var offers = (from p in entities.NIbs_AssignOffer
                          join q in entities.Nibs_Offer on p.OfferId equals q.OfferId
                          where freeItems.Contains(p.OfferId) && buyitems.Contains(p.OfferId)
                          select p.OfferId).FirstOrDefault();
            if (offers==0)
            {
                status = true;
                return status;
            }
            var OfferBuyItems = (from p in entities.Nibs_Offer_Buy_Items
                                  where p.OfferId == offers
                                  select new
                                  {
                                      ItemId = p.ItemId,
                                      Qty = p.Quantity
                                  }).FirstOrDefault();
            var OfferFreeItems = (from p in entities.Nibs_Offer_Free_Items
                                 where p.OfferId == offers
                                 select new
                                 {
                                     ItemId = p.ItemId,
                                     Qty = p.Quantity
                                 }).FirstOrDefault();

            if (OfferBuyItems.ItemId!=OfferFreeItems.ItemId)
            {
                status = true;
                return status;
            }
            else
            {
                var freeitem = (from p in entities.Nibs_Offer_Free_Items
                                where p.OfferId == offers
                                select new { ItemId = p.ItemId, ItemName = p.tblItem.Name, FreeQty = p.Quantity, Discount = p.Discount }).FirstOrDefault();
                if (freeitem!=null)
                {
                     var Prices = entities.tblBasePriceItems.Where(a => a.ItemId == freeitem.ItemId).FirstOrDefault();
                     var freeitems = (from item in xd.Descendants("Items")
                                      where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                                      && item.Element("ItemId").Value == OfferBuyItems.ItemId.ToString()
                                      && Convert.ToInt32(item.Element("FullQty").Value)>=OfferBuyItems.Qty
                                      select item).ToList();

                     if (freeitems.Count>0)
                     {
                         int buyqty = 0;
                         int offerqty=0;
                         foreach (XElement item in freeitems)
                         {
                            // freeitems.Sum(a => Convert.ToInt32(a.Element("FullQty")));
                             buyqty += Convert.ToInt32(item.Element("FullQty").Value);
                             offerqty += Convert.ToInt32(item.Element("OfferQty").Value);
                         }
                         // = freeitems.Sum(a => Convert.ToInt32(a.Element("OfferQty")));
                         int totalqty = buyqty;
                         int remaining = totalqty / OfferBuyItems.Qty;

                         int RemainingFull = totalqty - remaining;
                         decimal Discount = freeitem.Discount.Value;
                         decimal DiscountAmount=0;
                         decimal TotalAmount = Prices.FullPrice * buyqty;
                         if (Discount<100)
                         {
                             DiscountAmount= ((Discount / 100) * TotalAmount);
                         }
                         TotalAmount = TotalAmount- DiscountAmount;
                        // decimal RemainingAmount = TotalAmount - DiscountAmount;
                         decimal remaingvatamtchrg = Convert.ToDecimal((DiscountAmount * Convert.ToDecimal(Prices.Vat)) / 100);

                         foreach (XElement item in freeitems)
                         {
                             decimal vatamtchrg = Convert.ToDecimal(item.Element("VatAmountCharges").Value)+remaingvatamtchrg;
                             item.SetElementValue("VatAmt", Prices.Vat);
                                    item.SetElementValue("Amount", TotalAmount);
                                    item.SetElementValue("VatAmountCharges", vatamtchrg);
                                    item.SetElementValue("OfferQty", remaining);
                         }
                         //var newElement = new XElement("Items",
                         //   new XElement("UserId", oulte.ToString()),
                         //    new XElement("TableNo", TableNo),
                         //new XElement("ItemId", freeitem.ItemId),
                         //new XElement("ItemName", freeitem.ItemName),
                         //new XElement("FullQty", "0"),
                         //new XElement("HalfQty", "0"),
                         //new XElement("Fullprice", Prices.FullPrice),
                         //new XElement("HalfPrice", Prices.HalfPrice),
                         // new XElement("Amount", TotalAmount),
                         //new XElement("VatAmt", Prices.Vat),
                         // new XElement("VatAmountCharges", remaingvatamtchrg),
                         //  new XElement("OfferQty", remaining));
                         //xd.Element("Item").Add(newElement);
                         xd.Save(Path);
                         XDocument kot = XDocument.Load(KotPath);
                         var kotdata = (from p in kot.Descendants("Items")
                                        where p.Element("TableNo").Value == TableNo.ToString()
                                        && p.Element("ItemName").Value == freeitem.ItemName
                                        && p.Element("UserId").Value == oulte.ToString()
                                        select p).ToList();

                         foreach (XElement item in kotdata)
                         {
                             int Full = buyqty + remaining;
                             item.SetElementValue("FullQty", Full);
                         }
                         kot.Save(KotPath);
                         status = true;
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
                    oulte = Convert.ToInt32((from n in entities.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }
    }
}