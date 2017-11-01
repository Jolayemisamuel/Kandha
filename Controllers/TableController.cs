using NibsMVC.EDMX;
using NibsMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using WebMatrix.WebData;

namespace NibsMVC.Controllers
{
    public class TableController : Controller
    {
        //
        // GET: /Table/
        NIBSEntities db = new NIBSEntities();
        public ActionResult Index()
        {
            int id = getOutletId();
            var tabledata = (from p in db.tblTableMasters where p.OutletId == id select p).ToList();
            List<TableModel> list = new List<TableModel>();
            foreach (var item in tabledata)
            {
                TableModel model = new TableModel();
                model.TableId = item.TableId;
                model.OutletId = item.OutletId;
                model.TableNo = item.TableNo;
                model.AcType = item.AcType;
                list.Add(model);
            }

            return View(list);
        }
        public ActionResult Create()
        {
            var list = new SelectList(new[] 
                                      {
                                        new { Value = "AC", Text = "AC" },
                                        new { Value = "Non AC", Text = "Non AC" },
   
                                      }, "Value", "Text","AC");

            ViewBag.AcType = list;

            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CheckTable(TableModel model)
        {
            
            int OutletId = getOutletId();
            var user = db.tblTableMasters.Where(a => a.TableNo == model.TableNo   && a.OutletId == OutletId && a.AcType == model.AcType ).FirstOrDefault();
            
            return Json(user == null);
        }
        [HttpPost]
        public ActionResult Create(TableModel model)
        {
            try
            {
                int OutletId = getOutletId();
                var user = db.tblTableMasters.Where(a => a.TableNo == model.TableNo && a.OutletId == OutletId && a.AcType == model.AcType).FirstOrDefault();

                if (user == null)
                {
                    tblTableMaster tb = new tblTableMaster();
                    tb.OutletId = OutletId;
                    tb.TableNo = model.TableNo;
                    tb.AcType = model.AcType;
                    db.tblTableMasters.Add(tb);
                    db.SaveChanges();
                    TempData["erortbl"] = "OK";
                    TempData["deltbl"] = "The table has been added";
                    return RedirectToAction("Index", "Table");
                }
                else
                {
                    TempData["erortbl"] = "Already Exists !";
                    TempData["deltbl"] = "Already Exists !";
                    return RedirectToAction("Create", "Table");
                }
            }
            catch (Exception ex)
            {
                TempData["erortbl"] = "Wrong";
                TempData["deltbl"] = "something went wrong try again !";
                return RedirectToAction("Create", "Table");
            }


        }
        public ActionResult Delete(int id = 0)
        {
            try
            {
                var type = (from q in db.tblOperators where q.UserId == WebSecurity.CurrentUserId select q.Type).FirstOrDefault();
                if (type == "Manager")
                {
                    var tablNo = string.Empty;
                    var data = db.tblTableMasters.Find(id);
                    if (data != null)
                    {
                        tablNo = data.TableNo.ToString();
                    }
                    var filepath = Server.MapPath("~/xmltables/table" + tablNo + ".xml");
                    XDocument xd = XDocument.Load(filepath);
                    int oulte = getOutletId();
                    var items = from item in xd.Descendants("Items")
                                where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == tablNo.ToString()
                                select item;
                    if (items.Count() == 0)
                    {
                        var deletedata = (from p in db.tblTableMasters where p.TableId.Equals(id) && p.OutletId == WebSecurity.CurrentUserId select p).SingleOrDefault();
                        db.tblTableMasters.Remove(deletedata);
                        db.SaveChanges();
                        TempData["erortbl"] = "OK";
                        TempData["deltbl"] = "The table has been deleted";
                    }
                    else
                    {
                        TempData["erortbl"] = "Wrong";
                        TempData["deltbl"] = "this table in running so please dispatch order first";
                    }

                }
                else if (User.IsInRole("Outlet"))
                {
                    var tablNo = string.Empty;
                    var data = db.tblTableMasters.Find(id);
                    if (data != null)
                    {
                        tablNo = data.TableNo.ToString();
                    }
                    var filepath = Server.MapPath("~/xmltables/table" + tablNo + ".xml");
                    XDocument xd = XDocument.Load(filepath);
                    int oulte = getOutletId();
                    var items = from item in xd.Descendants("Items")
                                where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == tablNo.ToString()
                                select item;
                    if (items.Count() == 0)
                    {
                        var deletedata = (from p in db.tblTableMasters where p.TableId.Equals(id) && p.OutletId == WebSecurity.CurrentUserId select p).SingleOrDefault();
                        db.tblTableMasters.Remove(deletedata);
                        db.SaveChanges();
                        TempData["erortbl"] = "OK";
                        TempData["deltbl"] = "The table has been deleted";
                    }
                    else
                    {
                        TempData["erortbl"] = "Wrong";
                        TempData["deltbl"] = "this table in running so please dispatch order first";
                    }
                }
                else
                {
                    TempData["erortbl"] = "Wrong";

                    TempData["deltbl"] = "You are not authorised to Delete!!";
                }
            }
            catch (Exception ex)
            {
                TempData["deltbl"] = ex.Message;
            }
            return RedirectToAction("Index", "Table");
        }


        public int getOutletId()
        {
            string[] roles = Roles.GetRolesForUser();
            var oulte = 0;
            foreach (var role in roles)
            {
                if (role == "Outlet")
                {
                    oulte = WebSecurity.CurrentUserId;
                }
                else
                {
                    oulte = Convert.ToInt32((from n in db.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }
    }
}
