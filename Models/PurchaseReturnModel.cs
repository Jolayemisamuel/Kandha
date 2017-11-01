using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class PurchaseReturnModel
    {
        public int PurchaseReturnId { get; set; }
        public int VendorId { get; set; }
        public int RawMaterialId { get; set; }
        public string ReturnStatuss { get; set; }
        public string Unit { get; set; }
        public string  ReturnDescription { get; set; }
        public decimal ReturnQuantity { get; set; }
        public string ItemName { get; set; }
        public DateTime ReturnDate { get; set; }
        
    }
}