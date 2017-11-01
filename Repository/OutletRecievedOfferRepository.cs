using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using NibsMVC.Repository;
namespace NibsMVC.Repository
{
    public class OutletRecievedOfferRepository
    {
        NIBSEntities entities = new NIBSEntities();
        XMLTablesRepository obj = new XMLTablesRepository();
        AdminOfferRepository offer = new AdminOfferRepository();
        public List<GetAllOfferListModel> List()
        {
            AssignOfferInterface assign = new AssignOfferInterface();
            List<GetAllOfferListModel> list = new List<GetAllOfferListModel>();
            int OutletId = obj.getOutletId();
            var result = (from p in entities.Nibs_Offer
                          join q in entities.NIbs_AssignOffer
                              on p.OfferId equals q.OfferId
                          where q.UserId == OutletId
                          select new
                          {
                              OfferId = p.OfferId,
                              OfferType = p.OfferType
                          }).ToList();
            foreach (var item in result)
            {
                GetAllOfferListModel model = new GetAllOfferListModel();
                var OfferName = assign.OfferName(item.OfferType);
                if (OfferName == "Combo Offer")
                {
                    model.ComboOfferList = offer.GetComboOffer(item.OfferId);
                }
                else if (OfferName == "Amount Basis")
                {
                    model.BaseAmountList = offer.GetAmountBasis(item.OfferId);
                }
                else if (OfferName == "Happy Hours")
                {
                    model.HappyHoursList = offer.GetHappyHours(item.OfferId);
                }
                else
                {
                    model.BuyOfferList = offer.GetBuyOffer(item.OfferId, OfferName);
                }
                list.Add(model);
            }

            return list;
        }
    }
}