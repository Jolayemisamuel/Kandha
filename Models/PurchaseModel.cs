using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class PurchaseModel
    {
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "Please Enter Invoice No")]
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal NetAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ExtraCharge { get; set; }
        public string ExtraChargeDetail { get; set; }
        public string Unit { get; set; }
        public int OutletId { get; set; }
        public int VendorId { get; set; }
        public int PurchaseDetailsId { get; set; }
        public int ItemId { get; set; }
        public decimal DepositeAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal Quantity { get; set; }
        public string PaymentMode { get; set; }
        public string ChequeNo { get; set; }
        public string itemsdata { get; set; }
        public string ItemName { get; set; }
        public string Vendorname { get; set; }
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public decimal TaxPer { get; set; }
        public int PurchaseOrderId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Remarks { get; set; }
        public List<purchaseItemDetailsModel> getAllPurchaseItems { get; set; }
    }
    public class OutletPurchageModel
    {
        public int[] RowMaterialId { get; set; }
        public decimal[] Quantity { get; set; }
        public string[] Type { get; set; }
        public decimal[] txt_Amount { get; set; }
        public decimal[] TaxPer { get; set; }
        public int VendorId { get; set; }

        public string vendorname { get; set; }

        [Required(ErrorMessage = "Please Enter Invoice No")]
        public int InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal ExtraCharge { get; set; }
        public string ExtraChargeDetail { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentMode { get; set; }
        public decimal DepositeAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public int ChequeNo { get; set; }
        public string Remarks { get; set; }
        public int PurchaseOrderId { get; set; }
        public int PurchaseId { get; set; }
    }
    public class OutletPurchageReturnModel
    {
        public int OutletId { get; set; }
        public int VendorId { get; set; }
        public int RowMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReturnQuantity { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string VenderName { get; set; }
        public string RowMaterialName { get; set; }
        public string Reasion { get; set; }
        public int Purchasedetailid { get; set; }
        public int Purchaseid { get; set; }
    }
    public class purchaseItemDetailsModel
    {
        public int PurchaseDetailId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }

        public decimal  TaxPer { get; set; }

    }

    //Purchase Order 
    public class PurchaseOrderModel
    {
        public int POId { get; set; }
        public string  PONo { get; set; }
        public DateTime PODate { get; set; }
        public string Unit { get; set; }
        public int OutletId { get; set; }
        public int VendorId { get; set; }
        public int PurchaseOrderDetailId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string itemsdata { get; set; }
        public string ItemName { get; set; }
        public string Vendorname { get; set; }
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public int cycledays { get; set; }
        public List<purchaseOrderItemDetailsModel> getAllPurchaseOrderItems { get; set; }
    }

    public class purchaseOrderItemDetailsModel
    {
        public int PurchaseOrderDetailId { get; set; }
        public string Name { get; set; }
       
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }

    public class OutletPurchaseOrderModel
    {
        public int[] RowMaterialId { get; set; }
        public decimal[] Quantity { get; set; }
        public string[] Type { get; set; }
        public int VendorId { get; set; }
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
        
    }

    public static class DateTimeExtensions
    {
        public static string ToFinancialYear(this DateTime dateTime)
        {
            return "" + (dateTime.Month >= 4 ? dateTime.Year + 1 : dateTime.Year);
        }

        public static string ToFinancialYearShort(this DateTime dateTime)
        {
            return "" + (dateTime.Month >= 4 ? dateTime.AddYears(1).ToString("yy") : dateTime.ToString("yy"));
        }
    }
    public class VendorItemDetails
    {
        public int RawMaterialId { get; set; }
        public string Rawname { get; set; }
        public string unit { get; set; }
        public decimal quantity { get; set; }
        public decimal amount { get; set; }
        public decimal tax { get; set; }
        public decimal total { get; set; }
        public int OutletId { get; set; }
    }

    public class PurchaseItemDetails
    {
        public int PurchaseOrderId { get; set; }
        public int RawMaterialId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public string PONo { get; set; }
        public DateTime PODate { get; set; }
    }
}