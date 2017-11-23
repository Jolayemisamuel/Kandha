using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class StockTransferModel
    {
        public int OutletId { get; set; }
        public int TransferId { get; set; }
        public int RawMaterialId { get; set; }
        public int CategoryId { get; set; }
        public decimal TransferQuantity { get; set; }
        public DateTime TransferDate { get; set; }
        public DateTime PurchaseDate { get; set; }

        public string Stocktransdata { get; set; }
        public string categoryname { get; set; }
        public int SenderOutletId { get; set; }
        public int ReciverOutletId { get; set; }
        public int ReturnId { get; set; }
       // public string Type { get; set; }
        //public int ItemId { get; set; }
        //public int CategoryId { get; set; }
        public decimal ReturnQuantity { get; set; }
        public DateTime ReturnDate { get; set; }
        public string RStatus { get; set; }
        public string ReturnDescription { get; set; }
        public int ReturnSenderOutId { get; set; }
        public int ReturnReciverOutId { get; set; }
      
        public string Updatedata { get; set; }
    }
    public class OutletStockTransferModel
    {
        public int RecieverOutletId  { get; set; }
        public int DepartmentId { get; set; }
        public string TransferUnit { get; set; }
        public int RawMaterialId { get; set; }
        public int TransferQuantity { get; set; }
        public int RawcategoryId { get; set; }
        public DateTime  PurchseDate { get; set; }
        public int currentstock { get; set; }
        public int currentvalue { get; set; }
    }
    public class AddExtraStock 
    {
        public int RawcategoryId { get; set; }
        public int RawId { get; set; }
        public string RawMaterial { get; set; }
        [Required(ErrorMessage = "please enter Stock")]
        public decimal currentstock { get; set; }
        [Required(ErrorMessage = "please enter Rate")]
        public decimal currentvalue { get; set; }

     [DisplayFormat(ApplyFormatInEditMode = true,  DataFormatString = "{0:dd-MMM-yyyy}")]
        public string  stockDate { get; set; }
    }
    public class OutletStockTransferIndexModel
    {
        public int SenderOutletName { get; set; }
        public int RecieverOutletName { get; set; }

        public String  TransferDate { get; set; }
        public string Department { get; set; }
        public string RawCategory { get; set; }
        public int RawMaterialId { get; set; }
        public string RawMaterial { get; set; }

        public string Unit { get; set; }

        public decimal TransferQty { get; set; }

        //public List<GetStockTransferItemList> getStockTransferItemList { get; set; }
    }
    public class GetStockTransferItemList
    {
        public string RowMaterialName { get; set; }
        public string TransferDate { get; set; }
        public decimal TranferQuantity { get; set; }
    }
    public class outeltStockReturnModel
    {
        public int TransferId { get; set; }
        public string senderOutletName { get; set; }
        public int senderOutletId { get; set; }
        public string RecieveroutletName { get; set; }
        public int RecieveroutletId { get; set; }
        public int RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public decimal TransferQuantity { get; set; }
        public DateTime StockReturnDate { get; set; }
        public decimal ReturnQuantity { get; set; }
        public string Reasion { get; set; }
        public string Unit { get; set; }
    }
}