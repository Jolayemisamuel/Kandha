using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.EDMX;
using System.ComponentModel.DataAnnotations;
namespace NibsMVC.Models
{

    public class SearchReportsModel
    {
        [Required(ErrorMessage = "please select date from")]
        public string FromDate { get; set; }
        [Required(ErrorMessage = "please select date to")]
        public string ToDate { get; set; }
        public int? TokenNo { get; set; }
        public List<tblBillMaster> getbills { get; set; }
    }




    public class BillingModel
    {

        public int BillId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServicChargeAmt { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int TableNo { get; set; }
        public string CustomerName { get; set; }
        public int BillDetailsId { get; set; }
        public int CategoryId { get; set; }
        public int FullQty { get; set; }
        public int HalfQty { get; set; }
        public decimal Amount { get; set; }
        public int ItemId { get; set; }
        public string masterdata { get; set; }
        public string detailsitems { get; set; }
        public string BillType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Outletid { get; set; }
        public string takemasterdata { get; set; }
        public string takedetailsitems { get; set; }
        public int TokenNo { get; set; }
        public int SearchToken { get; set; }
        public string homemasterdata { get; set; }
        public string homedetailsitems { get; set; }
        public string Address { get; set; }
        public string mergetabledata { get; set; }
        public string OutletName { get; set; }
        public string PaymentType { get; set; }
        public List<AdminBillDetailsReportModel> getBillItemDetails { get; set; }
    }
    // for searching 
    public class AdminBillReportModel
    {
        public List<SelectListItem> getAllListOfOutlet { get; set; }
        public List<OpenFood> getAllOpenFood { get; set; }
        public DateTime SearchFrom { get; set; }
        public DateTime SearchTo { get; set; }
        public int BillNo { get; set; }
        public int OutletId { get; set; }
        public DateTime Today { get; set; }
        public List<BillingModel> getAllBillReports { get; set; }
    }
    public class AdminBillDetailsReportModel
    {
        public int BilldeatilId { get; set; }
        public string ItemName { get; set; }
        public int HalfQty { get; set; }
        public int FullQty { get; set; }
        public decimal Amount { get; set; }
    }
    public class NibsBillingModel
    {
        public List<BillTableModelList> getAllItem { get; set; }
        public List<BillTableModel> getAllTables { get; set; }
        public List<SelectListItem> getAllAutoCompleteItem { get; set; }
        public decimal ServiceCharge { get; set; }
    }

    public class BillTableModelList
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }

    }
    public class BillTableModel
    {
        public string TableNo { get; set; }
        public string Current { get; set; }

        public string AcType { get; set; }
    }
    public class shiftBillModel
    {
        public List<BillTableModel> getAllBooktable { get; set; }
        public List<BillTableModel> getAllfreetable { get; set; }
    }
    public class OrderDispatchModel
    {
        public int TableNo { get; set; }
        public string CustomerName { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentType { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDate { get; set; }
        public decimal Discount { get; set; }
    }
    public class MergedTableModel
    {
        public string MergedTable { get; set; }
        public string MasterTable { get; set; }
    }
    /// models for ajax call
    public class GetBillingSubItemModel
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
        public bool Outstock { get; set; }
        // vendor id for vendor billing
        public int VendorId { get; set; }


    }
    public class GetBillingModel
    {
        public int BillId { get; set; }
        public bool IsPrinted { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string TableNo { get; set; }
        public List<GetBillingItemModel> _getbillingItems { get; set; }
        public List<SelectListItem> getPaymentType { get; set; }
        public List<SelectListItem> getAllAutoCompleteItem { get; set; }
        public string PaymentType { get; set; }
        public DateTime? CheckDate { get; set; }
        public string ChequeNo { get; set; }
        public decimal Discount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountWithoutDiscount { get; set; }
        public List<GetKotItemModel> _getKotitems { get; set; }
        public string OrderDispatch { get; set; }
        public string CurrentTables { get; set; }
        public decimal ServicesCharge { get; set; }
        public string OrderType { get; set; }
        public int PackingCharges { get; set; }
        public string ContactNo { get; set; }

    }
    public class GetBillingItemModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int FullQty { get; set; }
        public decimal FullPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal Vat { get; set; }
        public int OfferQty { get; set; }

    }
    public class GetKotItemModel
    {
        public int TNo { get; set; }
        public string ItemName { get; set; }
        public int FullQty { get; set; }
        public int HalfQty { get; set; }
    }
    public class GetBillItemModel
    {
        public int ItemId { get; set; }
        public int RunningTable { get; set; }
        public int Qty { get; set; }
        public string Type { get; set; }

    }
    public class PrintBillModel
    {
        public int BillId { get; set; }
        public string TinNo { get; set; }
        public string ServiceTaxNo { get; set; }
        public string Address { get; set; }
        public string ContactA { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string TableNo { get; set; }
        public List<PrintItemModel> getAllItem { get; set; }
        public List<PrintVatModel> getAllVat { get; set; }
        public decimal ServicesCharge { get; set; }
        public decimal ServiceTax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmountAfterDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal discount { get; set; }
        public decimal PackingCharge { get; set; }
        public string CustomerContactNo { get; set; }
        public DateTime BillDate { get; set; }
        public string BillType { get; set; }
    }
    public class PrintItemModel
    {
        public string ItemName { get; set; }
        public string FullQty { get; set; }
        public string HalfQty { get; set; }
        public decimal Amount { get; set; }
        public decimal BasicPrice { get; set; }
    }
    public class PrintVatModel
    {
        public decimal Vat { get; set; }
        public decimal amtCharges { get; set; }
    }
    public class BillOpenFoodModel
    {
        public string CustomerName { get; set; }
        public int TableNo { get; set; }
        public string OpenFoodName { get; set; }
        public decimal OpenFoodPrice { get; set; }
        public int OpenFoodQuantity { get; set; }
        public decimal OpenFoodVat { get; set; }
    }
    public class DateWiseBillModel
    {
        public DateTime Date { get; set; }
    }
}