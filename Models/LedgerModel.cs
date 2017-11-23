using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class LedgerModel
    {
        public int Id { get; set; }
        public string LedgerName { get; set; }
        public int LedgerGroupId { get; set; }
        public string LedgerGroupName { get; set; }
        public DateTime Date { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Percentage { get; set; }
        public string TransferType { get; set; }
    }
    public class Ledgergroupmodel
    {
        public int RecordId { get; set; }
        public string GroupName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}