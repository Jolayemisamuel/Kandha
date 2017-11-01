using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Web.Security;
using WebMatrix.WebData;
using NibsMVC.Repository;
namespace NibsMVC.Controllers
{
    public class KitchenAlertController : Controller
    {
        //
        // GET: /KitchenAlert/
        NIBSEntities entities = new NIBSEntities();
        public ActionResult Index()
        {
            int Id=getOutletId();
            List<KitchenAlertsModel> lst = new List<KitchenAlertsModel>();
            var result = (from p in entities.tbl_KitchenStock
                          where p.OutletId == Id
                          select new
                          {
                              Qty = p.Quantity,
                              Unit = p.Unit,
                              Name=p.tbl_RawMaterials.Name
                          }).ToList();
            foreach (var item in result)
            {
                ConversionRepository conversion = new ConversionRepository();
                decimal Qty = conversion.ReturnConvertValues(item.Unit, item.Qty);
                if (Qty < 10)
                {
                    KitchenAlertsModel model = new KitchenAlertsModel();
                    model.ItemName=item.Name;
                    model.Qty=item.Qty;
                    model.Unit = item.Unit;
                    lst.Add(model);
                }
            }
           
            return View(lst);
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
                    oulte = Convert.ToInt32((from n in entities.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }
    }
}
