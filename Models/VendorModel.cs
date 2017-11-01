using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class VendorModel
    {
    }
    public class VenderRegistrationModel
    {
        [Key]
        public int VendorId { get; set; }
        [Required (ErrorMessage="please enter Name")]
        public string Name { get; set; }
           //[Required(ErrorMessage = "please enter Contact number")]
        public string ContactA { get; set; }
        public string ContactB { get; set; }
           [Required(ErrorMessage = "please enter Address")]
        public string Address { get; set; }
        public string TinNo { get; set; }
        public DateTime RegistrationDate { get; set; }
           //[Required(ErrorMessage = "please enter Email")]
        public string Email { get; set; }
        public int OutletId { get; set; }
        public bool Active { get; set; }

        // ****** New Feillds Added by Amjath 18.07.17 *********//
        public string GSTin { get; set; }
        public string Pan { get; set; }
        public decimal  ServiceTax { get; set; }

        //bank Details
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
        public string IfscCode { get; set; }
        public string Branch { get; set; }
        public string Paymentcycle { get; set; }

    }
}