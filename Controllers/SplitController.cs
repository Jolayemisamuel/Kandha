using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using NibsMVC.Models;
using NibsMVC.EDMX;
using WebMatrix.WebData;

namespace NibsMVC.Controllers
{
    public class SplitController : Controller
    {
        //
        // GET: /Split/
        NIBSEntities _entities = new NIBSEntities();
        public ActionResult Index()
        {
            return View();
        }
        public string SaveSplitData(string TableNo, string Type, string NoOfPersion, string Detail)
        {
            var name = WebSecurity.CurrentUserName;
            var outletid = (from n in _entities.tblOperators where n.Name.Equals(name) select n.OutletId).FirstOrDefault();
            int oulte = Convert.ToInt32(outletid);
            var Path = Server.MapPath("/xmlkot/Split.xml");
            var array = Detail.Split('^');
            XDocument xd = XDocument.Load(Path);
            var items = from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo 
                        select item;
            for (int i = 0; i < array.Length; i++)
            {
                if(array[i]!="")
                {
                    var FinalDetail = array[i].Split(',');
                    var newElement = new XElement("Items",
                                new XElement("UserId", oulte.ToString()),
                                 new XElement("TableNo", TableNo),
                                  new XElement("Type", Type),
                             new XElement("NoOfPersion", NoOfPersion),
                             new XElement("Name", FinalDetail[0]),
                             new XElement("Amount", FinalDetail[1]));
                    xd.Element("Item").Add(newElement);
                }

            }
            xd.Save(Path);
            return "Split successfully...";
        }

    }
}
