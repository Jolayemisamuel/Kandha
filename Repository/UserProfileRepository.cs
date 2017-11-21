using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using WebMatrix.WebData;
using System.Web.Security;
namespace NibsMVC.Repository
{

    public class UserProfileRepository
    {
        NIBSEntities entities = new NIBSEntities();
        //for Outlet Details
        #region
        public bool CreateOutlet(OutletModel model)
        {
            try
            {
                tblOutlet tb = new tblOutlet();
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email });
                Roles.AddUserToRole(model.UserName, "Outlet");
                int Id = WebSecurity.GetUserId(model.UserName);
                tb.Name = model.Name;
                tb.Active = model.Active;
                tb.Address = model.Address;
                tb.City = model.City;
                tb.ContactA = model.ContactA;
                tb.ContactB = model.ContactB;
                tb.Email = model.Email;
                tb.OutletType = "O";
                //tb.Password = model.Password;
                tb.RegistrationDate = DateTime.Now;
                tb.TinNo = model.TinNo;
                tb.UserName = model.UserName;
                tb.ServiceTaxNo = model.ServiceTaxNo;
                tb.OutletId = Id;
                entities.tblOutlets.Add(tb);
                entities.SaveChanges();
                AutoInventory auto = new AutoInventory();
                auto.IsInventory = false;
                auto.OutletId = tb.OutletId;
                entities.AutoInventories.Add(auto);
                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public OutletModel EditOutlet(int OutletId)
        {
                OutletModel model = new OutletModel();
                var data = entities.tblOutlets.Where(a => a.OutletId == OutletId).SingleOrDefault();
                model.Active = data.Active;
                model.Address = data.Address;
                model.City = data.City;
                model.ContactA = data.ContactA;
                model.ContactB = data.ContactB;
                model.Email = data.Email;
                model.Name = data.Name;
                model.OutletId = data.OutletId;
                model.OutletType = data.OutletType;
                model.RegistrationDate = data.RegistrationDate;
                model.TinNo = data.TinNo;
                model.UserName = data.UserName;
                model.ServiceTaxNo = data.ServiceTaxNo;
                return model;
        }
        public bool UpdateOutlet(OutletModel model)
        {
            try
            {
                tblOutlet tb = entities.tblOutlets.Where(a => a.OutletId == model.OutletId).SingleOrDefault();
                tb.Active = model.Active;
                tb.Address = model.Address;
                tb.City = model.City;
                tb.ContactA = model.ContactA;
                tb.ContactB = model.ContactB;
                tb.Email = model.Email;
                tb.Name = model.Name;
                tb.OutletId = model.OutletId;
                tb.OutletType = model.OutletType;
                tb.ServiceTaxNo = model.ServiceTaxNo;
                //tb.RegistrationDate = model.RegistrationDate;
                tb.TinNo = model.TinNo;
                
                tb.UserName = model.UserName;
                entities.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        //for VendorCategory Details
        #region

        public List<viewVendorCategoryModel> ShowAllVendorCategoryList()
        {
            List<viewVendorCategoryModel> List = new List<viewVendorCategoryModel>();
            var data = (from n in entities.AddCategoryVendors select n).AsEnumerable(); //entities.AddCategoryVendors.ToList();
            
            foreach (var item in data)
            {
                viewVendorCategoryModel model = new viewVendorCategoryModel();

                model.id = item.id;
                model.RawCategoryId = item.RawCategoryId;
                model.CategoryName = item.RawCategory.Name;
                model.VendorId = item.VendorId;
                model.VendorName = item.tblVendor.Name;
                List.Add(model);

            }
            return List;
        }
        public AddVendorCategoryModel EditVendorCategory(int Id)
        {
            AddVendorCategoryModel model = new AddVendorCategoryModel();

            if (Id != 0)
            {
                var data = entities.AddCategoryVendors.Where(x => x.id == Id).SingleOrDefault();
                model.RawCategoryId = data.RawCategoryId;
                //model.CategoryName = data.Category_name;
                model.VendorId = data.VendorId;
                //model.VendorName = data.Vendor_name;
               
                return model;
            }
            else
            {
                return model;
            }
        }
        public string SaveVendorCategory(AddVendorCategoryModel model)
        {
            AddCategoryVendor tb = new AddCategoryVendor();
            var duplicate = entities.AddCategoryVendors.Where(o => o.RawCategoryId.Equals(model.RawCategoryId) && o.VendorId.Equals(model.VendorId)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.id != 0)
                    {
                        tb = entities.AddCategoryVendors.Where(x => x.id == model.id).SingleOrDefault();
                        tb.RawCategoryId = model.RawCategoryId;
                        
                        tb.VendorId = model.VendorId;
                        
                        entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.RawCategoryId = model.RawCategoryId;
                        //tb.Category_name = model.CategoryName;
                        tb.VendorId = model.VendorId;
                        //tb.Vendor_name = model.VendorName;
                        entities.AddCategoryVendors.Add(tb);
                        entities.SaveChanges();
                        return "Record Saved Successfully...";
                    }

                }
                catch
                {
                    return "something Wrong try Agian !";

                }

            }
            else
            {
                return  " Already Exits";
            }

        }
        public string DeleteVendorCategory(int Id)
        {
            try
            {
                var data = entities.AddCategoryVendors.Where(x => x.id == Id).SingleOrDefault();
                entities.AddCategoryVendors.Remove(data);
                entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }

        #endregion
        #region
        public string AssignSubMenu(AssignSubMenuModel model)
        {
            tblAssignSubMenuItem tb = new tblAssignSubMenuItem();
            var duplicate = entities.tblAssignSubMenuItems.Where(o => o.ID.Equals(model.id)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.id != 0)
                    {
                        tb = entities.tblAssignSubMenuItems.Where(x => x.ID == model.id).SingleOrDefault();
                        tb.Mainitemid = Convert.ToInt32(model.MainItem);

                        tb.subitemid = model.SubItem;

                        entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.Mainitemid = Convert.ToInt32(model.MainItem);
                        //tb.Category_name = model.CategoryName;
                        tb.subitemid = model.SubItem;
                        //tb.Vendor_name = model.VendorName;
                        entities.tblAssignSubMenuItems.Add(tb);
                        entities.SaveChanges();
                        return "Record Saved Successfully...";
                    }

                }
                catch (Exception ex)
                {
                    return "something Wrong try Agian !";

                }

            }
            else
            {
                return " Already Exits";
            }

        }

        public List<AssignSubMenuModel> ShowAllSubMenuList()
        {
            List<AssignSubMenuModel> List = new List<AssignSubMenuModel>();
            var data = (from n in entities.tblAssignSubMenuItems select n).AsEnumerable(); //entities.AddCategoryVendors.ToList();

            foreach (var item in data)
            {
                AssignSubMenuModel model = new AssignSubMenuModel();

                model.id = item.ID;
                model.MainItemName = item.tblItem.Name;
                model.SubItemName = item.tblSubItem.Name;

                List.Add(model);

            }
            return List;
        }
        public string DeleteSubMenu(int Id = 0)
        {
            try
            {
                var data = entities.tblAssignSubMenuItems.Where(x => x.ID == Id).SingleOrDefault();
                entities.tblAssignSubMenuItems.Remove(data);
                entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }
        public AssignSubMenuModel EditSubMenu(int Id)
        {
            AssignSubMenuModel model = new AssignSubMenuModel();

            if (Id != 0)
            {
                var data = entities.tblAssignSubMenuItems.Where(x => x.ID == Id).SingleOrDefault();

                model.MainItem = data.tblItem.Name;
                model.SubItem = data.subitemid;

                return model;
            }
            else
            {
                return model;
            }
        }
        #endregion
    }
}