using NibsMVC.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Models;
using WebMatrix.WebData;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using NibsMVC.Repository;
using System.Web.Security;


namespace NibsMVC.Controllers
{
    public class KitchenStockController : Controller
    {
        //
        // GET: /KitchenStock/
        NIBSEntities db = new NIBSEntities();
        ConversionRepository convert = new ConversionRepository();
        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        SqlConnection con;
        SqlCommand cmd;
        XMLTablesRepository obj = new XMLTablesRepository();
        [HttpGet]
        public ActionResult Index()
        {
            int i = 1;
            int j = 1;
            int k = 1;
            int l = 1;
            int m = 1;
            //int n = 1;
            SearchModel model = new SearchModel();
            //List<KitchenStockAddModel> list = new List<KitchenStockAddModel>();
            //KitchenStockAddModel model = new KitchenStockAddModel();


            IEnumerable<SelectListItem> depts = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department.ToString() });
            ViewBag.depts = new SelectList(depts, "Value", "Text", "Department");

            IEnumerable<SelectListItem> catg = (from n in db.RawCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.catg = new SelectList(catg, "Value", "Text", "Category");

            IEnumerable<SelectListItem> Items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.Items = new SelectList(Items, "Value", "Text", "Name");

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();

            sb.Append(" WITH TempStckReport As ( select MaterialId ,r.Name ,category=c.Name ,os.date,r.units, PurQty=Qty ,rate,  ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append("d"+i +"_Qty=(select sum(qty) from tblTransfer t inner join tblTransByStock ts on ts.transferid=t.TransferId inner join tbl_Department l on l.DepartmentID=t.DepartmentId ");
                sb.Append(" where ts.stockid= os.id and ts.stocktype='os' and l.Department='" + dept.Text + "'),");
                i++;
            }

            sb.Append(" from tblOpStckRate os inner join  tbl_RawMaterials r on r.RawMaterialId = os.MaterialId inner join RawCategory c on ");
            sb.Replace(", from", " from");
            sb.Append(" c.RawCategoryID = r.rawcategoryId where MaterialId in ");
            sb.Append(" (select RawMaterialId  from tblTransfer )  ");

            sb.Append(" union all  ");

            sb.Append(" select MaterialId ,r.Name ,category=c.Name , gs.date,r.units,PurQty=Qty , Rate , ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append("d"+j +"_Qty=(select sum(qty) from tblTransfer t inner join tblTransByStock ts on ts.transferid=t.TransferId inner join tbl_Department l on l.DepartmentID=t.DepartmentId ");
                sb.Append(" where ts.stockid= gs.id and ts.stocktype='gs' and l.Department='" + dept.Text + "'),");
                j++;
            }
            sb.Append(" from  tblgrnstock gs inner join  tbl_RawMaterials r on r.RawMaterialId = gs.MaterialId  inner join RawCategory c on ");
            sb.Replace(", from", " from");
            sb.Append(" c.RawCategoryID = r.rawcategoryId where MaterialId in ");
            sb.Append(" (select RawMaterialId  from tblTransfer ) )");

            sb.Append(" select materialid,name,category,date,units,purqty=sum(purqty),PurVal=sum(purqty)*rate, ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append("d"+k+ "qtyf= isnull(sum(d"+k+"_Qty),0),");
                sb.Append("d"+k+ "valf=isnull(sum(d"+k+ "_Qty),0)*rate, ");
                k++;
            }
            sb.Append(" balqty=sum(purqty) ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append("-isnull(sum(d"+l+"_Qty),0) ");
                i++;

            }
            
            sb.Append("balval=(sum(purqty)*rate) ");
            sb.Replace(" balval", ", balval");
            foreach (SelectListItem dept in depts)
            {
                sb.Append("-(isnull(sum(d"+m+"_Qty),0)*rate) ");
                i++;
            }
            sb.Append(" from TempStckReport  group by materialid,name,category,date,units,rate ");
            sb.Replace(", from", " from");

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);
            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.List = list;
            ViewBag.flag = false;
            return View(model);

        }

        [HttpPost]
        public ActionResult Index(SearchModel model)
        {
            //List<KitchenStockAddModel> list = new List<KitchenStockAddModel>();
            //KitchenStockAddModel model = new KitchenStockAddModel();
            IEnumerable<SelectListItem> depts;

            if (model.Department != null)
            {
                depts = (from n in db.tbl_Department where n.Department == model.Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department });
                ViewBag.flag = true;
            }
            else if (model.Category != null)
            {
                ViewBag.flag = false;
                depts = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department.ToString() });
            }
            else
            {
                ViewBag.flag = false;
                depts = (from n in db.tbl_Department select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Department, Value = n.Department.ToString() });
            }
            ViewBag.depts = new SelectList(depts, "Value", "Text", "Department");


            IEnumerable<SelectListItem> catg = (from n in db.RawCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.catg = new SelectList(catg, "Value", "Text", "Category");

            IEnumerable<SelectListItem> items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.Items = new SelectList(items, "Value", "Text", "Name");


            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();


            sb.Append(" WITH TempStckReport As ( select MaterialId ,r.Name ,category=c.Name ,os.date,r.units, PurQty=Qty ,rate,  ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append(dept.Text + "_Qty=(select sum(qty) from tblTransfer t inner join tblTransByStock ts on ts.transferid=t.TransferId inner join tbl_Department l on l.DepartmentID=t.DepartmentId ");
                sb.Append(" where ts.stockid= os.id and ts.stocktype='os' ");
                if (model.FromDate != "" && model.ToDate != "" && model.FromDate != null && model.ToDate != null)
                    sb.Append(" and transferdate>= '" + model.FromDate + "' and transferdate<= '" + model.ToDate + "'");
                sb.Append(" and l.Department='" + dept.Text + "'),");
            }

            sb.Append(" from tblOpStckRate os inner join  tbl_RawMaterials r on r.RawMaterialId = os.MaterialId inner join RawCategory c on ");
            sb.Replace(", from", " from");
            sb.Append(" c.RawCategoryID = r.rawcategoryId where MaterialId in ");
            sb.Append(" (select RawMaterialId  from tblTransfer )  ");

            sb.Append(" union all  ");

            sb.Append(" select MaterialId ,r.Name ,category=c.Name , gs.date,r.units,PurQty=Qty , Rate , ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append(dept.Text + "_Qty=(select sum(qty) from tblTransfer t inner join tblTransByStock ts on ts.transferid=t.TransferId inner join tbl_Department l on l.DepartmentID=t.DepartmentId ");
                sb.Append(" where ts.stockid= gs.id and ts.stocktype='gs' ");
                if (model.FromDate != "" && model.ToDate != "" && model.FromDate != null && model.ToDate != null)
                    sb.Append(" and transferdate>= '" + model.FromDate + "' and transferdate<= '" + model.ToDate + "'");
                sb.Append(" and l.Department='" + dept.Text + "'),");
            }
            sb.Append(" from  tblgrnstock gs inner join  tbl_RawMaterials r on r.RawMaterialId = gs.MaterialId  inner join RawCategory c on ");
            sb.Replace(", from", " from");
            sb.Append(" c.RawCategoryID = r.rawcategoryId where MaterialId in ");
            sb.Append(" (select RawMaterialId  from tblTransfer ) )");

            sb.Append(" select materialid,name,category,date,units,purqty=sum(purqty),PurVal=sum(purqty)*rate, ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append(dept.Text + "qtyf= isnull(sum(" + dept.Text + "_Qty),0),");
                sb.Append(dept.Text + "valf=isnull(sum(" + dept.Text + "_Qty),0)*rate, ");

            }
            sb.Append(" balqty=sum(purqty) ");
            foreach (SelectListItem dept in depts)
            {
                sb.Append("-isnull(sum(" + dept.Text + "_Qty),0) ");

            }

            sb.Append("balval=(sum(purqty)*rate) ");
            sb.Replace(" balval", ", balval");
            foreach (SelectListItem dept in depts)
            {
                sb.Append("-(isnull(sum(" + dept.Text + "_Qty),0)*rate) ");
            }
            sb.Append(" from TempStckReport  group by materialid,name,category,date,units,rate ");
            sb.Replace(", from", " from");




            if (model.Item != "" && model.Item != null)
                sb.Append(" and Name= '" + model.Item + "'");
            else if (model.Category != "" && model.Category != null)
                sb.Append(" and category='" + model.Category + "'");


            sb.Append(" order by 1");

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

        public ActionResult addstock()
        {

            return View();

        }
        public ActionResult add(KitchenStockAddModel model)
        {
            try
            {
                //tblPurchaseMaster tb = new tblPurchaseMaster();
                //tb.ChequeNo = model.ChequeNo.ToString();
                //tb.DepositeAmount = model.DepositeAmount;
                //tb.ExtraCharge = model.ExtraCharge;
                //tb.ExtraChargeDetails = model.ExtraChargeDetail;
                //tb.InvoiceDate = model.InvoiceDate;
                //tb.InvoiceNo = model.InvoiceNo;
                //tb.NetAmount = model.NetAmount;
                //tb.OutletId = obj.getOutletId(); ;
                //tb.TotalAmount = model.TotalAmount;
                //tb.PaymenyMode = model.PaymentMode;
                //tb.RemainingAmount = model.RemainingAmount;
                //tb.Tax = model.Tax;
                //tb.VendorId = model.VendorId;
                //db.tblPurchaseMasters.Add(tb);
                //db.SaveChanges();
                //int OutletId = obj.getOutletId();
                //int Pid = (from p in db.tblPurchaseMasters where p.OutletId == OutletId select p.PurchaseId).Max();
                //for (int i = 0; i < model.RowMaterialId.Length; i++)
                //{
                //    tblPurchasedItem tbl = new tblPurchasedItem();
                //    tbl.RawMaterialId = model.RowMaterialId[i];
                //    tbl.Quantity = ConvertValues(model.Type[i], model.Quantity[i]);
                //    tbl.Unit = Type(model.Type[i]);
                //    tbl.Amount = model.txt_Amount[i];
                //    tbl.PurchaseId = Pid;
                //    db.tblPurchasedItems.Add(tbl);
                //    db.SaveChanges();
                //    tbl_KitchenStock stock = new tbl_KitchenStock();
                //    var stockdata = (from l in db.tbl_KitchenStock where l.OutletId == OutletId select new { l.RawMaterialId, l.Quantity }).ToList();
                //    int flag = 0;
                //    foreach (var item in stockdata)
                //    {
                //        if (item.RawMaterialId == Convert.ToInt32(model.RowMaterialId[i]))
                //        {
                //            flag = 1;
                //        }

                //    }
                //    if (flag == 0)
                //    {
                //        stock.OutletId = obj.getOutletId();
                //        stock.RawMaterialId = Convert.ToInt32(model.RowMaterialId[i]);
                //        stock.Unit = Type(model.Type[i]);
                //        stock.Quantity = ConvertValues(model.Type[i], model.Quantity[i]);
                //        db.tbl_KitchenStock.Add(stock);
                //        db.SaveChanges();
                //    }
                //    else if (flag == 1)
                //    {

                //        int rawmatId = Convert.ToInt32(model.RowMaterialId[i]);
                //        var stockhavingdata = (from p in db.tbl_KitchenStock where p.RawMaterialId == rawmatId && p.OutletId == WebSecurity.CurrentUserId select p).SingleOrDefault();
                //        stockhavingdata.Quantity = Convert.ToDecimal(stockhavingdata.Quantity + ConvertValues(model.Type[i], model.Quantity[i]));
                //        db.SaveChanges();
                //    }
                //}

                TempData["Perror"] = "Inserted Successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Perror"] = "Try Again !";
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public ActionResult CurrentStockReport()
        {
            SearchModel model = new SearchModel();

            IEnumerable<SelectListItem> catg = (from n in db.RawCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.catg = new SelectList(catg, "Value", "Text", "Category");

            IEnumerable<SelectListItem> Items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.Items = new SelectList(Items, "Value", "Text", "Name");

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();

            sb.Append(" with tempOldNew ");
            sb.Append(" as ");
            sb.Append(" ( ");
            sb.Append(" select r.name ,category =c.name,r.units, ");
            sb.Append(" oldstockQty= (select isnull(sum(qty-issqty),0) from   tblOpStckRate where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0)  ");
            sb.Append(" + (select isnull(sum(qty-issqty),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id <> (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId) ");
            sb.Append(" )   ");
            sb.Append(" ,oldstockVal= (select isnull(sum((qty-issqty)*Rate),0) from   tblOpStckRate where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0)  ");
            sb.Append(" + (select isnull(sum((qty-issqty)*rate),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id <> (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId) ");
            sb.Append(" )   ");
            sb.Append(" ,newstockQty=  ");
            sb.Append("  (select isnull(sum(qty-issqty),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id = (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId) ");
            sb.Append(" ) ");
            sb.Append(" ,newstockVal= ");
            sb.Append("  (select isnull(sum((qty-issqty)*rate),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id = (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId) ");
            sb.Append(" )");
            sb.Append(" from  ");
            sb.Append(" tbl_RawMaterials r    ");
            sb.Append(" inner join RawCategory c on c.RawCategoryID = r.rawcategoryId   ");
            sb.Append(" ) ");
            sb.Append(" select name ,category,units,oldstockqty,oldstockval,newstockqty,newstockVal,curStckqty= oldstockqty+newstockqty ,curStckval= oldstockval+newstockVal ");
            sb.Append(" from tempOldNew ");

            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;
            DataTable dt = new DataTable();
            con.Open();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            var data = sda.Fill(dt);

            List<DataRow> list = dt.AsEnumerable().ToList();
            con.Close();
            ViewBag.List = list;
            ViewBag.flag = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult CurrentStockReport(SearchModel model)
        {



            IEnumerable<SelectListItem> catg = (from n in db.RawCategories select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.catg = new SelectList(catg, "Value", "Text", "Category");

            IEnumerable<SelectListItem> items = (from n in db.tbl_RawMaterials select n).AsEnumerable().Select(n => new SelectListItem() { Text = n.Name, Value = n.Name.ToString() });
            ViewBag.Items = new SelectList(items, "Value", "Text", "Name");

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();

            sb.Append(" with tempOldNew ");
            sb.Append(" as ");
            sb.Append(" ( ");
            sb.Append(" select r.name ,category =c.name,r.units, ");
            sb.Append(" oldstockQty= (select isnull(sum(qty-issqty),0) from   tblOpStckRate where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0)  ");
            sb.Append(" + (select isnull(sum(qty-issqty),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id <> (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId ) ");
            sb.Append(" )   ");
            sb.Append(" ,oldstockVal= (select isnull(sum((qty-issqty)*Rate),0) from   tblOpStckRate where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0)  ");
            sb.Append(" + (select isnull(sum((qty-issqty)*rate),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id <> (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId) ");
            sb.Append(" )   ");
            sb.Append(" ,newstockQty=  ");
            sb.Append("  (select isnull(sum(qty-issqty),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id = (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId) ");
            sb.Append(" ) ");
            sb.Append(" ,newstockVal= ");
            sb.Append("  (select isnull(sum((qty-issqty)*rate),0) from   tblgrnstock where r.RawMaterialId =MaterialId and isnull(qty-issqty,0)>0 ");
            sb.Append(" and id = (select max(id) from tblgrnstock where r.RawMaterialId=MaterialId) ");
            sb.Append(" ) ");
            sb.Append(" from  ");
            sb.Append(" tbl_RawMaterials r    ");
            sb.Append(" inner join RawCategory c on c.RawCategoryID = r.rawcategoryId   ");
            if (model.Item != " " && model.Item != null)
                sb.Append(" Where r.Name= '" + model.Item + "'");
            else if (model.Category != " " && model.Category != null)
                sb.Append(" Where c.Name='" + model.Category + "'");
            sb.Replace(", from", " from");
            sb.Append(" ) ");
            sb.Append(" select name ,category,units,oldstockqty,oldstockval,newstockqty,newstockVal,curStckqty= oldstockqty+newstockqty ,curStckval= oldstockval+newstockVal ");
            sb.Append(" from tempOldNew ");



            cmd = new SqlCommand(sb.ToString(), con);
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

        [HttpGet]
        public ActionResult ExtraStock()
        {

            string category = "select * from RawCategory";


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
        public ActionResult ExtraStock(AddExtraStock Model1)
        {

            con = new SqlConnection(webconnection);
            StringBuilder sb = new StringBuilder();
            int rawid = Convert.ToInt32(Model1.RawMaterial);
            if (Model1.currentstock != null && Model1.currentvalue != null)
            {
                sb.Append("update tbl_KitchenStock set Quantity=" + Model1.currentstock + " where  RawMaterialId=" + Model1.RawMaterial);
                if (db.tblOpStckRates.Where(p => p.MaterialId.Equals(rawid)).Count() > 0)
                    sb.Append(" update tblOpStckRate set Qty= " + Model1.currentstock + ", Rate=" + Model1.currentvalue + " , Date = '" + Model1.stockDate + "' where  MaterialId=" + Model1.RawMaterial);
                else
                    sb.Append(" insert into tblOpStckRate values("+ Model1.RawMaterial + ", " + Model1.currentvalue + " ,'" + Model1.stockDate + "'," + Model1.currentstock + " , 0,0)");
            }
            


            cmd = new SqlCommand(sb.ToString(), con);
            cmd.CommandType = CommandType.Text;

            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("CurrentStockReport", "KitchenStock");

        }
        public JsonResult getite(int id)
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

        public JsonResult Stockavailable(string Id)
        {
            if (Id != null && Id != "")
            {
                int rawMaterialId = Convert.ToInt32(Id);
                int OutletId = obj.getOutletId();
                var quenti = (from p in db.tbl_KitchenStock where p.RawMaterialId == rawMaterialId && p.OutletId == OutletId select p.Quantity).FirstOrDefault();

                return Json(quenti.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Rateavailable(string Id)
        {
            if (Id != null && Id != "")
            {
                int rawMaterialId = Convert.ToInt32(Id);
                int OutletId = obj.getOutletId();
                var Rate = (from p in db.tblOpStckRates where p.MaterialId == rawMaterialId select p.Rate).FirstOrDefault();

                return Json(Rate.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
    }
}


