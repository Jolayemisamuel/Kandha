using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Models
{
    public class CategoryWiseReportModel
    {
        public Nullable<DateTime> Datefrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public int CategoryId { get; set; }
        public List<SelectListItem> getAllCategory { get; set; }
        public List<ItemsReportModel> getAllItem { get; set; }
    }
    public class ItemsReportModel
    {
        public string itemName { get; set; }
        public int Quantity { get; set; }
        public decimal NetAmount { get; set; }
    }
}