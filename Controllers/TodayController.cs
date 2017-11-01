using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Models;
using NibsMVC.EDMX;
using WebMatrix.WebData;
using System.Web.Security;
namespace NibsMVC.Controllers
{
    public class TodayController : Controller
    {
        //
        // GET: /Today/
        NIBSEntities entites = new NIBSEntities();
        public ActionResult Index()
        {
            int OutletId = getOutletId();
            DateTime dd = DateTime.Now.Date;
          //  var dd = Convert.ToDateTime(Date); 
            TodayBillReportModel model = new TodayBillReportModel();
            model.getAllRestroBill = getallRestroBill();
            model.getAllRestroVat = getAllVatAmount();
            var today=(from p in entites.tblBillMasters
                       where p.BillDate.Day==dd.Day && p.BillDate.Month==dd.Month
                       &&p.BillDate.Year==dd.Year && p.OutletId==OutletId
                       && (p.BillingType == "Ac Hall" || p.BillingType == "Dine In Hall")
                       select p).ToList();
            //var demo=entites.tblBillMasters.Where(a=>a.BillDate.Day==dd.Day)
            model.RestroTotalAmount = today.Sum(a=>a.TotalAmount);
            model.RestroTotalServiceChargeAmount = today.Sum(a => a.ServicChargesAmount);
            model.RestroTotalDiscountAmount = today.Sum(a => a.DiscountAmount);
            model.RestroTotalServiceTaxAmount = today.Sum(a => a.ServiceTax.Value);
            model.RestroTotalVatAmount = today.Sum(a => a.VatAmount);
            model.TotalRestroNetAmount = today.Sum(a => a.NetAmount);
            
            // Home Delivery Today Report
            var Home = entites.tblBillMasters.Where(a => a.BillDate.Day == dd.Day
                && a.BillDate.Month == dd.Month && a.BillDate.Year == dd.Year &&
                a.OutletId == OutletId && a.BillingType.Equals("Door Delivery Hall")).ToList();
            model.HomeTotalAmount = Home.Sum(a => a.TotalAmount);
            model.HomeTotalDiscountAmount = Home.Sum(a => a.DiscountAmount);
            model.HomeTotalServiceTaxAmount = Home.Sum(a => a.ServiceTax.Value);
            model.HomeTotalServiceChargeAmount = Home.Sum(a => a.ServicChargesAmount);
            model.HomeTotalVatAmount = Home.Sum(a => a.VatAmount);
            model.TotalHomeNetAmount = Home.Sum(a => a.NetAmount);
            model.getAllHomeBill = getallHomeBill();
            model.getAllHomeVat = getAllVatHomeAmount();

            // Takeaway Today Report
            var Take = entites.tblBillMasters.Where(a => a.BillDate.Day == dd.Day
               && a.BillDate.Month == dd.Month && a.BillDate.Year == dd.Year &&
               a.OutletId == OutletId && a.BillingType.Equals("Take Away Hall")).ToList();
            model.TakeTotalAmount = Take.Sum(a => a.TotalAmount);
            model.TakeTotalDiscountAmount = Take.Sum(a => a.DiscountAmount);
            model.TakeTotalServiceTaxAmount = Take.Sum(a => a.ServiceTax.Value);
            model.TakeTotalServiceChargeAmount = Take.Sum(a => a.ServicChargesAmount);
            model.TakeTotalVatAmount = Take.Sum(a => a.VatAmount);
            model.TotalTakeNetAmount = Take.Sum(a => a.NetAmount);
            model.getAllTakeBill = getallTakeBill();
            model.getAllTakeVat = getAllVatTakeAmount();
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
           
            return View(model);
        }
        #region Restro
        public List<TodayRestroBillModel> getallRestroBill()
        {
            int OutletId = getOutletId();
            List<TodayRestroBillModel> lst = new List<TodayRestroBillModel>();
            var Date = DateTime.Today.ToShortDateString();
            var dd = Convert.ToDateTime(Date) ;
            var result = (from p in entites.tblBillMasters
                          join q in entites.tblBillDetails on p.BillId equals q.BillId
                          join t in entites.tblItems on q.ItemId equals t.ItemId
                          join b in entites.tblBasePriceItems on t.ItemId equals b.ItemId
                          where p.BillDate.Day == dd.Day && p.BillDate.Month == dd.Month && p.BillDate.Year == dd.Year && (p.BillingType == "Ac Hall" || p.BillingType == "Dine In Hall") && p.OutletId == OutletId
                          select new
                          {
                              ItemName = t.Name,
                              Quantity = q.FullQty,
                              Price = b.FullPrice,
                              Amount = q.Amount,
                              BillId = p.BillId,
                          }).ToList();

            foreach (var item in result.GroupBy(a => a.ItemName))
            {
                TodayRestroBillModel model = new TodayRestroBillModel();
                decimal amount=item.Sum(a => a.Amount);
                model.Amount = Math.Round(amount, 2);
                model.TotalAmount = item.Sum(a => a.Amount);
                model.Name = item.FirstOrDefault().ItemName;
                model.Price = Math.Round(item.FirstOrDefault().Price, 2);
                model.Qty = item.Sum(a => a.Quantity).Value;
                lst.Add(model);
            }

            return lst;
        }
        public List<TotalRestroVatModel> getAllVatAmount()
        {
            int OutletId = getOutletId();
            List<TotalRestroVatModel> lst = new List<TotalRestroVatModel>();
            var Date = DateTime.Today.ToShortDateString();
            var dd = Convert.ToDateTime(Date);
            var result = (from p in entites.tblBillMasters
                          join q in entites.tblBillDetails on p.BillId equals q.BillId
                          where p.BillDate.Day == dd.Day && p.BillDate.Month == dd.Month && p.BillDate.Year == dd.Year && (p.BillingType == "Ac Hall" || p.BillingType == "Dine In Hall") && p.OutletId == OutletId
                          select new
                          {
                              Vat = q.Vat,
                              Amount = q.Amount,
                              ItemId = q.ItemId,
                              VatAmount = q.VatAmount,
                              Discount=p.Discount
                          }).ToList();
            
            foreach (var item in result.GroupBy(a => a.Vat))
            {
                TotalRestroVatModel model = new TotalRestroVatModel();
                model.Vat = item.Key.Value;
                decimal sum = item.Sum(a => a.VatAmount.Value);
                model.Amount = sum;
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
        public List<TodayHomeBillModel> getallHomeBill()
        {
            int OutletId = getOutletId();
            List<TodayHomeBillModel> lst = new List<TodayHomeBillModel>();
            var Date = DateTime.Today.ToShortDateString();
            var dd = Convert.ToDateTime(Date);
            var result = (from p in entites.tblBillMasters
                          join q in entites.tblBillDetails on p.BillId equals q.BillId
                          join t in entites.tblItems on q.ItemId equals t.ItemId
                          join b in entites.tblBasePriceItems on t.ItemId equals b.ItemId
                          where p.BillDate.Day == dd.Day && p.BillDate.Month == dd.Month && p.BillDate.Year == dd.Year && p.BillingType == "Door Delivery Hall" && p.OutletId == OutletId
                          select new
                          {
                              ItemName = t.Name,
                              Quantity = q.FullQty,
                              Price = b.FullPrice,
                              Amount = q.Amount,
                              BillId = p.BillId,
                          }).ToList();

            foreach (var item in result.GroupBy(a => a.ItemName))
            {
                TodayHomeBillModel model = new TodayHomeBillModel();
                decimal amount = item.Sum(a => a.Amount);
                model.Amount = Math.Round(amount, 2);
                model.TotalAmount = item.Sum(a => a.Amount);
                model.Name = item.FirstOrDefault().ItemName;
                model.Price = Math.Round(item.FirstOrDefault().Price, 2);
                model.Qty = item.Sum(a => a.Quantity).Value;
                lst.Add(model);
            }
            return lst;
        }
        public List<TotalHomeVatModel> getAllVatHomeAmount()
        {
            int OutletId = getOutletId();
            List<TotalHomeVatModel> lst = new List<TotalHomeVatModel>();
            var Date = DateTime.Today.ToShortDateString();
            var dd = Convert.ToDateTime(Date);
            var result = (from p in entites.tblBillMasters
                          join q in entites.tblBillDetails on p.BillId equals q.BillId
                          where p.BillDate.Day == dd.Day && p.BillDate.Month == dd.Month && p.BillDate.Year == dd.Year && p.BillingType == "Door Delivery Hall" && p.OutletId == OutletId
                          select new
                          {
                              Vat = q.Vat,
                              Amount = q.Amount,
                              ItemId = q.ItemId,
                              VatAmount = p.VatAmount,
                              Discount = p.Discount
                          }).ToList();
            foreach (var item in result.GroupBy(a => a.Vat))
            {
                TotalHomeVatModel model = new TotalHomeVatModel();
                model.Vat = item.Key.Value;
                decimal sum = item.Sum(a => a.VatAmount);
                model.Amount = Math.Round(sum, 2);
                lst.Add(model);
            }
            return lst;
        }
       
        #endregion

        #region Take
        public List<TodayTakeBillModel> getallTakeBill()
        {
            int OutletId = getOutletId();
            List<TodayTakeBillModel> lst = new List<TodayTakeBillModel>();
            var Date = DateTime.Today.ToShortDateString();
            var dd = Convert.ToDateTime(Date);
            var result = (from p in entites.tblBillMasters
                          join q in entites.tblBillDetails on p.BillId equals q.BillId
                          join t in entites.tblItems on q.ItemId equals t.ItemId
                          join b in entites.tblBasePriceItems on t.ItemId equals b.ItemId
                          where p.BillDate.Day == dd.Day && p.BillDate.Month == dd.Month && p.BillDate.Year == dd.Year && p.BillingType == "Take Away Hall" && p.OutletId == OutletId
                          select new
                          {
                              ItemName = t.Name,
                              Quantity = q.FullQty,
                              Price = b.FullPrice,
                              Amount = q.Amount,
                              BillId = p.BillId,
                          }).ToList();

            foreach (var item in result.GroupBy(a => a.ItemName))
            {
                TodayTakeBillModel model = new TodayTakeBillModel();
                decimal amount = item.Sum(a => a.Amount);
                model.Amount = Math.Round(amount, 2);
                model.TotalAmount = item.Sum(a => a.Amount);
                model.Name = item.FirstOrDefault().ItemName;
                model.Price = Math.Round(item.FirstOrDefault().Price, 2);
                model.Qty = item.Sum(a => a.Quantity).Value;
                lst.Add(model);
            }
            return lst;
        }
        public List<TotalTakeVatModel> getAllVatTakeAmount()
        {
            int OutletId = getOutletId();
            List<TotalTakeVatModel> lst = new List<TotalTakeVatModel>();
            var Date = DateTime.Today.ToShortDateString();
            var dd = Convert.ToDateTime(Date);
            var result = (from p in entites.tblBillMasters
                          join q in entites.tblBillDetails on p.BillId equals q.BillId
                          where p.BillDate.Day == dd.Day && p.BillDate.Month == dd.Month && p.BillDate.Year == dd.Year && p.BillingType == "Door Delivery Hall" && p.OutletId == OutletId
                          select new
                          {
                              Vat = q.Vat,
                              Amount = q.Amount,
                              ItemId = q.ItemId,
                              VatAmount = p.VatAmount,
                              Discount = p.Discount
                          }).ToList();
            foreach (var item in result.GroupBy(a => a.Vat))
            {
                TotalTakeVatModel model = new TotalTakeVatModel();
                model.Vat = item.Key.Value;
                decimal sum = item.Sum(a => a.VatAmount);
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
        public PartialViewResult _printXReading()
        {
             var Date = DateTime.Today.ToShortDateString();
            var dd = Convert.ToDateTime(Date); 
            int OutletId=getOutletId();
            X_ReadingReportModel model = new X_ReadingReportModel();
            var Result = (from p in entites.tblBillMasters

                          join q in entites.tblOutlets on p.OutletId equals q.OutletId
                          join r in entites.tblPurchaseMasters on p.OutletId equals r.OutletId

                          where p.BillDate.Day == dd.Day && p.BillDate.Month == dd.Month && p.BillDate.Year == dd.Year && p.OutletId == OutletId
                          select new
                          {
                              Address=q.Address,
                              billId=p.BillId,
                              TotalAmount=p.TotalAmount,
                              Discount=p.DiscountAmount,
                              Vat=p.VatAmount,
                              ServiceTax=p.ServicChargesAmount,
                              NetAmount=p.NetAmount,
                          }).ToList();
            model.BillStart = Result.First().billId;
            model.BillEnd = Result.Last().billId;
            return PartialView();
        }
    }
}
