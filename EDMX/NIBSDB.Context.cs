﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;


    public partial class NIBSEntities : DbContext
    {
        public NIBSEntities()
            : base("name=NIBSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<NIbs_AssignOffer> NIbs_AssignOffer { get; set; }
        public DbSet<NIbs_ComboOffer> NIbs_ComboOffer { get; set; }
        public DbSet<Nibs_HappyHours_Date> Nibs_HappyHours_Date { get; set; }
        public DbSet<Nibs_HappyHours_Day> Nibs_HappyHours_Day { get; set; }
        public DbSet<Nibs_HappyHours_Days> Nibs_HappyHours_Days { get; set; }
        public DbSet<Nibs_HappyHoursDates> Nibs_HappyHoursDates { get; set; }
        public DbSet<Nibs_Offer> Nibs_Offer { get; set; }
        public DbSet<Nibs_Offer_Amount> Nibs_Offer_Amount { get; set; }
        public DbSet<Nibs_Offer_Buy_Items> Nibs_Offer_Buy_Items { get; set; }
        public DbSet<Nibs_Offer_Days> Nibs_Offer_Days { get; set; }
        public DbSet<Nibs_Offer_Free_Items> Nibs_Offer_Free_Items { get; set; }
        public DbSet<OpenFood> OpenFoods { get; set; }
        public DbSet<tbl_KitchenRawIndent> tbl_KitchenRawIndent { get; set; }
        public DbSet<tbl_KitchenStock> tbl_KitchenStock { get; set; }
        public DbSet<tbl_OutletReturnItem> tbl_OutletReturnItem { get; set; }
        public DbSet<tbl_RawMaterials> tbl_RawMaterials { get; set; }
        public DbSet<tbl_ServiceTax> tbl_ServiceTax { get; set; }
        public DbSet<tblBasePriceItem> tblBasePriceItems { get; set; }
        public DbSet<tblCategory> tblCategories { get; set; }
        public DbSet<tblDeleteBillMaster> tblDeleteBillMasters { get; set; }
        public DbSet<tblDeletedetail> tblDeletedetails { get; set; }
        public DbSet<tblItem> tblItems { get; set; }
        public DbSet<tblMenuOutlet> tblMenuOutlets { get; set; }
        public DbSet<tblOperator> tblOperators { get; set; }
        public DbSet<tblOutlet> tblOutlets { get; set; }
        public DbSet<tblPurchasedItem> tblPurchasedItems { get; set; }
        public DbSet<tblPurchaseReturn> tblPurchaseReturns { get; set; }
        public DbSet<tblReturn> tblReturns { get; set; }
        public DbSet<tblServiceCharge> tblServiceCharges { get; set; }
        public DbSet<tblTableMaster> tblTableMasters { get; set; }
        public DbSet<tblTransfer> tblTransfers { get; set; }
        public DbSet<tblVendor> tblVendors { get; set; }
        public DbSet<AutoInventory> AutoInventories { get; set; }
        public DbSet<tblBillDetail> tblBillDetails { get; set; }
        public DbSet<tblBillMaster> tblBillMasters { get; set; }
        public DbSet<VendorPrice> VendorPrices { get; set; }
        public DbSet<Temp_VendorBilling> Temp_VendorBilling { get; set; }
        public DbSet<VendorBillingItem> VendorBillingItems { get; set; }
        public DbSet<VendorBillingMaster> VendorBillingMasters { get; set; }
        public DbSet<SelesVendorAmount> SelesVendorAmounts { get; set; }
        public DbSet<SelesVendorAmountDetail> SelesVendorAmountDetails { get; set; }
        public DbSet<OutletVendor> OutletVendors { get; set; }
        public DbSet<RawCategory> RawCategories { get; set; }
        public DbSet<tbl_Department> tbl_Department { get; set; }
        public DbSet<tblPurchaseOrderMaster> tblPurchaseOrderMasters { get; set; }
        public DbSet<tblPurchaseOrderItem> tblPurchaseOrderItems { get; set; }
        public DbSet<tblOpStckRate> tblOpStckRates { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<AddCategoryVendor> AddCategoryVendors { get; set; }
        public DbSet<tblPurchaseMaster> tblPurchaseMasters { get; set; }
        public DbSet<tblTransferReturnReport> tblTransferReturnReports { get; set; }
        public DbSet<tblGenBarcode> tblGenBarcodes { get; set; }
        public DbSet<tblSubItem> tblSubItems { get; set; }
        public DbSet<tbl_SubItemRawIndent> tbl_SubItemRawIndent { get; set; }
        public DbSet<tblAsgnRawSubItemDet> tblAsgnRawSubItemDets { get; set; }
        public DbSet<tblAssignRawSubItemMst> tblAssignRawSubItemMsts { get; set; }
        public DbSet<tblAssignSubMenuItem> tblAssignSubMenuItems { get; set; }
        public DbSet<tblLedgerCategory> tblLedgerCategories { get; set; }
        public DbSet<tblLedgerGroup> tblLedgerGroups { get; set; }
        public DbSet<tblTransByStock> tblTransByStocks { get; set; }
        public DbSet<tblTransRetRateDet> tblTransRetRateDets { get; set; }
        public DbSet<tblConsumption> tblConsumptions { get; set; }
        public DbSet<vwStockTransaction> vwStockTransactions { get; set; }
        public DbSet<vwStockTransactionsKitchen> vwStockTransactionsKitchens { get; set; }
        public DbSet<tblLedgerMaster> tblLedgerMasters { get; set; }
        public DbSet<Voucher_Entry_Credit> Voucher_Entry_Credit { get; set; }
        public DbSet<Voucher_Entry_Debit> Voucher_Entry_Debit { get; set; }
        public DbSet<tblGRNStock> tblGRNStocks { get; set; }
        public DbSet<vwItemIndentDetail> vwItemIndentDetails { get; set; }
        public DbSet<vwSubItemIndentDetail> vwSubItemIndentDetails { get; set; }
    
        public virtual int BillReportGenerate(string orderType, string paymentType, Nullable<System.DateTime> datefrom, Nullable<System.DateTime> dateto, Nullable<int> billNo, string newsql)
        {
            var orderTypeParameter = orderType != null ?
                new ObjectParameter("OrderType", orderType) :
                new ObjectParameter("OrderType", typeof(string));
    
            var paymentTypeParameter = paymentType != null ?
                new ObjectParameter("PaymentType", paymentType) :
                new ObjectParameter("PaymentType", typeof(string));
    
            var datefromParameter = datefrom.HasValue ?
                new ObjectParameter("datefrom", datefrom) :
                new ObjectParameter("datefrom", typeof(System.DateTime));
    
            var datetoParameter = dateto.HasValue ?
                new ObjectParameter("dateto", dateto) :
                new ObjectParameter("dateto", typeof(System.DateTime));
    
            var billNoParameter = billNo.HasValue ?
                new ObjectParameter("BillNo", billNo) :
                new ObjectParameter("BillNo", typeof(int));
    
            var newsqlParameter = newsql != null ?
                new ObjectParameter("newsql", newsql) :
                new ObjectParameter("newsql", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BillReportGenerate", orderTypeParameter, paymentTypeParameter, datefromParameter, datetoParameter, billNoParameter, newsqlParameter);
        }
    
        public virtual ObjectResult<MovementAnalysisKitchen_Result> MovementAnalysisKitchen(string frDate, string toDate, Nullable<int> rawMaterialId)
        {
            var frDateParameter = frDate != null ?
                new ObjectParameter("frDate", frDate) :
                new ObjectParameter("frDate", typeof(string));
    
            var toDateParameter = toDate != null ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(string));
    
            var rawMaterialIdParameter = rawMaterialId.HasValue ?
                new ObjectParameter("rawMaterialId", rawMaterialId) :
                new ObjectParameter("rawMaterialId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<MovementAnalysisKitchen_Result>("MovementAnalysisKitchen", frDateParameter, toDateParameter, rawMaterialIdParameter);
        }
    
        public virtual ObjectResult<MovementAnalysisStore_Result> MovementAnalysisStore(string frDate, string toDate, Nullable<int> rawMaterialId)
        {
            var frDateParameter = frDate != null ?
                new ObjectParameter("frDate", frDate) :
                new ObjectParameter("frDate", typeof(string));
    
            var toDateParameter = toDate != null ?
                new ObjectParameter("toDate", toDate) :
                new ObjectParameter("toDate", typeof(string));
    
            var rawMaterialIdParameter = rawMaterialId.HasValue ?
                new ObjectParameter("rawMaterialId", rawMaterialId) :
                new ObjectParameter("rawMaterialId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<MovementAnalysisStore_Result>("MovementAnalysisStore", frDateParameter, toDateParameter, rawMaterialIdParameter);
        }
    
        public virtual int StockReturn(Nullable<int> transferid, Nullable<int> rawmaterialid, Nullable<decimal> item)
        {
            var transferidParameter = transferid.HasValue ?
                new ObjectParameter("transferid", transferid) :
                new ObjectParameter("transferid", typeof(int));
    
            var rawmaterialidParameter = rawmaterialid.HasValue ?
                new ObjectParameter("rawmaterialid", rawmaterialid) :
                new ObjectParameter("rawmaterialid", typeof(int));
    
            var itemParameter = item.HasValue ?
                new ObjectParameter("item", item) :
                new ObjectParameter("item", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("StockReturn", transferidParameter, rawmaterialidParameter, itemParameter);
        }
    
        public virtual ObjectResult<ItemIndentDetails_Result> ItemIndentDetails(Nullable<int> menuCategoryID, Nullable<int> itemId, Nullable<int> rawCatId, Nullable<int> rawMaterialId)
        {
            var menuCategoryIDParameter = menuCategoryID.HasValue ?
                new ObjectParameter("MenuCategoryID", menuCategoryID) :
                new ObjectParameter("MenuCategoryID", typeof(int));
    
            var itemIdParameter = itemId.HasValue ?
                new ObjectParameter("ItemId", itemId) :
                new ObjectParameter("ItemId", typeof(int));
    
            var rawCatIdParameter = rawCatId.HasValue ?
                new ObjectParameter("RawCatId", rawCatId) :
                new ObjectParameter("RawCatId", typeof(int));
    
            var rawMaterialIdParameter = rawMaterialId.HasValue ?
                new ObjectParameter("RawMaterialId", rawMaterialId) :
                new ObjectParameter("RawMaterialId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<ItemIndentDetails_Result>("ItemIndentDetails", menuCategoryIDParameter, itemIdParameter, rawCatIdParameter, rawMaterialIdParameter);
        }
    
        public virtual ObjectResult<SubItemIndentDetails_Result> SubItemIndentDetails(Nullable<int> menuCategoryID, Nullable<int> itemId, Nullable<int> subItemId, Nullable<int> rawCatId, Nullable<int> rawMaterialId)
        {
            var menuCategoryIDParameter = menuCategoryID.HasValue ?
                new ObjectParameter("MenuCategoryID", menuCategoryID) :
                new ObjectParameter("MenuCategoryID", typeof(int));
    
            var itemIdParameter = itemId.HasValue ?
                new ObjectParameter("ItemId", itemId) :
                new ObjectParameter("ItemId", typeof(int));
    
            var subItemIdParameter = subItemId.HasValue ?
                new ObjectParameter("subItemId", subItemId) :
                new ObjectParameter("subItemId", typeof(int));
    
            var rawCatIdParameter = rawCatId.HasValue ?
                new ObjectParameter("RawCatId", rawCatId) :
                new ObjectParameter("RawCatId", typeof(int));
    
            var rawMaterialIdParameter = rawMaterialId.HasValue ?
                new ObjectParameter("RawMaterialId", rawMaterialId) :
                new ObjectParameter("RawMaterialId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SubItemIndentDetails_Result>("SubItemIndentDetails", menuCategoryIDParameter, itemIdParameter, subItemIdParameter, rawCatIdParameter, rawMaterialIdParameter);
        }
    }
}
