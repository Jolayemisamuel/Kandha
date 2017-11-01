using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class TodayBillReportModel
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalServiceTax { get; set; }
        public decimal TotalServiceCharge { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalCashAmount { get; set; }
        public decimal TotalCardAmount { get; set; }
        public List<TodayCashAmount> getCashAmount { get; set; }
        // for Restro Bill
        public decimal RestroTotalAmount { get; set; }
        public decimal RestroTotalServiceTaxAmount { get; set; }
        public decimal RestroTotalServiceChargeAmount { get; set; }
        public decimal RestroTotalVatAmount { get; set; }
        public decimal TotalRestroNetAmount { get; set; }
        public decimal RestroTotalDiscountAmount { get; set; }
        public List<TotalRestroVatModel> getAllRestroVat { get; set; }
        public List<TodayRestroBillModel> getAllRestroBill { get; set; }
        public List<RestroDiscount> getRestroDiscount { get; set; }
        public List<RestroService> getRestroService { get; set; }
        // for home delivery
        public decimal HomeTotalAmount { get; set; }
        public decimal HomeTotalServiceTaxAmount { get; set; }
        public decimal HomeTotalServiceChargeAmount { get; set; }
        public decimal HomeTotalVatAmount { get; set; }
        public decimal TotalHomeNetAmount { get; set; }
        public decimal HomeTotalDiscountAmount { get; set; }
        public List<TotalHomeVatModel> getAllHomeVat { get; set; }
        public List<TodayHomeBillModel> getAllHomeBill { get; set; }
        public List<HomeDiscount> getHomeDiscount { get; set; }
        public List<HomeService> getHomeService { get; set; }
        // for Take Away
        public decimal TakeTotalAmount { get; set; }
        public decimal TakeTotalServiceTaxAmount { get; set; }
        public decimal TakeTotalServiceChargeAmount { get; set; }
        public decimal TakeTotalVatAmount { get; set; }
        public decimal TotalTakeNetAmount { get; set; }
        public decimal TakeTotalDiscountAmount { get; set; }
        public List<TotalTakeVatModel> getAllTakeVat { get; set; }
        public List<TodayTakeBillModel> getAllTakeBill { get; set; }
        public List<TakeDiscount> getTakeDiscount { get; set; }
        public List<TakeService> getTakeService { get; set; }
        public Nullable<DateTime> Date { get; set; }
        [Required(ErrorMessage = "please select date")]
        public Nullable<DateTime> DateFrom { get; set; }
        [Required(ErrorMessage = "please select date")]
        public Nullable<DateTime> DateTo { get; set; }
        public decimal TotalRestroCashAmount { get; set; }
        public decimal TotalRestroCardAmount { get; set; }

    }
    #region Restro
    public class TodayRestroBillModel
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal ServiceTaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Discount { get; set; }
    }
    public class TotalRestroVatModel
    {
        public decimal Vat { get; set; }
        public decimal Amount { get; set; }
    }
    public class RestroDiscount
    {
        public decimal DiscountAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
    }
    public class RestroService
    {
        public decimal Tax { get; set; }
    }
    public class TodayCashAmount
    {
        public decimal CashAmount { get; set; }
        public decimal CardAmount { get; set; }

    }
    public class GetVatDiscountWiseModel
    {
        public List<VatDiscountWise> getdiscountwise { get; set; }
    }
    public class VatDiscountWise
    {
        public double Vat { get; set; }
        public decimal Amount { get; set; }
        public int ItemId { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Discount { get; set; }
    }
    #endregion
    #region Home
    public class TodayHomeBillModel
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal ServiceTaxAmount { get; set; }
        public decimal Discount { get; set; }
    }
    public class TotalHomeVatModel
    {
        public decimal Vat { get; set; }
        public decimal Amount { get; set; }
    }
    public class HomeDiscount
    {
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
    }
    public class HomeService
    {
        public decimal Tax { get; set; }
    }
    #endregion

    #region Take
    public class TodayTakeBillModel
    {
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ServiceChargeAmount { get; set; }
        public decimal ServiceTaxAmount { get; set; }
        public decimal Discount { get; set; }
    }
    public class TotalTakeVatModel
    {
        public decimal Vat { get; set; }
        public decimal Amount { get; set; }
    }
    public class TakeDiscount
    {
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
    }
    public class TakeService
    {
        public decimal Tax { get; set; }
    }
    #endregion


}