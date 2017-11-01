using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Data.Entity.Core.Objects;
namespace NibsMVC.Repository
{
    public class AdminSearchRepository
    {
        NIBSEntities entities = new NIBSEntities();
        public List<SelectListItem> GetlistOfOutlets()
        {
            var outletlist = new List<SelectListItem>();
            List<OutletModel> lst = (from p in entities.tblOutlets.Where(a => a.Active == true).ToList()
                                     select new OutletModel()
                                     {
                                         Name = p.Name,
                                         OutletId = p.OutletId
                                     }).ToList<OutletModel>();
            foreach (var item in lst)
            {
                outletlist.Add(
                    new SelectListItem
                    {
                        Value = item.OutletId.ToString(),
                        Text = item.Name
                    });
            }
            return outletlist;
        }
        public AdminBillReportModel getSearchData(AdminBillReportModel model,string type)
        {
           
            List<BillingModel> list = new List<BillingModel>();
            var result = (from p in entities.tblBillMasters
                          where p.BillId == model.BillNo
                          select p).ToList(); ;
            if (model.BillNo>0)
            {
                result = (from p in entities.tblBillMasters
                          where p.BillId == model.BillNo
                          && p.BillingType==type
                          select p).ToList();
            }
            else if (model.OutletId>0)
            {
                result = (from p in entities.tblBillMasters
                          where p.OutletId == model.OutletId
                          && p.BillingType == type
                          select p).ToList();
            }
            else
            {
                result = (from p in entities.tblBillMasters
                          where p.BillDate >= model.SearchFrom
                          && p.BillDate<=model.SearchTo
                          && p.BillingType == type
                          select p).ToList();
            }
            foreach (var item in result)
            {
                BillingModel m = new BillingModel();
                List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                m.BillId = item.BillId;
                m.BillDate = item.BillDate;
                m.TotalAmount = item.TotalAmount;
                m.VatAmount = item.VatAmount;
                m.ServicChargeAmt = item.ServicChargesAmount;
                m.DiscountAmount = item.DiscountAmount;
                m.NetAmount = item.NetAmount;
                m.TableNo = item.TableNo;
                if (item.TokenNo==null)
                {
                    m.TokenNo = 0;
                }
                else
                {
                    m.TokenNo = item.TokenNo.Value;
                }
                m.Outletid = item.OutletId;
                m.OutletName = (from p in entities.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                foreach (var i in entities.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                {
                    AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                    bill.Amount = i.Amount;
                    bill.BilldeatilId = i.BillDetailsId;
                    bill.FullQty = i.FullQty.Value;
                    bill.HalfQty = i.HalfQty.Value;
                    bill.ItemName = (from p in entities.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                    lstBill.Add(bill);
                }
                m.getBillItemDetails = lstBill;
                list.Add(m);

            }
            model.getAllBillReports = list;
            model.getAllListOfOutlet = GetlistOfOutlets();
            model.SearchFrom = model.SearchFrom;
            model.SearchTo = model.SearchTo;
            model.BillNo = model.BillNo;
            return model;
        }


        public AdminBillReportModel getOutletSearchData(AdminBillReportModel model, string type,int OutletId)
        {

            List<BillingModel> list = new List<BillingModel>();
            //var result = (from p in entities.tblBillMasters
            //              where p.BillId == model.BillNo
            //              && p.OutletId== OutletId
            //              select p).ToList();
            var result = entities.tblBillMasters.Where(a => a.BillId == model.BillNo && a.OutletId == OutletId).ToList();
            if (model.BillNo > 0)
            {
                //result = (from p in entities.tblBillMasters
                //          where p.BillId == model.BillNo
                //          && p.BillingType == type
                //          && p.OutletId == OutletId
                //          select p).ToList();
                result = entities.tblBillMasters.Where(a => a.BillId == model.BillNo && a.BillingType == type && a.OutletId == OutletId).ToList();
            }
            else if (model.OutletId > 0)
            {
                //result = (from p in entities.tblBillMasters
                //          where p.OutletId == model.OutletId
                //          && p.BillingType == type
                //          && p.OutletId == OutletId
                //          select p).ToList();
                result = entities.tblBillMasters.Where(a => a.OutletId == model.OutletId && a.BillingType == type && a.OutletId == OutletId).ToList();
            }
            else
            {
                //result = (from p in entities.tblBillMasters
                //          where p.BillDate.Date >= EntityFunctions.TruncateTime(model.SearchFrom)
                //          && p.BillDate.Date <= EntityFunctions.TruncateTime(model.SearchTo)
                //          && p.BillingType == type
                //          && p.OutletId == OutletId
                //          select p).ToList();
                result = entities.tblBillMasters.Where(a => EntityFunctions.TruncateTime(a.BillDate) >= model.SearchFrom.Date

                    && EntityFunctions.TruncateTime(a.BillDate) <= model.SearchTo.Date && a.BillingType == type && a.OutletId == OutletId).ToList();

            }
            foreach (var item in result)
            {
                BillingModel m = new BillingModel();
                List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                m.BillId = item.BillId;
                m.BillDate = item.BillDate;
                m.TotalAmount = Math.Round(item.TotalAmount,2);
                m.VatAmount = Math.Round(item.VatAmount,2);
                m.ServicChargeAmt = Math.Round(item.ServicChargesAmount,2);
                if (item.ServiceTax.HasValue)
                {
                    m.ServiceTax = item.ServiceTax.Value;
                }
                m.DiscountAmount = Math.Round(item.DiscountAmount,2);
                m.NetAmount = Math.Round(item.NetAmount,2);
                m.TableNo = item.TableNo;
                if (item.TokenNo==null)
                {
                    m.TokenNo =0;
                }
                else
                {
                    m.TokenNo = item.TokenNo.Value;
                }
                m.Outletid = item.OutletId;
                m.OutletName = (from p in entities.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                foreach (var i in entities.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                {
                    AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                    bill.Amount = Math.Round(i.Amount,2);
                    bill.BilldeatilId = i.BillDetailsId;
                    bill.FullQty = i.FullQty.Value;
                    bill.HalfQty = i.HalfQty.Value;
                    bill.ItemName = (from p in entities.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                    lstBill.Add(bill);
                }
                m.getBillItemDetails = lstBill;
                list.Add(m);

            }
            model.getAllBillReports = list;
            model.getAllListOfOutlet = GetlistOfOutlets();
            model.SearchFrom = model.SearchFrom;
            model.SearchTo = model.SearchTo;
            model.BillNo = model.BillNo;
            return model;
        }
    }
}