using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NibsMVC.Models
{
    public class Finance
    {
    }
    public class Receipt
    {
        public int VoucherEntryId { get; set; }
        [Required]
        [Display(Name = "Enter Voucher No")]
        public string VoucherNo { get; set; }
        
        public string RecordNo { get; set; }
        [Required]
        [Display(Name = "Please Select Ledger")]
        public string[] LedgerAccId { get; set; }
        [Required]
        [Display(Name = "Enter the Credit Amount")]
        public decimal[] CreditAmount { get; set; }
        public string[] ChequeNo { get; set; }
        [Required]
        [Display(Name = "Select Date")]
        public DateTime Date { get; set; }
        public DateTime[] ChequeDate { get; set; }
        [Required]
        [Display(Name = "PLEASE SELECT Ledger Account")]
        public string[] ByAccount { get; set; }
        [Required]
        [Display(Name = "Enter Debit Amount")]
        public decimal[] DrAmount { get; set; }
        public string[] CrNarration { get; set; }
        public string[] DrNarration { get; set; }
        
    }
    public class CashReceipt
    {
        public int VoucherEntryId { get; set; }
        //[Required]
        //[Display(Name = "Enter Voucher No")]
        public string VoucherNo { get; set; }
        
        public string RecordNo { get; set; }
        //[Required]
        //[Display(Name = "Please Select Ledger")]
        public string[] LedgerAccId { get; set; }
        //[Required]
        //[Display(Name = "Enter the Credit Amount")]
        public decimal[] CreditAmount { get; set; }        
        //[Required]
        //[Display(Name = "Select Date")]
        public DateTime Date { get; set; }       
        //[Required]
        //[Display(Name = "PLEASE SELECT Ledger Account")]
        public string[] ByAccount { get; set; }
        //[Required]
        //[Display(Name = "Enter Debit Amount")]
        public decimal[] DrAmount { get; set; }
        public string[] CrNarration { get; set; }
        public string[] DrNarration { get; set; }
    }
   
}