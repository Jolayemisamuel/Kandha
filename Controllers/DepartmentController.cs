using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Controllers
{
     [Authorize(Roles = "admin,outlet,operator")]
    public class DepartmentController : Controller
    {

        KitchenItemRepository obj = new KitchenItemRepository();
        NIBSEntities db = new NIBSEntities();
        // GET: Department
        public ActionResult Index()
        {
            return View(obj.ShowAllDepartmentList());
        }

      
        // GET: Department/Create
        public ActionResult Create(int Id = 0)
        {
            return View(obj.EditDepartment(Id));
        }

        // POST: Department/Create
        [HttpPost]
        public ActionResult Create(DepartmentModel model)
        {
            var Data = obj.SaveDepartment(model);
            TempData["Error"] = Data;
            return RedirectToAction("index");
        }

      

        // GET: Department/Delete/5
        public ActionResult Delete(int Id = 0)
        {
            var data = obj.DeleteDepartment (Id);
            TempData["Error"] = data;
            return RedirectToAction("Index");
        }



    }
}
