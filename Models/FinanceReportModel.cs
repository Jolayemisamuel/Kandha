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
        public int LedgerGroupId { get; set; }
        public string LedgerGroupName { get; set; }
        public string TransferType { get; set; }
        public int VoucherEntryId { get; set; }
        public DateTime VoucherDate { get; set; }
        public int VoucherNo { get; set; }
        public int RecordNo { get; set; }
        public DateTime RecordDate { get; set; }
        public int VoucherSno { get; set; }
        public int VoucherTb { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string VoucherType { get; set; }
        public DateTime CreateDate { get; set; }
        public string DebitNarration { get; set; }
        public string CreditNarration { get; set; }
        public int PurchaseId { get; set; }

    }
}