using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using NibsMVC.IRepository;
using System.Globalization;
using WebMatrix.WebData;
using NibsMVC.Controllers;
using System.Xml.Linq;
using System.Web.Security;
namespace NibsMVC.Repository
{
    public class OutletHomeRepository:IOutletHomeRepository
    {
        NIBSEntities entities = new NIBSEntities();
        AdminOfferRepository offer = new AdminOfferRepository();
        #region OutletHome
        public OutletHomeModel getOutletHomeReports()
        {
            OutletHomeModel model = new OutletHomeModel();
            var totalpurchage = entities.tblPurchaseMasters.ToList();
            decimal TotalPurchage = 0;
            if (totalpurchage.Count > 0)
            {
                TotalPurchage = totalpurchage.Sum(a => a.NetAmount);
            }
            var totalsale = entities.tblBillMasters.ToList();
            decimal TotalSale = 0;
            if (totalsale.Count > 0)
            {
                TotalSale = totalsale.Sum(a => a.NetAmount);
            }

            int TotalProfit = Convert.ToInt32(TotalSale) - Convert.ToInt32(TotalPurchage);
            model.TotalPurchage = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalPurchage);
            model.TotalSale = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalSale);
            model.TotalProfit = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalProfit);
            //int TotalPurchage = Convert.ToInt32(entities.tblPurchaseMasters.Where(a=>a.OutletId==WebSecurity.CurrentUserId).Sum(x => x.NetAmount));
            //int TotalSale = Convert.ToInt32(entities.tblBillMasters.Where(a => a.OutletId == WebSecurity.CurrentUserId).Sum(a => a.NetAmount));
            //int TotalProfit = TotalSale - TotalPurchage;
            //model.TotalPurchage = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalPurchage);
            //model.TotalSale = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalSale);
            //model.TotalProfit = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalProfit);
            model.getAllMenuList = getAllMenuItem();
            model.getAllVendorList = getAllVendorList();
            model.gettodayoffers = List();
            int OutletId = getOutletId();
            bool data = entities.AutoInventories.Where(a => a.OutletId == OutletId).Select(a => a.IsInventory.Value).FirstOrDefault();
           // model.getRunningTable = getRunningTable();
            model.AutoInventory = data;
            return model;
        }
        public List<GetAllOfferListModel> List()
        {
            AssignOfferInterface assign = new AssignOfferInterface();
            List<GetAllOfferListModel> list = new List<GetAllOfferListModel>();
            var CurrentDay = DateTime.Now.DayOfWeek.ToString();
            var dayoffer = entities.Nibs_Offer_Days.Where(a => a.Days.Contains(CurrentDay)).Select(a => a.OfferId).ToList();
           // XMLTablesRepository obj = new XMLTablesRepository();
            int OutletId = getOutletId();
            var result = (from p in entities.Nibs_Offer
                          join q in entities.NIbs_AssignOffer
                              on p.OfferId equals q.OfferId
                          where q.UserId == OutletId
                         && dayoffer.Contains(p.OfferId)
                          select new
                          {
                              OfferId = p.OfferId,
                              OfferType = p.OfferType
                          }).ToList();
            foreach (var item in result)
            {
                GetAllOfferListModel model = new GetAllOfferListModel();
                var OfferName = assign.OfferName(item.OfferType);
                if (OfferName == "Combo Offer")
                {
                    model.ComboOfferList = offer.GetComboOffer(item.OfferId);
                }
                else if (OfferName == "Amount Basis")
                {
                    model.BaseAmountList = offer.GetAmountBasis(item.OfferId);
                }
                else if (OfferName == "Happy Hours")
                {
                    model.HappyHoursList = offer.GetHappyHours(item.OfferId);
                }
                else
                {
                    model.BuyOfferList = offer.GetBuyOffer(item.OfferId, OfferName);
                }
                list.Add(model);
            }
            return list;
        }
        public List<GetAllOutletVender> getAllVendorList()
        {
            List<GetAllOutletVender> List = new List<GetAllOutletVender>();
            foreach (var item in entities.tblVendors.Where(x => x.OutletId == WebSecurity.CurrentUserId).Take(20).ToList())
            {
                GetAllOutletVender model = new GetAllOutletVender();
                model.Active = item.Active;
                model.Date = item.RegistrationDate.ToShortDateString();
                model.Name = item.Name;
                model.VendrId = item.VendorId;
                List.Add(model);
            }
            return List;
        }
        public List<AssignMenuItemModel> getAllMenuItem()
        {
            List<AssignMenuItemModel> List = new List<AssignMenuItemModel>();
            foreach (var item in entities.tblMenuOutlets.Where(a => a.OutletId == WebSecurity.CurrentUserId).ToList())
            {
                AssignMenuItemModel model = new AssignMenuItemModel();
                model.ItemId = item.ItemId;
                model.Name = item.tblItem.Name;
                model.FullPrice = item.FullPrice;
                model.HalfPrice = item.HalfPrice;
                model.CategoryName = item.tblCategory.Name;
                List.Add(model);
            }
            return List;
        }
        #endregion


        #region Cashier
        public CashierHomeModel getAllBillData()
        {
            CashierHomeModel model = new CashierHomeModel();
            model.getRBillList = getRBillingData();
            model.getTBillList = getTBillingData();
            model.getHBillList = getHBillingData();
            return model;
        }
        public List<CashierRHomeModel> getRBillingData()
        {
            List<CashierRHomeModel> List = new List<CashierRHomeModel>();
            var outltId = 99; //(from q in entities.tblOperators where q.UserId == WebSecurity.CurrentUserId select q.OutletId).FirstOrDefault();
            var billdata = (from p in entities.tblBillMasters where p.OutletId == outltId && (p.BillingType == "Ac Hall" || p.BillingType == "Dine In Hall")  select p).ToList();
            foreach (var item in billdata)
            {
                CashierRHomeModel model = new CashierRHomeModel();
                model.BillDate = item.BillDate;
                model.BillId = item.BillId;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.TableNo = item.TableNo;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                List.Add(model);
            }
            return List;
        }
        public List<CashierTHomeModel> getTBillingData()
        {
            List<CashierTHomeModel> List = new List<CashierTHomeModel>();
            var outltId = 99;//(from q in entities.tblOperators where q.UserId == WebSecurity.CurrentUserId select q.OutletId).FirstOrDefault();
            var billdata = (from p in entities.tblBillMasters where p.OutletId == outltId && p.BillingType == "Take Away Hall" select p).ToList();
            foreach (var item in billdata)
            {
                CashierTHomeModel model = new CashierTHomeModel();
                model.BillDate = item.BillDate;
                model.BillId = item.BillId;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                if (item.TokenNo.HasValue)
                {
                    model.TokenNo = item.TokenNo.Value;
                }
                else
                {
                    model.TokenNo = (int)item.TokenNo;//item.TableNo;
                }
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                List.Add(model);
            }
            return List;
        }
        public List<CashierHHomeModel> getHBillingData()
        {
            List<CashierHHomeModel> List = new List<CashierHHomeModel>();
            var outltId = 99;// (from q in entities.tblOperators where q.UserId == WebSecurity.CurrentUserId select q.OutletId).FirstOrDefault();
            var billdata = (from p in entities.tblBillMasters where p.OutletId == outltId && p.BillingType == "Door Delivery Hall" select p).ToList();
            foreach (var item in billdata)
            {
                CashierHHomeModel model = new CashierHHomeModel();
                model.BillDate = item.BillDate;
                model.BillId = item.BillId;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                if (item.TokenNo.HasValue)
                {
                    model.TokenNo = item.TokenNo.Value;
                }
                else
                {
                    model.TokenNo = (int)item.TokenNo ;//item.TableNo;
                }
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                List.Add(model);
            }
            return List;
        }
        #endregion

        public ChangeInventoryModel getinventory()
        {
            ChangeInventoryModel model = new ChangeInventoryModel();
            int OutletId = getOutletId();
            var data = entities.AutoInventories.Where(a => a.OutletId == OutletId).FirstOrDefault();
            if (data!=null)
            {
                model.IsInventory = data.IsInventory.Value;
                model.OutletId = OutletId;
            }
            return model;
        }
        public void UpdateInventory(ChangeInventoryModel model)
        {
            AutoInventory auto = new AutoInventory();
            auto = entities.AutoInventories.Where(a => a.OutletId == model.OutletId).FirstOrDefault();
            if (auto != null)
            {
                auto.IsInventory = model.IsInventory;
                entities.SaveChanges();
            }
        }
        public int getOutletId()
        {
            string[] roles = Roles.GetRolesForUser();
            var oulte = 0;
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    oulte = WebSecurity.CurrentUserId;
                }
                else
                {
                    oulte = Convert.ToInt32((from n in entities.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }

    }
}