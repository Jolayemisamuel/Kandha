using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
namespace NibsMVC.Repository
{
    public class QuickSideBarRepository
    {
        NIBSEntities entities = new NIBSEntities();
       public QuickSideBarModel getAllManagerList()
        {
            List<QuickSideBarManagerModel> lst = new List<QuickSideBarManagerModel>();
            List<QuickSideBarOperatorModel> lstoperator = new List<QuickSideBarOperatorModel>();
            QuickSideBarModel model = new QuickSideBarModel();
            var data = entities.tblOperators.Where(x => x.Type.Equals("Manager")).ToList();
            //foreach (var item in data)
            //{
            //    QuickSideBarManagerModel mo = new QuickSideBarManagerModel();
            //    mo.UserId = item.UserId.Value;
            //    mo.Name = item.Name;
            //    mo.OutletName = item.tblOutlet.Name;
            //    lst.Add(mo);
            //}
            //var data1 = entities.tblOperators.Where(x => x.Type.Equals("Manager")).ToList();
            //foreach (var item in data1)
            //{
            //    QuickSideBarOperatorModel mo = new QuickSideBarOperatorModel();
            //    mo.UserId = item.UserId.Value;
            //    mo.Name = item.Name;
            //    mo.OutletName = item.tblOutlet.Name;
            //    lstoperator.Add(mo);
            //}
            model.getmanagerlist = lst;
            model.getoperatorlist = lstoperator;
            return model;
        }
    }
}