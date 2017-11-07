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
    
    public partial class AccountMItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AccountMItem()
        {
            this.AccountMItems_DiscountMapping = new HashSet<AccountMItems_DiscountMapping>();
            this.AccountMItems_RelationList = new HashSet<AccountMItems_RelationList>();
            this.AccountMItems_RelationList1 = new HashSet<AccountMItems_RelationList>();
            this.AccountMItemsMultiUOMs = new HashSet<AccountMItemsMultiUOM>();
            this.CommodityM_AccountMItems_Mapping = new HashSet<CommodityM_AccountMItems_Mapping>();
            this.EXP_AccountMItems = new HashSet<EXP_AccountMItems>();
            this.Promo_BOGO_Header = new HashSet<Promo_BOGO_Header>();
            this.SAPItemMappings = new HashSet<SAPItemMapping>();
        }
    
        public decimal AccountMItems_UID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string LongItemName { get; set; }
        public string AdditionalDetails { get; set; }
        public decimal UOM_UID { get; set; }
        public decimal Category_UID { get; set; }
        public decimal Group_UID { get; set; }
        public decimal SubGroup_UID { get; set; }
        public string SalesDescription { get; set; }
        public decimal Sales_UOM_UID { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }
        public decimal ShelfLife { get; set; }
        public bool InUse { get; set; }
        public int OrderBy { get; set; }
        public bool SpecialItems { get; set; }
        public decimal QOH { get; set; }
        public decimal ConversionFactor { get; set; }
        public decimal Sales_QOH { get; set; }
        public decimal AccountM_UID { get; set; }
        public decimal DineGroup_UID { get; set; }
        public bool IsSharedItem { get; set; }
        public string PurchaseDescription { get; set; }
        public decimal Purchase_UOM_UID { get; set; }
        public decimal PriceList_Sales_UID { get; set; }
        public decimal PriceList_Purchase_UID { get; set; }
        public decimal Default_Sales_AccountM_UID { get; set; }
        public decimal Default_WIP_AccountM_UID { get; set; }
        public decimal Default_Purchase_AccountM_UID { get; set; }
        public decimal Default_Consumption_AccountM_UID { get; set; }
        public decimal Default_Scrap_AccountM_UID { get; set; }
        public decimal StandardPrice { get; set; }
        public bool IsBatchManagement { get; set; }
        public bool IsPOMust { get; set; }
        public decimal ROL { get; set; }
        public decimal MOQ { get; set; }
        public decimal MaximumOrderQty { get; set; }
        public bool IsLocalItem { get; set; }
        public decimal PreprationTime { get; set; }
        public string ShortName { get; set; }
        public Nullable<bool> IsShelfItem { get; set; }
        public bool IsItemCodeGenAutomatic { get; set; }
        public bool IsDespatchNeeded { get; set; }
        public bool IsSoldWithOtherItems { get; set; }
    
        public virtual AccountM AccountM { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountMItems_DiscountMapping> AccountMItems_DiscountMapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountMItems_RelationList> AccountMItems_RelationList { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountMItems_RelationList> AccountMItems_RelationList1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AccountMItemsMultiUOM> AccountMItemsMultiUOMs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommodityM_AccountMItems_Mapping> CommodityM_AccountMItems_Mapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXP_AccountMItems> EXP_AccountMItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Promo_BOGO_Header> Promo_BOGO_Header { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SAPItemMapping> SAPItemMappings { get; set; }
    }
}