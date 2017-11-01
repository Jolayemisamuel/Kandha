using NibsMVC.EDMX;
using NibsMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using NibsMVC.Repository;
namespace NibsMVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        NIBSEntities db = new NIBSEntities();
        AdminSearchRepository search = new AdminSearchRepository();
        public ActionResult Index()
        {
            return View();
        }

        //------------------All vendero of all outlet---------------------//
        public ActionResult VenderReport()
        {
            var data = (from p in db.tblVendors where p.Active == true select p).ToList();
            List<VenderRegistrationModel> model = new List<VenderRegistrationModel>();
            foreach (var item in data)
            {
                VenderRegistrationModel v = new VenderRegistrationModel();
                v.Address = item.Address;
                v.ContactA = item.ContactA;
                v.ContactB = item.ContactB;
                v.Email = item.Email;
                v.Name = item.Name;
                v.OutletId = item.OutletId;
                v.RegistrationDate = item.RegistrationDate;
                v.TinNo = item.TinNo;
                v.VendorId = item.VendorId;
                v.OutletId = item.OutletId;
                model.Add(v);

            }
            return View(model);
        }

        //-------------------------------All Menu assign data--------//
        public ActionResult MenuReport()
        {
            var assignmenudata = (from p in db.tblMenuOutlets select p).ToList();
            List<MenuModel> list = new List<MenuModel>();
            foreach (var item in assignmenudata)
            {
                MenuModel model = new MenuModel();
                model.CategoryId = item.CategoryId;
                model.OutletId = item.OutletId;
                model.ItemId = item.ItemId;
                model.FullPrice = item.FullPrice;
                model.HalfPrice = item.HalfPrice;
                model.MenuOutletId = item.MenuOutletId;
                list.Add(model);
            }
            return View(list);
        }

        public ActionResult Delete(int id = 0)
        {
            try
            {
                var delete = (from p in db.tblMenuOutlets where p.MenuOutletId.Equals(id) select p).SingleOrDefault();
                db.tblMenuOutlets.Remove(delete);
                db.SaveChanges();
                TempData["aerror"] = "Delete Successfully !";
            }
            catch (Exception ex)
            {
                TempData["aerror"] = ex.Message;
            }
            return RedirectToAction("MenuReport", "Admin");

        }

        public ActionResult DeleteAllMenu(int id = 0)
        {
            try
            {
                var deleteall = (from p in db.tblMenuOutlets where p.OutletId==id select p).ToList();
                foreach (var item in deleteall)
                {
                    db.tblMenuOutlets.Remove(item);
                    db.SaveChanges(); 
                }
               
                TempData["aerror"] = "Delete Successfully !";
            }
            catch (Exception ex)
            {
                TempData["aerror"] = ex.Message;
            }
            return RedirectToAction("MenuReport", "Admin");

        }

        public ActionResult Edit(int id = 0)
        {
            try
            {

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("MenuReport", "Admin");
        }

        //---------------------RestroBilling-------------//
        public ActionResult RestroBill()
        {
            AdminBillReportModel m = new AdminBillReportModel();
            var billdata = (from p in db.tblBillMasters where (p.BillingType == "Ac Hall" || p.BillingType == "Dine In Hall") && p.OutletId !=0 && p.BillDate == DateTime.Today select p).ToList();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in billdata)
            {
                BillingModel model = new BillingModel();
                List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                model.BillId = item.BillId;
                model.BillDate = item.BillDate;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.TableNo = item.TableNo.ToString();
                model.Outletid = item.OutletId;
                model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                foreach (var i in db.tblBillDetails.Where(a=>a.BillId==item.BillId).ToList())
                {
                    AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                    bill.Amount = i.Amount;
                    bill.BilldeatilId = i.BillDetailsId;
                    bill.FullQty = i.FullQty.Value;
                    bill.HalfQty = i.HalfQty.Value;
                    bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                    lstBill.Add(bill);
                }
                model.getBillItemDetails = lstBill;
                list.Add(model);
            }
            m.getAllBillReports = list;
            m.getAllListOfOutlet = search.GetlistOfOutlets();
            //m.SearchFrom = DateTime.Now.Date;
            //m.SearchTo = DateTime.Now.Date;
            //m.BillNo = 0;
            return View(m);
        }
        [HttpPost]
        public ActionResult RestroBill(AdminBillReportModel model)
        {
            return View(search.getSearchData(model,"R"));
        }
        //------------------------Take Away Bills of All Outlets----------------//
        public ActionResult TakeBills()
        {
            AdminBillReportModel m = new AdminBillReportModel();
            var billdata = (from p in db.tblBillMasters where p.BillingType == "Take Away Hall" && p.OutletId != 0 && p.BillDate==DateTime.Today  select p).ToList();
            ViewBag.alloutletList = search.GetlistOfOutlets();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in billdata)
            {
                BillingModel model = new BillingModel();
                List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                model.BillId = item.BillId;
                model.BillDate = item.BillDate;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.TableNo = item.TableNo;
                model.Outletid = item.OutletId;
                model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                foreach (var i in db.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                {
                    AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                    bill.Amount = i.Amount;
                    bill.BilldeatilId = i.BillDetailsId;
                    bill.FullQty = i.FullQty.Value;
                    bill.HalfQty = i.HalfQty.Value;
                    bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                    lstBill.Add(bill);
                }
                model.getBillItemDetails = lstBill;
                list.Add(model);
            }
            m.getAllBillReports = list;
            m.getAllListOfOutlet = search.GetlistOfOutlets();
            //m.SearchFrom = DateTime.Now.Date;
            //m.SearchTo = DateTime.Now.Date;
            //m.BillNo = 0;
            return View(m);
        }
        [HttpPost]
        public ActionResult TakeBills(AdminBillReportModel model)
        {
            return View(search.getSearchData(model, "Take Away Hall"));
        }
        //--------------------------Home Delivery Bills Reports of all Outlets------------//

        public ActionResult AllHomeDelivery()
        {

            //var billdata = (from p in db.tblBillMasters where p.BillingType.Equals("H") && p.OutletId != 0 select p).ToList();
            //List<BillingModel> list = new List<BillingModel>();
            //foreach (var item in billdata)
            //{
            //    BillingModel model = new BillingModel();
            //    model.BillId = item.BillId;
            //    model.BillDate = item.BillDate;
            //    model.TotalAmount = item.TotalAmount;
            //    model.VatAmount = item.VatAmount;
            //    model.ServicChargeAmt = item.ServicChargesAmount;
            //    model.DiscountAmount = item.DiscountAmount;
            //    model.NetAmount = item.NetAmount;
            //    model.TokenNo = Convert.ToInt32(item.TokenNo);
            //    model.CustomerName = item.CustomerName;
            //    model.Address = item.Address;
            //    model.Outletid = item.OutletId;
            //    list.Add(model);
            //}

            //return View(list);
            AdminBillReportModel m = new AdminBillReportModel();
            var billdata = (from p in db.tblBillMasters where p.BillingType == "Door Delivery Hall" && p.OutletId != 0 && p.BillDate == DateTime.Today select p).ToList();
            ViewBag.alloutletList = search.GetlistOfOutlets();
            List<BillingModel> list = new List<BillingModel>();
            foreach (var item in billdata)
            {
                BillingModel model = new BillingModel();
                List<AdminBillDetailsReportModel> lstBill = new List<AdminBillDetailsReportModel>();
                model.BillId = item.BillId;
                model.BillDate = item.BillDate;
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.ServicChargeAmt = item.ServicChargesAmount;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.CustomerName = item.CustomerName;
                model.Address = item.Address;
                model.TableNo = item.TableNo;
                model.Outletid = item.OutletId;
                model.OutletName = (from p in db.tblOutlets where p.OutletId == item.OutletId select p.Name).FirstOrDefault();
                foreach (var i in db.tblBillDetails.Where(a => a.BillId == item.BillId).ToList())
                {
                    AdminBillDetailsReportModel bill = new AdminBillDetailsReportModel();
                    bill.Amount = i.Amount;
                    bill.BilldeatilId = i.BillDetailsId;
                    bill.FullQty = i.FullQty.Value;
                    bill.HalfQty = i.HalfQty.Value;
                    bill.ItemName = (from p in db.tblItems where p.ItemId == i.ItemId select p.Name).FirstOrDefault();
                    lstBill.Add(bill);
                }
                model.getBillItemDetails = lstBill;
                list.Add(model);
            }
            m.getAllBillReports = list;
            m.getAllListOfOutlet = search.GetlistOfOutlets();
            //m.SearchFrom = DateTime.Now.Date;
            //m.SearchTo = DateTime.Now.Date;
            //m.BillNo = 0;
            return View(m);
        }
        [HttpPost]
        public ActionResult AllHomeDelivery(AdminBillReportModel model)
        {
            return View(search.getSearchData(model, "Door Delivery Hall"));
        }
        //-------------------Operator Reports------------------------//

        public ActionResult OperatorReports()
        {
            var data = (from p in db.tblOperators select p).ToList();
            List<OperatorModel> list = new List<OperatorModel>();
            foreach (var item in data)
            {
                OperatorModel model = new OperatorModel();
                model.ContactNo = item.ContactNo;
                model.Name = item.Name;
                model.operatorId = item.Operatorid;
                //model.Password = item.Password;
                model.Type = item.Type;
                model.Outletid = Convert.ToInt32(item.OutletId);
                list.Add(model);

            }
            return View(list);
        }

        //-------------------------All Stock Transfer Report----------------------//

        public ActionResult StockTransfers()
        {

            var datastock = (from p in db.tblTransfers select p).ToList();
            List<StockTransferModel> list = new List<StockTransferModel>();
            foreach (var item in datastock)
            {
                StockTransferModel model = new StockTransferModel();
              
                model.RawMaterialId = item.RawMaterialId;
                // model.OutletId = item.OutletId;
                model.TransferDate = item.TransferDate;
                model.TransferId = item.TransferId;
                model.TransferQuantity = item.TransferQuantity;
                model.ReciverOutletId = item.ReciverOutletId;
                model.SenderOutletId = item.SenderOutletId;
                list.Add(model);
            }
            return View(list);
        }

        //-----------------------All stock Return Report--------------------------//

        public ActionResult ReturnsReport()
        {
            var returndata = (from p in db.tblReturns select p).ToList();
            List<StockReturnModel> list = new List<StockReturnModel>();
            foreach (var item in returndata)
            {
                StockReturnModel model = new StockReturnModel();
               
                model.RawMaterialId = item.RawMaterialId;
               // model.OutletId = item.OutletId;
                model.ReturnDate = item.ReturnDate;
                model.ReturnDescription = item.ReturnDescription;
                model.ReturnQuantity = item.ReturnQuantity;
                model.RStatus = item.ReturnStatuss;
                model.SenderOutId = item.SenderOutletId;
                model.ReciverOutId = item.ReciverOutletId;
                model.ReturnId = item.ReturnId;
                list.Add(model);
            }
            return View(list);
        }

        //-------------------------------All Delete Bill Reports of Outlet-------------------//

        public ActionResult DOutletBills()
        {
            var dataofdelete = (from p in db.tblDeleteBillMasters select p).ToList();
            List<DeleteModel> list = new List<DeleteModel>();
            foreach (var item in dataofdelete)
            {
                DeleteModel model = new DeleteModel();
                model.Address = item.Address;
                model.BillDate = item.BillDate;
                model.BillingType = item.BillingType;
                model.CustomerName = item.CustomerName;
                model.DeleteId = item.DeleteId;
                model.DiscountAmount = item.DiscountAmount;
                model.NetAmount = item.NetAmount;
                model.Outletid = item.OutletId;
                model.ServicChargeAmt = item.ServiceCharAmt;
                model.TableNo = Convert.ToInt32(item.TableNo);
                model.TokenNo = Convert.ToInt32(item.TokenNo);
                model.TotalAmount = item.TotalAmount;
                model.VatAmount = item.VatAmount;
                model.DeleteDate = Convert.ToDateTime(item.DeleteDate);
                list.Add(model);
            }
            return View(list);
        }

        //--------------------------------Delete Bill by admin------------------------//
        public ActionResult DeleteBill(int id = 0)
        {
            try {
                var deletechildbil = (from p in db.tblDeletedetails where p.DeleteId.Equals(id) select p).ToList();
                foreach (var item in deletechildbil)
                {
                    db.tblDeletedetails.Remove(item);
                    db.SaveChanges();
                }
                var deletmainbil = (from q in db.tblDeleteBillMasters where q.DeleteId.Equals(id) select q).FirstOrDefault();
                db.tblDeleteBillMasters.Remove(deletmainbil);
                db.SaveChanges();
                TempData["error"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return RedirectToAction("DOutletBills", "Admin");
        }
    }
}
