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
    
    public partial class Promo_Value
    {
        public decimal UID { get; set; }
        public decimal Promo_Header_UID { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal ByValue { get; set; }
        public decimal ByPercentage { get; set; }
    
        public virtual Promo_Header Promo_Header { get; set; }
    }
}
