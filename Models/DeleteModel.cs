using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class DeleteModel
    {
        public int DeleteId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServicChargeAmt { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int TableNo { get; set; }
        public string CustomerName { get; set; }
        public int DeletedetailId { get; set; }
        public int CategoryId { get; set; }
        public int FullQty { get; set; }
        public int HalfQty { get; set; }
        public decimal Amount { get; set; }
        public int ItemId { get; set; }
        public string BillingType { get; set; }
        public int Outletid { get; set; }
        public int TokenNo { get; set; }
        public string Address { get; set; }
        public DateTime DeleteDate { get; set; }
    }
}