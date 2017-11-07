using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class BasePriceModel
    {
        public int BasePriceId { get; set; }
        public int CategoryIds { get; set; }
        public int ItemId { get; set; }
        public decimal FullPrice { get; set; }
        //public decimal HalfPrice { get; set; }
        public decimal Vat { get; set; }
        public string BasePriceData { get; set; }
        public string categoryname { get; set; }
    }
    public class BasePriceEditModel
    {
        public int[] EditItemId { get; set; }
        //public decimal[] EditHalfPrice { get; set; }
        public decimal[] EditFullPrice { get; set; }
        public decimal[] EditVat { get; set; }
        public int CategoryIds { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }

        public List<GetAllItemList> getAllItemList { get; set; }
    }
    public class BasePriceEditItemList
    {
        public int ItemId { get; set; }
        //public decimal HalfPrice { get; set; }
        public decimal FullPrice { get; set; }
        public string ItemName { get; set; }
    }
    public class GetAllItemList
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        //public decimal HalfPrice { get; set; }
        public decimal FullPrice { get; set; }
        public decimal Vat { get; set; }
        public string Type { get; set; }
    }
}