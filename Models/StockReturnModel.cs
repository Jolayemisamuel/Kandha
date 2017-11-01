using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class StockReturnModel
    {
        public int ReturnId { get; set; }
        public int ReturnQuantity { get; set; }
        public DateTime ReturnDate { get; set; }
        public string RStatus { get; set; }
        public string ReturnDescription { get; set; }
        public int SenderOutId { get; set; }
        public int ReciverOutId { get; set; }
        public int RawMaterialId { get; set; }

        
    }
}