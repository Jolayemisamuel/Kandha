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
    public class TakeAwayhappHourOfferRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public bool CallHappyHoursDaysOffer(string Id, string TableNo, string Path, string KotPath,string ItemName)
        {
            var flag = false;
            int oulte = getOutletId();
            XDocument xd = XDocument.Load(Path);
            int ItemId = Convert.ToInt32(Id);
            var CurrentDay = DateTime.Now.DayOfWeek;
            var CurrentTime = DateTime.Now.TimeOfDay;
            // check Days Offer Query
            var CheckDaysOffer = (from p in _entities.Nibs_HappyHours_Days
                                  join f in _entities.NIbs_AssignOffer on
                                  p.OfferId equals (f.OfferId)
                                  where p.FreeItemId == ItemId
                                  && p.TimeStart <= CurrentTime
                                  && p.TimeEnd >= CurrentTime
                                  && f.UserId == oulte
                                  select new
                                  {
                                      ItemId = p.FreeItemId,
                                      ItemIndex = p.ItemIndex,
                                      Discount = p.Discount,
                                      StartDay = p.StartDay,
                                      EndDay = p.EndDay
                                  }).FirstOrDefault();
            // check dates offer Query
            var CurrentDate = DateTime.Now.Date;
            var checkDatesOffer = (from p in _entities.Nibs_HappyHoursDates
                                   join f in _entities.NIbs_AssignOffer on
                                  p.OfferId equals (f.OfferId)
                                   where p.StartDate <= CurrentDate
                                   && p.EndDate >= CurrentDate
                                   && p.TimeStart <= CurrentTime
                                   && p.TimeEnd >= CurrentTime
                                   && p.FreeItemId == ItemId
                                   && f.UserId == WebSecurity.CurrentUserId
                                   select new
                                   {
                                       ItemId = p.FreeItemId,
                                       ItemIndex = p.ItemIndex,
                                       Discount = p.Discount,
                                       StartDate = p.StartDate,
                                       EndDate = p.EndDate
                                   }).FirstOrDefault();
            // check One Day Offer Query
            var CheckDayOffer = (from p in _entities.Nibs_HappyHours_Day
                                 join f in _entities.NIbs_AssignOffer on
                                  p.OfferId equals (f.OfferId)
                                 where p.FreeItemId == ItemId
                                 && p.TimeStart <= CurrentTime
                                 && p.TimeEnd >= CurrentTime
                                 && f.UserId == WebSecurity.CurrentUserId
                                 select new
                                 {
                                     ItemId = p.FreeItemId,
                                     ItemIndex = p.ItemIndex,
                                     Discount = p.Discount,
                                     Day = p.Day
                                 }).FirstOrDefault();
            // check One Date Offer Query
            var CheckDateOffer = (from p in _entities.Nibs_HappyHours_Date
                                  join f in _entities.NIbs_AssignOffer on
                                  p.OfferId equals (f.OfferId)
                                  where p.FreeItemId == ItemId
                                  && p.TimeStart <= CurrentTime
                                  && p.TimeEnd >= CurrentTime
                                  && f.UserId == WebSecurity.CurrentUserId
                                  select new
                                  {
                                      ItemId = p.FreeItemId,
                                      ItemIndex = p.ItemIndex,
                                      Discount = p.Discount,
                                      Date = p.Date
                                  }).FirstOrDefault();
            if (CheckDaysOffer != null)
            {
                if (CurrentDay >= (DayOfWeek)Enum.Parse(typeof(DayOfWeek), CheckDaysOffer.StartDay)
                && CurrentDay <= (DayOfWeek)Enum.Parse(typeof(DayOfWeek), CheckDaysOffer.EndDay))
                {
                    if (CheckDaysOffer != null)
                    {
                        string itemIndex = CheckDaysOffer.ItemIndex.ToString();
                        string dis = CheckDaysOffer.Discount.ToString();
                        flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path, KotPath,ItemName);
                    }

                }
            }
            else if (checkDatesOffer != null)
            {

                if (CurrentDate >= checkDatesOffer.StartDate && CurrentDate <= checkDatesOffer.EndDate)
                {
                    string itemIndex = checkDatesOffer.ItemIndex.ToString();
                    string dis = checkDatesOffer.Discount.ToString();
                    flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path, KotPath, ItemName);

                }
            }
            else if (CheckDayOffer != null)
            {
                if (CurrentDay == (DayOfWeek)Enum.Parse(typeof(DayOfWeek), CheckDayOffer.Day))
                {
                    string itemIndex = CheckDayOffer.ItemIndex.ToString();
                    string dis = CheckDayOffer.Discount.ToString();
                    flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path, KotPath, ItemName);
                }
            }
            else if (CheckDateOffer != null)
            {
                if (CurrentDate == CheckDateOffer.Date)
                {
                    string itemIndex = CheckDateOffer.ItemIndex.ToString();
                    string dis = CheckDateOffer.Discount.ToString();
                    flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path, KotPath, ItemName);

                }
            }
            else
            {
                flag = true;
            }
            return flag;
        }
        public bool SaveOfferApplied(string oulte, string TableNo, bool flag, string ItemIndex, string DisCount, string Path,string KotPath,string ItemName)
        {
            XDocument xd = XDocument.Load(Path);
            int Itemindex = Convert.ToInt32(ItemIndex);
            var FreeItems = (from p in xd.Descendants("Items")
                             where p.Element("UserId").Value == oulte.ToString()
                             && Convert.ToInt32(p.Element("FullQty").Value) >= Itemindex
                             select p).ToList();
            if (FreeItems.Count != 0)
            {
                foreach (XElement itemElement in FreeItems)
                {
                    int FullQty = Convert.ToInt32(itemElement.Element("FullQty").Value);
                    int Index = Convert.ToInt32(ItemIndex);
                    int Remaining = 0;
                    if (Index > 1)
                    {
                        Remaining = FullQty / Index;
                    }
                    else
                    {
                        Remaining = Index;
                    }
                    int RemainingFullQty = FullQty - Remaining;
                    decimal Fullprice = Convert.ToDecimal(itemElement.Element("FullPrice").Value);
                    decimal Amount = Convert.ToDecimal(itemElement.Element("TotalAmount").Value);
                    decimal Discount = Convert.ToDecimal(DisCount);
                    decimal DiscountedAmount = ((Discount / 100) * Fullprice) * Remaining;
                    decimal RemainingAmount = Amount - DiscountedAmount;
                    itemElement.SetElementValue("TotalAmount", RemainingAmount.ToString());
                    itemElement.SetElementValue("FullQty", RemainingFullQty);
                    itemElement.SetElementValue("OfferQty", Remaining);
                }
                xd.Save(Path);

                XDocument kot = XDocument.Load(KotPath);
                var kotitems = (from item in kot.Descendants("Items")
                               where item.Element("TokenNo").Value == TableNo.ToString() && item.Element("ItemName").Value == ItemName && item.Element("UserId").Value == oulte.ToString()
                               select item).ToList();
            }

            return flag;
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