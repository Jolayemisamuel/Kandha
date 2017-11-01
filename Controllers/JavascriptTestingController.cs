using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace NibsMVC.Controllers
{
    public class JavascriptTestingController : Controller
    {
        //
        // GET: /JavascriptTesting/
        NIBSEntities db = new NIBSEntities();
        StringBuilder sb = new StringBuilder();
        XMLTablesRepository xml = new XMLTablesRepository();
        NIbsBillingRepository nibsrepo = new NIbsBillingRepository();
        AdminSearchRepository search = new AdminSearchRepository();
        public ActionResult Index()
        {
            NibsBillingModel nibs = new NibsBillingModel();
            List<BillTableModelList> ItemLIst = new List<BillTableModelList>();

            int oulte = nibsrepo.getOutletId();
            var address = (from p in db.tblOutlets where p.OutletId == oulte select p.Address).SingleOrDefault();
            ViewBag.outletaddress = address;
            var category = (from q in db.tblCategories where q.Active == true && (from p in db.tblMenuOutlets where p.OutletId == oulte select p.CategoryId).Contains(q.CategoryId) select q).ToList();
            // var ItemList = db.tblMenuOutlets.Where(o => o.OutletId == WebSecurity.CurrentUserId).ToList();
            foreach (var item in category)
            {
                BillTableModelList model = new BillTableModelList();
                model.CategoryId = item.CategoryId;
                model.CategoryName = item.Name;
                model.Color = item.Color;
                model.TextColor = item.TextColor;
                ItemLIst.Add(model);

            }
            nibs.getAllItem = ItemLIst;
            var result = db.tblTableMasters.Where(o => o.OutletId == oulte).ToList();

            List<BillTableModel> List = new List<BillTableModel>();
            foreach (var item in result)
            {
                BillTableModel mo = new BillTableModel();
                var filepath = Server.MapPath("/xmltables/table" + item.TableNo + ".xml");
                if (System.IO.File.Exists(filepath))
                {
                    XDocument xd = XDocument.Load(filepath);
                    var items = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                                 select p).ToList();
                    if (items.Count > 0)
                    {
                        mo.Current = "current";
                    }
                }
                mo.TableNo = item.TableNo.ToString();
                List.Add(mo);
            }
            nibs.getAllTables = List;
            return View(nibs);
        }
        public PartialViewResult GetSubItem(string Id)
        {
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            lst = xml.GetAllItems(Id);
            return PartialView("GetSubItem", lst);
        }
        public PartialViewResult _CreatePartial(int Id)
        {
            Session["RunningTable"] = Id;
            GetBillingModel model = new GetBillingModel();
            var Path = Server.MapPath("/xmltables/table" + Id + ".xml");
            var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
            if (System.IO.File.Exists(Path))
            {
                model = xml.GetBillingItem(Id.ToString(), Path, KotFilePath);
            }
            else
            {
                int oulte = nibsrepo.getOutletId();
                XmlTextWriter writer = new XmlTextWriter(Server.MapPath("/xmltables/table" + Id + ".xml"), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Item");
                xml.createNode(oulte.ToString(), Id.ToString(), "0", " ", "0", "0", "0", "0", "0", "0", "0", "0", writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                // MessageBox.Show("XML File created ! ");
                model = xml.GetBillingItem(Id.ToString(), Path, KotFilePath);
            }

            return PartialView("_CreatePartial", model);
        }

    }
}
