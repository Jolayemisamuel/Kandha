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
    
    public partial class tblOperator
    {
        public int Operatorid { get; set; }
        public string Name { get; set; }
        public string ContactNo { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public Nullable<int> OutletId { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual tblOutlet tblOutlet { get; set; }
    }
}