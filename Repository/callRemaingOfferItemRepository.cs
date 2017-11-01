using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.IRepository;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Text;
namespace NibsMVC.Repository
{
    public class callRemaingOfferItemRepository : ICallRemainingOfferItems
    {
        NIBSEntities entities = new NIBSEntities();
        public StringBuilder getRemainingItem()
        {
            var data = (from p in entities.tblItems
                        where p.Active==true && !(from q in entities.Nibs_Offer_Buy_Items select q.ItemId).Contains(p.ItemId)
                        && !(from w in entities.Nibs_Offer_Free_Items select w.ItemId).Contains(p.ItemId)
                        && !(from e in entities.Nibs_Offer_Amount select e.ItemId).Contains(p.ItemId)
                        && !(from r in entities.Nibs_HappyHours_Date select r.FreeItemId).Contains(p.ItemId)
                        && !(from t in entities.Nibs_HappyHours_Day select t.FreeItemId).Contains(p.ItemId)
                        && !(from y in entities.Nibs_HappyHours_Days select y.FreeItemId).Contains(p.ItemId)
                        && !(from u in entities.Nibs_HappyHoursDates select u.FreeItemId).Contains(p.ItemId)
                        select new
                        {
                            ItemId=p.ItemId,
                            Name=p.Name
                        }).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append(item.ItemId + "^" + item.Name + "#");
            }
            return sb;
        }
    }
}