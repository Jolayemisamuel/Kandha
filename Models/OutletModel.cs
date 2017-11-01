using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Models
{
    public class OutletModel
    {
        public int OutletId { get; set; }
        public string Name { get; set; }
        public string ContactA { get; set; }
        public string ContactB { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string TinNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string OutletType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string ServiceTaxNo { get; set; }
    }
    public class AssignOfferOutletModel
    {
        public int Id   { get; set; }
        public int UserId { get; set; }
        public int OfferId { get; set; }
        public List<SelectListItem> getOutletList { get; set; }
    }
    public class GetOfferListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class AssignOfferToOutletModel
    {
        public string UserName { get; set; }
        public string OfferName { get; set; }
        public int OfferId { get; set; }
        public int Id { get; set; }
        public string Items { get; set; }
        public string Days { get; set; }
        public decimal Discount { get; set; }
    }
    public class ProfileModel
    {
        public int OutletId { get; set; }
        public string Name { get; set; }
        public string ContactA { get; set; }
        public string ContactB { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string TinNo { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string OutletType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string ProfileImage { get; set; }
    }
    public class lockModel
    {
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
    public class ChangeInventoryModel
    {
        public int OutletId { get; set; }
        public bool IsInventory { get; set; }
    }
   
}