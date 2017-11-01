using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.IRepository;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Globalization;
namespace NibsMVC.Repository
{
    public class AdminHomeRepository:IAdminHomeRepository
    {
        NIBSEntities entities = new NIBSEntities();
        public AdminHomeModel getAdminHomeReports()
        {
            AdminHomeModel model = new AdminHomeModel();
            var totalpurchage = entities.tblPurchaseMasters.ToList();
            decimal TotalPurchage=0;
            if(totalpurchage.Count>0)
            {
                TotalPurchage = totalpurchage.Sum(a => a.NetAmount);
            }
            var totalsale = entities.tblBillMasters.ToList();
            decimal TotalSale=0;
            if (totalsale.Count>0)
            {
                TotalSale=totalsale.Sum(a=>a.NetAmount);
            }

            int TotalProfit = Convert.ToInt32(TotalSale) - Convert.ToInt32(TotalPurchage);
            model.TotalPurchage = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalPurchage);
            model.TotalSale = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalSale);
            model.TotalProfit = string.Format(CultureInfo.InvariantCulture, "{0:N0}", TotalProfit);
            model.getAllOutletList = GetAllOulletList();
            model.getAllVendorList = getAllVendorList();
            model.getalldeletedbillReport = getalldeletedbillReport();
            return model;
        }
        public List<OutletListModel> GetAllOulletList()
        {
            List<OutletListModel> list = new List<OutletListModel>();
            foreach (var item in entities.tblOutlets.Take(20).ToList())
            {
                OutletListModel model = new OutletListModel();
                model.outletId = item.OutletId;
                model.Name = item.Name;
                model.Date = item.RegistrationDate.ToShortDateString();
                model.Active = item.Active;
                list.Add(model);
            }
            return list;
        }
        public List<GetAllVender> getAllVendorList()
        {
            List<GetAllVender> List = new List<GetAllVender>();
            foreach (var item in entities.tblVendors.Take(20).ToList())
            {
                GetAllVender model = new GetAllVender();
                model.Active = item.Active;
                model.Date = item.RegistrationDate.ToShortDateString();
                model.Name = item.Name;
                model.VendrId = item.VendorId;
                List.Add(model);
            }
            return List;
        }
        public List<AllDeletedBillModel> getalldeletedbillReport()
        {
            List<AllDeletedBillModel> List = new List<AllDeletedBillModel>();
            var Record = (from p in entities.tblDeleteBillMasters
                         join q in entities.tblDeletedetails
                         on p.DeleteId equals q.DeleteId
                         select new
                         {
                             ItemName=q.tblItem.Name,
                             OutLetName=p.tblOutlet.Name,
                             BillNo=p.BillNo,
                             DeleteDate=p.DeleteDate,
                             BillDate=p.BillDate,
                             NetAmount=p.NetAmount,
                             TableNo=p.TableNo,
                             TokenNO=p.TokenNo,
                             FullQty=q.FullQty,
                             HalfQty=q.HalfQty,
                             VatAmount=p.VatAmount,
                             ServiceCharge=p.ServiceCharAmt,
                             Discount=p.DiscountAmount,
                             TotalAmount=p.TotalAmount,
                             ItemAmount=q.Amount,
                             BillType=p.BillingType,
                             CustomerName=p.CustomerName,
                             Address=p.Address
                         }).ToList();
            foreach (var item in Record)
            {
                AllDeletedBillModel model = new AllDeletedBillModel();
                model.BillDate=item.BillDate.ToShortDateString();
                model.BillNo = item.BillNo;
                model.DeleteDate = item.DeleteDate.ToString();
                model.Discount = item.Discount;
                model.FullQty = item.FullQty.Value;
                model.HalfQty=item.HalfQty.Value;
                model.ItemAmount = item.ItemAmount;
                model.ItemName = item.ItemName;
                model.NetAmount = item.NetAmount;
                model.OutLetName = item.OutLetName;
                model.ServiceCharge = item.ServiceCharge;
                model.TableNo=item.TableNo;
                if (item.TokenNO==null)
                {
                    model.TokenNO =0;
                }
                else
                {
                    model.TokenNO = item.TokenNO.Value;
                }
               
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.BillType = item.BillType;
                model.CustomerName = item.CustomerName;
                model.Address = item.Address;
                List.Add(model);
                    
            }
            return List;
        }
       
    }
}