using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.IRepository;
using System.Web.Mvc;
using NibsMVC.Models;
using NibsMVC.EDMX;
using WebMatrix.WebData;
using System.Text;
namespace NibsMVC.Repository
{
    public class AssignOfferInterface : IAssignOfferInterface
    {
        NIBSEntities _entities = new NIBSEntities();
        public List<SelectListItem> GetAllOutletList()
        {
            var outletList = new List<SelectListItem>();
            List<OutletModel> lst = (from p in _entities.tblOutlets.Where(a => a.OutletId != WebSecurity.CurrentUserId).ToList()
                                     select new OutletModel
                                     {
                                         OutletId = p.OutletId,
                                         Name = p.Name
                                     }).ToList<OutletModel>();
            foreach (var item in lst)
            {
                outletList.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.OutletId.ToString()
                    });
            }
            return outletList;
        }
        public List<AssignOfferToOutletModel> getAssignOfferList()
        {
            var result = _entities.NIbs_AssignOffer.ToList();
            List<AssignOfferToOutletModel> List = new List<AssignOfferToOutletModel>();
            foreach (var item in result)
            {
                AssignOfferToOutletModel model = new AssignOfferToOutletModel();
                model.UserName = item.tblOutlet.Name;
                model.OfferName = OfferName(item.Nibs_Offer.OfferType);
                if (item.Nibs_Offer.OfferType=="hh")
                {
                    var days = _entities.Nibs_HappyHours_Days.Where(a=>a.OfferId==item.OfferId).ToList();
                    string adddays = string.Empty;
                    foreach (var items in days)
                    {
                        adddays = adddays + items.StartDay + " to " + items.EndDay + "( "+items.TimeStart+"-"+items.TimeEnd+" )";
                    }
                    model.Days = adddays;
                }
                model.Id = item.Id;
                List.Add(model);
            }
            return List;
        }
        public string getOffer(int Id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetBuyOffer(Id));
            sb.Append(GetAmountOffer(Id));
            sb.Append(GetComboOffer(Id));
            sb.Append(GetHappyHoursDay(Id));
            sb.Append(GetHappyHoursDate(Id));
            sb.Append(GetHappyHoursDates(Id));
            sb.Append(GetHappyHoursDays(Id));
            return sb.ToString();
        }
        // get offer list to assign outlet
        #region
        public StringBuilder GetBuyOffer(int Id)
        {
            StringBuilder sb = new StringBuilder();
            var offerid = (from f in _entities.Nibs_Offer_Free_Items
                           join b in _entities.Nibs_Offer_Buy_Items on f.OfferId equals (b.OfferId)
                           where (from i in _entities.tblMenuOutlets where i.OutletId == Id select i.ItemId).Contains(f.ItemId)
                           && !(from q in _entities.NIbs_AssignOffer where q.UserId == Id select q.OfferId).Contains(f.OfferId)
                           select new
                           {
                               OfferId = f.Nibs_Offer.OfferId,
                               Name = f.Nibs_Offer.OfferType
                           }).Distinct().ToList();
            if (offerid != null)
            {

                foreach (var item in offerid)
                {
                    sb.Append("<tr class='odd gradeX'>");
                    sb.Append("<td>");
                    sb.Append("<input type='checkbox' class='checkboxes' value='" + item.OfferId + "' id='" + item.OfferId + "' name='OfferCheck' />");
                    sb.Append("</td>");
                    sb.Append("<td>" + OfferName(item.Name) + "</td>");
                    string buyitems = string.Empty;
                    string free = string.Empty;
                    var result = _entities.Nibs_Offer_Buy_Items.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in result)
                    {
                        buyitems += items.tblItem.Name + ",";
                    }
                    int lengh = buyitems.Length;
                    buyitems.Substring(0, lengh - 1);
                    sb.Append("<td>" + buyitems );
                    var freeitems = _entities.Nibs_Offer_Free_Items.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in freeitems)
                    {
                        free = items.tblItem.Name ;
                    }
                    sb.Append("<span class='fa fa-arrow-circle-o-right' style='padding-left: 10px;padding-right: 10px;'></span>" + free);
                    sb.Append("</td>");
                    string days = string.Empty;
                    foreach (var items in _entities.Nibs_Offer_Days.Where(a=>a.OfferId==item.OfferId).ToList())
                    {
                        days += items.Days+",";
                    }
                    sb.Append("<td>" + days + "</td>");
                    sb.Append("</tr>");
                }

            }
            return sb;
        }
        public StringBuilder GetAmountOffer(int Id)
        {
            StringBuilder sb = new StringBuilder();
            var amountofferid = (from p in _entities.Nibs_Offer_Amount
                                 where (from i in _entities.tblMenuOutlets where i.OutletId == Id select i.ItemId).Contains(p.ItemId)
                                 && !(from q in _entities.NIbs_AssignOffer where q.UserId == Id select q.OfferId).Contains(p.OfferId)
                                 select new
                                 {
                                     OfferId = p.Nibs_Offer.OfferId,
                                     Name = p.Nibs_Offer.OfferType
                                 }).Distinct().ToList();
            if (amountofferid != null)
            {

                foreach (var item in amountofferid)
                {
                    sb.Append("<tr class='odd gradeX'>");
                    sb.Append("<td>");
                    sb.Append("<input type='checkbox' class='checkboxes' value='" + item.OfferId + "' id='" + item.OfferId + "' name='OfferCheck' />");
                    sb.Append("</td>");
                    sb.Append("<td>" + OfferName(item.Name) + "</td>");
                    string freeitem = string.Empty;
                    string days = string.Empty;
                    string Amount = string.Empty;
                    var result = _entities.Nibs_Offer_Amount.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in result)
                    {
                        freeitem += items.tblItem.Name + ",";
                        Amount = items.Amount.ToString();
                    }
                    int lengh = freeitem.Length;
                    freeitem.Substring(0, lengh - 1);
                    sb.Append("<td> Amount="+Amount+"<span class='fa fa-arrow-circle-o-right' style='padding-left: 10px;padding-right: 10px;'></span>" + freeitem + "</td>");
                    var freeitems = _entities.Nibs_Offer_Days.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in freeitems)
                    {
                        days += items.Days + ",";
                    }
                    sb.Append("<td>" + days + "</td>");
                    sb.Append("</tr>");
                }

            }
            return sb;
        }
        public StringBuilder GetComboOffer(int Id)
        {
            StringBuilder sb = new StringBuilder();
            var ComboOfferId = (from p in _entities.Nibs_Offer_Buy_Items
                                join f in _entities.NIbs_ComboOffer on
                                p.OfferId equals f.OfferId
                                where (from i in _entities.tblMenuOutlets where i.OutletId == Id select i.ItemId).Contains(p.ItemId)
                                && !(from q in _entities.NIbs_AssignOffer where q.UserId == Id select q.OfferId).Contains(p.OfferId)
                                select new
                                {
                                    OfferId = p.Nibs_Offer.OfferId,
                                    Name = p.Nibs_Offer.OfferType
                                }).Distinct().ToList();
            if (ComboOfferId != null)
            {

                foreach (var item in ComboOfferId)
                {
                    sb.Append("<tr class='odd gradeX'>");
                    sb.Append("<td>");
                    sb.Append("<input type='checkbox' class='checkboxes' value='" + item.OfferId + "' id='" + item.OfferId + "' name='OfferCheck' />");
                    sb.Append("</td>");
                    sb.Append("<td>" + OfferName(item.Name) + "</td>");
                    string buyitems = string.Empty;
                    string days = string.Empty;
                    string BaseAmount = string.Empty;
                    var result = _entities.Nibs_Offer_Buy_Items.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in result)
                    {
                        buyitems += items.tblItem.Name  + ",";
                    }
                    foreach (var items in _entities.NIbs_ComboOffer.Where(a=>a.OfferId==item.OfferId).ToList())
                    {
                        BaseAmount = items.BaseAmount.ToString();
                    }
                    int lengh = buyitems.Length;
                    buyitems.Substring(0, lengh - 1);
                    sb.Append("<td>" + buyitems + "<span class='fa fa-arrow-circle-o-right' style='padding-left: 10px;padding-right: 10px;'></span>" +BaseAmount+ "</td>");
                    var freeitems = _entities.Nibs_Offer_Days.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in freeitems)
                    {
                        days += items.Days + ",";
                    }
                    sb.Append("<td>" + days + "</td>");
                    sb.Append("</tr>");
                }

            }
            return sb;
        }
        public StringBuilder GetHappyHoursDay(int Id)
        {
            StringBuilder sb = new StringBuilder();
            var HappyHours = (from p in _entities.Nibs_HappyHours_Day
                                 where (from i in _entities.tblMenuOutlets where i.OutletId == Id select i.ItemId).Contains(p.FreeItemId)
                                 && !(from q in _entities.NIbs_AssignOffer where q.UserId == Id select q.OfferId).Contains(p.OfferId)
                                 select new
                                 {
                                     OfferId = p.Nibs_Offer.OfferId,
                                     Name = p.Nibs_Offer.OfferType
                                 }).Distinct().ToList();
            if (HappyHours != null)
            {

                foreach (var item in HappyHours)
                {
                    sb.Append("<tr class='odd gradeX'>");
                    sb.Append("<td>");
                    sb.Append("<input type='checkbox' class='checkboxes' value='" + item.OfferId + "' id='" + item.OfferId + "' name='OfferCheck' />");
                    sb.Append("</td>");
                    sb.Append("<td>" + OfferName(item.Name) + "</td>");
                    var result = _entities.Nibs_HappyHours_Day.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in result)
                    {
                        sb.Append("<td>" + items.tblItem.Name + "</td>");
                        sb.Append("<td>" + items.Day+"("+items.TimeStart+"To"+items.TimeEnd+")" + "</td>");
                    }
                    
                    sb.Append("</tr>");
                }

            }
            return sb;
        }
        public StringBuilder GetHappyHoursDays(int Id)
        {
            StringBuilder sb = new StringBuilder();
            var HappyHours = (from p in _entities.Nibs_HappyHours_Days
                              where (from i in _entities.tblMenuOutlets where i.OutletId == Id select i.ItemId).Contains(p.FreeItemId)
                              && !(from q in _entities.NIbs_AssignOffer where q.UserId == Id select q.OfferId).Contains(p.OfferId)
                              select new
                              {
                                  OfferId = p.Nibs_Offer.OfferId,
                                  Name = p.Nibs_Offer.OfferType
                              }).Distinct().ToList();
            if (HappyHours != null)
            {

                foreach (var item in HappyHours)
                {
                    sb.Append("<tr class='odd gradeX'>");
                    sb.Append("<td>");
                    sb.Append("<input type='checkbox' class='checkboxes' value='" + item.OfferId + "' id='" + item.OfferId + "' name='OfferCheck' />");
                    sb.Append("</td>");
                    sb.Append("<td>" + OfferName(item.Name) + "</td>");
                   
                    var result = _entities.Nibs_HappyHours_Days.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in result)
                    {
                        sb.Append("<td>" + items.tblItem.Name + "</td>");
                        sb.Append("<td>" + items.StartDay+" To "+items.EndDay + "(" + items.TimeStart + "To" + items.TimeEnd + ")" + "</td>");
                    }
                    
                    sb.Append("</tr>");
                }

            }
            return sb;
        }
        public StringBuilder GetHappyHoursDate(int Id)
        {
            StringBuilder sb = new StringBuilder();
            var HappyHours = (from p in _entities.Nibs_HappyHours_Date
                              where (from i in _entities.tblMenuOutlets where i.OutletId == Id select i.ItemId).Contains(p.FreeItemId)
                              && !(from q in _entities.NIbs_AssignOffer where q.UserId == Id select q.OfferId).Contains(p.OfferId)
                              select new
                              {
                                  OfferId = p.Nibs_Offer.OfferId,
                                  Name = p.Nibs_Offer.OfferType
                              }).Distinct().ToList();
            if (HappyHours != null)
            {

                foreach (var item in HappyHours)
                {
                    sb.Append("<tr class='odd gradeX'>");
                    sb.Append("<td>");
                    sb.Append("<input type='checkbox' class='checkboxes' value='" + item.OfferId + "' id='" + item.OfferId + "' name='OfferCheck' />");
                    sb.Append("</td>");
                    sb.Append("<td>" + OfferName(item.Name) + "</td>");
                   
                    var result = _entities.Nibs_HappyHours_Date.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in result)
                    {
                        sb.Append("<td>" + items.tblItem.Name + "</td>");
                        sb.Append("<td>" + items.Date.ToShortDateString() + "(" + items.TimeStart + "To" + items.TimeEnd + ")" + "</td>");
                    }
                    
                    sb.Append("</tr>");
                }

            }
            return sb;
        }
        public StringBuilder GetHappyHoursDates(int Id)
        {
            StringBuilder sb = new StringBuilder();
            var HappyHours = (from p in _entities.Nibs_HappyHoursDates
                              where (from i in _entities.tblMenuOutlets where i.OutletId == Id select i.ItemId).Contains(p.FreeItemId)
                              && !(from q in _entities.NIbs_AssignOffer where q.UserId == Id select q.OfferId).Contains(p.OfferId)
                              select new
                              {
                                  OfferId = p.Nibs_Offer.OfferId,
                                  Name = p.Nibs_Offer.OfferType
                              }).Distinct().ToList();
            if (HappyHours != null)
            {

                foreach (var item in HappyHours)
                {
                    sb.Append("<tr class='odd gradeX'>");
                    sb.Append("<td>");
                    sb.Append("<input type='checkbox' class='checkboxes' value='" + item.OfferId + "' id='" + item.OfferId + "' name='OfferCheck' />");
                    sb.Append("</td>");
                    sb.Append("<td>" + OfferName(item.Name) + "</td>");
                   
                    var result = _entities.Nibs_HappyHoursDates.Where(a => a.OfferId == item.OfferId).ToList();
                    foreach (var items in result)
                    {
                        sb.Append("<td>" + items.tblItem.Name + "</td>");
                        sb.Append("<td>" + items.StartDate.ToShortDateString() + " To " + items.EndDate.ToShortDateString() + "(" + items.TimeStart + "To" + items.TimeEnd + ")" + "</td>");
                    }
                   
                    sb.Append("</tr>");
                }

            }
            return sb;
        }
        #endregion
        public string OfferName(string Name)
        {
            var OfferName = "";
            if (Name == "b1g1")
            {
                OfferName = "Buy One Get One";
            }
            else if (Name == "b2g1")
            {
                OfferName = "Buy Two Get One";
            }
            else if (Name == "co")
            {
                OfferName = "Combo Offer";
            }
            else if (Name == "hh")
            {
                OfferName = "Happy Hours";
            }
            else
            {
                OfferName = "Amount Basis";
            }

            return OfferName;

        }
        // asssign offer to outlet
        #region
        public bool AssignOffer(int UserId, int[] OfferCheck)
        {
            try
            {
                NIbs_AssignOffer tb = new NIbs_AssignOffer();
                for (int i = 0; i < OfferCheck.Length; i++)
                {
                    if (OfferCheck[i] != 0)
                    {
                        tb.UserId = UserId;
                        tb.OfferId = OfferCheck[i];
                        _entities.NIbs_AssignOffer.Add(tb);
                        _entities.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region
        public bool DeleteAssignedOffer(int Id)
        {
            try
            {
                var result = _entities.NIbs_AssignOffer.Where(a => a.Id == Id).SingleOrDefault();
                _entities.NIbs_AssignOffer.Remove(result);
                _entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion

    }
}