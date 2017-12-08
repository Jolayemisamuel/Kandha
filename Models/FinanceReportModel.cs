using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NibsMVC.Models
{
    public class FinanceReportModel
    {
        public int LedgerMasterId { get; set; }
        public string LedgerName { get; set; }
        public DateTime LedgerDate { get; set; }
        public decimal OpeningBalance { get; set; }        
        public string TransferType { get; set; }       
        public int PurchaseId { get; set; }

    }
    public class DayBook
    {
        public int LedgerMasterId { get; set; }
        public string LedgerName { get; set; }
        public DateTime LedgerDate { get; set; }
        public decimal OpeningBalance { get; set; }       
        public string TransferType { get; set; }
        public int VoucherEntryId { get; set; }
        public DateTime VoucherDate { get; set; }        
        public int PurchaseId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime Todate { get; set; }
    }
}