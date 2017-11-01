using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace NibsMVC.Controllers
{
    public class OutletController : Controller
    {
        //
        // GET: /Outlet/
        NIBSEntities db = new NIBSEntities();
        UserProfileRepository user = new UserProfileRepository();
        
        #region Outlet
        [Authorize(Roles = "admin")]
        public ActionResult Index(string name)
        {
            var data = (from p in db.tblOutlets where p.OutletType.Equals("O") && p.Active == true select p).ToList();

            List<OutletModel> List = new List<OutletModel>();
            foreach (var item in data)
            {
                OutletModel model = new OutletModel();
                model.Active = item.Active;
                model.Address = item.Address;
                model.City = item.City;
                model.ContactA = item.ContactA;
                model.ContactB = item.ContactB;
                model.Email = item.Email;
                model.Name = item.Name;
                model.OutletId = item.OutletId;
                model.OutletType = item.OutletType;
                model.ServiceTaxNo = item.ServiceTaxNo;
                //model.Password = item.Password;
                model.RegistrationDate = item.RegistrationDate;
                model.TinNo = item.TinNo;
                model.UserName = item.UserName;
                List.Add(model);
            }
            return View(List);
        }
        [Authorize(Roles = "admin")]
        public ActionResult Blocked()
        {
            var data = (from p in db.tblOutlets where p.OutletType.Equals("O") && p.Active == false select p).ToList();

            List<OutletModel> List = new List<OutletModel>();
            foreach (var item in data)
            {
                OutletModel model = new OutletModel();
                model.Active = item.Active;
                model.Address = item.Address;
                model.City = item.City;
                model.ContactA = item.ContactA;
                model.ContactB = item.ContactB;
                model.Email = item.Email;
                model.Name = item.Name;
                model.OutletId = item.OutletId;
                model.OutletType = item.OutletType;
                model.ServiceTaxNo = item.ServiceTaxNo;
                //model.Password = item.Password;
                model.RegistrationDate = item.RegistrationDate;
                model.TinNo = item.TinNo;
                model.UserName = item.UserName;
                List.Add(model);
            }
            return View(List);
        }
        [Authorize(Roles = "admin")]
        public ActionResult CreateOutlet(int id = 0)
        {
            if (id > 0)
            {
                var data = (from p in db.tblOutlets.Where(o => o.OutletId == id) select p).SingleOrDefault();
                OutletModel model = new OutletModel();
                model.OutletId = data.OutletId;
                model.Active = data.Active;
                model.Address = data.Address;
                model.City = data.City;
                model.ContactA = data.ContactA;
                model.ContactB = data.ContactB;
                model.Email = data.Email;
                model.Name = data.Name;
                model.RegistrationDate = data.RegistrationDate;
                model.TinNo = data.TinNo;
                model.UserName = data.UserName;
                model.ServiceTaxNo = data.ServiceTaxNo;
                return View(model);
            }

            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult CreateOutlet(OutletModel model)
        {
            try
            {

                var data = user.CreateOutlet(model);
                if (data==true)
                {
                    TempData["Outlet"] = "Record Saved Successfully...";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Outlet"] = "UserName Already Exits !";
                    return RedirectToAction("Create");
                }
            }
            catch
            {
                TempData["Outlet"] = "Error Occured !";
                return RedirectToAction("CreateOutlet");
            }
           
        }
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int OutletId=0)
        {
           try
           {
               return View(user.EditOutlet(OutletId));
           }
            catch
           {
               return View();
           }
        }
        [HttpPost]
        public ActionResult Edit(OutletModel model)
        {
            var data = user.UpdateOutlet(model);
            if (data == true)
            {
                TempData["Outlet"] = "Record Saved Successfully...";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Outlet"] = "UserName Already Exits !";
                return RedirectToAction("Edit", new {OutletId=model.OutletId });
            }
        }
        public ActionResult Delete(int id = 0)
        {
            try
            {
                tblOutlet tb = db.tblOutlets.Where(a => a.OutletId == id).SingleOrDefault();
                tb.Active = false;
                db.SaveChanges();
                TempData["Outlet"] = "Blocked Successfully..!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Outlet"] = "something wrong Try Again !";
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "admin")]
        public ActionResult Unblock(int id = 0)
        {

            try
            {
                tblOutlet tb = db.tblOutlets.Where(a => a.OutletId == id).SingleOrDefault();
                tb.Active = true;
                db.SaveChanges();
                TempData["Outlet"] = "UnBlocked Successfully..!";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Outlet"] = "something wrong Try Again !";
                return RedirectToAction("Index");
            }

        }
        [Authorize(Roles = "Manager")]
        public ActionResult Profile()
        {
            int id = WebSecurity.GetUserId(WebSecurity.CurrentUserName);
            var data = db.tblOutlets.Where(x => x.OutletId == id).SingleOrDefault();
            ProfileModel model = new ProfileModel();
            model.Address = data.Address;
            model.City = data.City;
            model.ContactA = data.ContactA;
            model.ContactB = data.ContactB;
            model.Email = data.Email;
            model.Name = data.Name;
            model.OutletId = data.OutletId;
            model.ProfileImage = data.ProfileImage;
            model.RegistrationDate = data.RegistrationDate;
            model.TinNo = data.TinNo;
            model.UserName = data.UserName;
            return View(model);
        }
        public ActionResult ChangeProfile(ProfileModel model)
        {
            if (model.ProfileImage != null)
            {
                int id = WebSecurity.GetUserId(WebSecurity.CurrentUserName);
                tblOutlet tb = db.tblOutlets.Where(x => x.OutletId == id).SingleOrDefault();
                if (System.IO.File.Exists(tb.ProfileImage))
                {
                    System.IO.File.Delete(tb.ProfileImage);
                }
                tb.ProfileImage = Image();
                db.SaveChanges();
            }
            return RedirectToAction("Profile");
        }
        public string Image()
        {
            var file = Request.Files["ProfileImage"];
            if (file != null && file.ContentLength > 0)
            {

                var ext = Path.GetExtension(file.FileName);
                if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png" || ext.ToLower() == ".jpeg" || ext.ToLower() == ".gif")
                {
                    string path;
                    string fileName = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_").Replace(" ", "_") + ext;
                    var folserpath = Server.MapPath("~/Images/" + fileName);
                    file.SaveAs(folserpath);
                    return path = "/Images/" + fileName;
                }

                return null;
            }
            else
                return null;
        }
        #endregion

        #region Vendor
        [Authorize(Roles = "admin,Outlet,operator")]
        public ActionResult Vender()
        {

            int Uname = getOutletId();
            var data = (from p in db.tblVendors where p.OutletId == Uname  select p).ToList();
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
                v.Paymentcycle = item.Paymentcycle;
                v.Active = item.Active;
                model.Add(v);

            }
            return View(model);
        }
    [Authorize(Roles = "admin,Outlet,operator")]
        public ActionResult VenderRegistration(int id = 0)
        
        {
            ViewBag.lstofoutlet = new AddItemRepository().GetListofOutlet();
            if (id > 0)
            {
                var data = (from p in db.tblVendors.Where(o => o.VendorId == id) select p).SingleOrDefault();


                VenderRegistrationModel model = new VenderRegistrationModel();
                model.Address = data.Address;
                model.ContactA = data.ContactA;
                model.ContactB = data.ContactB;
                model.Email = data.Email;
                model.Name = data.Name;
                model.OutletId = data.OutletId;
                model.RegistrationDate = data.RegistrationDate;
                model.TinNo = data.TinNo;
               
                model.GSTin = data.GSTin;
                model.IfscCode = data.IfscCode;
                model.Pan = data.Pan;

                model.ServiceTax = (decimal)(data.ServiceTax == null ? 0 : data.ServiceTax);
                model.AccountName = data.AccountName;
                model.AccountNumber = data.AccountNumber;
                model.Bank = data.Bank;
                model.Branch = data.Branch;
                model.Paymentcycle = data.Paymentcycle;
               
                model.VendorId = data.VendorId;
                return View(model);
            }
            else
            {

                return View();
            }
        }
        [HttpPost]
        public ActionResult VenderRegistration(VenderRegistrationModel model)
        {
            try
            {

                tblVendor tb = new tblVendor();
                int Uname = getOutletId();

                if (model.VendorId > 0)
                {
                    tb = (from p in db.tblVendors where p.VendorId == model.VendorId select p).SingleOrDefault();
                }
                tb.Address = model.Address;
                tb.ContactA = model.ContactA;
                tb.ContactB = model.ContactB;
                tb.Email = model.Email;
                tb.Name = model.Name;
                tb.OutletId = 99;
                tb.RegistrationDate = DateTime.Now;
                tb.TinNo = model.TinNo;

                tb.GSTin = model.GSTin;
                tb.IfscCode = model.IfscCode;
                tb.Pan = model.Pan;

                tb.ServiceTax = (decimal)(model.ServiceTax == null ? 0 : model.ServiceTax);
                tb.AccountName = model.AccountName;
                tb.AccountNumber = model.AccountNumber;
                tb.Bank = model.Bank;
                tb.Branch = model.Branch;
                tb.Paymentcycle = model.Paymentcycle;

                tb.Active = model.Active;
                if (model.VendorId > 0)
                {
                    db.SaveChanges();
                    TempData["Message"] = "Edit Successfully !";
                }
                else
                {
                    db.tblVendors.Add(tb);
                    db.SaveChanges();
                    TempData["Message"] = "Insert Successfully !";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("Vender");
        }
        [Authorize(Roles = "admin,Outlet,Operator")]
        public ActionResult VenderDelete(int id)
        {
            try
            {
                var data = (from p in db.tblVendors where p.VendorId == id select p).SingleOrDefault();
                data.Active = false;
                db.SaveChanges();
                TempData["Message"] = "Delete Successfully !!";

            }
            catch
            {
                ModelState.AddModelError("", "Error Occured");
            }
            return RedirectToAction("Vender");
        }
        #endregion

        // for VendorCategory
        #region
   
        public ActionResult VendorCategoryReport()
        {


            return View(user.ShowAllVendorCategoryList());
        }
        public ActionResult AddCategoryVendor(int Id = 0)
        {
            IEnumerable<SelectListItem> Categorylist = (from q in db.RawCategories where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawCategoryID.ToString() });

            ViewBag.Categorylists = new SelectList(Categorylist, "Value", "Text");

            IEnumerable<SelectListItem> VendorList = (from z in db.tblVendors where z.Active == true select z).AsEnumerable().Select(z => new SelectListItem() { Text = z.Name, Value = z.VendorId.ToString() });

            ViewBag.VendorList = new SelectList(VendorList, "Value", "Text");

            return View(user.EditVendorCategory(Id));
        }
        [HttpPost]
        public ActionResult AddCategoryVendor(AddVendorCategoryModel model)
        {
            var Data = user.SaveVendorCategory(model);
            TempData["Error"] = Data;
            return RedirectToAction("VendorCategoryReport");
        }
        public ActionResult DeleteVendorCategory(int Id = 0)
        {
            var data = user.DeleteVendorCategory(Id);
            TempData["Error"] = data;
            return RedirectToAction("VendorCategoryReport");
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
                    oulte = Convert.ToInt32((from n in db.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return 99;
        }

    }
}
