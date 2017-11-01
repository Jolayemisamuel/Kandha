using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    
    public class KitchenStockModel
    {
        //public int KitchenStockId { get; set; }
        //public int RawMaterialId { get; set; }
        //public decimal Quantity { get; set; }
        //public string Unit { get; set; }
        //public int OutletId { get; set; }
        //public decimal RealQty { get; set; }
        //public string RealUnit { get; set; }
        public int bill_number { get; set; }
        public string item_description { get; set; }
        public string purchase_category { get; set; }
        public string umo { get; set; }
        public int available_stock { get; set; }
        public int new_purchase { get; set; }
        public int total_stock { get; set; }

    }
    public class KitchenStockAddModel
    {
        [Required(ErrorMessage = "please enter Item Description")]
        public string Item_Description { get; set; }
        [Required(ErrorMessage = "please enter Purchase Category")]
        public string Purchase_Category { get; set; }
        [Required(ErrorMessage = "please enter Product Unit")]
        public string UMO { get; set; }
        public int available_stock { get; set; }
        
        public int bill_number { get; set; }

    }
}