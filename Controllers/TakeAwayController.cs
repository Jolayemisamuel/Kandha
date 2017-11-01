using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Repository;
using NibsMVC.EDMX;
using NibsMVC.Models;
using WebMatrix.WebData;
using System.Xml.Linq;
namespace NibsMVC.Controllers
{
    [Authorize(Roles = "Outlet,Operator")]
    public class TakeAwayController : Controller
    {
        //
        // GET: /TakeAway/
        TakeawayRepository obj = new TakeawayRepository();
        NIbsTakeAwayRepository take = new NIbsTakeAwayRepository();
        NIBSEntities db = new NIBSEntities();
        XMLTablesRepository xml = new XMLTablesRepository();
        NIbsBillingRepository nibsrepo = new NIbsBillingRepository();
        public ActionResult Index()
        {
            var tokencount = (from q in db.tblBillMasters where q.BillingType.Equals("T") select new { BillId = q.BillId, Date = q.BillDate }).ToList();
            var Curentdate = DateTime.Now.Date;
            int no;
            if (tokencount.Count()>0)
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
        
        public string GetAllItem(string Id)
        {
            var Data = obj.GetAllItem(Id);
            return Data;
        }


        public string GetXmlData(string Id, string Qty, string Type, string TokenNo)
        {
            
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            var Data = obj.GetXmlData(Id, TakePath, Qty, Type, KotFilePath, TokenNo);
            return Data;
        }
        [HttpPost]
        public string DeleteNode(string Id, string TokenNo)
        {
            //var delarr = Id.Split('_');
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            var Data = obj.DeleteNode(Id, TakePath, KotFilePath, TokenNo);
            return Data;
        
        }
         [HttpPost]
        public string ClearKot(string TokenNo)
        {
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            var Data = obj.ClearKot(TakePath, KotFilePath, TokenNo);
            return Data;
        }

        public string Dispatch(OrderTakeDispatchModel model)
        {
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var data = obj.DispatchOrder(model, TakePath);
            return data;
        }
        public string CencelOrder(string Id)
        {
            var name = WebSecurity.CurrentUserName;
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            var outletid = (from n in db.tblOperators where n.Name.Equals(name) select n.OutletId).FirstOrDefault();
            int oulte = Convert.ToInt32(outletid);
            XDocument xd = XDocument.Load(TakePath);
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item);
            items.Remove();
            xd.Save(TakePath);
            var data = obj.ClearKot(TakePath, KotFilePath, Id);
            return data;

        }
        public ActionResult TakeAway()
        {
            ViewBag.TokenNo = take.GetTokenNo();
            return View(obj.GetAllCategoryList());
        }
        public PartialViewResult _GetAllSubItemPartial(int Id=0)
        {
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            lst = take.GetAllItems(Id.ToString());
            return PartialView("_GetAllSubItemPartial",lst);
        }
        public PartialViewResult _getTakeAwayBillingItemOnPageLoad(int TokenNo = 0)
        {
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            model = take.GetBillingItem(TakePath, KotFilePath);
            model.TableNo = TokenNo.ToString();
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        [HttpPost]
        public PartialViewResult _GetTakeAwayBillingItems(string Type,int Id=0,int Qty=0,int TokenNo=0)
        {
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = take.GetXmlData(Id.ToString(), TakePath, Qty.ToString(), Type, KotFilePath, TokenNo.ToString());
            if (status)
            {
                model = take.GetBillingItem(TakePath, KotFilePath);
                model.TableNo = TokenNo.ToString();
            }
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult _DeleteItem(int Id = 0)
        {

            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
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

            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var TakeReturn = Server.MapPath("/TakeAwayXML/TakeawayReturn.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = take._TransferToReturnXML(TakePath,TakeReturn,KotFilePath,Id);
            if (status)
            {
                model = take.GetBillingItem(TakePath, KotFilePath);
                model.TableNo = take.GetTokenNo().ToString();
            }
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult _clearKOT()
        {
            var TakePath = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = take.ClearKot(KotFilePath);
            if (status)
            {
                model = take.GetBillingItem( TakePath,KotFilePath);
                model.TableNo = take.GetTokenNo().ToString();
            }
            return PartialView("_GetTakeAwayBillingItems", model);
        }
        public PartialViewResult _printData(GetBillingModel model)
        {
            var Path = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            PrintBillModel m = new PrintBillModel();
            m = take.GetBill(Path, model);
            return PartialView("_printData", m);
        }
        public ActionResult CancelOrder()
        {
            int oulte = xml.getOutletId();
            GetBillingModel model = new GetBillingModel();
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            var Path = Server.MapPath("/TakeAwayXML/Takeaway.xml");
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
            var Path = Server.MapPath("/TakeAwayXML/Takeaway.xml");
            var KotFilePath = Server.MapPath("/TakeAwayXML/Kot.xml");
            var ReturnXml = Server.MapPath("/TakeAwayXML/TakeawayReturn.xml");
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
