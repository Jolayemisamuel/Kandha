using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class OfferCreationController : Controller
    {
        //
        // GET: /OfferCreation/
        NIBSEntities db = new NIBSEntities();
        AdminOfferRepository obj = new AdminOfferRepository();
        HappyHourRepository happy = new HappyHourRepository();
        public JsonResult OfferItems(string id)
        {
            //DistributorRepository dis = new DistributorRepository();
            AddItemRepository dis = new AddItemRepository();
            List<tblItem> offeritme = dis.offerlistitem(Convert.ToInt32(id));
            //bool Disname =;
            //return Json(Disname, JsonRequestBehavior.AllowGet);
            return Json(new SelectList(offeritme.ToArray(), "ItemId", "Name"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult OfferItemPrices(string id)
        {
            AddItemRepository dis = new AddItemRepository();
            string priceitem = dis.GetListofpriceitems(Convert.ToInt32(id));
            return Json(priceitem, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Index()
        {
           
            return View(obj.List());
        }
        public ActionResult Create()
        {
            IEnumerable<SelectListItem> ItemList = (from q in db.tblItems where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.ItemId.ToString() });
            ViewBag.ItemLists = new SelectList(ItemList, "Value", "Text");
            return View();
        }
        public string getRemaingItems(string Id)
        {
            callRemaingOfferItemRepository call = new callRemaingOfferItemRepository();
           var Data= call.getRemainingItem();
           return Data.ToString();
        }
        // save offers region
        #region Offer Creation
        [HttpPost]
        public ActionResult Create(OfferModel model)
        {
            var data = obj.Save(model);
            //return model;
            TempData["result"] = data;
            return RedirectToAction("Index");
        }
        public string GetItemId(string Name)
        {
            var Data = obj.GetItemId(Name);
            return Data;

        }
        [HttpPost]
        public ActionResult AmountBasis(AmountBasisOfferModel model)
        {
            var Data = obj.SaveAmountBasisOffer(model);
            TempData["result"] = Data;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ComboOffer(ComboOfferModel model)
        {
            var Data = obj.SaveComboOffer(model);
            TempData["result"] = Data;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SaveHappyHoursDates(HappyHoursDatesModel model)
        {
            var Data = happy.SaveHappyHoursDates(model);
            TempData["result"] = Data;
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult SaveHappyHoursDays(HappyHoursDaysModel model)
        {
            var Data = happy.SaveHappyHoursDays(model);
            TempData["result"] = Data;
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult SaveHappyHoursDay(HappyHoursDayModel model)
        {
            var Data = happy.SaveHappyHoursDay(model);
            TempData["result"] = Data;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult SaveHappyHoursDate(HappyHoursDateModel model)
        {
            var Data = happy.SaveHappyHoursDate(model);
            TempData["result"] = Data;
            return RedirectToAction("Index");
        }
        #endregion
        // delete offers region
        #region Offer Delete
        public ActionResult Delete(int OfferId = 0)
        {
            if (OfferId > 0)
            {
                bool data = obj.DeleteOffer(OfferId);
                if (data == true)
                {
                    TempData["result"] = "offer deleted successfully.. ";
                }
                else
                {
                    TempData["result"] = "something wrong Try Agian !";
                }
            }
            else
            {
                TempData["result"] = "something wrong Try Agian !";
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteComboOffer(int OfferId = 0)
        {
            if (OfferId > 0)
            {
                bool data = obj.DeleteCombo(OfferId);
                if (data == true)
                {
                    TempData["result"] = "offer deleted successfully.. ";
                }
                else
                {
                    TempData["result"] = "something wrong Try Agian !";
                }
            }
            else
            {
                TempData["result"] = "something wrong Try Agian !";
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteAmountBasisOffer(int OfferId = 0)
        {
            if (OfferId > 0)
            {
                bool data = obj.DeleteAmountBasis(OfferId);
                if (data == true)
                {
                    TempData["result"] = "offer deleted successfully.. ";
                }
                else
                {
                    TempData["result"] = "something wrong Try Agian !";
                }
            }
            else
            {
                TempData["result"] = "something wrong Try Agian !";
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteHappyHours(int OfferId = 0)
        {
            if (OfferId > 0)
            {
                bool data = obj.DeleteHappyHours(OfferId);
                if (data == true)
                {
                    TempData["result"] = "offer deleted successfully.. ";
                }
                else
                {
                    TempData["result"] = "something wrong Try Agian !";
                }
            }
            else
            {
                TempData["result"] = "something wrong Try Agian !";
            }
            return RedirectToAction("Index");
        }
        #endregion

        
    }
}
