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
    
    public partial class AccountMGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AccountMGroup()
        {
            this.AccTransLists = new HashSet<AccTransList>();
            this.AccTransListBranchLists = new HashSet<AccTransListBranchList>();
        }
    
        public decimal UID { get; set; }
        public decimal AccountM_UID { get; set; }
        public decimal UnderAccountM_UID { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public string PrintDescription { get; set; }
        public int AccountGroupType { get; set; }
        public int HLevel { get; set; }
        public bool IsMainHead { get; set; }
        public bool IsFixed { get; set; }
        public int Sortorder { get; set; }
    
        public virtual AccountM AccountM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccTransList> AccTransLists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccTransListBranchList> AccTransListBranchLists { get; set; }
    }
}