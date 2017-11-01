using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NibsMVC.EDMX;
using NibsMVC.Models;
namespace NibsMVC.IRepository
{
    interface IAdminHomeRepository
    {
        AdminHomeModel getAdminHomeReports();
    }
}
