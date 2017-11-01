using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Web.Security;
using WebMatrix.WebData;
using System.Globalization;
namespace NibsMVC.Controllers
{
    public class DateController : Controller
    {
        //
        // GET: /Date/
        NIBSEntities entites = new NIBSEntities();
        public ActionResult Index()
        {
            TodayBillReportModel model = new TodayBillReportModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(TodayBillReportModel model1)
        {

            int OutletId = getOutletId();
            DateTime dd = Convert.ToDateTime(model1.Date);
            DateTime datefrom = Convert.ToDateTime(model1.DateFrom);
            DateTime dateto = Convert.ToDateTime(model1.DateTo);
            //DateTime dd = Convert.ToDateTime(model1.Date);
            //  var dd = Convert.ToDateTime(Date); 

            TodayBillReportModel model = new TodayBillReportModel();
            model.getAllRestroBill = getallRestroBill(datefrom, dateto);
            model.getAllRestroVat = getAllVatAmount(datefrom, dateto);
            var restroBill = entites.tblBillMasters.Where(a => System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) >= datefrom.Date
                && System.Data.Entity.DbFunctions.TruncateTime(a.BillDate) <= dateto.Date && a.OutletId == OutletId).ToList();

            var today = restroBill.Where(a => ((a.BillingType == "Ac Hall" || a.BillingType == "Dine In Hall"))).ToList();
            //var demo=entites.tblBillMasters.Where(a=>a.BillDate.Day==dd.Day)
            model.RestroTotalAmount = today.Sum(a => a.TotalAmount);
            model.RestroTotalDiscountAmount = today.Sum(a => a.DiscountAmount);
            model.RestroTotalServiceTaxAmount = today.Sum(a => a.ServiceTax.Value);
            model.RestroTotalServiceChargeAmount = today.Sum(a => a.ServicChargesAmount);
            model.RestroTotalVatAmount = today.Sum(a => a.VatAmount);
            model.TotalRestroNetAmount = today.Sum(a => a.NetAmount);

            // Home Delivery Today Report
            var Home = restroBill.Where(a => a.BillingType.Equals("Door Delivery Hall")).ToList();
            model.HomeTotalAmount = Home.Sum(a => a.TotalAmount);
            model.HomeTotalDiscountAmount = Home.Sum(a => a.DiscountAmount);
            model.HomeTotalServiceTaxAmount = Home.Sum(a => a.ServiceTax.Value);
            model.HomeTotalServiceChargeAmount = Home.Sum(a => a.ServicChargesAmount);
            model.HomeTotalVatAmount = Home.Sum(a => a.VatAmount);
            model.TotalHomeNetAmount = Home.Sum(a => a.NetAmount);
            model.getAllHomeBill = getallHomeBill(datefrom, dateto);
            model.getAllHomeVat = getAllVatHomeAmount(datefrom, dateto);

            // Takeaway Today Report
            var Take = restroBill.Where(a => a.BillingType.Equals("Take Away Hall")).ToList();
            model.TakeTotalAmount = Take.Sum(a => a.TotalAmount);
            model.TakeTotalDiscountAmount = Take.Sum(a => a.DiscountAmount);
            model.TakeTotalServiceTaxAmount = Take.Sum(a => a.ServiceTax.Value);
            model.TakeTotalServiceChargeAmount = Take.Sum(a => a.ServicChargesAmount);
            model.TakeTotalVatAmount = Take.Sum(a => a.VatAmount);
            model.TotalTakeNetAmount = Take.Sum(a => a.NetAmount);
            model.getAllTakeBill = getallTakeBill(datefrom, dateto);
            model.getAllTakeVat = getAllVatTakeAmount(datefrom, dateto);
            // Calculate Total

            model.TotalAmount = model.RestroTotalAmount + model.HomeTotalAmount + model.TakeTotalAmount;
            model.TotalServiceTax = model.RestroTotalServiceTaxAmount + model.HomeTotalServiceTaxAmount + model.TakeTotalServiceTaxAmount;
            model.TotalServiceCharge = model.RestroTotalServiceChargeAmount + model.HomeTotalServiceChargeAmount + model.TakeTotalServiceChargeAmount;
            model.TotalVatAmount = model.RestroTotalVatAmount + model.HomeTotalVatAmount + model.TakeTotalVatAmount;
            model.TotalDiscount = model.RestroTotalDiscountAmount + model.HomeTotalDiscountAmount + model.TakeTotalDiscountAmount;
            model.NetAmount = model.TotalRestroNetAmount + model.TotalHomeNetAmount + model.TotalTakeNetAmount;
            // total cash and card amount
            var totalcash = entites.tblBillMasters.Where(a => a.PaymentType.Equals("Cash") && a.BillDate.Day == dd.Day && a.BillDate.Month == dd.Month && a.BillDate.Year == dd.Year && a.OutletId == OutletId).ToList();
            if (totalcash.Count > 0)
            {
                model.TotalCashAmount = Math.Round(totalcash.Sum(a => a.NetAmount), 2);
            }
            var totalCard = entites.tblBillMasters.Where(a => a.PaymentType.Equals("Card") && a.BillDate.Day == dd.Day && a.BillDate.Month == dd.Month && a.BillDate.Year == dd.Year && a.OutletId == OutletId).ToList();
            if (totalCard.Count > 0)
            {
                model.TotalCardAmount = Math.Round(totalCard.Sum(a => a.NetAmount), 2);
            }
            model.DateFrom = datefrom;
            model.DateTo = dateto;
            return View(model);
        }
        #region Restro
        public List<TodayRestroBillModel> getallRestroBill(DateTime datefrom, DateTime dateto)
        {
            int OutletId = getOutletId();
            List<TodayRestroBillModel> lst = new List<TodayRestroBillModel>();
            var dd = dateto;
            var result = (from p in entites.tblBillDetails
                          where System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= datefrom.Date &&
                           System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= dateto.Date
                           &&  ((p.tblBillMaster.BillingType == "Ac Hall" || p.tblBillMaster.BillingType == "Dine In Hall"))  && p.tblBillMaster.OutletId == OutletId
                          select new
                          {
                              ItemName = p.tblItem.Name,
                              Quantity = p.FullQty,
                              Amount = p.Amount,
                              BillId = p.BillId,
                          }).ToList();
            foreach (var item in result.GroupBy(a => a.ItemName))
            {
                if (item.FirstOrDefault().Quantity>0)
                {
                    TodayRestroBillModel model = new TodayRestroBillModel();
                    decimal amount = item.Sum(a => a.Amount);
                    model.Amount = amount;
                    model.TotalAmount = item.Sum(a => a.Amount);
                    model.Name = item.FirstOrDefault().ItemName;
                    model.Price = Convert.ToDecimal(item.FirstOrDefault().Amount / item.FirstOrDefault().Quantity);
                    model.Qty = item.Sum(a => a.Quantity).Value;
                    lst.Add(model);
                }
                
            }

            return lst;
        }
        public List<TotalRestroVatModel> getAllVatAmount(DateTime datefrom, DateTime dateto)
        {
            int OutletId = getOutletId();
            List<TotalRestroVatModel> lst = new List<TotalRestroVatModel>();
            var dd = Convert.ToDateTime(dateto);
            var result = (from p in entites.tblBillDetails
                          where System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= datefrom.Date &&
                           System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= dateto.Date
                           && ((p.tblBillMaster.BillingType == "Ac Hall" || p.tblBillMaster.BillingType == "Dine In Hall")) && p.tblBillMaster.OutletId == OutletId
                          select new
                          {
                              Vat = p.Vat,
                              VatAmount = p.VatAmount,
                              Amount = p.Amount,
                              BillId = p.BillId,
                          }).ToList();

            foreach (var item in result.GroupBy(a => a.Vat))
            {
                TotalRestroVatModel model = new TotalRestroVatModel();
                model.Vat = item.Key.Value;
                decimal sum = item.Sum(a => a.VatAmount.Value);
                model.Amount = Math.Round(sum, 2);
                lst.Add(model);
            }
            return lst;
        }
        //public decimal GetDiscountWiseVat(IGrouping<VatDiscountWise> lst)
        //{
        //    decimal discountAmount = 0;
        //    return discountAmount;
        //}

        #endregion

        #region Home
        public List<TodayHomeBillModel> getallHomeBill(DateTime datefrom, DateTime dateto)
        {
            int OutletId = getOutletId();
            List<TodayHomeBillModel> lst = new List<TodayHomeBillModel>();
            var dd = Convert.ToDateTime(dateto);
            var result = (from p in entites.tblBillDetails
                          where System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= datefrom.Date &&
                           System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= dateto.Date
                           && p.tblBillMaster.BillingType.Equals("Door Delivery Hall") && p.tblBillMaster.OutletId == OutletId
                          select new
                          {
                              ItemName = p.tblItem.Name,
                              Quantity = p.FullQty,
                              Amount = p.Amount,
                              BillId = p.BillId,
                          }).ToList();

            foreach (var item in result.GroupBy(a => a.ItemName))
            {
                if (item.FirstOrDefault().Quantity>0)
                {
                    TodayHomeBillModel model = new TodayHomeBillModel();
                    decimal amount = item.Sum(a => a.Amount);
                    model.Amount = Math.Round(amount, 2);
                    model.TotalAmount = item.Sum(a => a.Amount);
                    model.Name = item.FirstOrDefault().ItemName;
                    model.Price = Convert.ToDecimal(item.FirstOrDefault().Amount / item.FirstOrDefault().Quantity);
                    model.Qty = item.Sum(a => a.Quantity).Value;
                    lst.Add(model);
                }
               
            }
            return lst;
        }
        public List<TotalHomeVatModel> getAllVatHomeAmount(DateTime datefrom, DateTime dateto)
        {
            int OutletId = getOutletId();
            List<TotalHomeVatModel> lst = new List<TotalHomeVatModel>();
            var dd = Convert.ToDateTime(dateto);
            var result = (from p in entites.tblBillDetails
                          where System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= datefrom.Date &&
                           System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= dateto.Date
                           && p.tblBillMaster.BillingType.Equals("Door Delivery Hall") && p.tblBillMaster.OutletId == OutletId
                          select new
                          {
                              Vat = p.Vat,
                              VatAmount = p.VatAmount,
                              Amount = p.Amount,
                              BillId = p.BillId,
                          }).ToList();
            foreach (var item in result.GroupBy(a => a.Vat))
            {
                TotalHomeVatModel model = new TotalHomeVatModel();
                model.Vat = item.Key.Value;
                decimal sum = item.Sum(a => a.VatAmount.Value);
                model.Amount = Math.Round(sum, 2);
                lst.Add(model);
            }
            return lst;
        }

        #endregion

        #region Take
        public List<TodayTakeBillModel> getallTakeBill(DateTime datefrom, DateTime dateto)
        {
            int OutletId = getOutletId();
            List<TodayTakeBillModel> lst = new List<TodayTakeBillModel>();
           
            var result = (from p in entites.tblBillDetails
                          where System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= datefrom.Date &&
                           System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= dateto.Date
                           && p.tblBillMaster.BillingType.Equals("Door Delivery Hall") && p.tblBillMaster.OutletId == OutletId
                          select new
                          {
                              ItemName = p.tblItem.Name,
                              Quantity = p.FullQty,
                              Amount = p.Amount,
                              BillId = p.BillId,
                          }).ToList();

            foreach (var item in result.GroupBy(a => a.ItemName))
            {
                if (item.FirstOrDefault().Quantity>0)
                {
                    TodayTakeBillModel model = new TodayTakeBillModel();
                    decimal amount = item.Sum(a => a.Amount);
                    model.Amount = Math.Round(amount, 2);
                    model.TotalAmount = item.Sum(a => a.Amount);
                    model.Name = item.FirstOrDefault().ItemName;
                    model.Price = Convert.ToDecimal(item.FirstOrDefault().Amount / item.FirstOrDefault().Quantity);
                    model.Qty = item.Sum(a => a.Quantity).Value;
                    lst.Add(model);
                }
                
            }
            return lst;
        }
        public List<TotalTakeVatModel> getAllVatTakeAmount(DateTime datefrom, DateTime dateto)
        {
            int OutletId = getOutletId();
            List<TotalTakeVatModel> lst = new List<TotalTakeVatModel>();
            var dd = Convert.ToDateTime(dateto);
            var result = (from p in entites.tblBillDetails
                          where System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= datefrom.Date &&
                           System.Data.Entity.DbFunctions.TruncateTime(p.tblBillMaster.BillDate) >= dateto.Date
                           && p.tblBillMaster.BillingType.Equals("Door Delivery Hall") && p.tblBillMaster.OutletId == OutletId
                          select new
                          {
                              Vat = p.Vat,
                              VatAmount = p.VatAmount,
                              Amount = p.Amount,
                              BillId = p.BillId,
                          }).ToList();
            foreach (var item in result.GroupBy(a => a.Vat))
            {
                TotalTakeVatModel model = new TotalTakeVatModel();
                model.Vat = item.Key.Value;
                decimal sum = item.Sum(a => a.VatAmount.Value);
                model.Amount = Math.Round(sum, 2);
                lst.Add(model);
            }
            return lst;
        }

        #endregion
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
                    oulte = Convert.ToInt32((from n in entites.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }


    }
}
