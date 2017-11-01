using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NibsMVC.Models;

namespace NibsMVC.IRepository
{
    interface IOutletHomeRepository
    {
        OutletHomeModel getOutletHomeReports();
        List<CashierRHomeModel> getRBillingData();
        List<CashierTHomeModel> getTBillingData();
        List<CashierHHomeModel> getHBillingData();
    }
}
