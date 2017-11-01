using NibsMVC.EDMX;
using NibsMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace NibsMVC.Controllers
{
    public class OperatorController : Controller
    {
        //
        // GET: /Operator/
        NIBSEntities db = new NIBSEntities();
        public ActionResult Index()
        {
            var data = (from p in db.tblOperators where p.OutletId == WebSecurity.CurrentUserId select p).ToList();
            List<OperatorModel> list = new List<OperatorModel>();
            foreach (var item in data)
            {
                OperatorModel model = new OperatorModel();
                model.ContactNo = item.ContactNo;
                model.Name = item.Name;
                model.operatorId = item.Operatorid;
                model.Type = item.Type;
                list.Add(model);

            }
            return View(list);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult checkUserName(string UserName)
        {
            var user = Membership.GetUser(UserName);
            return Json(user == null);
        }
        public ActionResult Delete(int id = 0)
        {
            try
            {
                var datadelete = (from p in db.tblOperators where p.Operatorid.Equals(id) select p).SingleOrDefault();
                db.tblOperators.Remove(datadelete);
                db.SaveChanges();
                TempData["optrerror"] = "Delete Successfully !!";
                TempData["Ok"] = "OK";
            }
            catch (Exception ex)
            {
                TempData["optrerror"] = ex.Message;
                TempData["Ok"] = "Wrong";
            }
            return RedirectToAction("Index", "Operator");
        }
        public ActionResult Create(int id = 0)
        {
            OperatorModel model = new OperatorModel();
            if (id > 0)
            {
                var data = (from p in db.tblOperators where p.Operatorid.Equals(id) select p).SingleOrDefault();
               
                UsersContext context = new UsersContext();
                var email = context.UserProfiles.Where(a => a.UserId == data.UserId).FirstOrDefault();
                model.Email = email.Email;
                model.Active = data.Active;
                model.ContactNo = data.ContactNo;
                model.Name = data.Name;
                model.UserName = email.UserName;
                model.UserId = data.UserId.Value;
                model.operatorId = data.Operatorid;
                model.Type = data.Type;
                model.operatorId = data.Operatorid;
                return View(model);
            }
            else
            {
                return View(model);
            }

        }

        [HttpPost]
        public ActionResult Create(OperatorModel model)
        {
            try {
                
                tblOperator tb = new tblOperator();
                int UserId = 0;
                if (model.operatorId==0)
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email });
                    Roles.AddUserToRole(model.UserName, "Operator");
                 UserId = WebSecurity.GetUserId(model.UserName);
                }
               
                if (model.operatorId >0)
                {
                    tb = (from p in db.tblOperators where p.Operatorid.Equals(model.operatorId) select p).SingleOrDefault();
                    UpdateOperatorEmail(model.Email, model.UserId);
                }
                tb.Active = model.Active;
                tb.ContactNo = model.ContactNo;
                tb.Name = model.Name;
                tb.Type = model.Type;
                
                tb.OutletId = WebSecurity.CurrentUserId;
                if (model.operatorId==0)
                {
                     tb.UserId = UserId;
                }
                if (model.operatorId > 0)
                {
                    db.SaveChanges();
                    TempData["optrerror"] = "Record Updated Successfully...";
                    TempData["Ok"] = "OK";
                }
                else {
                    db.tblOperators.Add(tb);
                    db.SaveChanges();
                    TempData["optrerror"] = "Record Saved Successfully...";
                    TempData["Ok"] = "OK";

                }
            }
            catch(Exception ex) {
                TempData["optrerror"] = ex.Message;
                TempData["Ok"] = "Wrong";
            }
            return RedirectToAction("Index", "Operator");
        }
        public void UpdateOperatorEmail(string email,int UserId)
        {

            UsersContext context = new UsersContext();
            UserProfile tb = new UserProfile();
            tb = context.UserProfiles.Find(UserId);
            tb.Email = email;
            context.SaveChanges();

        }

    }
}
