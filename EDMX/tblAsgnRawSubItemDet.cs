//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NibsMVC.EDMX
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblAsgnRawSubItemDet
    {
        public int Id { get; set; }
        public int AsgnRawSubItemMstId { get; set; }
        public int RawMaterialId { get; set; }
        public decimal Qty { get; set; }
    
        public virtual tblAssignRawSubItemMst tblAssignRawSubItemMst { get; set; }
    }
}
