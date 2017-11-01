using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Repository;
using NibsMVC.Models;
using System.Web.Security;
using NibsMVC.Filters;
using WebMatrix.WebData;
using NibsMVC.EDMX;
using System.Xml.Linq;
namespace NibsMVC.Controllers
{

    public class HomeController : Controller
    {
        AdminHomeRepository repo = new AdminHomeRepository();
        NIBSEntities entities = new NIBSEntities();
        OutletHomeRepository outrepo = new OutletHomeRepository();
        [Authorize(Roles = "admin,Outlet,Operator")]
        public ActionResult Index()
        {
            string[] roles = Roles.GetRolesForUser();
            foreach (var role in roles)
            {
                if (role == "admin")
                {
                    return View(repo.getAdminHomeReports());
                }
                else if (role == "Outlet")
                {
                    ViewBag.RunningTable = getRunningTable();
                    return View(outrepo.getOutletHomeReports());
                }
                else
                {
                    return View(outrepo.getAllBillData());
                }
            }
            return View();

        }

        
        public List<GetRunningTable> getRunningTable()
        {
            List<GetRunningTable> lst = new List<GetRunningTable>();
            foreach (var item in entities.tblTableMasters.Where(a => a.OutletId == WebSecurity.CurrentUserId).ToList())
            {
                //var filepath = Server.MapPath("/xmltables/table" + TableNo + ".xml");
                GetRunningTable model = new GetRunningTable();

                var path = Server.MapPath("/xmltables/table" + item.TableNo + ".xml");
                if (System.IO.File.Exists(path))
                {
                    XDocument xd = XDocument.Load(path);
                    var items = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == WebSecurity.CurrentUserId.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                                 select p).ToList();
                    if (items.Count > 0)
                    {
                        model.TableNo = item.TableNo;
                        lst.Add(model);
                    }
                }

                
            }
            return lst;
        }
        [NonAction]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        [NonAction]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ChangeInventory()
        {

            return View(outrepo.getinventory());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeInventory(ChangeInventoryModel model)
        {
            outrepo.UpdateInventory(model);
            return RedirectToAction("Index","Home");
        }
    }
}
