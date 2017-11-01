using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class ServiceChargesModel
    {
        public int ServiceId { get; set; }
        public String ServicName { get; set; }
        public decimal ServiceCharges { get; set; }
    }
}