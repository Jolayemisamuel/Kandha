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
    
    public partial class KOTTransList_DiscountList
    {
        public decimal UID { get; set; }
        public decimal KOTTransList_UID { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal DiscountAmt { get; set; }
        public string DiscountDescription { get; set; }
        public decimal ItemsM_UID { get; set; }
    
        public virtual KOTTransList KOTTransList { get; set; }
    }
}