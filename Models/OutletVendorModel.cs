using NibsMVC.EDMX;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Models
{
    public class OutletVendorModel
    {
        public List<SelectListItem> getAllVendor { get; set; }
        public List<SelectListItem> getAllCategory { get; set; }
        public List<OutletVendorItemModel> getAllItems { get; set; }
        public int VendorId { get; set; }
        public int CategoryId { get; set; }
        public int[] MenuOutletId { get; set; }
        public decimal[] Price { get; set; }
        public bool IsRunningBill { get; set; }
    }
    public class CreateVendorBillingModel
    {
        public int VendorId { get; set; }
        public List<SelectListItem> getAllVendors { get; set; }
        public List<tblCategory> getAllCategory { get; set; }
        public bool Ispending { get; set; }
        public decimal AmountBalance { get; set; }
    }
    public class OutletVendorItemModel
    {
        public int MenuOutletId { get; set; }
        public decimal ActualPrice { get; set; }
        public decimal Baseprice { get; set; }
        public string ItemName { get; set; }
    }
    public class VendorBillingItemModel
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string ItemName { get; set; }
        public decimal ActualPrice { get; set; }
        public decimal BasePrice { get; set; }
        public int Qty { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Vat { get; set; }
        public bool Iskot { get; set; }
        public int KotQty { get; set; }
        public int QtyAfterClearKot { get; set; }
        public bool IsPrinted { get; set; }
    }
   public class VendorBillingModel
   {
     
       public int VendorId { get; set; }
       public List<VendorBillingItemModel> getAllItems { get; set; }
       public decimal DisCount { get; set; }
       public decimal DisCountPrice { get; set; }
       public decimal VatAmount { get; set; }
       public decimal ServicTaxAmount { get; set; }
       public decimal ServiceChargeAmount { get; set; }
       public decimal Totalamount { get; set; }
       public decimal NetAmount { get; set; }
       [Required(ErrorMessage="please select payment method")]

       public string PaymentMethod { get; set; }
       public decimal DepositAmount { get; set; }
       public decimal remainingAmount { get; set; }
       public string ChequeDate { get; set; }
       public string ChequeNo { get; set; }
       public List<SelectListItem> getPaymentMethd { get; set; }
       public bool IsPrinted { get; set; }
   }
   public class VendorPrintBillModel
   {
       public int BillId { get; set; }
       public string TinNo { get; set; }
       public string ServiceTaxNo { get; set; }
       public string Address { get; set; }
       public string ContactA { get; set; }
       public string VendorName { get; set; }
       public string VendorAddress { get; set; }
       
       public List<PrintItemModel> getAllItem { get; set; }
       public List<PrintVatModel> getAllVat { get; set; }
       public decimal ServicesCharge { get; set; }
       public decimal ServiceTax { get; set; }
       public decimal TotalAmount { get; set; }
       public decimal VatAmount { get; set; }
       public decimal NetAmount { get; set; }
       public string VendorContactNo { get; set; }
       public DateTime BillDate { get; set; }
       public bool Isprinted { get; set; }
   }
    public class BillReportModel
    {
        public int VendorId { get; set; }
        public List<SelectListItem> getAllVendor { get; set; }
        public List<VendorBillingMaster> getAllBill { get; set; }
        public decimal? TotalDipositAmount { get; set; }
        public decimal? TotalRemainingAmount { get; set; }
    }
    public class AddVendorPayment
    {
        public int VendorId { get; set; }
        public List<SelectListItem> getAllVendors { get; set; }
        public decimal StatusAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public bool Ispending { get; set; }
    }
   public class VendorPaymentModel
   {
       public int VedorId { get; set; }
       public string VendorName { get; set; }
       public decimal  DepositAmount { get; set; }
       public decimal PurchaseAmount { get; set; }
       public decimal StatusBalance { get; set; }
       public List<SelesVendorAmountDetail> getAllAmount { get; set; }
   }
}