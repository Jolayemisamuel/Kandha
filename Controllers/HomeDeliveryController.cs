using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Repository;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Xml.Linq;
using WebMatrix.WebData;
using System.Web.Security;

namespace NibsMVC.Controllers
{
    [Authorize(Roles = "Outlet,Operator")]
    public class HomeDeliveryController : Controller
    {
        //
        // GET: /HomeDelivery/
        TakeawayRepository obj = new TakeawayRepository();
        NibsHomeDeliveryRepository take = new NibsHomeDeliveryRepository();
        NIBSEntities db = new NIBSEntities();
        XMLTablesRepository xml = new XMLTablesRepository();
        NIbsBillingRepository nibsrepo = new NIbsBillingRepository();
        #region Old
        public ActionResult Index()
        {
            var tokencount = (from q in db.tblBillMasters where q.BillingType.Equals("Door Delivery Hall") select new { BillId = q.BillId, Date = q.BillDate }).ToList();
            var Curentdate = DateTime.Now.Date;
            int no;
            if (tokencount.Count() > 0)
            {
                int tokenMax = db.tblBillMasters.Max(a => a.BillId);
                var LastDate = (from p in db.tblBillMasters where p.BillId == tokenMax select p.BillDate).SingleOrDefault();
                if (Curentdate == LastDate)
                {
                    no = tokencount.Count;
                    no = no + 1;
                }
                else
                {
                    no = 1;
                }
            }
            else
            {
                no = 1;
            }
            ViewBag.tokenno = no;
            var takeserviccharg = (from p in db.tbl_ServiceTax select p.ServiceCharge).FirstOrDefault();
            if (takeserviccharg != 0)
            {
                ViewBag.taksrvicechargs = takeserviccharg;
            }
            else
            {
                //ViewBag.taksrvicechargs = 4.940;
                ViewBag.taksrvicechargs = 5.6;
            }
            var name = WebSecurity.CurrentUserName;
            var outletid = (from n in db.tblOperators where n.Name.Equals(name) select n.OutletId).FirstOrDefault();
            int oulte = Convert.ToInt32(outletid);
            var address = (from p in db.tblOutlets where p.OutletId == oulte select p.Address).SingleOrDefault();
            ViewBag.outletaddress = address;
            return View(obj.GetAllCategoryList());
        }
        
        #endregion

        public ActionResult Home()
        {
            ViewBag.TokenNo = take.GetTokenNo();
            return View(obj.GetAllCategoryList());
        }
        public PartialViewResult _GetAllSubItemPartial(int Id = 0)
        {
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            lst = take.GetAllItems(Id.ToString());
            return PartialView("_GetAllSubItemPartial", lst);
        }
        public PartialViewResult _getTakeAwayBillingItemOnPageLoad(int TokenNo = 0)
        {
            var TakePath = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            var KotFilePath = Server.MapPath("/HomeDeliveryXml/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            model = take.GetBillingItem(TakePath, KotFilePath);
            model.TableNo = TokenNo.ToString();
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        [HttpPost]
        public PartialViewResult _GetTakeAwayBillingItems(string Type, int Id = 0, int Qty = 0, int TokenNo = 0)
        {
            var TakePath = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            var KotFilePath = Server.MapPath("/HomeDeliveryXml/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = take.GetXmlData(Id.ToString(), TakePath, Qty.ToString(), Type, KotFilePath, TokenNo.ToString());
            if (status)
            {
                model = take.GetBillingItem(TakePath, KotFilePath);
                //model.TableNo = TokenNo.ToString();
            }
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult _DeleteItem(int Id = 0)
        {

            var TakePath = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = take.DeleteNode(Id.ToString(), TakePath, KotFilePath);
            if (status)
            {
                model = take.GetBillingItem(TakePath, KotFilePath);
                model.TableNo = take.GetTokenNo().ToString();
            }
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult _ReturnItem(int Id = 0)
        {

            var TakePath = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            var TakeReturn = Server.MapPath("/HomeDeliveryXml/HomeDeliveryReturn.xml");
            var KotFilePath = Server.MapPath("/HomeDeliveryXml/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = take._TransferToReturnXML(TakePath, TakeReturn, KotFilePath, Id);
            if (status)
            {
                model = take.GetBillingItem(TakePath, KotFilePath);
                model.TableNo = take.GetTokenNo().ToString();
            }
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult _clearKOT()
        {
            var TakePath = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            var KotFilePath = Server.MapPath("/HomeDeliveryXml/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = take.ClearKot(KotFilePath);
            if (status)
            {
                model = take.GetBillingItem(TakePath, KotFilePath);
                model.TableNo = take.GetTokenNo().ToString();
            }
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult _printData(GetBillingModel model)
        {
            var Path = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            PrintBillModel m = new PrintBillModel();
            m = take.GetBill(Path, model);
            return PartialView("_printData", m);
        }
        public ActionResult CancelOrder()
        {
            int oulte = xml.getOutletId();
            GetBillingModel model = new GetBillingModel();
            var KotFilePath = Server.MapPath("/HomeDeliveryXml/Kot.xml");
            var Path = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            XDocument xd = XDocument.Load(Path);
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item);
            items.Remove();
            xd.Save(Path);
            bool Update = take.ClearKot(KotFilePath);
            if (Update)
            {
                model = take.GetBillingItem(Path, KotFilePath);
            }

            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult DispatchOrder(GetBillingModel model)
        {
            var Path = Server.MapPath("/HomeDeliveryXml/HomeDelivery.xml");
            var KotFilePath = Server.MapPath("/HomeDeliveryXml/Kot.xml");
            var ReturnXml = Server.MapPath("/HomeDeliveryXml/HomeDeliveryReturn.xml");
            take.SaveReturnItem(ReturnXml);
            bool status = take.DispatchOrder(model, Path);
            ModelState.Clear();
            model = new GetBillingModel();
            model.TableNo = take.GetTokenNo().ToString();
            model.getPaymentType = take.getPaymentMode();
            model = take.GetBillingItem(Path, KotFilePath);
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        
        

    }
}
