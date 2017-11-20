using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class MenuModel
    {
        public int MenuOutletId { get; set; }
        public int OutletId { get; set; }
        public int ItemId { get; set; }
        public decimal FullPrice { get; set; }
        public decimal HalfPrice { get; set; }
        public int CategoryId { get; set; }
        public string assigndata { get; set; }
        public int BasePricId { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public decimal Vat { get; set; }
      
    }
    public class AssignMenuModel
    {
        public int OutletId { get; set; }
        public int CategoryId { get; set; }
        public int[] ItemId { get; set; }
        public decimal[] FullPrice { get; set; }
        public decimal[] HalfPrice { get; set; }
        public int[] BasePriceId { get; set; }
    }

    public class AssignRawSubItemModel
    {
        
        public int SubItemId { get; set; }
        public int Portion { get; set; }
        public DateTime  DateTime { get; set; }
        public int[] RawMaterialId { get; set; }
        public decimal[] qty { get; set; }
        
    }
}