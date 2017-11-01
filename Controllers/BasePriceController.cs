using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class BasePriceController : Controller
    {
        //
        // GET: /BasePrice/
        NIBSEntities db = new NIBSEntities();
        AddItemRepository dis = new AddItemRepository();
        public JsonResult BaseItem(int id, string AcType)
        {
            //DistributorRepository dis = new DistributorRepository();

            List<GetAllItemList> basepriceitems = dis.BaseItemwise(id, AcType);
            //bool Disname =;
            //return Json(Disname, JsonRequestBehavior.AllowGet);

            return Json(new SelectList(basepriceitems.ToArray(), "ItemId", "Name"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            var BaseData = (from p in db.tblBasePriceItems select p).ToList();
            List<BasePriceModel> list = new List<BasePriceModel>();
            foreach (var item in BaseData)
            {
                BasePriceModel model = new BasePriceModel();
                model.CategoryIds = item.CategoryId;
                model.ItemId = item.ItemId;
                model.FullPrice = item.HalfPrice;
                model.HalfPrice = item.HalfPrice;
                model.BasePriceId = item.BasePriceId;
                model.Vat = item.Vat;
                list.Add(model);
            }
            return View(list);
        }
        [Authorize]
        public ActionResult Create(int Id = 0, string AcType = "")
        {

            IEnumerable<SelectListItem> list = new SelectList(new[] 
                                      {
                                        new { Value = "AC", Text = "AC" },
                                        new { Value = "Non AC", Text = "Non AC" },
   
                                      }, "Value", "Text", "AC");

            ViewBag.AcType = new SelectList(list, "Value", "Text"); ;

            IEnumerable<SelectListItem> Categorylist = (from q in db.tblCategories where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.CategoryId.ToString() });
            ViewBag.Categorylists = new SelectList(Categorylist, "Value", "Text");


            BasePriceEditModel model = new BasePriceEditModel();
            if (Id != 0 && AcType != null)
            {
                model.CategoryId = Id;
                model.AcType = AcType;
                List<GetAllItemList> lst = dis.BaseItemwise(Id, AcType);
                model.getAllItemList = lst;
            }
            return View(model);

        }
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            var Editdata = (from p in db.tblBasePriceItems where p.CategoryId == id select p).ToList();
            var Category = (from p in db.tblCategories
                            where p.CategoryId == id
                            select
                                new
                                {
                                    CategoryId = p.CategoryId,
                                    CategoryName = p.Name
                                }).SingleOrDefault();
            BasePriceEditModel model = new BasePriceEditModel();
            model.CategoryId = Category.CategoryId;
            model.CategoryName = Category.CategoryName;
            List<GetAllItemList> lst = new List<GetAllItemList>();
            foreach (var item in Editdata)
            {
                GetAllItemList list = new GetAllItemList();
                list.FullPrice = item.FullPrice;
                list.HalfPrice = item.HalfPrice;
                list.ItemId = item.ItemId;
                list.ItemName = item.tblItem.Name;
                lst.Add(list);
            }
            model.getAllItemList = lst;
            return View(model);
        }

        public ActionResult Delete(int id = 0)
        {
            try
            {
                var deletedata = (from p in db.tblBasePriceItems where p.BasePriceId.Equals(id) select p).SingleOrDefault();
                db.tblBasePriceItems.Remove(deletedata);
                db.SaveChanges();
                TempData["perror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["perror"] = ex.Message;
            }

            return RedirectToAction("Index", "BasePrice");
        }
        [HttpPost]
        public ActionResult EditBasePrice(BasePriceEditModel model)
        {
            try
            {
                for (int i = 0; i < model.EditItemId.Length; i++)
                {
                    int obj = model.EditItemId[i];
                    tblBasePriceItem tb = (from p in db.tblBasePriceItems
                                           where p.ItemId == obj &&
                                           p.CategoryId == model.CategoryId
                                           select p).SingleOrDefault();
                    tb.FullPrice = model.EditFullPrice[i];
                    tb.HalfPrice = model.EditHalfPrice[i];
                    db.SaveChanges();
                    TempData["perror"] = "Record Updated Successfully.....";
                }
            }
            catch (Exception ex)
            {
                TempData["perror"] = ex.Message;
            }
            return RedirectToAction("Index", "BasePrice");
        }

        [HttpPost]
        public ActionResult Create(BasePriceEditModel model)
        {
            try
            {

                for (int i = 0; i < model.EditItemId.Length; i++)
                {
                    int ItemId = model.EditItemId[i];

                    var tb = (from p in db.tblBasePriceItems where p.ItemId == ItemId && p.CategoryId == model.CategoryId && p.AcType==model.AcType select p).SingleOrDefault();
                    if (tb == null)
                    {
                        tblBasePriceItem tbl = new tblBasePriceItem();
                        tbl.ItemId = model.EditItemId[i];
                        tbl.FullPrice = model.EditFullPrice[i];
                        tbl.HalfPrice = model.EditHalfPrice[i];
                        tbl.Vat = 18;//Convert.ToDecimal(model.EditVat[i]);
                        tbl.CategoryId = model.CategoryId;
                        tbl.AcType = "AC";
                        db.tblBasePriceItems.Add(tbl);
                        db.SaveChanges();

                        tblBasePriceItem tblnon = new tblBasePriceItem();
                        tblnon.ItemId = model.EditItemId[i];
                        tblnon.FullPrice = model.EditFullPrice[i];
                        tblnon.HalfPrice = model.EditHalfPrice[i];
                        tblnon.Vat = 12;//Convert.ToDecimal(model.EditVat[i]);
                        tblnon.CategoryId = model.CategoryId;
                        tblnon.AcType = "Non AC";
                        db.tblBasePriceItems.Add(tblnon);
                        db.SaveChanges();
                    }
                    else
                    {

                        if (model.AcType == "AC")
                        {
                            var tbAC = (from p in db.tblBasePriceItems where p.ItemId == ItemId && p.CategoryId == model.CategoryId && p.AcType == "AC" select p).SingleOrDefault();
                            tbAC.AcType = model.AcType;
                            tbAC.FullPrice = model.EditFullPrice[i];
                            tbAC.HalfPrice = model.EditHalfPrice[i];
                            tbAC.Vat = 18;//Convert.ToDecimal(model.EditVat[i]);
                            db.SaveChanges();
                        }
                        else
                        {
                            var tbNonAC = (from p in db.tblBasePriceItems where p.ItemId == ItemId && p.CategoryId == model.CategoryId && p.AcType == "Non AC" select p).SingleOrDefault();
                            tbNonAC.AcType = model.AcType;
                            tbNonAC.FullPrice = model.EditFullPrice[i];
                            tbNonAC.HalfPrice = model.EditHalfPrice[i];
                            tbNonAC.Vat = 12;//Convert.ToDecimal(model.EditVat[i]);
                            db.SaveChanges();
                        }
                    }




                }
                TempData["perror"] = "Record Saved Successfully...";
                return RedirectToAction("Index", "BasePrice");

            }
            catch (Exception ex)
            {
                TempData["Berror"] = ex.Message;
                return RedirectToAction("Create", "BasePrice");
            }

        }


        public JsonResult EditBasePrice(string Id)
        {
            if (Id != null && Id != "")
            {
                string[] str = Id.Split('-');
                for (var i = 0; i < str.Length; i++)
                {
                    string[] arr = str[i].Split('_');
                    int id = Convert.ToInt32(arr[0]);
                    tblBasePriceItem tbl = db.tblBasePriceItems.Where(a => a.BasePriceId == id).FirstOrDefault();
                    tbl.FullPrice = Convert.ToDecimal(arr[1].Replace('^', '.'));
                    tbl.HalfPrice = Convert.ToDecimal(arr[2].Replace('^', '.'));
                    db.SaveChanges();
                }
                TempData["perror"] = "Edit Successfully !!";
                return Json("Update Succefully", JsonRequestBehavior.AllowGet);
            }

            return Json("Not Update", JsonRequestBehavior.AllowGet);
        }



    }
}
