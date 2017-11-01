using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace NibsMVC.Controllers
{

    public class MenuController : Controller
    {
        //
        // GET: /Menu/
        NIBSEntities db = new NIBSEntities();
        [HttpPost]
        public string MenuAssignCatItem(int CategoryId, int OutletId)
        {
            AddItemRepository dis = new AddItemRepository();
            List<MenuAssignItemsModel> assignitem = dis.Itemwise(CategoryId, OutletId);
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='table table-bordered' id='tblmenuitems'>");
            sb.Append("<thead>");
            sb.Append("<tr><th>Select</th><th>Full Price</th><th>Half Price</th><th></th>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            foreach (var item in assignitem)
            {

                if (item != null)
                {
                    if (item.Assigned == true)
                    {
                        sb.Append("<tr><td><input type='checkbox' id='" + item.ItemId + "' name='ItemId' checked value='" + item.ItemId + "' class='checkbox'>" + item.ItemName + "</td>");
                        sb.Append("<td><input type='textbox' class='form-control'name='FullPrice' style='margin:2px 6px'value='" + item.FullPrice + "' id='" + item.ItemId + "' ></td>");
                        sb.Append("<td><input type='textbox' class='form-control' name='HalfPrice' style='margin:2px 6px' value='" + item.HalfPrice + "'  id='" + item.ItemId + "' >");
                        sb.Append("<input type='hidden' name='BasePriceId' value='" + item.BasePriceId + "'/></td></tr>");
                    }
                    else
                    {
                        sb.Append("<tr><td><input type='checkbox' id='" + item.ItemId + "' name='ItemId' value='" + item.ItemId + "' class='checkbox'>" + item.ItemName + "</td>");
                        sb.Append("<td><input type='textbox' class='form-control'name='FullPrice' style='margin:2px 6px'value='" + item.FullPrice + "' id='" + item.ItemId + "' readonly></td>");
                        sb.Append("<td><input type='textbox' class='form-control' name='HalfPrice' style='margin:2px 6px' value='" + item.HalfPrice + "'  id='" + item.ItemId + "' readonly>");
                        sb.Append("<input type='hidden' name='BasePriceId' value='" + item.BasePriceId + "'/></td></tr>");
                    }

                    //
                }


            }
            sb.Append("</tbody>");
            sb.Append("<table>");
            return sb.ToString();
        }
        //public JsonResult CatItem(int id)
        //{
        //    //DistributorRepository dis = new DistributorRepository();
        //    //AddItemRepository dis = new AddItemRepository();
        //    //List<tblItem> assignitem = dis.Itemwise(id);
        //    //bool Disname =;
        //    //return Json(Disname, JsonRequestBehavior.AllowGet);
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<table class='table table-bordered' id='tblmenuitems'>");
        //    sb.Append("<thead>");
        //    sb.Append("<tr><th>Select</th><th>Full Price</th><th>Half Price</th><th></th>");
        //    sb.Append("</thead>");
        //    sb.Append("<tbody>");
        //    foreach (var item in assignitem)
        //    {
        //        sb.Append("<tr><td><input type='checkbox' id='" + item.ItemId + "' name='ItemId' value='" + item.ItemId + "' class='checkbox'>" + item.Name + "</td>");
        //        sb.Append("<td><input type='textbox' class='form-control'name='FullPrice' style='margin:2px 6px'value='" + item.tblBasePriceItems.First().FullPrice + "' id='" + item.ItemId + "' readonly></td>");
        //        sb.Append("<td><input type='textbox' class='form-control' name='HalfPrice' style='margin:2px 6px' value='" + item.tblBasePriceItems.First().HalfPrice + "'  id='" + item.ItemId + "' readonly>");
        //        sb.Append("<input type='hidden' name='BasePriceId' value='" + item.tblBasePriceItems.FirstOrDefault().BasePriceId + "'/></td></tr>");

        //    }
        //    sb.Append("</tbody>");
        //    sb.Append("<table>");


        //        return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Index()
        {
            int outletid = getOutletId();
            var assignmenudata = (from p in db.tblMenuOutlets where p.OutletId == outletid select p).ToList();
            List<MenuModel> list = new List<MenuModel>();
            foreach (var item in assignmenudata)
            {
                MenuModel model = new MenuModel();
                model.CategoryId = item.CategoryId;
                model.OutletId = item.OutletId;
                model.ItemId = item.ItemId;
                model.FullPrice = item.FullPrice;
                model.HalfPrice = item.HalfPrice;
                model.MenuOutletId = item.MenuOutletId;
                model.BasePricId = Convert.ToInt32(item.BasePriceId);
                model.ItemName = item.tblItem.Name;
                model.CategoryName = item.tblItem.tblCategory.Name;
                model.Vat = item.tblBasePriceItem.Vat;
                list.Add(model);
            }
            return View(list);
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
        public ActionResult Create()
        {

            CreateView();

            return View();
        }
        private void CreateView()
        {

            IEnumerable<SelectListItem> OutletList = (from m in db.tblOutlets where m.OutletType == "O" && m.Active == true select m).AsEnumerable().Select(m => new SelectListItem() { Text = m.Name, Value = m.OutletId.ToString() });
            ViewBag.OutletLists = new SelectList(OutletList, "Value", "Text");
            IEnumerable<SelectListItem> Categorylist = (from q in db.tblCategories where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.CategoryId.ToString() });

            ViewBag.Categorylists = new SelectList(Categorylist, "Value", "Text");
        }
        //[HttpPost]
        //public ActionResult Create(MenuModel Model)
        //{
        //    try
        //    {

        //        tblMenuOutlet tb = new tblMenuOutlet();                
        //        string[] Data = Model.assigndata.Split('-');
        //        string[] DataItem = Data[1].Split(';');
        //        if (DataItem.ToString() != string.Empty)
        //        {
        //            for (int i = 0; i <DataItem.Length ; i++)
        //            {
        //                if (DataItem[i] != null && DataItem[i] != "")
        //                {
        //                    string[] DataItems = DataItem[i].Split('^');
        //                    tb.OutletId = Model.OutletId;
        //                    tb.CategoryId = Convert.ToInt32(Data[0]); 
        //                        tb.ItemId = Convert.ToInt32(DataItems[0]);
        //                        tb.FullPrice = Convert.ToDecimal(DataItems[1]);
        //                        tb.HalfPrice = Convert.ToDecimal(DataItems[2]);
        //                        tb.BasePriceId = Convert.ToInt32(DataItems[3]);

        //                        db.tblMenuOutlets.Add(tb);
        //                        db.SaveChanges();
        //                    }
        //            }
        //        }
        //        TempData["menuerror"] = "Inserted Successfully !";
        //                   }
        //    catch(Exception ex)
        //    {
        //        TempData["menuerror"] = ex.Message;
        //    }

        //    CreateView();
        //    return View();
        //}

        [HttpPost]
        public ActionResult Create(AssignMenuModel model)
        {
            try
            {
                var delete = (from p in db.tblMenuOutlets where p.CategoryId == model.CategoryId && p.OutletId == model.OutletId select p).ToList();
                foreach (var item in delete)
                {
                    db.tblMenuOutlets.Remove(item);
                    db.SaveChanges();
                }
                tblMenuOutlet tb = new tblMenuOutlet();
                for (int i = 0; i < model.ItemId.Length; i++)
                {
                    tb.BasePriceId = model.BasePriceId[i];
                    tb.CategoryId = model.CategoryId;
                    tb.FullPrice = model.FullPrice[i];
                    tb.HalfPrice = model.HalfPrice[i];
                    tb.ItemId = model.ItemId[i];
                    tb.OutletId = model.OutletId;
                    db.tblMenuOutlets.Add(tb);
                    db.SaveChanges();
                }
                TempData["menuerror"] = "Menu Assign Succesfully...";
                return RedirectToAction("MenuReport", "Admin");
            }
            catch
            {
                TempData["menuerror"] = "Try Agian !";
                return RedirectToAction("MenuReport", "Admin");
            }

        }

    }


}

