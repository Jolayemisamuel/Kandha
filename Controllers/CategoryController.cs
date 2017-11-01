using NibsMVC.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Models;
using NibsMVC.Repository;
using System.IO;

namespace NibsMVC.Controllers
{
    [Authorize(Roles = "admin,outlet,operator")]
    public class CategoryController : Controller
    {
        NIBSEntities db = new NIBSEntities();
        AddItemRepository obj = new AddItemRepository();
        #region Category
        public ActionResult Index()
        {
            var data = (from p in db.tblCategories where p.Active == true select p).ToList();
            List<AddCategoryModel> List = new List<AddCategoryModel>();
            foreach (var item in data)
            {
                AddCategoryModel model = new AddCategoryModel();
                model.CategoryId = item.CategoryId;
                model.Name = item.Name;
                model.Color = item.Color;
                // model.Type = item.Type;
                List.Add(model);
            }
            return View(List);
        }
        public ActionResult Create(int id = 0)
        {
            if (id > 0)
            {
                var data = (from p in db.tblCategories where p.CategoryId == id select p).SingleOrDefault();
                AddCategoryModel model = new AddCategoryModel();
                model.CategoryId = data.CategoryId;
                model.Name = data.Name;
                model.Active = data.Active;
                model.Color = data.Color;
                model.TextColor = data.TextColor;
                // model.Type = data.Type;
                return View(model);
            }
            else
            {

                return View();
            }
        }
        [HttpPost]
        public ActionResult Create(AddCategoryModel model)
        {
            tblCategory tb = new tblCategory();
           
            
                try
                {
                    if (model.CategoryId > 0)
                    {
                        tb = (from p in db.tblCategories.Where(o => o.CategoryId == model.CategoryId) select p).SingleOrDefault();
                        tb.Name = model.Name;
                        tb.Active = model.Active;
                        tb.Color = model.Color;
                        tb.TextColor = model.TextColor;
                        db.SaveChanges();
                        TempData["categroy"] = "Record Edit Successfully..!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var cattableid = (from p in db.tblCategories where p.Name == model.Name select p).SingleOrDefault();
                        if (cattableid==null)
                        {
                            tb.Name = model.Name;
                            tb.Active = model.Active;
                            tb.Color = model.Color;
                            tb.TextColor = model.TextColor;
                            db.tblCategories.Add(tb);
                            db.SaveChanges();
                            TempData["categroy"] = "Record Insert Successfully..!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["Crecategroy"] = "This Category is Already exist..!";
                            return RedirectToAction("Create");
                        }
                    }
                    
                    
                }
                catch (Exception ex)
                {
                    TempData["categroy"] = ex.Message;
                    return RedirectToAction("Create");
                }

                

            
        }
        public ActionResult Delete(int id)
        {
            try
            {
                var data = (from p in db.tblCategories where p.CategoryId == id select p).SingleOrDefault();
                data.Active = false;
                db.SaveChanges();
                TempData["Message"] = "Delete Successfully !!";
            }
            catch
            {
                ModelState.AddModelError("", "Error Occured !");
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region Items
        public ActionResult ItemDetails()
        {
            IEnumerable<SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() });
            ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");
            var data = (from p in db.tblItems where p.Active==true select p ).ToList();
            var rotiya = (from p in db.tblItems where p.Active == true && p.CategoryId==73 select p).ToList();
            List<AddItemModel> List = new List<AddItemModel>();
            foreach (var item in data)
            {
                AddItemModel model = new AddItemModel();
                model.ItemCategoryId = item.CategoryId;
                model.Name = item.Name;
                model.Description = item.Description;
                model.ItemCode = item.ItemCode;
                model.ItemId = item.ItemId;
                model.ItemImage = item.ItemImage;
                //model.MinimumQuantity = item.MinimumQuantity;
                model.Name = item.Name;
                //model.Unit = item.Unit;
                List.Add(model);
            }
            return View(List);

        }
        [HttpPost]
        public ActionResult ItemDetails(AddItemModel model)
        {
            IEnumerable<SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() });
            ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");
            var data = (from p in db.tblItems where p.Active == true && p.CategoryId == model.SearchCategoryId select p).ToList();
            List<AddItemModel> List = new List<AddItemModel>();
            foreach (var item in data)
            {
                AddItemModel emodel = new AddItemModel();
                emodel.ItemCategoryId = item.CategoryId;
                emodel.Name = item.Name;
                emodel.Description = item.Description;
                emodel.ItemCode = item.ItemCode;
                emodel.ItemId = item.ItemId;
                emodel.ItemImage = item.ItemImage;
                //model.MinimumQuantity = item.MinimumQuantity;
                emodel.Name = item.Name;
                //model.Unit = item.Unit;
                List.Add(emodel);
            }
            return View(List);

        }
        public ActionResult AddItem(int id = 0)
        {
            //ViewBag.lstofCategory = new AddItemRepository().GetListofcategory();
            AddItemModel model = new AddItemModel();
            if (id > 0)
            {
                var data = (from p in db.tblItems where p.ItemId == id select p).SingleOrDefault();
               
                model.ItemCategoryId = data.CategoryId;
                model.Description = data.Description;
                model.ItemCode = data.ItemCode;
                model.ItemId = data.ItemId;
                model.ItemImage = data.ItemImage;
                model.Name = data.Name;
                model.lstofCategory = obj.GetListofcategory();
                return View(model);
            }
            else
            {
                //IEnumerable<SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() });
                //ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");
                //ViewBag.lstofCategory = new AddItemRepository().GetListofcategory();
                model.lstofCategory = obj.GetListofcategory();
                return View(model);
            }
            // var modellist = new AddItemModel();



        }
        [HttpPost]
        public ActionResult AddItem(AddItemModel model)
        {
            tblItem tb = new tblItem();
            var itemcheck = (from p in db.tblItems where p.Name == model.Name && p.Active == true && p.CategoryId == model.ItemCategoryId select p).SingleOrDefault();
            if (itemcheck == null)
            {
                try
                {

                    if (model.ItemId > 0)
                    {
                        tb = (from p in db.tblItems where p.ItemId == model.ItemId select p).SingleOrDefault();
                    }
                    tb.CategoryId = model.ItemCategoryId;
                    tb.Description = model.Description;
                    tb.ItemCode = model.ItemCode;
                    tb.ItemImage = Image();
                    tb.Name = model.Name;
                    tb.Active = model.Active;
                    if (model.ItemId > 0)
                    {
                        db.SaveChanges();
                        TempData["item"] = "Edit Successfully..!";
                        return RedirectToAction("ItemDetails");
                    }
                    else
                    {
                        db.tblItems.Add(tb);
                        db.SaveChanges();
                        TempData["item"] = "Record Insert Successfully..!";
                        return RedirectToAction("ItemDetails");
                    }
                }
                catch (Exception ex)
                {
                    TempData["item"] = ex.Message;
                    return RedirectToAction("ItemDetails");
                }
            }
            else
            {
                TempData["itemCreate"] = "This Item is already exist in this Category..";
                return RedirectToAction("AddItem");
            }


        }
        public ActionResult EditItem(int id = 0)
        {
            AddItemModel model = new AddItemModel();
            if (id > 0)
            {
                var data = (from p in db.tblItems where p.ItemId == id select p).SingleOrDefault();

                model.ItemCategoryId = data.CategoryId;
                model.Description = data.Description;
                model.ItemCode = data.ItemCode;
                model.ItemId = data.ItemId;
                model.ItemImage = data.ItemImage;
                model.Name = data.Name;
                model.lstofCategory = obj.GetListofcategory();
                model.Active = data.Active;
                return View(model);
            }
            else
            {
                //IEnumerable<SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() });
                //ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");
                //ViewBag.lstofCategory = new AddItemRepository().GetListofcategory();
                model.lstofCategory = obj.GetListofcategory();
                return View(model);
            }
        }
        [HttpPost]
         public ActionResult EditItem(AddItemModel model)
        {
            try
            {
                tblItem tb = db.tblItems.Where(a => a.ItemId == model.ItemId).SingleOrDefault();
                tb.CategoryId = model.ItemCategoryId;
                tb.Description = model.Description;
                tb.ItemCode = model.ItemCode;
                tb.ItemImage = Image();
                tb.Name = model.Name;
                tb.Active = model.Active;
                db.SaveChanges();
                TempData["item"]="Record Updated Successfully...";
                return RedirectToAction("ItemDetails");
            }
            catch
            {
                TempData["item"] = "Record Not Updated  !";
                return RedirectToAction("AddItem", new { id=model.ItemId});
            }
        }
        #endregion
        public ActionResult Deleteitem(int id)
        {
            try
            {
                tblItem tb = (from p in db.tblItems where p.ItemId == id select p).SingleOrDefault();
                tb.Active = false;
                db.SaveChanges();
                TempData["Message"] = "Delete Successfully !!";
            }
            catch
            {
                ModelState.AddModelError("", "Error Occured !");
            }
            return RedirectToAction("ItemDetails");
        }
        
        //code for add/edit/ delete items in Category
        public ActionResult BlockedItem()
        {
            List<lstOfBlockItems> lst = new List<lstOfBlockItems>();
            AdminBlockItemModel mo = new AdminBlockItemModel();
           try
           {
               
               foreach (var item in db.tblItems.Where(x=>x.Active==false).ToList())
               {
                   lstOfBlockItems model = new lstOfBlockItems();
                   model.CategoryId = item.CategoryId;
                   model.Name = item.Name;
                   model.ItemCode = item.ItemCode;
                   model.ItemId = item.ItemId;
                   model.ItemImage = item.ItemImage;
                   model.CategoryName = item.tblCategory.Name;
                   lst.Add(model);

               }
               mo.lstOfItems = lst;
               mo.lstofCategory = obj.GetListofcategory();
               return View(mo);
           }
           catch
           {
               return View(lst);
           }
        }
        [HttpPost]
        public ActionResult BlockedItem(int CategoryId = 0)
        {
            List<lstOfBlockItems> lst = new List<lstOfBlockItems>();
            AdminBlockItemModel mo = new AdminBlockItemModel();
            try
            {

                foreach (var item in db.tblItems.Where(x => x.CategoryId == CategoryId && x.Active == false).ToList())
                {
                    lstOfBlockItems model = new lstOfBlockItems();
                    model.CategoryId = item.CategoryId;
                    model.Name = item.Name;
                    model.ItemCode = item.ItemCode;
                    model.ItemId = item.ItemId;
                    model.ItemImage = item.ItemImage;
                    model.CategoryName = item.tblCategory.Name;
                    lst.Add(model);

                }
                mo.lstOfItems = lst;
                mo.lstofCategory = obj.GetListofcategory();
                mo.CategoryId = CategoryId;
                return View(mo);
            }
            catch
            {
                return View(lst);
            }
        }
        public ActionResult Unblock(int id=0)
        {
            try
            {
                var data = db.tblItems.Where(a => a.ItemId == id).SingleOrDefault();
                data.Active = true;
                db.SaveChanges();
                return RedirectToAction("BlockedItem");
            }
            catch
            {
                return RedirectToAction("BlockedItem");
            }
           
        }
        public string Image()
        {
            var file = Request.Files["ItemImage"];
            if (file != null && file.ContentLength > 0)
            {

                var ext = Path.GetExtension(file.FileName);
                if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".gif")
                {
                    string path;
                    string fileName = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_").Replace(" ", "_") + ext;
                    var folserpath = Server.MapPath("~/ItemImages/" + fileName);
                    file.SaveAs(folserpath);
                    return path = "/ItemImages/" + fileName;
                }

                return null;
            }
            else
                return null;
        }
        
        
        public ActionResult KitchenItemReport()
        {
            KitchenItemRepository obj = new KitchenItemRepository();
            
            return View(obj.ShowListofKitchenItems());
        }

       
    }
}
