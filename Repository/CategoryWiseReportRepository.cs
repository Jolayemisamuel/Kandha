using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Repository;
using NibsMVC.Models;
using System.Web.Security;
using WebMatrix.WebData;
namespace NibsMVC.Repository
{
    public class CategoryWiseReportRepository
    {
        NIBSEntities entities = new NIBSEntities();
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
        public CategoryWiseReportModel getReport()
        {
            CategoryWiseReportModel model=new CategoryWiseReportModel();
            return model;
        }
    }
}