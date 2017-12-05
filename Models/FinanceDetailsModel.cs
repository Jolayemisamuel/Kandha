using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NibsMVC.Models
{
    public class FinanceDetailsModel
    {
    }
    public class ReceiptDetails
    {
        public int voucherentryid { get; set; }
        public int voucherno { get; set; }
        public DateTime? voucherdate { get; set; }                
        public int ledgerid { get; set; }
        public string ledgername { get; set; }

        public int recordno { get; set; }
        public decimal debitamount { get; set; }
        public string checkno { get; set; }
        public DateTime? checkdate { get; set; }
        public string crdescription { get; set; }
        public decimal creditamount { get; set; }
        public string drdescription { get; set; }
        public int debitledgerid { get; set; }
        public string debitledgername { get; set; }
        public int creditledgerid { get; set; }
        public string creditledgername { get; set; }
        //public List<ReceiptItemDetailModel> getAllReceiptItemDetails { get; set; }

    }
    //public class ReceiptItemDetailModel
    //{
    //    public int recordno { get; set; }
    //    public decimal debitamount { get; set; }
    //    public string checkno { get; set; }
    //    public DateTime checkdate { get; set; }
    //    public string crdescription { get; set; }
    //    public decimal creditamount { get; set; }
    //    public string drdescription { get; set; }
    //    public int voucherentryid { get; set; }
    //    public int voucherno { get; set; }
    //    public DateTime voucherdate { get; set; }
    //    public int debitledgerid { get; set; }

    //    public string debitledgername { get; set; }

    //    public int creditledgerid { get; set; }
    //    public string creditledgername { get; set; }


    //}
}