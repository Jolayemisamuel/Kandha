using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
namespace NibsMVC.Repository
{
    public class AdminOfferRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        #region
        public string GetItemId(string Name)
        {
            var Id = _entities.tblItems.Where(o => o.Name.Equals(Name)).Select(o => o.ItemId).FirstOrDefault();
            return Id.ToString();
        }
        public string Save(OfferModel model)
        {
            try
            {
                Nibs_Offer offers = new Nibs_Offer();
                offers.OfferType = model.OfferType;
                _entities.Nibs_Offer.Add(offers);
                _entities.SaveChanges();
                int Max = _entities.Nibs_Offer.Max(x => x.OfferId);
                var Days = model.Days.Split(',');
                for (int i = 0; i < Days.Length; i++)
                {
                    Nibs_Offer_Days days = new Nibs_Offer_Days();
                    days.Days = Days[i];
                    days.OfferId = Max;
                    _entities.Nibs_Offer_Days.Add(days);
                    _entities.SaveChanges();

                }
                var BuyItems = model.BuyItemId.Split(',');
                if (BuyItems.Length > 1)
                {
                    for (int i = 0; i < BuyItems.Length; i++)
                    {
                        Nibs_Offer_Buy_Items buy = new Nibs_Offer_Buy_Items();
                        buy.ItemId = Convert.ToInt32(BuyItems[i]);
                        buy.OfferId = Max;
                       // buy.Quantity = model.BuyQuantity;
                        buy.Quantity = model.BuyQuantity;
                        _entities.Nibs_Offer_Buy_Items.Add(buy);
                        _entities.SaveChanges();
                    }
                }
                else
                {
                    Nibs_Offer_Buy_Items buy = new Nibs_Offer_Buy_Items();
                    buy.ItemId = Convert.ToInt32(model.BuyItemId);
                    buy.OfferId = Max;
                    buy.Quantity = model.BuyQuantity;
                    _entities.Nibs_Offer_Buy_Items.Add(buy);
                    _entities.SaveChanges();
                }
                Nibs_Offer_Free_Items free = new Nibs_Offer_Free_Items();
                free.ItemId = Convert.ToInt32(model.FreeItemId);
                free.OfferId = Max;
                free.Quantity = model.FreeQuantity;
                free.Discount = model.Discount;
                _entities.Nibs_Offer_Free_Items.Add(free);
                _entities.SaveChanges();
                return "record saved successfully..";
            }
            catch
            {
                return "something wrong try again !";
            }
        }
        private int ItemId(string Id)
        {
            var ItemID = _entities.tblItems.Where(x => x.Name.Equals(Id)).Select(x => x.ItemId).SingleOrDefault();
            return ItemID;
        }
        public string SaveAmountBasisOffer(AmountBasisOfferModel model)
        {
            try
            {
                Nibs_Offer offers = new Nibs_Offer();
                offers.OfferType = model.OfferType;
                _entities.Nibs_Offer.Add(offers);
                _entities.SaveChanges();
                int Max = _entities.Nibs_Offer.Max(x => x.OfferId);
                var Days = model.Days.Split(',');
                for (int i = 0; i < Days.Length; i++)
                {
                    Nibs_Offer_Days days = new Nibs_Offer_Days();
                    days.Days = Days[i];
                    days.OfferId = Max;
                    _entities.Nibs_Offer_Days.Add(days);
                    _entities.SaveChanges();

                }
                Nibs_Offer_Amount tb = new Nibs_Offer_Amount();
                tb.Amount = model.Amount;
                tb.Discount = model.Discount;
                tb.ItemId = model.ItemId;
                tb.OfferId = Max;
                tb.Quantity = model.Quantity;
                _entities.Nibs_Offer_Amount.Add(tb);
                _entities.SaveChanges();
                return "record saved successfully..";
            }
            catch
            {
                return "Try Again !";
            }

        }
        public string SaveComboOffer(ComboOfferModel model)
        {
            try
            {
                Nibs_Offer offers = new Nibs_Offer();
                offers.OfferType = model.OfferType;
                _entities.Nibs_Offer.Add(offers);
                _entities.SaveChanges();
                int Max = _entities.Nibs_Offer.Max(x => x.OfferId);
                var Days = model.Days.Split(',');
                for (int i = 0; i < Days.Length; i++)
                {
                    Nibs_Offer_Days days = new Nibs_Offer_Days();
                    days.Days = Days[i];
                    days.OfferId = Max;
                    _entities.Nibs_Offer_Days.Add(days);
                    _entities.SaveChanges();

                }
                var ComboItems = model.ComboItemId.Split(',');
                for (int i = 0; i < ComboItems.Length; i++)
                {
                    Nibs_Offer_Buy_Items buy = new Nibs_Offer_Buy_Items();
                    buy.ItemId = Convert.ToInt32(ComboItems[i]);
                    buy.OfferId = Max;
                    buy.Quantity = model.FreeQuantity;
                    _entities.Nibs_Offer_Buy_Items.Add(buy);
                    _entities.SaveChanges();
                }
                NIbs_ComboOffer combo = new NIbs_ComboOffer();
                combo.OfferId = Max;
                combo.BaseAmount = model.BaseAmount;
                _entities.NIbs_ComboOffer.Add(combo);
                _entities.SaveChanges();
                return "record saved successfully..";
            }
            catch
            {
                return "something wrong try again !";
            }
        }
        #endregion

        // get all offer region
        #region
        public List<GetAllOfferListModel> List()
        {
            AssignOfferInterface assign = new AssignOfferInterface();
            List<GetAllOfferListModel> lst = new List<GetAllOfferListModel>();
            
            var result = _entities.Nibs_Offer.ToList();
            foreach (var item in result)
            {
                GetAllOfferListModel model = new GetAllOfferListModel();
                var OfferName = assign.OfferName(item.OfferType);
                if (OfferName == "Combo Offer")
                {
                    model.ComboOfferList = GetComboOffer(item.OfferId);
                }
                else if (OfferName == "Amount Basis")
                {
                    model.BaseAmountList = GetAmountBasis(item.OfferId);
                }
                else if (OfferName == "Happy Hours")
                {
                    model.HappyHoursList = GetHappyHours(item.OfferId);
                }
                else
                {
                    model.BuyOfferList = GetBuyOffer(item.OfferId,OfferName);
                }
                
                lst.Add(model);
            }
            return lst;
        }
        public List<ComboOfferListModel> GetComboOffer(int OfferId)
        {
            List<ComboOfferListModel> lst = new List<ComboOfferListModel>();
            string BuyItem = string.Empty;
            string Days = string.Empty;
            foreach (var item in _entities.NIbs_ComboOffer.Where(a => a.OfferId == OfferId).ToList())
            {
                ComboOfferListModel model = new ComboOfferListModel();
                foreach (var items in _entities.Nibs_Offer_Days.Where(a => a.OfferId == item.OfferId).ToList())
                {
                    Days += items.Days + ",";
                }
                foreach (var items in _entities.Nibs_Offer_Buy_Items.Where(a => a.OfferId == item.OfferId).ToList())
                {
                    BuyItem += items.tblItem.Name + ",";
                }
                model.OfferId = item.OfferId;
                model.BuyItems = BuyItem;
                model.Days = Days;
                model.BaseAmount = item.BaseAmount;
                lst.Add(model);
            }
            return lst;
        }
        public List<ShowBuyOfferModel> GetBuyOffer(int OfferId,string OfferName)
        {
            List<ShowBuyOfferModel> lst = new List<ShowBuyOfferModel>();

            string days = string.Empty;
            string BuyItems = string.Empty;
            string FreeItems = string.Empty;
            foreach (var item in _entities.Nibs_Offer.Where(a => a.OfferId == OfferId).ToList())
            {
                ShowBuyOfferModel model = new ShowBuyOfferModel();
                foreach (var items in _entities.Nibs_Offer_Days.Where(a => a.OfferId == OfferId).ToList())
                {
                    days += items.Days + ",";
                }
               
                foreach (var items in _entities.Nibs_Offer_Buy_Items.Where(a => a.OfferId == OfferId).ToList())
                {
                    BuyItems += items.tblItem.Name + ",";
                }
                
                foreach (var items in _entities.Nibs_Offer_Free_Items.Where(a => a.OfferId == OfferId).ToList())
                {
                    FreeItems += items.tblItem.Name;
                    model.Discount = items.Discount.Value;
                    model.Quantity = model.Quantity;
                }
               

                model.FreeItems = FreeItems;
                model.BuyItems = BuyItems;
                model.Days = days;
                model.OfferId = OfferId;
                model.OfferName = OfferName;
                lst.Add(model);
            }
            return lst;
        }
        public List<BaseAmountOfferListModel> GetAmountBasis(int OfferId)
        {
            List<BaseAmountOfferListModel> lst = new List<BaseAmountOfferListModel>();
            foreach (var item in _entities.Nibs_Offer_Amount.Where(a => a.OfferId == OfferId).ToList())
            {
                BaseAmountOfferListModel model = new BaseAmountOfferListModel();
                string Days = string.Empty;
                foreach (var items in _entities.Nibs_Offer_Days.Where(a => a.OfferId == item.OfferId).ToList())
                {
                    Days += items.Days + ",";
                }
                model.Amount = item.Amount;
                model.AmtId = item.AmtId;
                model.Discount = item.Discount;
                model.ItemId = item.tblItem.Name;
                model.OfferId = item.Nibs_Offer.OfferId;
                model.Quantity = item.Quantity.Value;
                model.OfferId = OfferId;
                lst.Add(model);

            }
            return lst;
        }
        public List<GetHappyHaourListModel> GetHappyHours(int OfferId)
        {
            List<GetHappyHaourListModel> lst = new List<GetHappyHaourListModel>();
            foreach (var item in _entities.Nibs_HappyHours_Date.Where(a=>a.OfferId==OfferId).ToList())
            {
                GetHappyHaourListModel model = new GetHappyHaourListModel();
                model.Discount = item.Discount;
                model.from = item.Date.ToShortTimeString();
                model.ItemIndex = item.ItemIndex.Value;
                model.ItemName = item.tblItem.Name;
                model.OfferId = item.OfferId;
                model.TimeEnd = item.TimeEnd.ToString();
                model.TimeFrom = item.TimeStart.ToString();
                model.To = item.Date.ToShortTimeString();
                lst.Add(model);
            }
            foreach (var item in _entities.Nibs_HappyHours_Day.Where(a => a.OfferId == OfferId).ToList())
            {
                GetHappyHaourListModel model = new GetHappyHaourListModel();
                model.Discount = item.Discount;
                model.from = item.Day;
                model.ItemIndex = item.ItemIndex.Value;
                model.ItemName = item.tblItem.Name;
                model.OfferId = item.OfferId;
                model.TimeEnd = item.TimeEnd.ToString();
                model.TimeFrom = item.TimeStart.ToString();
                model.To = item.Day;
                lst.Add(model);
            }
            foreach (var item in _entities.Nibs_HappyHours_Days.Where(a => a.OfferId == OfferId).ToList())
            {
                GetHappyHaourListModel model = new GetHappyHaourListModel();
                model.Discount = item.Discount;
                model.from = item.StartDay;
                model.ItemIndex = item.ItemIndex.Value;
                model.ItemName = item.tblItem.Name;
                model.OfferId = item.OfferId;
                model.TimeEnd = item.TimeEnd.ToString();
                model.TimeFrom = item.TimeStart.ToString();
                model.To = item.EndDay;
                lst.Add(model);
            }
            foreach (var item in _entities.Nibs_HappyHoursDates.Where(a => a.OfferId == OfferId).ToList())
            {
                GetHappyHaourListModel model = new GetHappyHaourListModel();
                model.Discount = item.Discount;
                model.from = item.StartDate.ToShortDateString();
                model.ItemIndex = item.ItemIndex.Value;
                model.ItemName = item.tblItem.Name;
                model.OfferId = item.OfferId;
                model.TimeEnd = item.TimeEnd.ToString();
                model.TimeFrom = item.TimeStart.ToString();
                model.To = item.EndDate.ToShortDateString();
                lst.Add(model);
            }
            return lst;
        }
        #endregion

        // delete offer region
        #region
        public bool DeleteOffer(int OfferId)
        {
            try
            {
                AssignOfferInterface assign = new AssignOfferInterface();
                var assignOffer = _entities.NIbs_AssignOffer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                if(assignOffer==null)
                {

               
                var result = _entities.Nibs_Offer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                foreach (var item in _entities.Nibs_Offer_Days.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_Offer_Days.Remove(item);

                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_Offer_Buy_Items.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_Offer_Buy_Items.Remove(item);

                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_Offer_Free_Items.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_Offer_Free_Items.Remove(item);

                }
                _entities.SaveChanges();
                var OfferName = assign.OfferName(result.OfferType);
                if (OfferName == "Amount Basis")
                {
                    foreach (var items in _entities.Nibs_Offer_Amount.Where(a => a.OfferId == OfferId).ToList())
                    {
                        _entities.Nibs_Offer_Amount.Remove(items);
                    }
                }
                _entities.SaveChanges();
                if (OfferName == "Combo Offer")
                {
                    foreach (var items in _entities.NIbs_ComboOffer.Where(a => a.OfferId == OfferId).ToList())
                    {
                        _entities.NIbs_ComboOffer.Remove(items);
                    }
                }
                _entities.SaveChanges();
                _entities.Nibs_Offer.Remove(result);
                _entities.SaveChanges();


                return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteCombo(int OfferId)
        {
            try
            {
                var result = _entities.Nibs_Offer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                var assignOffer = _entities.NIbs_AssignOffer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                if (assignOffer==null)
                {
                    
                
                foreach (var item in _entities.NIbs_ComboOffer.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.NIbs_ComboOffer.Remove(item);
                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_Offer_Buy_Items.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_Offer_Buy_Items.Remove(item);
                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_Offer_Days.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_Offer_Days.Remove(item);
                }
                _entities.SaveChanges();
                _entities.Nibs_Offer.Remove(result);
                _entities.SaveChanges();
                return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteAmountBasis(int OfferId)
        {
            try
            {
                var result = _entities.Nibs_Offer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                var assignOffer = _entities.NIbs_AssignOffer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                if (assignOffer==null)
                {
                    
                
                foreach (var item in _entities.Nibs_Offer_Amount.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_Offer_Amount.Remove(item);
                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_Offer_Days.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_Offer_Days.Remove(item);
                }
                _entities.SaveChanges();
                _entities.Nibs_Offer.Remove(result);
                _entities.SaveChanges();
                return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteHappyHours(int OfferId)
        {
            try
            {
                var result = _entities.Nibs_Offer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                var assignOffer = _entities.NIbs_AssignOffer.Where(a => a.OfferId == OfferId).SingleOrDefault();
                if (assignOffer==null)
                {
                    
                
                foreach (var item in _entities.Nibs_HappyHours_Date.Where(a=>a.OfferId==OfferId).ToList())
                {
                    _entities.Nibs_HappyHours_Date.Remove(item);
                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_HappyHours_Day.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_HappyHours_Day.Remove(item);
                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_HappyHours_Days.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_HappyHours_Days.Remove(item);
                }
                _entities.SaveChanges();
                foreach (var item in _entities.Nibs_HappyHoursDates.Where(a => a.OfferId == OfferId).ToList())
                {
                    _entities.Nibs_HappyHoursDates.Remove(item);
                }
                _entities.SaveChanges();
                _entities.Nibs_Offer.Remove(result);
                _entities.SaveChanges();
                return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion




    }
}