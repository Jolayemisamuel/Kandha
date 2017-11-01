using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Request.Cookies["UserSetting"] != null && Request.Cookies["UserSetting"]["UserName"] != null && Request.Cookies["UserSetting"]["UserName"] != "")
            {
                if (Session["UserName"] == null || Session["UserName"].ToString() == "")
                {
               
                    Session["UserName"] = Request.Cookies["UserSetting"]["UserName"];
                }
            }
            base.OnActionExecuting(filterContext);
        }
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // Check if this action has NotAuthorizeAttribute
        //    object[] attributes = filterContext.ActionDescriptor.GetCustomAttributes(true);
        //    if (attributes.Any(a => a is NotAuthorizeAttribute)) return;

        //    // Must login
        //    if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        filterContext.Result = new HttpUnauthorizedResult();
        //    }
        //}
        //public class NotAuthorizeAttribute : FilterAttribute
        //{
        //    // Does nothing, just used for decoration
        //}

    }
}
