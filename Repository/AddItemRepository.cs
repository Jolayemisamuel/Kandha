using NibsMVC.EDMX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Models;
using WebMatrix.WebData;
using System.ComponentModel.DataAnnotations;
namespace NibsMVC.Repository
{
    public class AddItemRepository
    {
        NIBSEntities db = new NIBSEntities();
        public List<SelectListItem> GetListofcategory()
        {

            var selList = new List<SelectListItem>();
            List<GetUnitCategory> lst = (from item in db.tblCategories.ToList()
                                          select new GetUnitCategory()
                                          {
                                              CategoryId = item.CategoryId,
                                              Name = item.Name
                                          }).ToList<GetUnitCategory>();
            foreach (var item in lst)
            {
                selList.Add(new SelectListItem
                {
                    Text = item.Name.ToString(),
                    Value = item.CategoryId.ToString()
                });
            }
            return selList;
        }

        public List<SelectListItem> GetListofDepartment()
        {

            var selList = new List<SelectListItem>();
            List<DepartmentModel > lst = (from item in db.tbl_Department.ToList()
                                          where item .Active == true 
                                         select new DepartmentModel()
                                         {
                                             DepartmentId = item.DepartmentID,
                                             Department  = item.Department 
                                         }).ToList<DepartmentModel>();
            foreach (var item in lst)
            {
                selList.Add(new SelectListItem
                {
                    Text = item.Department .ToString(),
                    Value = item.DepartmentId.ToString()
                });
            }
            return selList;
        }

        internal List<GetAllItemList> BaseItemwise(int id, string acType, object editItemId)
        {
            throw new NotImplementedException();
        }

        //public List<SelectListItem> GetListofItems()
        //{

        //    var seIList = new List<SelectListItem>();
        //    List<MenuModel> lst = (from item in db.tblItems.ToList()
        //                                  select new MenuModel()
        //                                  {
        //                                      CategoryId = item.CategoryId,
        //                                      Name = item.Name
        //                                  }).ToList<MenuModel>();
        //    foreach (var item in lst)
        //    {
        //        seIList.Add(new SelectListItem
        //        {
        //            Text = item.Name.ToString(),
        //            Value = item.ItemId.ToString()
        //        });
        //    }
        //    return seIList;
        //}
        public List<SelectListItem> GetListofOutlet()
        {

            var selList = new List<SelectListItem>();
            List<OutletModel> lst = (from item in db.tblOutlets.ToList()
                                     select new OutletModel()
                                          {
                                              OutletId = item.OutletId,
                                              Name = item.Name
                                          }).ToList<OutletModel>();
            foreach (var item in lst)
            {
                selList.Add(new SelectListItem
                {
                    Text = item.Name.ToString(),
                    Value = item.OutletId.ToString()
                });
            }
            return selList;
        }
        public List<tblCategory> CategoryWise(string id)
        {

            var Dname = (from p in db.tblCategories where p.Active == true select p).ToList();
            return Dname;
        }

        public List<tblItem> offerlistitem(int id)
        {
            var offeritme = (from p in db.tblItems where p.CategoryId.Equals(id) && p.Active == true select p).ToList();
            return offeritme;
        }

        public List<GetAllItemList> BaseItemwise(int Id,string AcType)
        {

            var result = (from p in db.tblItems
                           join q in db.tblBasePriceItems
                              on p.ItemId equals q.ItemId  into record
                          from q in record.DefaultIfEmpty()
                          where (p.CategoryId == Id) 
                          select new
                          {
                              ItemId = p.ItemId,
                              Name = p.Name,
                              //Vat = (q.AcType == null) ? 0 : (q.AcType == "AC") ? 18 : 12 , //(q.Vat == null) ? 0 : q.Vat,
                              FullPrice = (q.FullPrice == null) ? 0 : q.FullPrice,
                              //HalfPrice = (q.HalfPrice == null) ? 0 : q.HalfPrice,
                              AcType = (q.Type == null) ? "" : q.Type,
                          });
            List<GetAllItemList> lst = new List<GetAllItemList>();
            foreach (var item in result)
            {
                if (item.AcType == "" || item.AcType == AcType)
                {
                GetAllItemList model = new GetAllItemList();
                model.FullPrice = item.FullPrice;
                //model.HalfPrice = item.HalfPrice;
                model.ItemId = item.ItemId;
                model.ItemName = item.Name;
                //model.Vat = Convert.ToDecimal(item.Vat);
                lst.Add(model);
                }
            }

            return lst;
        }
        public List<MenuAssignItemsModel> Itemwise(int CategoryId, int OutletId)
        {
            List<MenuAssignItemsModel> lst = new List<MenuAssignItemsModel>();
            var Items = (from q in db.tblItems
                          join p in db.tblBasePriceItems on q.ItemId equals p.ItemId
                         where q.CategoryId == CategoryId
                             && q.Active == true
                         select new
                         {
                             ItemId=q.ItemId,
                             ItemName=q.Name,
                             //Half=p.HalfPrice,
                             Full=p.FullPrice,
                             BasePriceId=p.BasePriceId
                         }).ToList();
            foreach (var item in Items)
            {
                var ItemId = (from p in db.tblMenuOutlets 
                              where p.ItemId == item.ItemId 
                              && p.CategoryId == CategoryId
                              && p.OutletId == OutletId select p.ItemId).ToList();
                MenuAssignItemsModel model = new MenuAssignItemsModel();
                model.FullPrice = item.Full;
                //model.HalfPrice = item.Half;
                model.ItemId = item.ItemId;
                model.ItemName = item.ItemName;
                if (ItemId.Count>0)
                {
                    model.Assigned = true;
                }
                else
                {
                    model.Assigned = false;
                }
                model.BasePriceId = item.BasePriceId;
                lst.Add(model);
            }


            return lst;
        }
        public List<tblOutlet> Outletwise(int id)
        {
            var outletdetails = (from q in db.tblOutlets where q.OutletId == id select q).ToList();
            return outletdetails;
        }


        public List<tblItem> offeritem(int p)
        {
            var list = (from q in db.tblItems where q.CategoryId.Equals(p) select q).ToList();
            return list;
        }

        public List<tblBasePriceItem> Offeritem(int id)
        {
            var itemdetails = (from p in db.tblBasePriceItems where p.ItemId.Equals(id) select p).ToList();
            return itemdetails;
        }



        public string GetListofpriceitems(int id)
        {
            var priceitem = (from q in db.tblBasePriceItems where q.ItemId.Equals(id) select q).FirstOrDefault();
            string price = priceitem.FullPrice.ToString ();//+ "-" + priceitem.HalfPrice;
            return price;
        }



        public List<string> billingitemlist(int id)
        {
            var name = WebSecurity.CurrentUserName;
            var outlet = (from p in db.tblOperators where p.Name.Equals(name) select p.OutletId).FirstOrDefault();
            int outId = Convert.ToInt32(outlet);
            var billitesm = (from p in db.tblItems where (from q in db.tblMenuOutlets where q.OutletId == outId && p.CategoryId == id && p.Active == true select q.ItemId).Contains(p.ItemId) select p).ToList();
            List<string> obj = new List<string>();
            foreach (var item in billitesm)
            {
                string biite = item.Name + "-" + item.ItemId;
                obj.Add(biite);
            }

            return obj;

        }



        public string billscollectin(int id)
        {
            var bilitems = (from p in db.tblBasePriceItems where p.ItemId.Equals(id) select p).FirstOrDefault();
            string items = bilitems.FullPrice.ToString() ;// + "-" + bilitems.HalfPrice + "-" + bilitems.Vat;
            return items;
        }



    }

}