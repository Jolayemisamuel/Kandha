using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Models
{
    public class CategoryItemModel
    {
    }
    public class GetUnitCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
    }
    public class AddItemModel
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public int MinimumQuantity { get; set; }
        //public string Unit { get; set; }
        public int ItemCategoryId { get; set; }
        public int DepartmentId { get; set; }
        public string ItemImage { get; set; }
        public string ItemCode { get; set; }
        public bool Active { get; set; }
        public int SearchCategoryId { get; set; }
        public int SearchDepartmentId { get; set; }
        public List<SelectListItem> lstofCategory { get; set; }

        public List<SelectListItem> lstofDepartment { get; set; }
    }

    public class AddSubItemModel
    {
        public int subItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }



    public class AdminBlockItemModel
    {
        public List<SelectListItem> lstofCategory { get; set; }
        public List<SelectListItem> lstofDepartment { get; set; }
        public List<lstOfBlockItems> lstOfItems { get; set; }
        public int CategoryId { get; set; }
    }
    public class AdminBlockSubItemModel
    {
     
        public List<lstOfBlockSubItems> lstOfSubItems { get; set; }
      
    }
    public class lstOfBlockSubItems
    {
        public int SubItemId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
       
    }


    public class lstOfBlockItems
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }

        public string Department { get; set; }
        public int DepartmentId { get; set; }
        public string ItemImage { get; set; }
        public string ItemCode { get; set; }
    }
    public class MenuAssignItemsModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        //public decimal HalfPrice { get; set; }
        public decimal FullPrice { get; set; }
        public bool Assigned { get; set; }
        public int BasePriceId { get; set; }
    }
    public class UnitModel1
    {
        public int Unitid { get; set; }
        public string UnitName { get; set; }
        public bool Active { get; set; }
    }
}