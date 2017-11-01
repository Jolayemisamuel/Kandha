using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace NibsMVC.Controllers
{
    public class StockTransferController : Controller
    {
        //
        // GET: /StockTransfer/
        NIBSEntities db = new NIBSEntities();
        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        SqlConnection con;
        SqlCommand cmd;
        ConversionRepository convert = new ConversionRepository();
        KitchenItemRepository obj1 = new KitchenItemRepository();
        XMLTablesRepository obj = new XMLTablesRepository();
        public JsonResult OutletDetails(int id)
        {
            AddItemRepository outlist = new AddItemRepository();
            List<tblOutlet> outletdetails = outlist.Outletwise(id);
            StringBuilder od = new StringBuilder();
            od.Append("<table class='table table-bordered' id='tblmenuitems'>");
            od.Append("<thead>");
            od.Append("<tr><th>Name</th><th>Contact</th><th>Address</th><th>Email</th><th>Tin No</th><th>City</th>");
            od.Append("</thead>");
            od.Append("<tbody>");
            //sb.Append("<label class='col-md-12'>Select Item</label><label class='col-md-12'>Full Price</label><label class='col-md-12'>Half Price</label>");


            foreach (var item in outletdetails)
            {
                od.Append("<tr><td><input type='textbox' class='form-control' value='" + item.Name + "' readonly></td><td><input type='textbox' class='form-control' value='" + item.ContactA + "' readonly></td><td><input type='textbox' class='form-control' value='" + item.Address + "' readonly></td><td><input type='textbox' class='form-control' value='" + item.Email + "' readonly></td><td><input type='textbox' class='form-control' value='" + item.TinNo + "' readonly></td><td><input type='textbox' class='form-control' value='" + item.City + "' readonly></td></tr>");
            }
            od.Append("</tbody>");
            od.Append("<table>");
            return Json(od.ToString(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Stockavailability(string Id)
        {

            if (Id != null && Id != "")
            {
                int rawMaterialId = Convert.ToInt32(Id);
                int OutletId = obj.getOutletId();
                var quenti = (from p in db.tbl_KitchenStock where p.RawMaterialId == rawMaterialId && p.OutletId == OutletId select new { Unit = p.Unit, Quantity = p.Quantity }).FirstOrDefault();
                
                decimal aval = convert.ReturnConvertValues(quenti.Unit, quenti.Quantity);

                int MatId = Convert.ToInt32(Id);
                string result = (from p in db.tbl_RawMaterials
                                 where p.RawMaterialId == MatId
                                 select p.units).SingleOrDefault();


                return Json(aval.ToString() + '^' + result, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public JsonResult purchaseavailablestock(string Id)
        {

            Id = Id.Replace("dot", ".");
            string[] remaingdata = Id.Split(';');
            string[] remaingdatas;
            if (remaingdata.ToString() != string.Empty)
            {
                for (int i = 0; i < remaingdata.Length; i++)
                {
                    if (remaingdata[i] != null && remaingdata[i] != "")
                    {
                        remaingdatas = remaingdata[i].Split('^');
                        var id = Convert.ToInt32(remaingdatas[0]);

                        Decimal amount = 0;
                        decimal sendquatity = 0;
                        int MatId = Convert.ToInt32(id);
                        string result = (from p in db.tbl_RawMaterials
                                         where p.RawMaterialId == MatId
                                         select p.units).SingleOrDefault();
                        if ((remaingdatas[3] == "Kgs" || result == "Gms") || (remaingdatas[3] == "Ltr" || remaingdatas[3] == "ML"))
                        {
                            amount = Convert.ToDecimal(remaingdatas[5]) ;
                            sendquatity = Convert.ToDecimal(remaingdatas[1]);
                        }
                        else
                        {
                            amount = Convert.ToDecimal(remaingdatas[5]);
                            sendquatity = Convert.ToDecimal(remaingdatas[1]);

                        }


                        var Reciverout = Convert.ToInt32(remaingdatas[2]);

                        var Kitchenstock = (from p in db.tbl_KitchenStock where p.RawMaterialId == id select p).First(); //&& p.OutletId == WebSecurity.CurrentUserId
                        Kitchenstock.Quantity = amount;

                        // incress stock at Reciver outlet--------------------------Pending --------------//
                        var anothrkitcnstock = (from q in db.tbl_KitchenStock where q.RawMaterialId == id select q).FirstOrDefault(); //&& q.OutletId == Reciverout
                        if (anothrkitcnstock == null)
                        {
                            tbl_KitchenStock tb = new tbl_KitchenStock();
                            tb.RawMaterialId = id;
                            tb.Quantity = anothrkitcnstock.Quantity;//sendquatity;
                            tb.OutletId = Reciverout;
                            tb.Unit = Kitchenstock.Unit;
                            db.tbl_KitchenStock.Add(tb);
                            db.SaveChanges();
                        }
                        else
                        {
                            //decimal stock = anothrkitcnstock.Quantity;
                            anothrkitcnstock.Quantity = anothrkitcnstock.Quantity; //+ sendquatity
                        }
                        db.SaveChanges();

                        try
                        {

                            tblTransfer tbl = new tblTransfer();
                            tbl.OutletId = WebSecurity.CurrentUserId;
                            //tbl.PurchaseDate = Convert.ToDateTime(remaingdatas[5]);
                            tbl.RawMaterialId = id;
                            tbl.ReciverOutletId = Reciverout;
                            tbl.SenderOutletId = WebSecurity.CurrentUserId;
                            tbl.TransferDate = DateTime.Now.Date;
                            string departmrnt = remaingdatas[4].ToString();
                            var dept = (from p in db.tbl_Department where p.Department == departmrnt select p).FirstOrDefault();
                            tbl.DepartmentId = dept.DepartmentID;
                            decimal IssQty = Convert.ToDecimal(remaingdatas[1]);
                            if (remaingdatas[3] == "Kgs" || remaingdatas[3] == "Ltr")
                            {
                                tbl.TransferQuantity = Convert.ToDecimal(remaingdatas[1]) ;

                            }
                            else
                            {
                                tbl.TransferQuantity = (Convert.ToDecimal(remaingdatas[1]));

                            }

                            db.tblTransfers.Add(tbl);
                            db.SaveChanges();


                            var TranferId = (from p in db.tblTransfers select p.TransferId).Max();

                            string qry = "select id,MaterialId,Rate,Date,Qty,IssQty=isnull(IssQty,0),table1='os' from tblOpStckRate where MaterialId = " + id + " and isnull(issQty,0)<qty ";
                            qry = qry + " union all ";
                            qry = qry + " select id,MaterialId,Rate,Date,Qty,IssQty=isnull(IssQty,0),table1='gs' from tblgrnstock where MaterialId = " + id + " and isnull(issQty,0)<qty order by date";

                            con = new SqlConnection(webconnection);
                            cmd = new SqlCommand(qry, con);
                            DataTable dt = new DataTable();
                            SqlDataAdapter sda = new SqlDataAdapter(cmd);
                            sda.Fill(dt);

                            qry = "";
                            foreach (DataRow row in dt.Rows)
                            {
                                if (IssQty > 0)
                                {
                                    if (row["table1"].ToString() == "os")
                                    {
                                        if (Convert.ToDecimal(row["Qty"]) - Convert.ToDecimal(row["IssQty"]) <= IssQty)
                                        {

                                            qry = qry + Environment.NewLine + " update tblOpStckRate set issqty = " + Convert.ToDecimal(row["Qty"]) + " where id = " + row["id"].ToString();
                                            IssQty -= (Convert.ToDecimal(row["Qty"]) - Convert.ToDecimal(row["IssQty"]));

                                            qry = qry + Environment.NewLine + " insert into tblTransByStock values(" + TranferId + "," + row["id"].ToString() + ", " + (Convert.ToDecimal(row["Qty"]) - Convert.ToDecimal(row["IssQty"])) + ",'os' )";
                                        }
                                        else
                                        {
                                            qry = qry + Environment.NewLine + " update tblOpStckRate set issqty = isnull(issqty,0) +  " + IssQty + " where id = " + row["id"].ToString();

                                            qry = qry + Environment.NewLine + " insert into tblTransByStock values(" + TranferId + "," + row["id"].ToString() + ", " + IssQty + ",'os' )";
                                            IssQty -= IssQty;
                                        }

                                    }
                                    else
                                    {
                                        if (Convert.ToDecimal(row["Qty"]) - Convert.ToDecimal(row["IssQty"]) <= IssQty)
                                        {

                                            qry = qry + Environment.NewLine + " update tblGRNStock set issqty = " + Convert.ToDecimal(row["Qty"]) + " where id = " + row["id"].ToString();
                                            IssQty -= (Convert.ToDecimal(row["Qty"]) - Convert.ToDecimal(row["IssQty"]));
                                            qry = qry + Environment.NewLine + " insert into tblTransByStock values(" + TranferId + "," + row["id"].ToString() + ", " + (Convert.ToDecimal(row["Qty"]) - Convert.ToDecimal(row["IssQty"])) + ",'gs' )";

                                        }
                                        else
                                        {
                                            qry = qry + Environment.NewLine + " update tblGRNStock set issqty += " + IssQty + " where id = " + row["id"].ToString();

                                            qry = qry + Environment.NewLine + " insert into tblTransByStock values(" + TranferId + "," + row["id"].ToString() + ", " + IssQty + ",'gs' )";
                                            IssQty -= IssQty;
                                        }

                                    }
                                }
                            }

                            con = new SqlConnection(webconnection);
                            cmd = new SqlCommand(qry, con);
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();


                        }
                        catch
                        {
                            ModelState.AddModelError("", "Error Occured !");
                        }


                    }
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {

            IEnumerable<SelectListItem> Departmentlist = (from q in db.tbl_Department where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Department, Value = q.DepartmentID.ToString() });
            ViewBag.Departmentlist = new SelectList(Departmentlist, "Value", "Text", "Department");

            IEnumerable<SelectListItem> CategoryList = (from q in db.RawCategories where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawCategoryID.ToString() });
            ViewBag.Category = new SelectList(CategoryList, "Value", "Text", "Category");

            IEnumerable<SelectListItem> Items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.Items = new SelectList(Items, "Value", "Text", "Items");


            var name = obj.getOutletId();
            //var outid = (from p in db.tblOperators where p.==name select p.OutletId).SingleOrDefault();
            //int id = Convert.ToInt32(outid);
            List<OutletStockTransferIndexModel> list = new List<OutletStockTransferIndexModel>();
            var SenderOutletId = (from p in db.tblTransfers
                                  where p.SenderOutletId == name
                                  group p by p.ReciverOutletId into g
                                  select new
                                  {
                                      ReciverOutletId = g.Key,
                                  }).SingleOrDefault();

            //foreach (var item in SenderOutletId)
            //{


            //List<GetStockTransferItemList> Itemlist = new List<GetStockTransferItemList>();
            var data = db.tblTransfers.ToList(); //.Where(a => a.ReciverOutletId ==99)
            foreach (var items in data)
            {
                OutletStockTransferIndexModel mo = new OutletStockTransferIndexModel();
                mo.RecieverOutletName = 99;// SenderOutletId.ReciverOutletId;
                mo.SenderOutletName = obj.getOutletId();

                mo.Department = items.tbl_Department.Department;
                mo.RawCategory = items.tbl_RawMaterials.RawCategory.Name;
                mo.RawMaterial = items.tbl_RawMaterials.Name;
                mo.TransferDate = items.TransferDate.ToShortDateString();
                mo.TransferQty = items.TransferQuantity;
                mo.Unit = "";
                mo.RawMaterialId = items.RawMaterialId;
                //GetStockTransferItemList model = new GetStockTransferItemList();
                //model.RowMaterialName = items.tbl_RawMaterials.Name;
                //model.TranferQuantity = (items.TransferQuantity) / 1000;
                //model.TransferDate = items.TransferDate.ToShortDateString();
                //Itemlist.Add(model);
                list.Add(mo);
            }
            //mo.getStockTransferItemList = Itemlist;

            // }
            return View(list);
        }

        public ActionResult Delete(int id = 0)
        {
            try
            {

                var deletedata = (from p in db.tblReturns where p.ReturnId == id select p).SingleOrDefault();
                db.tblReturns.Remove(deletedata);
                db.SaveChanges();
                TempData["Retrnstckreport"] = "Delete Successfully !!";
                //TempData["adminerror"] = "Delete Successfully !!";
            }
            catch (Exception ex)
            {
                TempData["Retrnstckreport"] = ex.Message;

            }
            return RedirectToAction("ReturnStockReport", "StockTransfer");
        }

        public ActionResult DeleteStockRecive(int id = 0)
        {
            try
            {
                var usname = WebSecurity.CurrentUserName;
                var type = (from q in db.tblOperators where q.Name.Equals(usname) select q.Type).FirstOrDefault();
                if (type == "Manager")
                {
                    var deletedata = (from p in db.tblTransfers where p.TransferId == id select p).SingleOrDefault();
                    db.tblTransfers.Remove(deletedata);
                    db.SaveChanges();
                    TempData["sterror"] = "Delete Successfully !!";
                }
                else
                {
                    TempData["streciveerr"] = "You are not authorised to Delete!!";
                }

            }
            catch (Exception ex)
            {
                TempData["sterror"] = ex.Message;
            }
            return RedirectToAction("StockRecive", "StockTransfer");
        }

        public ActionResult Create()
        {
            int OutletId = obj.getOutletId();
            string category = "select * from RawCategory";
            IEnumerable<SelectListItem> Outletlist = (from q in db.tblOutlets where q.OutletType == "O" && q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.OutletId.ToString() }); //&& q.OutletId != WebSecurity.CurrentUserId
            ViewBag.OutletLists = new SelectList(Outletlist, "Value", "Text");

            IEnumerable<SelectListItem> Departmentlist = (from q in db.tbl_Department where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Department, Value = q.DepartmentID.ToString() });
            ViewBag.Departmentlist = new SelectList(Departmentlist, "Value", "Text", "Department");

            //IEnumerable<SelectListItem> CategoryList = (from q in db.RawCategories where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawCategoryID.ToString() });
            //ViewBag.CategoryList = new SelectList(CategoryList, "Value", "Text", "Category");

            //IEnumerable<SelectListItem> Items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            //ViewBag.Items = new SelectList(Items, "Value", "Text", "Items");

            //var purchasestock = (from p in db.tblPurchasedItems where p.ItemId == id select new { p.ItemId, p.Quantity });
            //int itmID = Convert.ToInt32(form["avlstock"]);
            //ViewBag.value = (from p in db.tblPurchasedItems where p.ItemId == itmID select p.Quantity);


            //IEnumerable<SelectListItem> itemlist = (from q in db.tbl_RawMaterials select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawCategory.RawCategoryID.ToString() }); //where q.OutletId == WebSecurity.CurrentUserId
            //ViewBag.itemlists = new SelectList(itemlist, "Value", "Text");
            con = new SqlConnection(webconnection);
            cmd = new SqlCommand(category, con);

            DataSet ds = new DataSet();
            List<string> li = new List<string>();
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem { Text = "--Choose The Item--", Value = "0" }); 
            foreach (DataRow row in dt.Rows)
            {

                list.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[1]), Value = Convert.ToString(row.ItemArray[0]) });

            }



            ViewBag.CategoryList = list;

            return View();
        }
        public JsonResult getitem(int id)
        {
            string itemstring = "select * from tbl_RawMaterials where rawcategoryId ='" + id + "'";

            con = new SqlConnection(webconnection);
            cmd = new SqlCommand(itemstring, con);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem { Text = "--Select Category--", Value = "0" });
            foreach (DataRow row in dt.Rows)
            {

                list.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[1]), Value = Convert.ToString(row.ItemArray[0]) });

            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));


        }
        public ActionResult StockRecive()
        {
            var name = obj.getOutletId();
            //var outid = (from q in db.tblOperators where q.Name.Equals(name) select q.OutletId).FirstOrDefault();
            //int outids = Convert.ToInt32(outid);
            var Outletstock = (from p in db.tblTransfers where p.ReciverOutletId == name select p).ToList();
            List<StockTransferModel> list = new List<StockTransferModel>();
            foreach (var item in Outletstock)
            {

                StockTransferModel model = new StockTransferModel();
                // model.CategoryId = item.CategoryId;
                model.RawMaterialId = item.RawMaterialId;
                //model.OutletId = Convert.ToInt32(item.OutletId);
                model.SenderOutletId = Convert.ToInt32(item.SenderOutletId);
                model.ReciverOutletId = Convert.ToInt32(item.ReciverOutletId);
                model.TransferDate = item.TransferDate;
                model.TransferId = item.TransferId;
                model.TransferQuantity = (item.TransferQuantity) / 1000;
                list.Add(model);
            }
            return View(list);
        }

        // stock return page Outlet//
        public ActionResult stockReturn(int Transferid = 0, int RawMaterialId = 0, decimal Quantity = 0)
        {
            string qry = " delete from tblTransByStock where TransferId  =  " + Transferid;
            qry = qry + " delete from tblTransfer where TransferId =   " + Transferid;
            
            qry = qry + " update  tbl_KitchenStock set quantity = quantity + " + Quantity + " where RawMaterialId = " + RawMaterialId;
            con = new SqlConnection(webconnection);
            con.Open();
            cmd = new SqlCommand(qry, con);
            cmd.ExecuteNonQuery();
            con.Close();


            qry = " select *,table1='gs' from tblGRNStock where IssQty <> 0   and MaterialId = " + RawMaterialId;
            qry = qry + " union all ";
            qry = qry + "  select *,table1='os' from tblOpStckRate where IssQty <> 0   and MaterialId = "+ RawMaterialId + " order by date desc";

            con = new SqlConnection(webconnection);
            cmd = new SqlCommand(qry, con);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            qry = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (Quantity > 0)
                {
                    if (dr["table1"].ToString() == "os")
                    {
                        if (Quantity <= Convert.ToDecimal(dr["IssQty"]))
                        {
                            qry = qry + " update tblOpStckRate  set IssQty = IssQty - " + Quantity + " where MaterialId = " + RawMaterialId + " and id=" +dr["id"] ;
                            Quantity -= Quantity;
                        }
                        else
                        {
                            qry = qry + " update tblOpStckRate  set IssQty = IssQty - " + Convert.ToDecimal(dr["IssQty"]) + " where MaterialId = " + RawMaterialId + " and id=" + dr["id"];
                            Quantity -= Convert .ToDecimal(dr["IssQty"]);
                        }
                            
                    }
                    else
                    {
                        if (Quantity <= Convert.ToDecimal(dr["IssQty"]))
                        {
                            qry = qry + " update tblGRNStock  set IssQty = IssQty - " + Quantity + " where MaterialId = " + RawMaterialId + " and id=" + dr["id"];
                            Quantity -= Quantity;
                        }
                        else
                        {
                            qry = qry + " update tblGRNStock  set IssQty = IssQty - " + Convert.ToDecimal(dr["IssQty"]) + " where MaterialId = " + RawMaterialId + " and id=" + dr["id"];
                            Quantity -= Convert.ToDecimal(dr["IssQty"]);
                        }


                        
                    }
                }
            }
            con = new SqlConnection(webconnection);
            con.Open();
            cmd = new SqlCommand(qry, con);
            cmd.ExecuteNonQuery();
            con.Close();


            //con = new SqlConnection(webconnection);
            //con.Open();
            //cmd = new SqlCommand("StockReturn", con);
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Add("@transferid", Transferid);
            //cmd.Parameters.Add("@rawmaterialid", RawMaterialId);
            //cmd.Parameters.Add("@item", Quantity);
            //SqlDataAdapter sda = new SqlDataAdapter(cmd);
            //cmd.ExecuteNonQuery();
            //con.Close();
            TempData["Perror"] = "Stock Returned Successfully !!";
            return RedirectToAction("TransferStockReport", "StockTransfer");
          
        }

        public ActionResult ReturnStockReport()
        {
            int Id = obj.getOutletId();
            var returndata = (from p in db.tblReturns where p.SenderOutletId == Id select p).ToList();
            List<StockReturnModel> list = new List<StockReturnModel>();
            foreach (var item in returndata)
            {
                StockReturnModel model = new StockReturnModel();

                model.RawMaterialId = item.RawMaterialId;
                // model.OutletId = item.OutletId;
                model.SenderOutId = Convert.ToInt32(item.SenderOutletId);
                model.ReciverOutId = Convert.ToInt32(item.ReciverOutletId);
                model.ReturnDate = item.ReturnDate;
                model.ReturnDescription = item.ReturnDescription;
                model.ReturnQuantity = (item.ReturnQuantity) / 1000;
                model.RStatus = item.ReturnStatuss;
                model.ReturnId = item.ReturnId;
                list.Add(model);
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult Create(OutletStockTransferModel Model)
        {

            return RedirectToAction("TransferStockReport", "StockTransfer");

        }
        [HttpGet]
        public ActionResult AddStock()
        {
            int OutletId = obj.getOutletId();
            string category = "select * from RawCategory";
            IEnumerable<SelectListItem> Outletlist = (from q in db.tblOutlets where q.OutletType == "O" && q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.OutletId.ToString() }); //&& q.OutletId != WebSecurity.CurrentUserId
            ViewBag.OutletLists = new SelectList(Outletlist, "Value", "Text");

            IEnumerable<SelectListItem> Departmentlist = (from q in db.tbl_Department where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Department, Value = q.DepartmentID.ToString() });
            ViewBag.Departmentlist = new SelectList(Departmentlist, "Value", "Text", "Department");

            //IEnumerable<SelectListItem> CategoryList = (from q in db.RawCategories where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawCategoryID.ToString() });
            //ViewBag.CategoryList = new SelectList(CategoryList, "Value", "Text", "Category");

            //IEnumerable<SelectListItem> Items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            //ViewBag.Items = new SelectList(Items, "Value", "Text", "Items");

            //var purchasestock = (from p in db.tblPurchasedItems where p.ItemId == id select new { p.ItemId, p.Quantity });
            //int itmID = Convert.ToInt32(form["avlstock"]);
            //ViewBag.value = (from p in db.tblPurchasedItems where p.ItemId == itmID select p.Quantity);


            //IEnumerable<SelectListItem> itemlist = (from q in db.tbl_RawMaterials select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawCategory.RawCategoryID.ToString() }); //where q.OutletId == WebSecurity.CurrentUserId
            //ViewBag.itemlists = new SelectList(itemlist, "Value", "Text");
            con = new SqlConnection(webconnection);
            cmd = new SqlCommand(category, con);

            DataSet ds = new DataSet();
            List<string> li = new List<string>();
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem { Text = "--Choose The Item--", Value = "0" }); 
            foreach (DataRow row in dt.Rows)
            {

                list.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[1]), Value = Convert.ToString(row.ItemArray[0]) });

            }



            ViewBag.CategoryList = list;

            return View();
        }
        [HttpPost]
        public ActionResult AddStock(AddExtraStock Model1, int id)
        {

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();
            if (Model1.currentstock != null)
            {
                sb.Append("update tblOpStckRate set Rate=" + Model1.currentstock + " where  MaterialId=" + id);
            }

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();

            //var Data = obj1.SaveStock(Model1);
            //TempData["Error"] = Data;

            return RedirectToAction("CurrentStockReport", "KitchenStock");

        }
        public string getTransferMaterialUnit(string MaterialId)
        {
            int MatId = Convert.ToInt32(MaterialId);
            string result = (from p in db.tbl_RawMaterials
                             where p.RawMaterialId == MatId
                             select p.units).SingleOrDefault();
            return result.ToString();
        }
        public string getTransferMaterialValue(string MaterialId, DateTime PurchaseDate)
        {
            int MatId = Convert.ToInt32(MaterialId);
            decimal result = Convert.ToDecimal((from p in db.tblPurchasedItems
                                                where p.RawMaterialId == MatId && (from q in db.tblPurchaseMasters where q.InvoiceDate == PurchaseDate select q.PurchaseId).Contains(p.PurchaseId)
                                                select p.Amount / p.Quantity).SingleOrDefault());
            return result.ToString("0.00");
        }
        // Update stock after stock return it update master stock outlet stock and insert data into return table//
        public JsonResult SetstockQuantity(string Id)
        {
            var name = WebSecurity.CurrentUserName;
            //string[] retrnarry;
            var otid = (from p in db.tblOperators where p.Name.Equals(name) select p.OutletId).FirstOrDefault();
            int otids = Convert.ToInt32(otid);
            if (Id != null && Id != "")
            {
                string[] str = Id.Split(';');
                tblReturn ret = new tblReturn();
                //for (var i = 0; i < str.Length; i++)
                //{
                //    string[] arr = str[i].Split('^');
                //    int tid = Convert.ToInt32(arr[0]);
                //    int iteid = Convert.ToInt32(arr[2]);
                //    int usrtransfer = Convert.ToInt32(arr[3]);
                //}
                for (int i = 0; i < str.Length; i++)
                {
                    string[] retarry = str[i].Split('^');
                    ret.RawMaterialId = Convert.ToInt32(retarry[2]);
                    ret.ReturnDate = DateTime.Now.Date;
                    ret.ReturnDescription = retarry[5];
                    ret.ReciverOutletId = Convert.ToInt32(retarry[6]);
                    ret.ReturnQuantity = Convert.ToInt32(retarry[3]);
                    ret.ReturnStatuss = "confirm";
                    ret.SenderOutletId = otids;
                    db.tblReturns.Add(ret);
                    tbl_KitchenStock tbki = new tbl_KitchenStock();
                    int rawid = Convert.ToInt32(retarry[2]);
                    decimal retrnquantity = Convert.ToDecimal(retarry[3]);
                    int recivoutId = Convert.ToInt32(retarry[6]);
                    var kitchenstock = (from p in db.tbl_KitchenStock where p.OutletId == otids && p.RawMaterialId == rawid select p).SingleOrDefault();
                    kitchenstock.Quantity = kitchenstock.Quantity - retrnquantity;
                    var anotherkitechstock = (from q in db.tbl_KitchenStock where q.OutletId == recivoutId && q.RawMaterialId == rawid select q).SingleOrDefault();
                    anotherkitechstock.Quantity = anotherkitechstock.Quantity + retrnquantity;
                    var transferupdate = (from p in db.tblTransfers where p.ReciverOutletId == otids && p.SenderOutletId == recivoutId && p.RawMaterialId == rawid select p).SingleOrDefault();
                    transferupdate.TransferQuantity = transferupdate.TransferQuantity - retrnquantity;
                    db.SaveChanges();
                }
                return Json("Update Succefully", JsonRequestBehavior.AllowGet);
            }
            return Json("Not Update", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult TransferStockReport()
        {

            SearchModel model = new SearchModel();


            IEnumerable<SelectListItem> depts = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department.ToString() });
            ViewBag.depts = new SelectList(depts, "Value", "Text", "Department");

            IEnumerable<SelectListItem> catg = (from n in db.RawCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.catg = new SelectList(catg, "Value", "Text", "Category");

            IEnumerable<SelectListItem> Items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.Items = new SelectList(Items, "Value", "Text", "Name");

            con = new SqlConnection(webconnection);

            string SearchList = "select t.TransferDate,d.Department,c.Name,r.Name,r.units,(t.TransferQuantity) as Issued, t.TransferId,t.RawMaterialId from tblTransfer t inner join tbl_Department d on t.DepartmentId=d.DepartmentID inner join tbl_RawMaterials r on r.RawMaterialId=t.RawMaterialId inner join RawCategory c on c.RawCategoryID=r.rawcategoryId";

            cmd = new SqlCommand(SearchList, con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.List = list;

            return View(model);

        }

        public JsonResult getitems(string id)
        {

            string itemstring = "select t.RawMaterialId,t.Name from RawCategory r inner join tbl_RawMaterials t on t.rawcategoryId=r.RawCategoryID where r.Name='" + id + "'";

            con = new SqlConnection(webconnection);
            cmd = new SqlCommand(itemstring, con);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);
            List<SelectListItem> list = new List<SelectListItem>();
            //list.Add(new SelectListItem { Text = "--Select Category--", Value = "0" });
            foreach (DataRow row in dt.Rows)
            {

                list.Add(new SelectListItem { Text = Convert.ToString(row.ItemArray[1]), Value = Convert.ToString(row.ItemArray[0]) });

            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));


        }

        [HttpPost]
        public ActionResult TransferStockReport(SearchModel model)
        {
            //List<KitchenStockAddModel> list = new List<KitchenStockAddModel>();
            //KitchenStockAddModel model = new KitchenStockAddModel();
            IEnumerable<SelectListItem> depts;

            if (model.Department != null)
            {
                depts = (from n in db.tbl_Department where n.Department == model.Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department });

            }
            else if (model.Category != null)
            {

                depts = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department.ToString() });
            }
            else
            {

                depts = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department.ToString() });
            }
            ViewBag.depts = new SelectList(depts, "Value", "Text", "Department");


            IEnumerable<SelectListItem> catg = (from n in db.RawCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.catg = new SelectList(catg, "Value", "Text", "Category");

            IEnumerable<SelectListItem> items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.Items = new SelectList(items, "Value", "Text", "Name");


            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();
            sb.Append("select t.TransferDate,d.Department,c.Name,r.Name,r.units,t.TransferQuantity, t.TransferId,t.RawMaterialId as Issued from tblTransfer t inner join tbl_Department d on t.DepartmentId=d.DepartmentID inner join tbl_RawMaterials r on r.RawMaterialId=t.RawMaterialId inner join RawCategory c on c.RawCategoryID=r.rawcategoryId");
            if (model.Item != " " && model.Item != null)
                sb.Append(" Where r.name= '" + model.Item + "'");
            else if (model.Category != " " && model.Category != null)
                sb.Append(" Where c.name='" + model.Category + "'");
            else if (model.Department != "" && model.Department != null)
                sb.Append(" Where d.Department='" + model.Department + "'");
            else if (model.FromDate != "" && model.ToDate != "" && model.FromDate != null && model.ToDate != null)
                sb.Append(" Where t.TransferDate between'" + model.FromDate + "' and '" + model.ToDate + "'");


            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.List = list;
            depts = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department.ToString() });
            ViewBag.depts = new SelectList(depts, "Value", "Text", "Department");

            return View(model);

        }

        //public ActionResult stockReturn(int TransferId ,int RawMaterialId)
        //{
        //    var ReturnData = (from p in db.tblTransfers
        //                      where p.TransferId=convert(TransferId)
        //                      select p).SingleOrDefault();
        //    OutletPurchageReturnModel model = new OutletPurchageReturnModel();
        //    model.Amount = ReturnData.Amount;
        //    model.OutletId = obj.getOutletId();
        //    model.Quantity = ReturnConvertValues(ReturnData.Unit, ReturnData.Quantity);
        //    model.Type = ReturnType(ReturnData.Unit);
        //    //model.VendorId = ReturnData.tblPurchaseMaster.VendorId;
        //    model.RowMaterialId = ReturnData.RawMaterialId;
        //    //model.VenderName = ReturnData.tblPurchaseMaster.tblVendor.Name;
        //    model.RowMaterialName = ReturnData.tbl_RawMaterials.Name;
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult PurchaseReturn(OutletPurchageReturnModel model)
        //{
        //    var Data = (from p in db.tblPurchasedItems
        //                where p.RawMaterialId == model.RowMaterialId
        //                select p).SingleOrDefault();
        //    decimal perticularAmount = Data.Amount / Data.Quantity;
        //    decimal returnAmount = perticularAmount * model.ReturnQuantity;
        //    tblPurchaseReturn tb = new tblPurchaseReturn();
        //    tb.OutletId = obj.getOutletId();
        //    tb.RawMaterialId = model.RowMaterialId;
        //    tb.ReturnDate = DateTime.Now.Date;
        //    tb.ReturnDescription = model.Reasion;
        //    tb.ReturnQuantity = Convert.ToInt32(ConvertValues(model.Type, model.ReturnQuantity));
        //    tb.vendorId = model.VendorId;
        //    tb.ReturnStatuss = "Pending";
        //    db.tblPurchaseReturns.Add(tb);
        //    //db.SaveChanges();
        //    //remaining stock
        //    int Outlet = obj.getOutletId();
        //    decimal remaingquantity = ConvertValues(model.Type, model.Quantity) - ConvertValues(model.Type, model.ReturnQuantity);
        //    tbl_KitchenStock DataStock = (from p in db.tbl_KitchenStock where p.RawMaterialId == model.RowMaterialId && p.OutletId == Outlet select p).SingleOrDefault();
        //    DataStock.Quantity = remaingquantity;
        //    db.SaveChanges();
        //    return RedirectToAction("ReturnPurchaseReport");
        //}

        //public ActionResult Fillitem(int Category)
        //{
        //    var item = db.tbl_RawMaterials.Where(c => c.rawcategoryId == Category);
        //    return Json(item, JsonRequestBehavior.AllowGet);
        //}

    }
}
