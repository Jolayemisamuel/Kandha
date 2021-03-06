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
    
    public partial class RoomAllocationTran
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RoomAllocationTran()
        {
            this.RoomAllocationAdvances = new HashSet<RoomAllocationAdvance>();
            this.RoomAllocationLists = new HashSet<RoomAllocationList>();
            this.RoomBillTrans = new HashSet<RoomBillTran>();
        }
    
        public decimal UID { get; set; }
        public decimal Branch_UID { get; set; }
        public decimal GuestsM_UID { get; set; }
        public string GuestName { get; set; }
        public System.DateTime CheckInDateTime { get; set; }
        public System.DateTime CheckOutDateTime { get; set; }
        public int StatusM_UID { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int NumberOfRooms { get; set; }
        public int NumberOfPersons { get; set; }
        public Nullable<decimal> RoomReservationTrans_UID { get; set; }
        public decimal Incharge_UID { get; set; }
    
        public virtual GuestM GuestM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomAllocationAdvance> RoomAllocationAdvances { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomAllocationList> RoomAllocationLists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomBillTran> RoomBillTrans { get; set; }
    }
}
