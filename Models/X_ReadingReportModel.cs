using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class X_ReadingReportModel
    {
        public string Address { get; set; }
        public string CashierName { get; set; }
        public DateTime Date { get; set; }
        public string POS { get; set; }
        public string Shift { get; set; }
        public string Mode { get; set; }
        public int BillStart { get; set; }
        public int BillEnd { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalVatAmount { get; set; }
        public decimal TotalServiceTaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal RoundOf { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalReceipts { get; set; }
        public decimal TotalOrderTender { get; set; }
        public decimal TotalUtilizedAmt { get; set; }
        public decimal TotalOrderAdvance { get; set; }
        public decimal TotalCreditSales { get; set; }
    }
}