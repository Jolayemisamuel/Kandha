using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Web.Security;
using WebMatrix.WebData;
namespace NibsMVC.Repository
{
    public class AutoInventoryRepository
    {
        NIBSEntities entities = new NIBSEntities();
        public void AutoInventory(int ItemId, int FullQty, int HalfQty, int OutletId)
        {
            bool data = entities.AutoInventories.Where(a => a.OutletId == OutletId).Select(a => a.IsInventory.Value).FirstOrDefault();
            if (data)
            {
                var Ingredient = (from p in entities.tbl_KitchenRawIndent where p.ItemId == ItemId select p).ToList();
                decimal expenseStock = 0;
                decimal remaining = 0;
                foreach (var item in Ingredient)
                {
                    //tbl_KitchenStock tb = new tbl_KitchenStock();
                    //tb = (from p in entities.tbl_KitchenStock
                    //      where p.OutletId == OutletId
                    //      && p.RawMaterialId == item.RawMaterialId
                    //      select p).FirstOrDefault();
                    //var expenseStock = item.Quantity * FullQty;
                    //var remaining = tb.Quantity - expenseStock;

                    //tb.Quantity = remaining;
                    //entities.SaveChanges();
                    tbl_KitchenStock tb = new tbl_KitchenStock();
                    tb = (from p in entities.tbl_KitchenStock
                          where p.OutletId == OutletId
                          && p.RawMaterialId == item.RawMaterialId
                          select p).FirstOrDefault();
                    if (item.Unit == "kg" || item.Unit == "ltr")
                    {
                        expenseStock = (item.Quantity * 1000) * FullQty;
                        remaining = tb.Quantity - expenseStock;
                    }

                    else
                    {
                        expenseStock = item.Quantity * FullQty;
                        remaining = tb.Quantity - expenseStock;
                    }
                    tb.Quantity = remaining;
                    entities.SaveChanges();
                }
            }
        }
        public int AlertKitchenStocks()
        {
            int Count = 0;
            int Id = getOutletId();
            bool data = entities.AutoInventories.Where(a => a.OutletId == Id).Select(a => a.IsInventory.Value).FirstOrDefault();
            if (data)
            {
                var alerts = (from p in entities.tbl_KitchenStock
                              where p.OutletId == Id
                              select new
                              {
                                  Qty = p.Quantity,
                                  Unit = p.Unit
                              }).ToList();
                foreach (var item in alerts)
                {
                    ConversionRepository conversion = new ConversionRepository();
                    decimal Qty = conversion.ReturnConvertValues(item.Unit, item.Qty);
                    if (Qty < 5)
                    {
                        Count = Count + 1;
                    }
                }
            }
            
            return Count;
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