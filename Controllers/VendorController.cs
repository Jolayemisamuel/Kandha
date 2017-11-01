using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Repository;
using NibsMVC.EDMX;
using NibsMVC.Models;
namespace NibsMVC.Controllers
{
    public class VendorController : Controller
    {
        OutletVendorRepository vendor = new OutletVendorRepository();

        #region Vendor Account
        public ActionResult Index()
        {
            return View(vendor.getAllVendor());
        }
        public ActionResult Create(int Id=0)
        {
            return View(vendor.getOutLetVendor(Id));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveVendor(OutletVendor model)
        {
            vendor.saveVendor(model);
            return RedirectToAction("Index");
        }
        public ActionResult DeleteVendor(int Id)
        {
            vendor.DeleteVendor(Id);
            return RedirectToAction("Index");
        }

        #endregion

        #region Item Price For Vendor
        public ActionResult setPrice()
        {
            return View(vendor.getVendorsForPriceUpdate());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult _getItemToUpdate(int CategoryId,int VendorId)
        {
            OutletVendorModel model = new OutletVendorModel();
            model.getAllItems = vendor.getAllItems(CategoryId, VendorId);
            model.IsRunningBill = vendor.checkBillingIsRunning(VendorId);
            return PartialView(model);
        }
        public ActionResult UpdatePrice(OutletVendorModel model)
        { 
            vendor.UpdatePrice(model);
            return RedirectToAction("setPrice");
        }
        #endregion

        #region Vendor Billing
        public ActionResult AddVendorPay(int VendorId = 0)
        {
            return View(vendor.getVenderPayment(VendorId));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddVendorPay(AddVendorPayment model)
        {
            decimal value;
            if (Decimal.TryParse(model.PaidAmount.ToString(),out value))
            {
                if (model.PaidAmount > 0)
                {
                    vendor.UpdateBalance(model.VendorId, model.PaidAmount);
                    TempData["paymentStatus"] = "payment updated successfully.....";
                    return RedirectToAction("VendorPaymentDetail");
                }
                else
                {
                    return View(vendor.getVenderPayment(model.VendorId));
                }
            }
           
            else
            {
                return View(vendor.getVenderPayment(model.VendorId));
            }
            
        }
        public ActionResult VendorPaymentDetail()
        {
            return View(vendor.getAllVendorAmount());
        }
        public ActionResult CreateBilling(int VendorId=0)
        {
            return View(vendor.getCteateBilling(VendorId));
        }
        public PartialViewResult _BillingItem(int CategoryId,int VendorId)
        {
            return PartialView(vendor.getAllItemForBilling(CategoryId,VendorId));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult _BillingTable(int ItemId,int Qty,int VendorId)
        {
            return PartialView(vendor.getBill(ItemId, Qty, VendorId));
        }
        public bool ClearKot(int VendorId=0)
        {
            bool status = false;
            if (vendor.clearKot(VendorId))
            {
                status = true;
            }
            return status;
        }
        public PartialViewResult _delete(int Id,int VendorID)
        {
            vendor.DeleteItem(Id);
            return PartialView("_BillingTable",vendor.getBillondelete(VendorID));
        }
        public PartialViewResult _printBill(int VendorId)
        {
            vendor.PrintBill(VendorId);
            return PartialView(vendor.getprintBill(VendorId));
        }
        public PartialViewResult _getBillItemOnPrint( int VendorID)
        {
            return PartialView("_BillingTable", vendor.getBillondelete(VendorID));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DispatchOrder(VendorBillingModel model)
        {
            vendor.DispatchOrder(model);
            return RedirectToAction("CreateBilling");
        }
        public ActionResult BillReport(int VendorId = 0)
        {

            return View(vendor.getBillReport(VendorId));
        }
        public PartialViewResult _printBillAgain(int Id)
        {
            VendorBillingMaster Outletvendor = vendor.getOldBill(Id);
            //var vendordetail = vendor.getOutletVwendor(Outletvendor.VendorId);
            //ViewBag.TinNo = vendordetail.tblOutlet.TinNo;
            //ViewBag.ServiceTaxNo = vendordetail.tblOutlet.ServiceTaxNo;
            //ViewBag.ContactA = vendordetail.tblOutlet.ContactA;
            //ViewBag.Address = vendordetail.tblOutlet.Address;

            return PartialView(Outletvendor);
        }
        #endregion
    }
}
