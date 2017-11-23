using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Web.Mvc;
using System.Text;
using WebMatrix.WebData;
using System.Xml.Linq;

namespace NibsMVC.Repository
{
    public class LedgerRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public Ledgergroupmodel EditLedgerGroupModel(int Id)
        {
            Ledgergroupmodel model = new Ledgergroupmodel();

            if (Id != 0)
            {
                var data = _entities.tblLedgerGroups.Where(x => x.Record_Id == Id).SingleOrDefault();
                model.GroupName = data.LedgerGroupName;
                model.CategoryId = data.Category;
                model.CategoryName = data.tblLedgerCategory.LedgerCategoryName;
                model.RecordId = data.Record_Id;             
                return model;
            }
            else
            {
                return model;
            }
        }
        public string SaveLedgerGroup(Ledgergroupmodel model)
        {
            tblLedgerGroup tb = new tblLedgerGroup();
            var duplicate = _entities.tblLedgerGroups.Where(o => o.LedgerGroupName.Equals(model.GroupName) && o.Category.Equals(model.CategoryId)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.RecordId != 0)
                    {
                        tb = _entities.tblLedgerGroups.Where(x => x.Record_Id== model.RecordId).SingleOrDefault();
                        
                        tb.LedgerGroupName = model.GroupName;
                        tb.Category = model.CategoryId;                        
                        _entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.LedgerGroupName = model.GroupName;
                        tb.Category = model.CategoryId;
                        _entities.tblLedgerGroups.Add(tb);                        
                        _entities.SaveChanges();
                        return "Record Saved Successfully...";
                    }

                }
                catch
                {
                    return "something Wrong try Agian !";

                }

            }
            else
            {
                return model.GroupName + " Already Exits";
            }

        }
        public string DeleteLedgerGroup(int Id)
        {
            try
            {
                var data = _entities.tblLedgerGroups.Where(x => x.Record_Id == Id).SingleOrDefault();
                
                _entities.tblLedgerGroups.Remove(data);
               
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }
        public List<Ledgergroupmodel> ShowAllLedgerGroupList()
        {
            List<Ledgergroupmodel> List = new List<Ledgergroupmodel>();
            var data = _entities.tblLedgerGroups.ToList();
            foreach (var item in data)
            {
                Ledgergroupmodel model = new Ledgergroupmodel();
                model.CategoryName = item.tblLedgerCategory.LedgerCategoryName;
                model.GroupName = item.LedgerGroupName;
                model.RecordId = item.Record_Id;
                List.Add(model);

            }
            return List;
        }
    }
}