using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using WebMatrix.WebData;
using System.Xml.Linq;
using System.Web.Security;
namespace NibsMVC.Repository
{
    public class AdminCallHappyHoursRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public bool CallHappyHoursDaysOffer(string Id, string TableNo, string Path, string KotPath)
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
                                   where System.Data.Entity.DbFunctions.TruncateTime(p.StartDate) <= System.Data.Entity.DbFunctions.TruncateTime(DateTime.Now)
                                   && System.Data.Entity.DbFunctions.TruncateTime(p.EndDate) >= System.Data.Entity.DbFunctions.TruncateTime(DateTime.Now)
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
                                  && p.Date==CurrentDate
                                  //&& p.Date.Day==CurrentDate.Day && p.Date.Month==CurrentDate.Month && p.Date.Year==CurrentDate.Year
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
                        flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path,CheckDaysOffer.ItemId);
                    }

                }
            }
            else if (checkDatesOffer != null)
            {

                if (CurrentDate >= checkDatesOffer.StartDate && CurrentDate <= checkDatesOffer.EndDate)
                {
                    string itemIndex = checkDatesOffer.ItemIndex.ToString();
                    string dis = checkDatesOffer.Discount.ToString();
                    flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path,checkDatesOffer.ItemId);

                }
            }
            else if (CheckDayOffer != null)
            {
                if (CurrentDay == (DayOfWeek)Enum.Parse(typeof(DayOfWeek), CheckDayOffer.Day))
                {
                    string itemIndex = CheckDayOffer.ItemIndex.ToString();
                    string dis = CheckDayOffer.Discount.ToString();
                    flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path,CheckDayOffer.ItemId);
                }
            }
            else if (CheckDateOffer != null)
            {
                if (CurrentDate == CheckDateOffer.Date)
                {
                    string itemIndex = CheckDateOffer.ItemIndex.ToString();
                    string dis = CheckDateOffer.Discount.ToString();
                    flag = SaveOfferApplied(oulte.ToString(), TableNo, flag, itemIndex, dis, Path,CheckDateOffer.ItemId);

                }
            }
            else
            {
                flag = true;
            }
            return flag;
        }
        public bool SaveOfferApplied(string oulte, string TableNo, bool flag, string ItemIndex, string DisCount, string Path,int ItemId)
        {
            XDocument xd = XDocument.Load(Path);
            int Itemindex = Convert.ToInt32(ItemIndex);
            var FreeItems = (from p in xd.Descendants("Items")
                             where p.Element("UserId").Value == oulte.ToString()
                             && p.Element("TableNo").Value == TableNo
                             && p.Element("ItemId").Value == ItemId.ToString()
                             && Convert.ToInt32(p.Element("FullQty").Value) >= Itemindex
                             select p).ToList();
          
            if (FreeItems.Count != 0)
            {
                foreach (XElement itemElement in FreeItems)
                {
                    int FullQty = Convert.ToInt32(itemElement.Element("FullQty").Value);
                    int OfferQty = Convert.ToInt32(itemElement.Element("OfferQty").Value);
                    int totalQty = FullQty + OfferQty;
                    int Index = Convert.ToInt32(ItemIndex);
                    int Remaining = 0;
                    int reminder = totalQty % Index;
                    if (reminder == 0)
                    {
                        if (Index==1)
                        {
                            Remaining = totalQty / Index;
                            int RemainingFullQty = totalQty - Remaining;
                            decimal Fullprice = Convert.ToDecimal(itemElement.Element("Fullprice").Value);
                            decimal Amount = Convert.ToDecimal(itemElement.Element("Amount").Value);
                            decimal Discount = Convert.ToDecimal(DisCount);
                            decimal DiscountedAmount = ((Discount / 100) * Fullprice) * FullQty;
                            decimal RemainingAmount = Amount - DiscountedAmount;
                            decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                            decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                            decimal remaingvatamtchrg = ((RemainingAmount * vatamt) / 100);
                            itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                            itemElement.SetElementValue("Amount", RemainingAmount.ToString());
                            itemElement.SetElementValue("FullQty", RemainingFullQty);
                            itemElement.SetElementValue("OfferQty", Remaining);
                        }
                        else
                        {
                            Remaining = totalQty / Index;
                            int RemainingFullQty = totalQty - Remaining;
                            decimal Fullprice = Convert.ToDecimal(itemElement.Element("Fullprice").Value);
                            decimal Amount = Convert.ToDecimal(itemElement.Element("Amount").Value);
                            decimal TotalAmount = Convert.ToDecimal(itemElement.Element("Fullprice").Value) * totalQty;
                            decimal Discount = Convert.ToDecimal(DisCount);
                            decimal DiscountedAmount = ((Discount / 100) * Fullprice) * RemainingFullQty;
                            decimal RemainingAmount = TotalAmount - DiscountedAmount;
                            decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                            decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                            decimal remaingvatamtchrg = ((RemainingAmount * vatamt) / 100);
                            itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                            itemElement.SetElementValue("Amount", DiscountedAmount.ToString());
                            itemElement.SetElementValue("FullQty", RemainingFullQty);
                            itemElement.SetElementValue("OfferQty", Remaining);
                        }
                        
                    }
                    else
                    {
                        int TotalQty = totalQty;
                        int CallIndex = Index;
                      
                        int indexQty = TotalQty / Index;
                        Remaining = indexQty;
                        int RemainingFullQty = totalQty - Remaining;
                        decimal Fullprice = Convert.ToDecimal(itemElement.Element("Fullprice").Value);
                        decimal Amount = Convert.ToDecimal(itemElement.Element("Amount").Value);
                        decimal TotalAmount = Convert.ToDecimal(itemElement.Element("Fullprice").Value) * totalQty;
                        decimal Discount = Convert.ToDecimal(DisCount);
                        decimal DiscountedAmount = ((Discount / 100) * Fullprice) * Remaining;
                        decimal RemainingAmount = TotalAmount - DiscountedAmount;
                        decimal vatamtchrg = Convert.ToDecimal(itemElement.Element("VatAmountCharges").Value);
                        decimal vatamt = Convert.ToDecimal(itemElement.Element("VatAmt").Value);
                        decimal remaingvatamtchrg = ((RemainingAmount * vatamt) / 100);
                        itemElement.SetElementValue("VatAmountCharges", remaingvatamtchrg);
                        itemElement.SetElementValue("Amount", RemainingAmount.ToString());
                        itemElement.SetElementValue("FullQty", RemainingFullQty);
                        itemElement.SetElementValue("OfferQty", Remaining);
                    }
                    

                }
                xd.Save(Path);
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