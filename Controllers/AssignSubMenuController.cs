using NibsMVC.EDMX;
using NibsMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Web.Security;
using NibsMVC.Repository;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CarlosAg.ExcelXmlWriter;
using CarlosAg.Utils;
using HtmlAgilityPack;
using itextsharp;
using iTextSharp;
using itextsharp.pdfa;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace NibsMVC.Controllers
{
    public class AssignSubMenuController : Controller
    {

        NIBSEntities db = new NIBSEntities();
        XMLTablesRepository obj = new XMLTablesRepository();
        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        SqlConnection con;
        SqlCommand cmd;
        UserProfileRepository user = new UserProfileRepository();
        public ActionResult Index()
        {
            return View(user.ShowAllSubMenuList());
        }

        public JsonResult getMenuItem(int id)
        {
            StringBuilder strbuild = new StringBuilder();

            var cat = (from p in db.tblItems
                       where p.CategoryId.Equals(id)
                       select new { Key = p.ItemId, Name = p.Name }).
                      GroupBy(a => a.Key).ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var key in cat)
            {
                list.Add(new SelectListItem { Value = Convert.ToString(key.Key), Text = Convert.ToString(key.FirstOrDefault().Name) });
                //strbuild.Append("<option value='" + key.Key + "'>" + key.FirstOrDefault().Name + "</option>");
            }

            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));

        }
        [HttpGet]
        public ActionResult AssignSubMenuItem(int id=0)
        {
            AssignSubMenuModel model = new AssignSubMenuModel();

            IEnumerable<SelectListItem> MainMenuCategory = (from n in db.tblCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.CategoryId.ToString() });
            ViewBag.MainMenu = new SelectList(MainMenuCategory, "Value", "Text", "MenuCategory");

            IEnumerable<SelectListItem> SubMenu = (from n in db.tblSubItems select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.SubItemId.ToString() });
            ViewBag.SubMenu = new SelectList(SubMenu, "Value", "Text", "SubMenu");

            return View(user.EditSubMenu(id));
        }
        [HttpPost]
        public ActionResult AssignSubMenuItem(AssignSubMenuModel model,int id=0)
        {
            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();

            if (id==0)
            {
                sb.Append("insert into tblAssignSubMenuItem values('"+model.MainItem+",'"+model.SubItem+"')");
            }
            else if(id!=0)
            {
                sb.Append("update tblAssignSubMenuItem set subitemid='" + model.MainItem +"' where ID='"+id);
            }

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;

            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("Index");

            //var Data = user.AssignSubMenu(model);
            //TempData["Error"] = Data;
            
            //return RedirectToAction("Index");
        }
        public ActionResult Delete(int id=0)
        {
            var data = user.DeleteSubMenu(id);
            TempData["Error"] = data;
            return RedirectToAction("Index");
        }
    }
}