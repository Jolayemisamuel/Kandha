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
        KOTEntities dbKOT = new KOTEntities();


        AddItemRepository obj = new AddItemRepository();
        #region Category
        public ActionResult Index()
        {

            var subgrpKOT = (from p in dbKOT.VM_ItemSubGroups select p).ToList();
            foreach (var item in subgrpKOT)
            {

                var table = db.tblCategories.Where(a => a.Name == item.ItemSubGroup).FirstOrDefault();
                if (table == null)
                {
                    tblCategory tb = new tblCategory();
                    tb.Name = item.ItemSubGroup;
                    tb.Active = true;
                    var random = new Random();
                    var color = String.Format("#{0:X6}", random.Next(0x1000000));
                    tb.Color = color;
                    var forerandom = new Random();
                    color = String.Format("#{0:X6}", forerandom.Next(0x1000000));
                    tb.TextColor = color;
                    db.tblCategories.Add(tb);
                    db.SaveChanges();

                }
            }



            var data = (from p in db.tblCategories where p.Active == true select p).ToList();
            List<GetUnitCategory> List = new List<GetUnitCategory>();
            foreach (var item in data)
            {
                GetUnitCategory model1 = new GetUnitCategory();
                model1.CategoryId = item.CategoryId;
                model1.Name = item.Name;
                model1.Color = item.Color;
                // model.Type = item.Type;
                List.Add(model1);
            }
            return View(List);
        }
        public ActionResult Create(int id = 0)
        {
            if (id > 0)
            {
                var data = (from p in db.tblCategories where p.CategoryId == id select p).SingleOrDefault();
                GetUnitCategory model = new GetUnitCategory();
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
        public ActionResult Create(GetUnitCategory model)
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
                    if (cattableid == null)
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


        public ActionResult SubItemDetails()
        {
            
            var data = (from p in db.tblSubItems  where p.Active == true select p).ToList();
            
            List<AddSubItemModel> List = new List<AddSubItemModel>();
            foreach (var item in data)
            {
                AddSubItemModel model = new AddSubItemModel();
                
                model.Name = item.Name;
                model.Description = item.Description;
                model.subItemId = item.SubItemId;
                model.Name = item.Name;
                List.Add(model);
            }
            return View(List);

        }


        public ActionResult ItemDetails()
        {
            var itemsKOT = (from p in dbKOT.VM_PAOItemsFood select p).ToList();
            var itemsCount = (from p in db.tblItems select p).ToList().Count ;
            if (itemsKOT.Count > itemsCount)
            {
                foreach (var item in itemsKOT)
                {
                    var itemCategoryId = (from p in db.tblCategories where p.Name == item.ItemSubGroup select p.CategoryId).FirstOrDefault();

                    var itemcheck = (from p in db.tblItems where p.Name == item.ItemName && p.CategoryId == itemCategoryId select p).SingleOrDefault();
                    if (itemcheck == null)
                    {
                        tblItem tb = new tblItem();
                        tb.CategoryId = itemCategoryId;
                        tb.DepartmentId = 2;
                        tb.Description = item.AdditionalDetails;
                        tb.ItemCode = item.ItemCode;
                        tb.ItemImage = "";
                        tb.Name = item.ItemName;
                        tb.Active = true;
                        db.tblItems.Add(tb);
                        db.SaveChanges();
                    }

                }
            }

            List <SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() }).ToList ();
            CategoryList.Add(new SelectListItem() {  Text ="--Select Category-- ",Value ="0" });
            CategoryList = CategoryList.OrderBy(o => o.Value ).ToList();
           //CategoryList.Sort();
            ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");

            List<SelectListItem> DepartmentList = (from m in db.tbl_Department where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Department, Value = m.DepartmentID.ToString() }).ToList ();
            DepartmentList.Add(new SelectListItem() { Text = "--Select Department-- ", Value = "0" });
            DepartmentList = DepartmentList.OrderBy(o => o.Value).ToList();

            ViewBag.departmentLists = new SelectList(DepartmentList, "Value", "Text", "DepartmentId");

            var data = (from p in db.tblItems where p.Active == true select p).ToList();
            //var rotiya = (from p in db.tblItems where p.Active == true && p.CategoryId==73 select p).ToList();
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
                model.DepartmentId = item.DepartmentId;

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

           List<SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() }).ToList();
            CategoryList.Add(new SelectListItem() { Text = "--Select Category-- ", Value = "0" });
            CategoryList = CategoryList.OrderBy(o => o.Value).ToList();
            //CategoryList.Sort();
            ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");

            List<SelectListItem> DepartmentList = (from m in db.tbl_Department where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Department, Value = m.DepartmentID.ToString() }).ToList();
            DepartmentList.Add(new SelectListItem() { Text = "--Select Department-- ", Value = "0" });
            DepartmentList = DepartmentList.OrderBy(o => o.Value).ToList();

            ViewBag.departmentLists = new SelectList(DepartmentList, "Value", "Text", "DepartmentId");

            var  data = (from p in db.tblItems where p.Active == true  select p).ToList();
            if (model.SearchCategoryId > 0 && model.SearchDepartmentId == 0)
                data = (from p in db.tblItems where p.Active == true && p.CategoryId == model.SearchCategoryId select p).ToList();
            else if (model.SearchCategoryId == 0 && model.SearchDepartmentId > 0)
                data = (from p in db.tblItems where p.Active == true && p.DepartmentId  == model.SearchDepartmentId select p).ToList();
            else if (model.SearchCategoryId > 0 && model.SearchDepartmentId > 0)
                data = (from p in db.tblItems where p.Active == true && p.CategoryId == model.SearchCategoryId && p.DepartmentId == model.SearchDepartmentId  select p).ToList();
            else
              data = (from p in db.tblItems where p.Active == true  select p).ToList();



            List<AddItemModel> List = new List<AddItemModel>();
            foreach (var item in data)
            {
                AddItemModel emodel = new AddItemModel();
                emodel.ItemCategoryId = item.CategoryId;
                emodel.DepartmentId  = item.DepartmentId;
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
                model.lstofDepartment = obj.GetListofDepartment();
                return View(model);
            }
            else
            {
                //IEnumerable<SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() });
                //ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");
                //ViewBag.lstofCategory = new AddItemRepository().GetListofcategory();
                model.lstofCategory = obj.GetListofcategory();
                model.lstofDepartment = obj.GetListofDepartment();
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
                    tb.DepartmentId = model.DepartmentId;
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


        public ActionResult AddSubItem(int id = 0)
        {
            
            AddSubItemModel model = new AddSubItemModel();
            if (id > 0)
            {
                var data = (from p in db.tblSubItems  where p.SubItemId == id select p).SingleOrDefault();
                model.Description = data.Description;
                model.subItemId = data.SubItemId;
                model.Name = data.Name;
                return View(model);
            }
            else
            {
                return View(model);
            }
            



        }
        [HttpPost]
        public ActionResult AddSubItem(AddSubItemModel model)
        {
            tblSubItem tb = new tblSubItem();
            var subitemcheck = (from p in db.tblSubItems where p.Name == model.Name && p.Active == true  select p).SingleOrDefault();
            if (subitemcheck == null)
            {
                try
                {

                    if (model.subItemId > 0)
                    {
                        tb = (from p in db.tblSubItems where p.SubItemId == model.subItemId select p).SingleOrDefault();
                    }
                    
                    tb.Description = model.Description;
                    tb.Name = model.Name;
                    tb.Active = model.Active;
                    
                    if (model.subItemId > 0)
                    {
                        db.SaveChanges();
                        TempData["item"] = "Edit Successfully..!";
                        return RedirectToAction("SubItemDetails");
                    }
                    else
                    {
                        db.tblSubItems.Add(tb);
                        db.SaveChanges();
                        TempData["item"] = "Record Insert Successfully..!";
                        return RedirectToAction("SubItemDetails");
                    }
                }
                catch (Exception ex)
                {
                    TempData["item"] = ex.Message;
                    return RedirectToAction("SubItemDetails");
                }
            }
            else
            {
                TempData["itemCreate"] = "This Item is already exist ";
                return RedirectToAction("AddSubItem");
            }


        }
        public ActionResult EditItem(int id = 0)
        {
            AddItemModel model = new AddItemModel();
            if (id > 0)
            {
                var data = (from p in db.tblItems where p.ItemId == id select p).SingleOrDefault();

                model.ItemCategoryId = data.CategoryId;
                model.DepartmentId = data.DepartmentId;
                model.Description = data.Description;
                model.ItemCode = data.ItemCode;
                model.ItemId = data.ItemId;
                model.ItemImage = data.ItemImage;
                model.Name = data.Name;
                model.lstofCategory = obj.GetListofcategory();
                model.lstofDepartment = obj.GetListofDepartment();
                model.Active = data.Active;
                return View(model);
            }
            else
            {
                //IEnumerable<SelectListItem> CategoryList = (from m in db.tblCategories where m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.CategoryId.ToString() });
                //ViewBag.categoryLIsts = new SelectList(CategoryList, "Value", "Text", "CategoryId");
                //ViewBag.lstofCategory = new AddItemRepository().GetListofcategory();
                model.lstofCategory = obj.GetListofcategory();
                model.lstofDepartment = obj.GetListofDepartment();
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
                tb.DepartmentId = model.DepartmentId;
                tb.Description = model.Description;
                tb.ItemCode = model.ItemCode;
                tb.ItemImage = Image();
                tb.Name = model.Name;
                tb.Active = model.Active;
                db.SaveChanges();
                TempData["item"] = "Record Updated Successfully...";
                return RedirectToAction("ItemDetails");
            }
            catch
            {
                TempData["item"] = "Record Not Updated  !";
                return RedirectToAction("AddItem", new { id = model.ItemId });
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


        public ActionResult DeleteSubitem(int id)
        {
            try
            {
                //if ((from p in db.tbl_SubItemRawIndent where p.SubItemId.Equals(id) select p ).Count() == 0)
                    //{
                    //    
                    tblSubItem tb = (from p in db.tblSubItems where p.SubItemId == id select p).SingleOrDefault();
                    tb.Active = false;
                    db.SaveChanges();
                    TempData["Message"] = "Delete Successfully !!";
                //}
                //else
                //    TempData["Message"] = "Used in Sub Menu Item Ingrediant !!";
            }
            catch
            {
                ModelState.AddModelError("", "Error Occured !");
            }
            return RedirectToAction("SubItemDetails");
        }

        //code for add/edit/ delete items in Category
        public ActionResult BlockedSubItem()
        {
            List<lstOfBlockSubItems> lst = new List<lstOfBlockSubItems>();
            AdminBlockSubItemModel mo = new AdminBlockSubItemModel();
            try
            {

                foreach (var item in db.tblSubItems.Where(x => x.Active == false).ToList())
                {
                    lstOfBlockSubItems model = new lstOfBlockSubItems();
                    
                    model.Name = item.Name;
                    
                    model.SubItemId = item.SubItemId;
                    
                    lst.Add(model);

                }
                mo.lstOfSubItems = lst;
                return View(mo);
            }
            catch
            {
                return View(lst);
            }
        }
        [HttpPost]
        public ActionResult BlockedSubItem(int SubItemId = 0)
        {
            List<lstOfBlockSubItems> lst = new List<lstOfBlockSubItems>();
            AdminBlockSubItemModel mo = new AdminBlockSubItemModel();
            try
            {

                foreach (var item in db.tblSubItems.Where(x =>  x.Active == false).ToList())
                {
                    lstOfBlockSubItems model = new lstOfBlockSubItems();
                    
                    model.Name = item.Name;
                    
                    model.SubItemId = item.SubItemId;
                    
                    lst.Add(model);

                }
                mo.lstOfSubItems = lst;
                return View(mo);
            }
            catch
            {
                return View(lst);
            }
        }
        public ActionResult BlockedItem()
        {
            List<lstOfBlockItems> lst = new List<lstOfBlockItems>();
            AdminBlockItemModel mo = new AdminBlockItemModel();
            try
            {

                foreach (var item in db.tblItems.Where(x => x.Active == false).ToList())
                {
                    lstOfBlockItems model = new lstOfBlockItems();
                    model.CategoryId = item.CategoryId;
                    model.Name = item.Name;
                    model.ItemCode = item.ItemCode;
                    model.ItemId = item.ItemId;
                    model.ItemImage = item.ItemImage;
                    model.CategoryName = item.tblCategory.Name;
                    model.DepartmentId = item.DepartmentId;
                    model.Department = item.tbl_Department.Department;
                    lst.Add(model);

                }
                mo.lstOfItems = lst;
                mo.lstofCategory = obj.GetListofcategory();
                mo.lstofDepartment = obj.GetListofDepartment();
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
                    model.DepartmentId = item.DepartmentId;
                    model.Department = item.tbl_Department.Department;

                    lst.Add(model);

                }
                mo.lstOfItems = lst;
                mo.lstofCategory = obj.GetListofcategory();
                mo.lstofDepartment = obj.GetListofDepartment();
                mo.CategoryId = CategoryId;
                return View(mo);
            }
            catch
            {
                return View(lst);
            }
        }
        public ActionResult Unblock(int id = 0)
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

        public ActionResult UnblockSubItem(int id = 0)
        {
            try
            {
                var data = db.tblSubItems.Where(a => a.SubItemId == id).SingleOrDefault();
                data.Active = true;
                db.SaveChanges();
                return RedirectToAction("BlockedSubItem");
            }
            catch
            {
                return RedirectToAction("BlockedSubItem");
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
        public void updateDepartment(string ItemId, string DeptId)
        {
            int itemid = Convert.ToInt32(ItemId);
            int deptid = Convert.ToInt32(DeptId);
            tblItem tb = db.tblItems.Where(a => a.ItemId == itemid).SingleOrDefault();
            tb.DepartmentId = deptid;
            //tb.CategoryId = tb.CategoryId;
            //tb.Description = tb.Description;
            //tb.ItemCode = tb.ItemCode;
            //tb.ItemImage = tb.ItemImage;
            //tb.Name = model.Name;
            //tb.Active = model.Active;
            db.SaveChanges();
            TempData["item"] = "Record Updated Successfully...";
        }

    }
}
