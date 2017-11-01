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
    [Authorize(Roles = "admin")]
    public class AssignOfferController : Controller
    {
        //
        // GET: /AssignOffer/
        AssignOfferInterface obj = new AssignOfferInterface();
        public ActionResult Index()
        {

            return View(obj.getAssignOfferList());
        }
        public ActionResult Create()
        {
            AssignOfferOutletModel model = new AssignOfferOutletModel();
            model.getOutletList = obj.GetAllOutletList();
            return View(model);
        }
        public string getOffer(string Id)
        {
            int id = Convert.ToInt32(Id);
            var Data = obj.getOffer(id);
            return Data.ToString();
        }
        [HttpPost]
        public ActionResult AssignOffer(string UserId,int[] OfferCheck)
        {
            int userid = Convert.ToInt32(UserId);
            var data = obj.AssignOffer(userid, OfferCheck);
            if (data==true)
            {
                TempData["result"] = "Offer Assigned Successfully...";
            }
            else
            {
                TempData["result"] = "Something Wrong ! Try Agian";
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Id=0)
        {
            if (Id>0)
            {
                var Data = obj.DeleteAssignedOffer(Id);
                if (Data==true)
                {
                    TempData["result"] = "Offer deleted Successfully...";
                }
                else
                {
                    TempData["result"] = "This offer is used in Assign Offer please delete first from there";
                }
                return RedirectToAction("Index");
            }
            else
            {
                TempData["result"] = "Something Wrong ! Try Agian";
                return RedirectToAction("Index");
            }
        }
    }
}
