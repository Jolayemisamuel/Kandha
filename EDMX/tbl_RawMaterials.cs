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
    
    public partial class tbl_RawMaterials
    {
        public tbl_RawMaterials()
        {
            this.tbl_KitchenRawIndent = new HashSet<tbl_KitchenRawIndent>();
            this.tbl_KitchenStock = new HashSet<tbl_KitchenStock>();
            this.tblPurchasedItems = new HashSet<tblPurchasedItem>();
            this.tblPurchaseReturns = new HashSet<tblPurchaseReturn>();
            this.tblReturns = new HashSet<tblReturn>();
            this.tblTransfers = new HashSet<tblTransfer>();
            this.tblPurchaseOrderItems = new HashSet<tblPurchaseOrderItem>();
            this.tblOpStckRates = new HashSet<tblOpStckRate>();
            this.tblTransferReturnReports = new HashSet<tblTransferReturnReport>();
            this.tblGenBarcodes = new HashSet<tblGenBarcode>();
            this.tbl_SubItemRawIndent = new HashSet<tbl_SubItemRawIndent>();
            this.tblAsgnRawSubItemDets = new HashSet<tblAsgnRawSubItemDet>();
        }
    
        public int RawMaterialId { get; set; }
        public string Name { get; set; }
        public int rawcategoryId { get; set; }
        public string units { get; set; }
        public Nullable<decimal> reorder { get; set; }
        public string barcode { get; set; }
    
        public virtual ICollection<tbl_KitchenRawIndent> tbl_KitchenRawIndent { get; set; }
        public virtual ICollection<tbl_KitchenStock> tbl_KitchenStock { get; set; }
        public virtual ICollection<tblPurchasedItem> tblPurchasedItems { get; set; }
        public virtual ICollection<tblPurchaseReturn> tblPurchaseReturns { get; set; }
        public virtual ICollection<tblReturn> tblReturns { get; set; }
        public virtual ICollection<tblTransfer> tblTransfers { get; set; }
        public virtual RawCategory RawCategory { get; set; }
        public virtual ICollection<tblPurchaseOrderItem> tblPurchaseOrderItems { get; set; }
        public virtual ICollection<tblOpStckRate> tblOpStckRates { get; set; }
        public virtual tbl_RawMaterials tbl_RawMaterials1 { get; set; }
        public virtual tbl_RawMaterials tbl_RawMaterials2 { get; set; }
        public virtual tbl_RawMaterials tbl_RawMaterials11 { get; set; }
        public virtual tbl_RawMaterials tbl_RawMaterials3 { get; set; }
        public virtual tbl_RawMaterials tbl_RawMaterials12 { get; set; }
        public virtual tbl_RawMaterials tbl_RawMaterials4 { get; set; }
        public virtual ICollection<tblTransferReturnReport> tblTransferReturnReports { get; set; }
        public virtual ICollection<tblGenBarcode> tblGenBarcodes { get; set; }
        public virtual ICollection<tbl_SubItemRawIndent> tbl_SubItemRawIndent { get; set; }
        public virtual ICollection<tblAsgnRawSubItemDet> tblAsgnRawSubItemDets { get; set; }
    }
}
