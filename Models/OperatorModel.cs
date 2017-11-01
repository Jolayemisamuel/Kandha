using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NibsMVC.Models
{
    public class OperatorModel
    {
        public int operatorId { get; set; }
        [Required (ErrorMessage="please select type")]
        public string Type { get; set; }
        [StringLength(15, ErrorMessage = " Name must be a string with a minimum length of 6 and a maximum length of 15.", MinimumLength = 5)]
        [Required(ErrorMessage = "please enter name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "please enter username")]
        [System.Web.Mvc.Remote("checkUserName", "Operator", HttpMethod = "POST", ErrorMessage = "user already Exits")]
        [StringLength(15, ErrorMessage = "User Name must be a string with a minimum length of 6 and a maximum length of 15.", MinimumLength = 6)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "please enter contact no")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Table No must be a natural number")]
        [StringLength(10,ErrorMessage="please enter correct mobile number",MinimumLength=10)]
        
        public string ContactNo { get; set; }
        [Required(ErrorMessage = "please enter password")]
        [StringLength(15, ErrorMessage = " Password must be a string with a minimum length of 6 and a maximum length of 15.", MinimumLength = 6)]
        public string Password { get; set; }
        public bool Active { get; set; }
        public int Outletid { get; set; }
         [Required(ErrorMessage = "please enter email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",ErrorMessage="entered email in invalid")]
        public string Email { get; set; }
        public int UserId { get; set; }
    }
}