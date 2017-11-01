using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Repository;
using NibsMVC.Models;
using WebMatrix.WebData;
using System.Web.Security;
using System.Xml.Linq;
namespace NibsMVC.Repository
{
    public class CheckStockItemRepository
    {
        NIBSEntities entities = new NIBSEntities();
        //XMLTablesRepository xml = new XMLTablesRepository();
        public string OutStockItems(int BillId)
        {
            int Id = getOutletId();
            string items = string.Empty;
            var data = (from p in entities.tblBillDetails
                        join q in entities.tbl_KitchenRawIndent on p.ItemId equals q.ItemId
                        join r in entities.tbl_KitchenStock on q.RawMaterialId equals r.RawMaterialId
                        where p.BillId == BillId
                        && r.OutletId == Id
                        select new
                        {
                            ItemId = p.ItemId,
                            Qty = r.Quantity
                        }).ToList();
            foreach (var item in data)
            {
                if (item.Qty == 0)
                {
                    items = item.ItemId + ",";
                }
            }
            return items;
        }
        public bool CheckOutStockItem(int ItemId)
        {
            bool status = true;
            int Id = getOutletId();
            bool data = entities.AutoInventories.Where(a => a.OutletId == Id).Select(a => a.IsInventory.Value).FirstOrDefault();

            if (data)
            {
                var dd = (from p in entities.tbl_KitchenRawIndent
                          join q in entities.tbl_KitchenStock
                              on p.RawMaterialId equals q.RawMaterialId
                          where p.ItemId == ItemId
                          select new { ItemId = p.ItemId, Qty = q.Quantity }).ToList();

                foreach (var item in dd)
                {
                    if (item.Qty == 0)
                    {
                        status = false;
                        return status;
                    }
                }

            }

            return status;
        }
        public bool CheckOutStockItemWithQty(int ItemId, int Qty, string TableNo, string Path)
        {
            XDocument xd = XDocument.Load(Path);
            int Id = getOutletId();
            var result = (from item in xd.Descendants("Items")
                          where item.Element("UserId").Value == Id.ToString()
                          && item.Element("TableNo").Value == TableNo
                          && item.Element("ItemId").Value == ItemId.ToString()
                          select item).FirstOrDefault();
            if (result != null)
            {
                Qty = Qty + Convert.ToInt32(result.Element("FullQty").Value);
            }
            bool data = entities.AutoInventories.Where(a => a.OutletId == Id).Select(a => a.IsInventory.Value).FirstOrDefault();
            bool status = true;

            if (data)
            {
                var dd = (from p in entities.tbl_KitchenRawIndent
                          join q in entities.tbl_KitchenStock
                              on p.RawMaterialId equals q.RawMaterialId
                          where p.ItemId == ItemId
                          select new { ItemId = p.ItemId, Qty = q.Quantity, RequiredQuantity = p.Quantity }).ToList();

                foreach (var item in dd)
                {
                    decimal quantity = item.RequiredQuantity * Qty;
                    if (item.Qty < quantity)
                    {
                        status = false;
                        return status;
                    }
                    if (item.Qty == 0)
                    {
                        status = false;
                        return status;
                    }
                }
            }


            return status;
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