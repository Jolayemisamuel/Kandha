using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    // models for create offers
    #region
    public class OfferCreationModel
    {
        public int Categoryid { get; set; }
        public int ItemId { get; set; }
        public string Offertype { get; set; }
        public string TitleName { get; set; }
        public bool Applyall { get; set; }
        public float Discount { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string offervaldata { get; set; }
        public decimal FullPrice { get; set; }
        public decimal HalfPrice { get; set; }
    }
    public class OfferModel
    {
        public int OfferId { get; set; }
        public string OfferType { get; set; }
        public string Days { get; set; }
        public string BuyItemId { get; set; }
        public int FreeItemId { get; set; }
        public int BuyQuantity { get; set; }
        public int FreeQuantity { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
    }
    public class BuyOneGetOneOfferModel
    {
        public string OfferType { get; set; }
        public string BuyOneGetOneDays { get; set; }
        public int BuyOneGetOneBuyItemId { get; set; }
        public int BuyOneGetOneBuyFreeItemId { get; set; }
    }
    public class AmountBasisOfferModel
    {
        public string OfferType { get; set; }
        public int AmountId { get; set; }
        public decimal Amount { get; set; }
        public int ItemId { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public int OfferId { get; set; }
        public string Days { get; set; }
    }
    public class ComboOfferModel
    {
        public string OfferType { get; set; }
        public string Days { get; set; }
        public string ComboItemId { get; set; }
        public decimal BaseAmount { get; set; }
        public int FreeQuantity { get; set; }
    }
    public class HappyHoursDatesModel
    {
        public int HHDatesId { get; set; }
        public string HHOfferType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int FreeItemId { get; set; }
        public decimal Discount { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int OfferId { get; set; }
        public string OfferType { get; set; }
        public int ItemIndex { get; set; }
    }
    public class HappyHoursDaysModel
    {
        public int HHDaysId { get; set; }
        public string HHOfferType { get; set; }
        public string StartDay { get; set; }
        public string EndDay { get; set; }
        public int FreeItemId { get; set; }
        public decimal Discount { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int OfferId { get; set; }
        public string OfferType { get; set; }
        public int ItemIndex { get; set; }
    }
    public class HappyHoursDayModel
    {
        public int HHDayId { get; set; }
        public string HHOfferType { get; set; }
        public string Day { get; set; }
        public int FreeItemId { get; set; }
        public decimal Discount { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int OfferId { get; set; }
        public string OfferType { get; set; }
        public int ItemIndex { get; set; }
    }
    public class HappyHoursDateModel
    {
        public int HHDateId { get; set; }
        public string HHOfferType { get; set; }
        public string Date { get; set; }
        public int FreeItemId { get; set; }
        public decimal Discount { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public int OfferId { get; set; }
        public string OfferType { get; set; }
        public int ItemIndex { get; set; }
    }
    #endregion


    // models for get offers
    #region
    public class GetAllOfferListModel
    {
        public int OfferId { get; set; }
        public string OfferName { get; set; }
        public string BuyItems { get; set; }
        public string FreeItems { get; set; }
        public string Days { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public List<BaseAmountOfferListModel> BaseAmountList { get; set; }
        public List<ComboOfferListModel> ComboOfferList { get; set; }
        public List<ShowBuyOfferModel> BuyOfferList { get; set; }
        public List<GetHappyHaourListModel> HappyHoursList { get; set; }
    }
    public class ShowBuyOfferModel
    {
        public int OfferId { get; set; }
        public string OfferName { get; set; }
        public string BuyItems { get; set; }
        public string FreeItems { get; set; }
        public string Days { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
    }
    public class BaseAmountOfferListModel
    {
        public int AmtId { get; set; }
        public string ItemId { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public int OfferId { get; set; }
        public int Quantity { get; set; }
    }
    public class ComboOfferListModel
    {
        public int OfferId { get; set; }
        public string Days { get; set; }
        public string BuyItems { get; set; }
        public decimal BaseAmount { get; set; }
    }
    public class GetHappyHaourListModel
    {
        public int OfferId { get; set; }
        public string from { get; set; }
        public string To { get; set; }
        public string TimeFrom { get; set; }
        public string TimeEnd { get; set; }
        public string ItemName { get; set; }
        public decimal Discount { get; set; }
        public int ItemIndex { get; set; }
    }
    #endregion
    

}