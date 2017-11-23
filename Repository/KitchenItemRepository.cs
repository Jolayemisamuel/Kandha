using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Web.Mvc;
using System.Text;
using WebMatrix.WebData;
using System.Xml.Linq;

namespace NibsMVC.Repository
{
    public class KitchenItemRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public List<AddItemModel> ShowListofKitchenItems()
        {
            List<AddItemModel> List = new List<AddItemModel>();
            var data = _entities.tblItems.Where(x => x.Active==true).ToList();
            foreach (var item in data)
            {
                AddItemModel model = new AddItemModel();
                model.Active = item.Active;
                model.ItemCategoryId = item.CategoryId;
                model.Description = item.Description;
                model.ItemCode = item.ItemCode;
                model.ItemId = item.ItemId;
                model.ItemImage = item.ItemImage;
                //model.MinimumQuantity = item.MinimumQuantity;
                model.Name = item.Name;
                //model.Unit = item.Unit;
                List.Add(model);

            }
            return List;
        }
        // Kitchen Raw Material
        #region
        public List<RawMaterialsModel> ShowAllRawMaterialsList()
        {
            List<RawMaterialsModel> List = new List<RawMaterialsModel>();
            var data = _entities.tbl_RawMaterials.ToList();
            foreach (var item in data)
            {
                RawMaterialsModel model = new RawMaterialsModel();
                model.RawMaterialId = item.RawMaterialId;
                model.Name = item.Name;
                model.RawCategoryId = item.rawcategoryId;
                model.RawCategoryName = item.RawCategory.Name ;
                model.Units = item.units;
                model.reorder = Convert.ToDecimal(item.reorder);
                model.Barcode = item.barcode;
              
                List.Add(model);

            }
            return List;
        }
        public List<BarcodeGenerateModel> ShowAllBarcodeGenList()
        {
            List<BarcodeGenerateModel> List = new List<BarcodeGenerateModel>();
            var data = _entities.tblGenBarcodes.ToList();
            foreach (var item in data)
            {
                BarcodeGenerateModel model = new BarcodeGenerateModel();
                model.RawMaterialsId = item.RawMaterialId;
                model.Name = item.tbl_RawMaterials.Name ;
                model.RawCategoryId = item.tbl_RawMaterials.rawcategoryId ;
                model.RawCategoryName = item.tbl_RawMaterials.RawCategory.Name ;
                model.NoOfBarcode = item.NoOfBarcode;
                model.Id = item.Id;
                List.Add(model);

            }
            return List;
        }
        public RawMaterialsModel EditRawMaterial(int Id)
        {
            RawMaterialsModel model = new RawMaterialsModel();

            if (Id != 0)
            {
                var data = _entities.tbl_RawMaterials.Where(x => x.RawMaterialId == Id).SingleOrDefault();
                model.RawMaterialId = data.RawMaterialId;
                model.Name = data.Name;
                model.RawCategoryName = data.RawCategory.Name;
                model.RawCategoryId = data.rawcategoryId;
                model.Units = data.units;
                model.reorder = Convert.ToDecimal(data.reorder);
                return model;
            }
            else
            {
                return model;
            }
        }
        public string SaveRawMaterial(RawMaterialsModel model)
        {
            tbl_RawMaterials tb = new tbl_RawMaterials();
            tblOpStckRate tb1 = new tblOpStckRate();
            tbl_KitchenStock tb2 = new tbl_KitchenStock();
            var duplicate = _entities.tbl_RawMaterials.Where(o =>o.Name.Equals(model.Name) && o.units.Equals(model.Units) && o.rawcategoryId.Equals(model.RawCategoryId)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.RawMaterialId != 0)
                    {
                        tb = _entities.tbl_RawMaterials.Where(x => x.RawMaterialId == model.RawMaterialId).SingleOrDefault();
                        tb2 = _entities.tbl_KitchenStock.Where(x => x.RawMaterialId == model.RawMaterialId).SingleOrDefault();
                        tb.Name = model.Name;
                        tb.units = model.Units;
                        tb.reorder = model.reorder;
                        tb.rawcategoryId = model.RawCategoryId;
                        tb2.Unit = model.Units;
                        _entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.Name = model.Name;
                        tb.units = model.Units;
                        tb.reorder = model.reorder;
                        tb.rawcategoryId = model.RawCategoryId;
                        tb1.MaterialId = model.RawMaterialId;
                        tb2.RawMaterialId = model.RawMaterialId;
                        tb1.Rate = Convert.ToDecimal("0.00");
                        tb1.Date=Convert.ToDateTime("2017/09/30");
                        tb1.Qty = Convert.ToDecimal("0.00");
                        tb1.IssQty = Convert.ToDecimal("0.00");
                        tb2.Quantity = Convert.ToDecimal("0.00");
                        tb2.Unit = model.Units;
                        tb2.OutletId = Convert.ToInt32("99");
                        _entities.tbl_RawMaterials.Add(tb);
                        _entities.tblOpStckRates.Add(tb1);
                        _entities.tbl_KitchenStock.Add(tb2);
                        _entities.SaveChanges();
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
                return model.Name +" Already Exits";
            }

        }

        public string SaveBarcodeGen(BarcodeGenerateModel  model)
        {
            tblGenBarcode tb = new tblGenBarcode();
                try
                {
           
                        tb.CreateDateTime  = DateTime.Now ;
                        tb.UserId  = WebSecurity .CurrentUserId ;
                        tb.RawMaterialId  = model.RawMaterialsId ;
                        tb.NoOfBarcode  = model.NoOfBarcode ;
                        _entities.tblGenBarcodes.Add(tb);
                        _entities.SaveChanges();
                        return "Record Saved Successfully...";
                    

                }
                catch
                {
                    return "something Wrong try Agian !";

                }
            
        }

        public string SaveStock(AddExtraStock Model1)
        {
            tbl_KitchenStock tb = new tbl_KitchenStock();
            var duplicate = _entities.tbl_KitchenStock.Where(o =>o.RawMaterialId.Equals(Model1.RawId) && o.Quantity.Equals(Model1.currentstock)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (Model1.RawMaterial != null )
                    {
                        tb = _entities.tbl_KitchenStock.Where(x => x.RawMaterialId == Model1.RawId).SingleOrDefault();
                        tb.Quantity = Model1.currentstock;
                        _entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.Quantity = Model1.currentstock;
                        _entities.tbl_KitchenStock.Add(tb);
                        _entities.SaveChanges();
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
                return Model1.RawId +" Already Exits";
            }

        }
        
        public string DeleteRawMaterial(int Id)
        {
            try
            {
                var data = _entities.tbl_RawMaterials.Where(x => x.RawMaterialId == Id).SingleOrDefault();
                var data1 = _entities.tbl_KitchenStock.Where(x =>x.RawMaterialId==Id).SingleOrDefault();
                var data2 = _entities.tblOpStckRates.Where(x => x.MaterialId == Id).SingleOrDefault();
                _entities.tbl_RawMaterials.Remove(data);
                _entities.tbl_KitchenStock.Remove(data1);
                _entities.tblOpStckRates.Remove(data2);
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }

        public string DeleteBarcodeGen(int Id)
        {
            try
            {
                var data = _entities.tblGenBarcodes.Where(x => x.Id == Id).SingleOrDefault();
                _entities.tblGenBarcodes.Remove(data);
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }

        public byte[] GenerateBarcode(string valueToEncode)
        {
            byte[] imgBuffer =null ;
            try
            {
                
                //    public void GetBarcodeImage(string valueToEncode)
                //{
                //Create an instance of BarcodeProfessional class
                using (Neodynamic.Web.MVC.Barcode.BarcodeProfessional bcp = new
                 Neodynamic.Web.MVC.Barcode.BarcodeProfessional())
                {
                    //Set the desired barcode type or symbology
                    bcp.Symbology = Neodynamic.Web.MVC.Barcode.Symbology.Code93;
                    //Set value to encode
                    bcp.Code = valueToEncode;
                    bcp.Text = "dndklsf";
                    //Generate barcode image
                     imgBuffer = bcp.GetBarcodeImage(
                                 System.Drawing.Imaging.ImageFormat.Png);

                    //Write image buffer to Response obj
                    //System.Web.HttpContext.Current.Response.ContentType = "image/png";
                    //System.Web.HttpContext.Current.Response.BinaryWrite(imgBuffer);


                }
                //}
                return imgBuffer;
            }
            catch
            {
                return imgBuffer;
            }
        }
        #endregion

        #region
        public List<AddRawCategoryModel> ShowAllRawCategoriesList()
        {
            List<AddRawCategoryModel> List = new List<AddRawCategoryModel>();
            var data = _entities.RawCategories.ToList();
            foreach (var item in data)
            {
                AddRawCategoryModel model = new AddRawCategoryModel();
                model.RawCategoryId  = item.RawCategoryID ;
                model.Name = item.Name;
                model.Active = item.Active; 
                List.Add(model);

            }
            return List;
        }
        public string DeleteRawCategory(int Id)
        {
            try
            {
                var data = _entities.RawCategories.Where(x => x.RawCategoryID == Id).SingleOrDefault();
                _entities.RawCategories.Remove(data);
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }
        public string DeleteRawIndent(int Id)
        {
            try
            {
                //var data = _entities.tblItems.Where(x => x.ItemId == Id).SingleOrDefault();         
                
                //_entities.tblItems.Remove(data);
                var dataindent = (from p in _entities.tbl_KitchenRawIndent where p.ItemId.Equals(Id) select p).ToList();
                foreach (var item in dataindent)
                {
                    _entities.tbl_KitchenRawIndent.Remove(item);
                    //_entities.SaveChanges();

                }
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }
        public string DeleteSubRawIndent(int Id,string path)
        {
            try
            {

                XDocument xd = XDocument.Load(path);
                //var ids = Id.Split('_');
                //int RawMaterialId = Convert.ToInt32(ids[1]);
                
                var result = (from item in xd.Descendants("SubItems")
                              where item.Element("UserId").Value == "99" &&
                              item.Element("SubItemId").Value == Id.ToString() 
                              
                              select item);
                result.Remove();
                xd.Save(path);


                var data = _entities.tblSubItems.Where(x => x.SubItemId == Id).SingleOrDefault();

                //_entities.tblSubItems.Remove(data);
                var dataindent = (from p in _entities.tbl_SubItemRawIndent where p.SubItemId.Equals(Id) select p).ToList();
                foreach (var item in dataindent)
                {
                    _entities.tbl_SubItemRawIndent.Remove(item);
                    //_entities.SaveChanges();

                }
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }
        public AddRawCategoryModel EditRawCategory(int Id)
        {
            AddRawCategoryModel model = new AddRawCategoryModel();

            if (Id != 0)
            {
                var data = _entities.RawCategories.Where(x => x.RawCategoryID  == Id).SingleOrDefault();
                model.RawCategoryId = data.RawCategoryID;
                model.Name = data.Name;
                return model;
            }
            else
            {
                return model;
            }
        }
        public string SaveRawCategory(AddRawCategoryModel  model)
        {
            RawCategory tb = new RawCategory();
            var duplicate = _entities.RawCategories.Where(o => o.Name.Equals(model.Name) && o.Active.Equals(model.Active)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.RawCategoryId  != 0)
                    {
                        tb = _entities.RawCategories.Where(x => x.RawCategoryID  == model.RawCategoryId).SingleOrDefault();
                        tb.Name = model.Name;
                        tb.Active = model.Active;
                        _entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.Name = model.Name;
                        tb.Active = model.Active;
                        _entities.RawCategories.Add(tb);
                        _entities.SaveChanges();
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
                return model.Name + "Already Exits";
            }

        }
         #endregion

        #region
        public List<DepartmentModel> ShowAllDepartmentList()
        {
            List<DepartmentModel> List = new List<DepartmentModel>();
            var data = _entities.tbl_Department.ToList();
            foreach (var item in data)
            {
                DepartmentModel model = new DepartmentModel();
                model.DepartmentId  = item.DepartmentID ;
                model.Department = item.Department;
                model.Active = item.Active;
                List.Add(model);

            }
            return List;
        }
        public string DeleteDepartment(int Id)
        {
            try
            {
                var data = _entities.tbl_Department .Where(x => x.DepartmentID  == Id).SingleOrDefault();
                _entities.tbl_Department .Remove(data);
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }
        public DepartmentModel  EditDepartment(int Id)
        {
            DepartmentModel model = new DepartmentModel();

            if (Id != 0)
            {
                var data = _entities.tbl_Department.Where(x => x.DepartmentID  == Id).SingleOrDefault();
                model.DepartmentId  = data.DepartmentID ;
                model.Department  = data.Department ;
                model.Active = data.Active;
                return model;
            }
            else
            {
                return model;
            }
        }
        public string SaveDepartment(DepartmentModel  model)
        {
            tbl_Department tb = new tbl_Department();
            var duplicate = _entities.tbl_Department.Where(o => o.Department.Equals(model.Department) && o.Active.Equals(model.Active)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.DepartmentId  != 0)
                    {
                        tb = _entities.tbl_Department.Where(x => x.DepartmentID == model.DepartmentId).SingleOrDefault();
                        tb.Department = model.Department;
                        tb.Active = model.Active;
                        _entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.Department = model.Department;
                        tb.Active = model.Active;
                        _entities.tbl_Department.Add(tb);
                        _entities.SaveChanges();
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
                return model.Department  + "Already Exits";
            }

        }
        #endregion



        // Kitchen Raw Indent
        #region
        public KitchenRawIndentModel AddKitchenRawIndent()
        {
            KitchenRawIndentModel model = new KitchenRawIndentModel();
            model.lstofCategorirs = GetListofCategories();
            model.GetListOfKitchenRawIndents = GetListOfRawIndents();
            model.GetAllAutocomplete = GetList();
            model.lstofUnits = GetListofUnits();
            model.lstofRawCategories = GetListofRawCategories();
            return model;
        }
        public List<ListofRawIndentItem> ListKitchenRawIndent(int id = 0)
        {
            List<ListofRawIndentItem> List = new List<ListofRawIndentItem>();
            List<ListofRawIndentItem> edit = (from n in _entities.tbl_KitchenRawIndent.Where(x => x.ItemId == id)
                                              select n).AsEnumerable().Select(n => new ListofRawIndentItem()
                                              {
                                                  id = n.Id,
                                                  unit = n.Unit,
                                                  portion = Convert.ToInt32(n.Portion),
                                                  quantity = Convert.ToDecimal(n.Quantity),
                                                  category = n.tblCategory.Name,
                                                  item = n.tblItem.Name,
                                                  rawitem = n.tbl_RawMaterials.Name
                                              }).ToList<ListofRawIndentItem>();
            foreach (var item in edit)
            {
                ListofRawIndentItem model = new ListofRawIndentItem();
                model.id = item.id;
                model.category = item.category;
                model.item = item.item;
                model.quantity = item.quantity;
                model.rawitem = item.rawitem;
                model.unit = item.unit;
                model.portion = item.portion;
                List.Add(model);

            }
            return List;
        }
        public SubItemRawIndentModel AddSubMenuRawIndent()
        {
            SubItemRawIndentModel model = new SubItemRawIndentModel();
            model.lstofsubItems  = GetListofSubItems ();
          model.lstofUnits = GetListofUnits();
            model.lstofRawCategories = GetListofRawCategories();
            return model;
        }
        public List<ListOfRawIndent> GetListOfRawIndents()
        {
            List<ListOfRawIndent> List = new List<ListOfRawIndent>();
            var data = _entities.tbl_KitchenRawIndent.ToList();
            foreach (var item in data)
            {
                ListOfRawIndent model = new ListOfRawIndent();
                model.CategoryId = item.tblCategory.CategoryId;
                model.ItemId = item.tblItem.ItemId;
                model.Quantity = item.Quantity;
                model.RawMaterialId = item.tbl_RawMaterials.RawMaterialId;
                model.Unit = item.Unit;
                model.Portion = Convert.ToInt32(item.Portion);
                List.Add(model);

            }
            return List;
        }
        public List<RawAutocompleteModel> GetList()
        {
            List<RawAutocompleteModel> List = new List<RawAutocompleteModel>();
            var data = _entities.tbl_RawMaterials.ToList();
            foreach (var item in data)
            {
                RawAutocompleteModel model = new RawAutocompleteModel();
                model.Id = item.RawMaterialId;
                model.Name = item.Name;
                List.Add(model);
            }
            return List;
        }
        public string GetListofItems(int Id)
        {
            var data = _entities.tblItems.Where(x => x.tblCategory.CategoryId == Id).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append("<option value=" + item.ItemId + ">" + item.Name + "</option>");
            }
            return sb.ToString();
        }
        public string GetListofRawItems(int Id)
        {
            var data = _entities.tbl_RawMaterials.Where(x => x.RawCategory.RawCategoryID == Id).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append("<option value=" + item.RawMaterialId + ">" + item.Name + "</option>");
            }
            return sb.ToString();
        }

        public List<SelectListItem> GetListofCategories()
        {
            
            var selList = new List<SelectListItem>();
            List<GetUnitCategory> lst = (from item in _entities.tblCategories.Where(x => x.Active == true ).ToList()
                                          select new GetUnitCategory()
                                          {
                                              CategoryId = item.CategoryId,
                                              Name = item.Name
                                          }).ToList<GetUnitCategory>();
            foreach (var item in lst)
            {
                selList.Add(new SelectListItem
                {
                    Text = item.Name.ToString(),
                    Value = item.CategoryId.ToString()
                });
            }
            return selList;
        }
        public List<SelectListItem> GetListofRawCategories()
        {

            var selRawList = new List<SelectListItem>();
            List<GetUnitCategory> lst = (from item in _entities.RawCategories.Where(x => x.Active == true).ToList()
                                         select new GetUnitCategory()
                                         {
                                             CategoryId = item.RawCategoryID,
                                             Name = item.Name
                                         }).ToList<GetUnitCategory>();
            foreach (var item in lst)
            {
                selRawList.Add(new SelectListItem
                {
                    Text = item.Name.ToString(),
                    Value = item.CategoryId.ToString()
                });
            }
            return selRawList;
        }

        public List<SelectListItem> GetListofUnits()
        {

            var selList1 = new List<SelectListItem>();
            List<UnitModel1> lst1 = (from item1 in _entities.Units.Where(x => x.Active == true).ToList()
                                          select new UnitModel1()
                                          {
                                              Unitid = item1.UnitId,
                                              UnitName = item1.UnitName
                                          }).ToList<UnitModel1>();
            foreach (var item1 in lst1)
            {
                selList1.Add(new SelectListItem
                {
                    Text = item1.UnitName.ToString(),
                    Value = item1.Unitid.ToString()
                });
            }
            return selList1;
        }

        public List<SelectListItem> GetListofSubItems()
        {

            var selList1 = new List<SelectListItem>();
            List<AddSubItemModel> lst1 = (from item1 in _entities.tblSubItems.Where(x => x.Active == true).ToList()
                                     select new AddSubItemModel()
                                     {
                                         subItemId  = item1.SubItemId,
                                         Name  = item1.Name 
                                     }).ToList();
            foreach (var item1 in lst1)
            {
                selList1.Add(new SelectListItem
                {
                    Text = item1.Name.ToString(),
                    Value = item1.subItemId.ToString()
                });
            }
            return selList1;
        }
        public string UpdateKitchenRawMaterail(KitchenRawIndentModel model,string path)
        {
            int oulte = 99; //WebSecurity.CurrentUserId;
            int unitid = Convert.ToInt32(model.Unit);
            XDocument xd = XDocument.Load(path);
            var unit = _entities.Units.Where(x => x.UnitId == unitid ).SingleOrDefault();
 
            //var items = from item in xd.Descendants("Items")
            //            where item.Element("UserId").Value == oulte.ToString() 
            //            && item.Element("RawCategoryId").Value == model.RawCategoryId.ToString() &&
            //            item.Element("ItemId").Value == model.ItemId.ToString() &&
            //            item.Element("RawMaterialId").Value == RawId.ToString() 
            //            select item;
            // if (items.Count() > 0)
            // {
            //     foreach (XElement itemElement in items)
            //     {
                    
            //         itemElement.SetElementValue("UserId", oulte);
            //         itemElement.SetElementValue("RawCategoryId", model.RawCategoryId);
            //         itemElement.SetElementValue("ItemId", model.ItemId);
            //         itemElement.SetElementValue("RawMaterialId", RawId);
            //         itemElement.SetElementValue("Unit", model.Unit);
            //         itemElement.SetElementValue("Quantity", model.Quantity);
                     
            //         xd.Save(path);
            //     }
            // }
            //else
            // {
                 var newElement = new XElement("Items",
                         new XElement("UserId", oulte),
                         new XElement("RawCategoryId", model.RawCategoryId),
                         new XElement("ItemId", model.ItemId),
                         new XElement("RawMaterialId", model.RawMaterialId ),
                         new XElement("Unit", unit.UnitName ),
                         new XElement("Quantity", model.Quantity));
                 xd.Element("Item").Add(newElement);
                 xd.Save(path);
             //}
             return FillXmlData(path);
        }

        public string UpdateSubItemRawMaterial(SubItemRawIndentModel model, string path)
        {
            int oulte = 99; 
            int unitid = Convert.ToInt32(model.Unit);
            XDocument xd = XDocument.Load(path);
            var unit = _entities.Units.Where(x => x.UnitId == unitid).SingleOrDefault();

           
            var newElement = new XElement("SubItems",
                    new XElement("UserId", oulte),
                    new XElement("RawCategoryId", model.RawMaterialCategoryId),
                    new XElement("SubItemId", model.SubItemId),
                    new XElement("RawMaterialId", model.RawMaterialId),
                    new XElement("Unit", unit.UnitName),
                    new XElement("Quantity", model.Quantity),
                    new XElement("Portion", model.Portion)
                    );

            xd.Element("SubItem").Add(newElement);
            xd.Save(path);
            //}
            return UpdateSubItemRawMaterial(model.SubItemId,path);

        }

        public string UpdateSubItemRawMaterial(int  SubItemid, string path)
        {
           
            XDocument xd = XDocument.Load(path);

            int oulte = 99; //WebSecurity.CurrentUserId;
            var result = from item in xd.Descendants("SubItems")
                         where item.Element("UserId").Value == oulte.ToString() && item.Element("SubItemId").Value == SubItemid.ToString()
                         select item;
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            sb.Append("<table class='table'><thead><tr><th>S.No</th><th>Sub Item</th><th>Portion</th><th>RawMaterial</th><th>Unit</th><th>Quantity</th><th>Remove</th></tr></thead>");
            sb.Append("<tbody>");
            foreach (var item in result)
            {
                int Id = Convert.ToInt32(item.Element("SubItemId").Value);
                var data = (from p in _entities.tblSubItems where p.SubItemId == Id select new { ItemName = p.Name }).FirstOrDefault();
                int RawId = Convert.ToInt32(item.Element("RawMaterialId").Value);
                var Name = _entities.tbl_RawMaterials.Where(x => x.RawMaterialId == RawId).Select(x => x.Name).SingleOrDefault();
                sb.Append("<tr>");
                sb.Append("<td>" + counter + "</td><td>" + data.ItemName + "</td><td>" + item.Element("Portion").Value + "</td>");
                sb.Append("<td>" + Name + "</td><td>" + item.Element("Unit").Value + "</td><td>" + item.Element("Quantity").Value + "</td>");
                sb.Append("<td><a href='#' id=" + Id + "_" + RawId + " class='delete_raw'><span class='fa fa-trash-o'><span></a></td>");
                sb.Append("</tr>");
                counter++;

            }
            sb.Append("</tbody></table>");
            return sb.ToString();

        }
        public string FillSubItemXmlData(string path)
        {
            XDocument xd = XDocument.Load(path);

            int oulte = 99; //WebSecurity.CurrentUserId;
            var result = from item in xd.Descendants("SubItems")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item;
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            sb.Append("<table class='table'><thead><tr><th>S.No</th><th>Sub Item</th><th>Portion</th><th>RawMaterial</th><th>Unit</th><th>Quantity</th><th>Remove</th></tr></thead>");
            sb.Append("<tbody>");
            foreach (var item in result)
            {
                int Id = Convert.ToInt32(item.Element("SubItemId").Value);
                var data = (from p in _entities.tblSubItems where p.SubItemId == Id select new { ItemName = p.Name }).FirstOrDefault();
                int RawId = Convert.ToInt32(item.Element("RawMaterialId").Value);
                var Name = _entities.tbl_RawMaterials.Where(x => x.RawMaterialId == RawId).Select(x => x.Name).SingleOrDefault();
                sb.Append("<tr>");
                sb.Append("<td>" + counter + "</td><td>" + data.ItemName + "</td><td>" + item.Element("Portion").Value + "</td>");
                sb.Append("<td>" + Name + "</td><td>" + item.Element("Unit").Value + "</td><td>" + item.Element("Quantity").Value + "</td>");
                sb.Append("<td><a href='#' id=" + Id + "_" + RawId + " class='delete_raw'><span class='fa fa-trash-o'><span></a></td>");
                sb.Append("</tr>");
                counter++;
                
            }
            sb.Append("</tbody></table>");
            return sb.ToString();
        }



        public string FillXmlData(string path)
        {
            XDocument xd = XDocument.Load(path);

            int oulte = 99; //WebSecurity.CurrentUserId;
            var result = from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString() 
                         select item;
            StringBuilder sb = new StringBuilder();
            int counter = 1;
            sb.Append("<table class='table'><thead><tr><th>S.No</th><th>Category Name</th><th>ItemName</th><th>RawMaterial Name</th><th>Unit</th><th>Quantity</th></tr></thead>");
            sb.Append("<tbody>");
            foreach (var item in result)
            {
                int Id=Convert.ToInt32(item.Element("ItemId").Value);
                var data = (from p in _entities.tblItems where p.ItemId == Id select new { ItemName = p.Name, CategoryName = p.tblCategory.Name}).FirstOrDefault();
                int RawId=Convert.ToInt32(item.Element("RawMaterialId").Value);
                var Name = _entities.tbl_RawMaterials.Where(x => x.RawMaterialId == RawId).Select(x => x.Name).SingleOrDefault();
                sb.Append("<tr>");
                sb.Append("<td>" + counter + "</td><td>" + data.CategoryName+ "</td><td>" + data.ItemName + "</td>");
                sb.Append("<td>" + Name + "</td><td>" + item.Element("Unit").Value + "</td><td>" + item.Element("Quantity").Value + "</td>");
                sb.Append("<td><a href='#' id=" + Id + "_" + RawId + " class='delete_raw'><span class='fa fa-trash-o'><span></a></td>");
                sb.Append("</tr>");
                counter++;
               
            }
            sb.Append("</tbody></table>");
            return sb.ToString();
        }
        public string SaveRawMaterial(string path)
        {
            try
            {
                XDocument xd = XDocument.Load(path);

                int oulte = 99; //WebSecurity.CurrentUserId;
                var result = (from item in xd.Descendants("Items")
                              where item.Element("UserId").Value == oulte.ToString()

                              select item).ToList();
                foreach (var item in result)
                {
                    tbl_KitchenRawIndent tb = new tbl_KitchenRawIndent();
                    tb.CategoryId = Convert.ToInt32(item.Element("RawCategoryId").Value);
                    tb.ItemId = Convert.ToInt32(item.Element("ItemId").Value);
                    tb.Quantity = Convert.ToDecimal(item.Element("Quantity").Value);
                    tb.RawMaterialId = Convert.ToInt32(item.Element("RawMaterialId").Value);
                    tb.Unit = item.Element("Unit").Value;
                    _entities.tbl_KitchenRawIndent.Add(tb);
                    _entities.SaveChanges();
                   
                }
                var items = (from item in xd.Descendants("Items")
                             where item.Element("UserId").Value == oulte.ToString()
                             select item);
                items.Remove();
                xd.Save(path);
                return "Record Inserted Successfully...";
            }
            catch
            {
                return "something Wrong ! Try Agian ";
            }

           
        }
        
        public string SaveSubItemRawMaterial(string path)
        {
            try
            {
                XDocument xd = XDocument.Load(path);

                int oulte = 99; //WebSecurity.CurrentUserId;
                var result = (from item in xd.Descendants("SubItems")
                              where item.Element("UserId").Value == oulte.ToString()

                              select item).ToList();
                foreach (var item in result)
                {
                    int subItemId = Convert.ToInt32(item.Element("SubItemId").Value);
                    int RawId = Convert.ToInt32(item.Element("RawMaterialId").Value);
                    var checkItem = ( from p in _entities.tbl_SubItemRawIndent where p.SubItemId == subItemId && p.RawMaterialId ==RawId select p);
                    tbl_SubItemRawIndent tb = new tbl_SubItemRawIndent();
                    if (checkItem.Count() == 0)
                    {
                        
                        tb.Portion = Convert.ToInt32(item.Element("Portion").Value);
                        tb.SubItemId = Convert.ToInt32(item.Element("SubItemId").Value);
                        tb.Qty = Convert.ToDecimal(item.Element("Quantity").Value);
                        tb.RawMaterialId = Convert.ToInt32(item.Element("RawMaterialId").Value);
                        tb.Unit = item.Element("Unit").Value;
                        _entities.tbl_SubItemRawIndent.Add(tb);
                    }
                    else
                    {
                        tb.Portion = Convert.ToInt32(item.Element("Portion").Value);
                        tb.Qty = Convert.ToDecimal(item.Element("Quantity").Value);
                        tb.Unit = item.Element("Unit").Value;
                    }
                    _entities.SaveChanges();

                }
                //var items = (from item in xd.Descendants("Items")
                //             where item.Element("UserId").Value == oulte.ToString()
                //             select item);
                //items.Remove();
                //xd.Save(path);
                return "Record Inserted Successfully...";
            }
            catch
            {
                return "something Wrong ! Try Agian ";
            }


        }
        public List<ListOfKitchenRawIndent> ListOfMaterial()
        {
            var data = _entities.tbl_KitchenRawIndent.GroupBy(x=>x.ItemId).ToList();
            List<ListOfKitchenRawIndent> List = new List<ListOfKitchenRawIndent>();
            foreach (var item in data)
            {
                ListOfKitchenRawIndent model = new ListOfKitchenRawIndent();
                model.ItemId = item.FirstOrDefault().tblItem.Name;
                model.Item = item.FirstOrDefault().tblItem.ItemId;
                model.RawCategoryId = item.FirstOrDefault().tblCategory.Name;
                model.Portion = Convert.ToInt32(item.FirstOrDefault().Portion);
                var items = _entities.tbl_KitchenRawIndent.Where(x=> x.ItemId == item.Key).ToList();
                List<InnerKitchenRawIndent> l = new List<InnerKitchenRawIndent>();
                foreach (var it in  items)
                {
                    InnerKitchenRawIndent m = new InnerKitchenRawIndent();
                    m.Quantity = it.Quantity;
                    m.RawMaterialId = it.tbl_RawMaterials.Name;
                    m.Unit = it.tbl_RawMaterials.units;
                    l.Add(m);
                }
                model.ListOfInnerMaterial = l;
                List.Add(model);
                
            }
            return List;
        }

        public List<ListOfSubItemRawIndent> ListOfSubItemMaterial()
        {
            var data = _entities.tbl_SubItemRawIndent.GroupBy(x => x.SubItemId).ToList();
            List<ListOfSubItemRawIndent> List = new List<ListOfSubItemRawIndent>();
            foreach (var item in data)
            {
                ListOfSubItemRawIndent model = new ListOfSubItemRawIndent();
                model.SubItemId = item.FirstOrDefault().tblSubItem.Name;
                model.SubItem = item.FirstOrDefault().tblSubItem.SubItemId;
                model.Portion = item.FirstOrDefault().Portion;
                var items = _entities.tbl_SubItemRawIndent.Where(x => x.SubItemId == item.Key).ToList();
                List<InnerSubItemRawIndent> l = new List<InnerSubItemRawIndent>();
                foreach (var it in items)
                {
                    InnerSubItemRawIndent m = new InnerSubItemRawIndent();
                    m.Quantity = it.Qty;
                    m.RawMaterialId = it.tbl_RawMaterials.Name;
                    m.Unit = it.tbl_RawMaterials.units;
                    l.Add(m);
                }
                model.ListOfSubInnerMaterial = l;
                List.Add(model);

            }
            return List;
        }

        public List<ListOfAssignedSubItemRaw> ListOfAssignedSubItemMaterial()
        {
            var data = _entities.tblAssignRawSubItemMsts.ToList();
            List<ListOfAssignedSubItemRaw> List = new List<ListOfAssignedSubItemRaw>();
            foreach (var item in data)
            {
                ListOfAssignedSubItemRaw model = new ListOfAssignedSubItemRaw();
                model.SubItemId = item.tblSubItem.Name;
                model.SubItem = item.tblSubItem.SubItemId;
                model.Portion = item.Portion;
                model.Date = item.AssignDate;
                model.Id = item.Id;
                var items = _entities.tblAsgnRawSubItemDets.Where(x => x.AsgnRawSubItemMstId== item.Id).ToList();
                List<InnerAssignedSubItemRaw> l = new List<InnerAssignedSubItemRaw>();
                foreach (var it in items)
                {
                    //List<InnerAssignedSubItemRaw> l = new List<InnerAssignedSubItemRaw>();
                    InnerAssignedSubItemRaw m = new InnerAssignedSubItemRaw();
                    m.Quantity = it.Qty;
                    m.RawMaterialId = it.tbl_RawMaterials.Name;
                    var unit = _entities.tbl_SubItemRawIndent.Where(p => p.SubItemId.Equals(item.SubItemId) && p.RawMaterialId.Equals(it.RawMaterialId)).FirstOrDefault();
                    m.Unit = unit.Unit;
                    l.Add(m);
                }
                model.ListOfAssgnedSubItemRaw = l;
                List.Add(model);

            }
            return List;
        }


        public string DeleteRaw(string Id,string path)
        {
            XDocument xd = XDocument.Load(path);
            var ids=Id.Split('_');
            var RawMaterialId=ids[1];
            var ItemId=ids[0];
            var result = (from item in xd.Descendants("Items")
                          where item.Element("UserId").Value == "99" &&
                          item.Element("ItemId").Value == ItemId &&
                          item.Element("RawMaterialId").Value == RawMaterialId
                          select item);
            result.Remove();
            xd.Save(path);
            return FillXmlData(path);
        }

        public string DeleteSubRaw(string Id, string path)
        {
           

            XDocument xd = XDocument.Load(path);
            var ids = Id.Split('_');
            int RawMaterialId = Convert.ToInt32(ids[1]);
            int SubItemId = Convert.ToInt32(ids[0]);
            var result = (from item in xd.Descendants("SubItems")
                          where item.Element("UserId").Value == "99" &&
                          item.Element("SubItemId").Value == SubItemId.ToString() &&
                          item.Element("RawMaterialId").Value == RawMaterialId.ToString()
                          select item);
            result.Remove();
            xd.Save(path);

            var itemCheck = (from p in _entities.tbl_SubItemRawIndent where p.RawMaterialId == RawMaterialId && p.SubItemId==SubItemId select p).SingleOrDefault();
            if (itemCheck != null)
            {
                if (itemCheck.id > 0)
                {
                    //tbl_SubItemRawIndent tb = new tbl_SubItemRawIndent();
                    //tb.id = itemCheck.id;
                    _entities.tbl_SubItemRawIndent.Remove(itemCheck);
                    _entities.SaveChanges();
                }
            }

            return UpdateSubItemRawMaterial(SubItemId,path);
        }

        public string DeleteAssignedSubItem(int Id)
        {


            

            var itemCheck = (from p in _entities.tblAsgnRawSubItemDets where p.AsgnRawSubItemMstId.Equals(Id) select p).ToList();
            if (itemCheck != null)
            {
                if (itemCheck.Count() > 0)
                {
                    foreach (var it in itemCheck)
                    {
                        _entities.tblAsgnRawSubItemDets.Remove(it);
                        _entities.SaveChanges();
                    }
                }
            }
            var itemCheckMst = (from p in _entities.tblAssignRawSubItemMsts where p.Id.Equals(Id) select p).FirstOrDefault();
            if (itemCheckMst != null)
            {
                if (itemCheckMst.Id > 0)
                {
                    
                        _entities.tblAssignRawSubItemMsts.Remove(itemCheckMst);
                        _entities.SaveChanges();
                    
                }
            }

            return "Deleted Successfully";
        }
        #endregion
    }
}