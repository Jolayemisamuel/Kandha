//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NibsMVC.EDMX
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReportBillList
    {
        public decimal UID { get; set; }
        public decimal BillTrans_UID { get; set; }
        public decimal ItemsM_UID { get; set; }
        public decimal UOM_UID { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public System.DateTime OrderTime { get; set; }
        public decimal Value { get; set; }
        public decimal StatusM_UID { get; set; }
        public string Remarks { get; set; }
    
        public virtual ReportBill ReportBill { get; set; }
    }
}