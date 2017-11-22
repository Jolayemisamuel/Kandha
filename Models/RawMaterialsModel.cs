using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Models
{

    public class DepartmentModel
    {
        public int DepartmentId { get; set; }

        public string Department { get; set; }
        public bool Active { get; set; }
    }


    public class RawMaterialsModel
    {
        public int RawMaterialId { get; set; }

        public int RawCategoryId { get; set; }

        public string RawCategoryName { get; set; }
        public string Name { get; set; }
        public string Units { get; set; }        
        public decimal reorder { get; set; }

        public string Barcode { get; set; }

       

    }
    public class BarcodeGenerateModel
    {
        public int Id { get; set; }
        public int RawMaterialsId { get; set; }

        public int RawCategoryId { get; set; }

        public string RawCategoryName { get; set; }
        public string Name { get; set; }
       
        public int NoOfBarcode { get; set; }


    }
    public class AddRawCategoryModel
    {
        public int RawCategoryId { get; set; }
        public string Name { get; set; }
        
        public bool Active { get; set; }
       
    }

    public class AddVendorCategoryModel
    {
        public int id { get; set; }
        public int RawCategoryId { get; set; }
        //public string CategoryName { get; set; }
        public int VendorId { get; set; }
        //public string VendorName { get; set; }
    }
    public class viewVendorCategoryModel
    {
        public int id { get; set; }
        public int RawCategoryId { get; set; }
        public string CategoryName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
    }

    public class KitchenRawIndentModel
    {
        public int Id { get; set; }
        public int RawCategoryId { get; set; }
        public int RawMaterialCategoryId { get; set; }
        public int ItemId { get; set; }
        public string RawMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public int Portion { get; set; }
        public List<SelectListItem> lstofCategorirs { get; set; }
        public List<ListOfRawIndent> GetListOfKitchenRawIndents { get; set; }
        public List<RawAutocompleteModel> GetAllAutocomplete { get; set; }
        public List<SelectListItem> lstofUnits { get; set; }
        public List<SelectListItem> lstofRawCategories { get; set; }
        public List<ListofRawIndentItem> lstofrawitem { get; set; }
    }

    public class SubItemRawIndentModel
    {
        public int Id { get; set; }
        
        public int RawMaterialCategoryId { get; set; }
        public int SubItemId { get; set; }
        public string RawMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public int Portion { get; set; }
        public List<SelectListItem> lstofUnits { get; set; }
        public List<SelectListItem> lstofRawCategories { get; set; }
        public List<SelectListItem> lstofsubItems { get; set; }
    }
    public class ListOfRawIndent
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ItemId { get; set; }
        public int RawMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public int Portion { get; set; }
    }
    public class RawAutocompleteModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ListofRawIndentItem
    {
        public int id { get; set; }
        public string category { get; set; }
        public string item { get; set; }
        public string rawitem { get; set; }
        public decimal quantity { get; set; }
        public int portion { get; set; }
        public string unit { get; set; }
    }
    public class ListOfKitchenRawIndent
    {
        public string RawCategoryId { get; set; }
        public string ItemId { get; set; }
        public int Item { get; set; }
        public int Portion { get; set; }
        public List<InnerKitchenRawIndent> ListOfInnerMaterial { get; set; }
    }
    public class ListOfSubItemRawIndent
    {
       
        public string SubItemId { get; set; }
        public int SubItem { get; set; }
        public int Portion { get; set; }
        public List<InnerSubItemRawIndent> ListOfSubInnerMaterial { get; set; }
    }
    public class InnerKitchenRawIndent
    {
        public string RawMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
    public class InnerSubItemRawIndent
    {
        public string RawMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
    public class ListOfAssignedSubItemRaw
    {
        public int Id { get; set; }
        public string SubItemId { get; set; }
        public int SubItem { get; set; }
        public int Portion { get; set; }
        public DateTime Date { get; set; }
        public List<InnerAssignedSubItemRaw> ListOfAssgnedSubItemRaw { get; set; }
    }
    public class InnerAssignedSubItemRaw
    {
        public string RawMaterialId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
    }
    public class AssignSubMenuModel
    {
        public int id { get; set; }
        public int MenuCategoryId { get; set; }
        public string MainItem { get; set; }
        public int MainItemId { get; set; }
        public string MainItemName { get; set; }
        public int SubItem { get; set; }
        public string SubItemName { get; set; }
        public List<InnerSubMenuItem> ListOfInnerSubMenuItem { get; set; }
    }
    public class InnerSubMenuItem
    {
        public string SubMenuNames { get; set; }

    }
}