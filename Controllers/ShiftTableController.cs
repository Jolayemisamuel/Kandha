using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System.Xml;
using WebMatrix.WebData;

namespace NibsMVC.Controllers
{
    public class ShiftTableController : Controller
    {
        //
        // GET: /ShiftTable/
        ShiftTableRepository obj = new ShiftTableRepository();
        NIBSEntities db = new NIBSEntities();
        XMLTablesRepository xml = new XMLTablesRepository();
        public ActionResult Index()
        {
            return View();
        }
        public string ShiftTable(ShiftTableModel model)
        {
            var ShidtPath = Server.MapPath("/xmltables/table" + model.TableForShift + ".xml");
            var name = WebSecurity.CurrentUserName;
            var outletid = (from n in db.tblOperators where n.Name.Equals(name) select n.OutletId).FirstOrDefault();
            int oulte = Convert.ToInt32(outletid);
            XmlTextWriter writer = new XmlTextWriter(Server.MapPath("/xmltables/table" + model.MasterTable + ".xml"), System.Text.Encoding.UTF8);
            writer.WriteStartDocument(true);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartElement("Item");
            xml.createNode(oulte.ToString(),model.MasterTable.ToString(),"0", " ", "0", "0", "0", "0", "0", "0", "0","0", writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            var MasterPath = Server.MapPath("/xmltables/table" + model.MasterTable + ".xml");
            var Data = obj.ShiftTable(model, ShidtPath, MasterPath);
            return Data;
        }

    }
}
