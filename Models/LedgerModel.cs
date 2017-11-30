using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace NibsMVC.Models
{
    public class LedgerModel
    {
        public int Id { get; set; }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Record_Id { get; set; }
        [Required(ErrorMessage = "Please Enter Ledger Name")]
        public string LedgerName { get; set; }
        [Required(ErrorMessage = "please Select Ledger Group")]
        public int LedgerGroupId { get; set; }
        public string LedgerGroupName { get; set; }
        public DateTime Date { get; set; }
        public decimal? OpeningBalance { get; set; }
        public decimal? Percentage { get; set; }
        public string TransferType { get; set; }
    }
    public class Ledgergroupmodel
    {
        public int RecordId { get; set; }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int RecordNum { get; set; }
        public string GroupName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}