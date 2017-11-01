using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Models
{
    public class BillSearchReportModel
    {
        public string OrderType { get; set; }
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateFrom { get; set; }
        //[DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateTo { get; set; }
        public int? BillNo { get; set; }
        public string PaymentType { get; set; }
        public List<SelectListItem> getPaymentType { get; set; }
        public List<SelectListItem> getBillingType { get; set; }
        public List<TotalVatModel> GetallVats { get; set; }
        public string allbill { get; set; }
        //public DataSet getbills { get; set; }
        //public ObservableCollection<ShowBillingReportModel> allbill { get; set; }
    }
    public class TotalVatModel
    {
        public decimal Vat { get; set; }
        public decimal Amount { get; set; }
    }
    public class ShowBillingReportModel
    {
        public int BillNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ServiceChage { get; set; }
        public decimal ServiceTax { get; set; }
        public DateTime BillDate { get; set; }
        public int TableNo { get; set; }

    }
    public class searchBillingItemModel
    {
        public int BillId { get; set; }
        public List<BillItemReportModel> getItems { get; set; }
    }
    public class BillItemReportModel
    {
        public string ItemName { get; set; }
        public int HalfQty { get; set; }
        public int FullQty { get; set; }
        public decimal Amount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Vat { get; set; }
    }
}