using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Models;
using NibsMVC.EDMX;
using WebMatrix.WebData;
using System.Web.Security;

namespace NibsMVC.Controllers
{
    public class LockController : Controller
    {
        NIBSEntities db = new NIBSEntities();
        [Authorize]
        public ActionResult Index()
        {
            int id = WebSecurity.GetUserId(WebSecurity.CurrentUserName);
            LoginModel model = new LoginModel();
            if(id<=0)
            {
                Response.Redirect("/Account/Login");
            }
            else
            {
                var data = db.tblOutlets.Where(a => a.OutletId == id).SingleOrDefault();
               
                model.UserName = data.UserName;
                //model.Name = data.Name;
                //model.Email = data.Email;
                //model.ProfileImage = data.ProfileImage;
                ViewBag.Name = data.Name;
                ViewBag.Email = data.Email;
                ViewBag.ProfileImage = data.ProfileImage;
                WebSecurity.Logout();
            }
           
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LockLogin(lockModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //var data = (from p in db.tblOutlets where p.UserName == model.UserName && p.Password == model.Password select p).SingleOrDefault();
                //if (data != null)
                //{
                //    Session["UserName"] = model.UserName;

                //    return RedirectToAction("Index", "Home");
                //}
                if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                {
                    Session["UserName"] = model.UserName;
                    if (model.UserName == "admin")
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    else if (Roles.IsUserInRole("Outlet"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        HttpCookie usersetting = new HttpCookie("UserSetting");
                        usersetting["UserType"] = "Operator";
                        usersetting["UserName"] = model.UserName;

                        usersetting.Expires = DateTime.Now.AddDays(1d);
                        Response.Cookies.Add(usersetting);

                        return RedirectToAction("Billing", "Billing");
                    }

                }
                else
                {
                    TempData["Error"] = "password provided is incorrect.";
                    return RedirectToAction("Index");
                }
                
            }


            // If we got this far, something failed, redisplay form

            return View(model);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}
