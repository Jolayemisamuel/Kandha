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
        #region
        public Ledgergroupmodel EditLedgerGroupModel(int Id)
        {
            Ledgergroupmodel model = new Ledgergroupmodel();

            if (Id != 0)
            {
                var data = _entities.tblLedgerGroups.Where(x => x.LedgerGroupId == Id).SingleOrDefault();
                model.GroupName = data.LedgerGroupName;
                model.CategoryId = data.CategoryId;
                model.CategoryName = data.tblLedgerCategory.LedgerCategoryName;
                model.RecordId = data.LedgerGroupId;             
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
            var duplicate = _entities.tblLedgerGroups.Where(o => o.LedgerGroupName.Equals(model.GroupName) && o.CategoryId.Equals(model.CategoryId)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.RecordId != 0)
                    {
                        tb = _entities.tblLedgerGroups.Where(x => x.LedgerGroupId== model.RecordId).SingleOrDefault();
                        
                        tb.LedgerGroupName = model.GroupName;
                        tb.CategoryId = model.CategoryId;                        
                        _entities.SaveChanges();
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.LedgerGroupName = model.GroupName;
                        tb.CategoryId = model.CategoryId;
                        _entities.tblLedgerGroups.Add(tb);  
                        _entities.SaveChanges();

                        return "Record Saved Successfully...";
                    }

                }
                catch(Exception ex)
                {
                    return ex.Message;

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
                var data = _entities.tblLedgerGroups.Where(x => x.LedgerGroupId == Id).SingleOrDefault();
                
                _entities.tblLedgerGroups.Remove(data);
               
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "Please Delete the Ledger First Then Delete the Ledger Group";
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
                model.RecordId = item.LedgerGroupId;
                
                List.Add(model);

            }
            return List;
        }
        #endregion
        #region
        public LedgerModel EditLedgerModel(int Id)
        {
            LedgerModel model = new LedgerModel();            

            if (Id != 0)
            {
                var data = _entities.tblLedgerMasters.Where(x => x.LedgerMasterId == Id).SingleOrDefault();
                model.Id = data.LedgerMasterId;
                
                model.LedgerName = data.LedgerName;
                model.LedgerGroupId = data.tblLedgerGroup.LedgerGroupId;
                model.LedgerGroupName = data.tblLedgerGroup.LedgerGroupName;
                model.Date = data.Date;
                model.OpeningBalance = data.OPBalance;
                model.Percentage = data.Percentage;
                model.TransferType = data.Transfer_Type;
                
                return model;
            }
            else
            {
                return model;
            }
        }
        public string SaveLedger(LedgerModel model)
        {
            tblLedgerMaster tb = new tblLedgerMaster();
            
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;
            

            var duplicate = _entities.tblLedgerMasters.Where(o => o.LedgerName.Equals(model.LedgerName) && o.tblLedgerGroup.LedgerGroupId.Equals(model.LedgerGroupId) && o.OPBalance.Equals(model.OpeningBalance) && o.Percentage.Equals(model.Percentage) && o.Transfer_Type.Equals(model.TransferType)).SingleOrDefault();
            if (duplicate == null)
            {
                try
                {
                    if (model.Id != 0)
                    {
                        tb = _entities.tblLedgerMasters.Where(x => x.LedgerMasterId == model.Id).SingleOrDefault();

                        tb.LedgerName = model.LedgerName;                        
                        tb.LedgerGroup = model.LedgerGroupId;
                        tb.Date = model.Date;
                        tb.OPBalance = model.OpeningBalance;
                        tb.Percentage = model.Percentage;
                        tb.RecordId = model.Record_Id;
                        tb.Transfer_Type = model.TransferType;
                        _entities.SaveChanges();

                        int id = tb.LedgerMasterId;

                        if (model.TransferType == "Credit")
                        {
                            Voucher_Entry_Credit tb1 = new Voucher_Entry_Credit();

                            if (_entities.Voucher_Entry_Credit.Where(x => x.voucher_ledger_accout_id == id) != null)
                            {

                                tb1 = _entities.Voucher_Entry_Credit.Where(x => x.voucher_ledger_accout_id == id && x.voucher_type=="OB").SingleOrDefault();

                                tb1.voucher_no = tb.RecordId.ToString();
                                tb1.Voucher_date = model.Date;
                                tb1.record_no = tb.RecordId.ToString();
                                tb1.record_date = model.Date;
                                tb1.voucher_sno = 1;
                                tb1.voucher_tb = "To";
                                tb1.voucher_cr_amount = model.OpeningBalance;
                                
                                tb1.voucher_year = FinYear.Trim();
                                tb1.create_date = DateTime.Now;
                                tb1.from_form_name = "Ledger_master";
                                tb1.from_form_id = id;
                                tb1.account_type = "OB";
                                tb1.userid = WebSecurity.CurrentUserId;
                                _entities.Voucher_Entry_Credit.Add(tb1);
                                _entities.SaveChanges();
                            }
                            else
                            {
                                tb1.voucher_no = tb.RecordId.ToString();
                                tb1.Voucher_date = model.Date;
                                tb1.record_no = tb.RecordId.ToString();
                                tb1.record_date = model.Date;
                                tb1.voucher_sno = 1;
                                tb1.voucher_tb = "To";
                                tb1.voucher_ledger_accout_id = id;
                                tb1.voucher_cr_amount = model.OpeningBalance;
                                tb1.voucher_type = "OB";
                                tb1.voucher_year = FinYear.Trim();
                                tb1.create_date = DateTime.Now;
                                tb1.from_form_name = "Ledger_master";
                                tb1.from_form_id = id;
                                tb1.account_type = "OB";
                                tb1.userid = WebSecurity.CurrentUserId;
                                _entities.Voucher_Entry_Credit.Add(tb1);
                                _entities.SaveChanges();
                            }
                        }
                        else if (model.TransferType == "Debit")
                        {
                            Voucher_Entry_Debit tb2 = new Voucher_Entry_Debit();

                            if (_entities.Voucher_Entry_Debit.Where(x => x.voucher_ledger_accout_id == id) != null)
                            {

                                tb2 = _entities.Voucher_Entry_Debit.Where(x => x.voucher_ledger_accout_id == id && x.voucher_type=="OB").SingleOrDefault();

                                tb2.voucher_no = tb.RecordId.ToString();
                                tb2.voucher_date = model.Date;
                                tb2.record_no = tb.RecordId.ToString();
                                tb2.record_date = model.Date;
                                tb2.voucher_sno = 1;
                                tb2.voucher_tb = "To";
                                tb2.voucher_dbt_amount = model.OpeningBalance;
                                
                                tb2.voucher_year = DateTime.Now.ToFinancialYear();//FinYear.Trim();
                                tb2.create_date = DateTime.Now;
                                tb2.from_form_name = "Ledger_master";
                                tb2.from_form_id = model.Id;
                                tb2.account_type = "OB";
                                tb2.userid = WebSecurity.CurrentUserId;
                                _entities.Voucher_Entry_Debit.Add(tb2);
                                _entities.SaveChanges();
                            }
                            else
                            {
                                tb2.voucher_no = tb.RecordId.ToString();
                                tb2.voucher_date = model.Date;
                                tb2.record_no = tb.RecordId.ToString();
                                tb2.record_date = model.Date;
                                tb2.voucher_sno = 1;
                                tb2.voucher_tb = "To";
                                tb2.voucher_ledger_accout_id = id;
                                tb2.voucher_dbt_amount = model.OpeningBalance;
                                tb2.voucher_type = "OB";
                                tb2.voucher_year = DateTime.Now.ToFinancialYear();//FinYear.Trim();
                                tb2.create_date = DateTime.Now;
                                tb2.from_form_name = "Ledger_master";
                                tb2.from_form_id = model.Id;
                                tb2.account_type = "OB";
                                tb2.userid = WebSecurity.CurrentUserId;
                                _entities.Voucher_Entry_Debit.Add(tb2);
                                _entities.SaveChanges();
                            }

                        }
                        return "Record Updated Successfully...";
                    }
                    else
                    {
                        tb.LedgerName = model.LedgerName;
                        tb.LedgerGroup = model.LedgerGroupId;
                        tb.Date = model.Date;
                        tb.OPBalance = model.OpeningBalance;
                        if (_entities.tblLedgerMasters.Select(p=>p.RecordId).Count() > 0)
                        {
                            tb.RecordId = _entities.tblLedgerMasters.Select(p => p.RecordId).Max() + 1;
                        }
                        else
                        {
                            tb.RecordId = 1;
                        }
                        tb.Percentage = model.Percentage;
                        tb.Transfer_Type = model.TransferType;
                        _entities.tblLedgerMasters.Add(tb);
                        _entities.SaveChanges();
                        int id = tb.LedgerMasterId;

                        if (model.TransferType=="Credit")
                        {
                            Voucher_Entry_Credit tb1 = new Voucher_Entry_Credit();
                            tb1.voucher_no = tb.RecordId.ToString();
                            tb1.Voucher_date = model.Date;
                            tb1.record_no = tb.RecordId.ToString();
                            tb1.record_date = model.Date;
                            tb1.voucher_sno = 1;
                            tb1.voucher_tb = "To";
                            tb1.voucher_ledger_accout_id = id;
                            tb1.voucher_cr_amount = model.OpeningBalance;
                            tb1.voucher_type = "OB";
                            tb1.voucher_year = FinYear.Trim();
                            tb1.create_date = DateTime.Now;
                            tb1.from_form_name = "Ledger_master";
                            tb1.from_form_id = id;
                            tb1.account_type = "OB";
                            tb1.userid = WebSecurity.CurrentUserId;
                            _entities.Voucher_Entry_Credit.Add(tb1);
                            _entities.SaveChanges();
                        }
                        else if(model.TransferType=="Debit")
                        {
                            Voucher_Entry_Debit tb2 = new Voucher_Entry_Debit();
                            tb2.voucher_no = tb.RecordId.ToString();
                            tb2.voucher_date = model.Date;
                            tb2.record_no = tb.RecordId.ToString();
                            tb2.record_date = model.Date;
                            tb2.voucher_sno = 1;
                            tb2.voucher_tb = "To";
                            tb2.voucher_ledger_accout_id = tb.LedgerMasterId;
                            tb2.voucher_dbt_amount = model.OpeningBalance;
                            tb2.voucher_type = "OB";
                            tb2.voucher_year = DateTime.Now.ToFinancialYear();//FinYear.Trim();
                            tb2.create_date = DateTime.Now;
                            tb2.from_form_name = "Ledger_master";
                            tb2.from_form_id = model.Id;
                            tb2.account_type = "OB";
                            tb2.userid = WebSecurity.CurrentUserId;
                            _entities.Voucher_Entry_Debit.Add(tb2);
                            _entities.SaveChanges();
                        }

                       
                        return "Record Saved Successfully...";
                    }

                }
                catch(Exception ex)
                {
                    
                    return ex.Message; 

                }

            }
            else
            {
                return model.LedgerName + " Already Exits";
            }

        }
        public string DeleteLedger(int Id)
        {
            try
            {
                var data = _entities.tblLedgerMasters.Where(x => x.LedgerMasterId == Id).SingleOrDefault();
                if (data.Transfer_Type == "Credit")
                {
                    var data1 = _entities.Voucher_Entry_Credit.Where(x => x.voucher_ledger_accout_id == Id).SingleOrDefault();
                    _entities.Voucher_Entry_Credit.Remove(data1);
                }
                else
                {
                    var data1 = _entities.Voucher_Entry_Debit.Where(x => x.voucher_ledger_accout_id == Id).SingleOrDefault();
                    _entities.Voucher_Entry_Debit.Remove(data1);
                }
                _entities.tblLedgerMasters.Remove(data);
                
                _entities.SaveChanges();
                return "Record deleted Successfully...";
            }
            catch
            {
                return "something Wrong try Agian !";
            }
        }
        public List<LedgerModel> ShowAllLedgerList()
        {
            List<LedgerModel> List = new List<LedgerModel>();
            var data = _entities.tblLedgerMasters.ToList();
            foreach (var item in data)
            {
                LedgerModel model = new LedgerModel();
                model.Id = item.LedgerMasterId;
                model.LedgerName = item.LedgerName;
                model.LedgerGroupId = item.tblLedgerGroup.LedgerGroupId;
                model.LedgerGroupName = item.tblLedgerGroup.LedgerGroupName;
                model.OpeningBalance = item.OPBalance;
                model.Date = item.Date;
                model.Percentage = item.Percentage;
                model.TransferType = item.Transfer_Type;
                List.Add(model);

            }
            return List;
        }
        #endregion
        #region
        
        
        #endregion
    }
}