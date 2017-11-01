using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class TableModel
    {
        public int TableId { get; set; }
        public int OutletId { get; set; }
        [Required(ErrorMessage="please enter table number")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Table No must be a natural number")]
        //[System.Web.Mvc.Remote("CheckTable", "Table", HttpMethod = "POST", ErrorMessage = "Table No already Exits")]
        public int TableNo { get; set; }

         //[System.Web.Mvc.Remote("CheckTable", "Table", HttpMethod = "POST", ErrorMessage = "Table No already Exits")]
        public string AcType { get; set; }
    }
}