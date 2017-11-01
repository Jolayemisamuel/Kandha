using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class AdminHomeModel
    {
        public string TotalPurchage { get; set; }
        public string TotalSale { get; set; }
        public string TotalProfit { get; set; }
        public List<OutletListModel> getAllOutletList { get; set; }
        public List<GetAllVender> getAllVendorList { get; set; }
        public List<AllDeletedBillModel> getalldeletedbillReport { get; set; }
    }
    public class OutletListModel
    {
        public int outletId { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public bool Active { get; set; }
    }
    public class GetAllVender
    {
        public int VendrId { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public bool Active { get; set; }
    }
    public class AllDeletedBillModel
    {
        public string ItemName { get; set; }
        public string OutLetName { get; set; }
        public int BillNo { get; set; }
        public string DeleteDate { get; set; }
        public string BillDate { get; set; }
        public decimal NetAmount { get; set; }
        public string TableNo { get; set; }
        public int TokenNO { get; set; }
        public int FullQty { get; set; }
        public int HalfQty { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ItemAmount { get; set; }
        public string BillType { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
    }
    #region OutletReports model
    public class OutletHomeModel
    {
        public string TotalPurchage { get; set; }
        public string TotalSale { get; set; }
        public string TotalProfit { get; set; }
        public List<GetAllOutletVender> getAllVendorList { get; set; }
        public List<AssignMenuItemModel> getAllMenuList { get; set; }
        public List<GetRunningTable> getRunningTable { get; set; }
        public List<GetAllOfferListModel> gettodayoffers { get; set; }
        public bool AutoInventory { get; set; }
    }
    public class GetAllOutletVender
    {
        public int VendrId { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public bool Active { get; set; }
    }
    public class GetRunningTable
    {
        public int TableNo { get; set; }
    }
    public class AssignMenuItemModel
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public decimal HalfPrice { get; set; }
        public decimal FullPrice { get; set; }
        public string CategoryName { get; set; }
    }
    #endregion
    #region Cashier
    public class CashierHomeModel
    {

        public List<CashierRHomeModel> getRBillList { get; set; }
        public List<CashierTHomeModel> getTBillList { get; set; }
        public List<CashierHHomeModel> getHBillList { get; set; }
    }
    public class CashierRHomeModel
    {
        public int BillId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServicChargeAmt { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string TableNo { get; set; }
    }
    public class CashierTHomeModel
    {
        public int BillId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServicChargeAmt { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int TokenNo { get; set; }
    }
    public class CashierHHomeModel
    {
        public int BillId { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServicChargeAmt { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int TokenNo { get; set; }
    }
    #endregion

}