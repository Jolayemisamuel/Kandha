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
    
    public partial class Shift_Waiter_TableLink
    {
        public decimal UID { get; set; }
        public System.DateTime DocumentDate { get; set; }
        public decimal BranchM_UID { get; set; }
        public decimal TableM_UID { get; set; }
        public decimal ShiftM_UID { get; set; }
        public decimal EmployeeM_UID { get; set; }
        public decimal Captain_UID { get; set; }
    
        public virtual ShiftM ShiftM { get; set; }
        public virtual TableM TableM { get; set; }
    }
}