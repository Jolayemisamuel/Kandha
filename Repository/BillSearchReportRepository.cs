using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Text;
namespace NibsMVC.Repository
{
    public class BillSearchReportRepository
    {
        NIBSEntities entities = new NIBSEntities();
        public BillSearchReportModel getModel()
        {
            BillSearchReportModel model = new BillSearchReportModel();
            model.getPaymentType = getPaymentType();
            model.getBillingType = getBillingType();
            return model;
        }
        public List<SelectListItem> getPaymentType()
        {
            var lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Cash", Value = "Cash" });
            lst.Add(new SelectListItem { Text = "Cheque", Value = "Cheque" });
            lst.Add(new SelectListItem { Text = "Card", Value = "Card" });
            lst.Add(new SelectListItem { Text = "Cash&Card", Value = "Cash&Card" });
            return lst;
        }
        public List<SelectListItem> getBillingType()
        {
            var lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Text = "Billing", Value = "R" });
            lst.Add(new SelectListItem { Text = "Packing", Value = "T" });
            lst.Add(new SelectListItem { Text = "Home Delivery", Value = "H" });

            return lst;
        }
        public BillSearchReportModel getSearchModel(BillSearchReportModel m)
        {
           
            BillSearchReportModel model = new BillSearchReportModel();
            DataSet ds = new DataSet();
            string datefrom;
            string dateto;
            if (m.DateTo.HasValue)
            {
                dateto = Convert.ToDateTime(m.DateTo).AddDays(1).ToShortDateString();
            }
            else
            {
                dateto = string.Empty;
            }
            if (m.DateFrom.HasValue)
            {
                datefrom = Convert.ToDateTime(m.DateFrom).ToShortDateString();
            }
            else
            {
                datefrom = string.Empty;
            }
            ds = excuteSp(m.OrderType == null ? string.Empty : m.OrderType, m.PaymentType == null ? string.Empty : m.PaymentType, m.BillNo == null ? 0 : Convert.ToInt32(m.BillNo), datefrom, dateto);



            if (ds.Tables.Count > 0)
            {
                string s = GetJson(ds.Tables[0]);
                model.allbill = s;
            }

            model.getPaymentType = getPaymentType();
            model.getBillingType = getBillingType();
            model.GetallVats = getAllvats();
            return model;

        }

       
        public List<TotalVatModel> getAllvats()
        {
            //string datefrom;
            //string dateto;
            //if (m.DateTo.HasValue)
            //{
            //    dateto = Convert.ToDateTime(m.DateTo).AddDays(1).ToShortDateString();
            //}
            //else
            //{
            //    dateto = string.Empty;
            //}
            //if (m.DateFrom.HasValue)
            //{
            //    datefrom = Convert.ToDateTime(m.DateFrom).ToShortDateString();
            //}
            //else
            //{
            //    datefrom = string.Empty;
            //}
            var lst = new List<TotalVatModel>();
            //var result=entities.tblBillDetails.Where(a=>a.tblBillMaster.BillId==m.BillNo 
            //    || System.Data.Entity.DbFunctions.TruncateTime(a.tblBillMaster.BillDate)>=Convert.ToDateTime(m.DateFrom.Value.Date))
            var data = entities.tblBasePriceItems.ToList();
            foreach (var item in data.GroupBy(a=>a.Vat))
            {
                TotalVatModel model = new TotalVatModel();
                model.Vat = item.Key;

                lst.Add(model);
            }
            return lst;
        }
        string GetJson(DataTable table)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;
            foreach (DataRow dataRow in table.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn column in table.Columns)
                {
                    row.Add(column.ColumnName.Trim(), dataRow[column]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
        public DataSet excuteSp(string ordertype, string Paymenttype, int BillNo, string fromdate, string dateto)
        {
            SqlConnection con = new SqlConnection();

            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            try
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                con.Open();
                cmd.CommandText = "exec BillReportGenerate @OrderType, @PaymentType, @datefrom, @dateto, @BillNo";
                cmd.Parameters.AddWithValue("@OrderType", ordertype);
                cmd.Parameters.AddWithValue("@PaymentType", Paymenttype);
                cmd.Parameters.AddWithValue("@datefrom", fromdate);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@BillNo", BillNo);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);

            }
            catch (Exception ex)
            {

            }
            return ds;
        }
        public List<BillItemReportModel> getBillItems(int BillId)
        {
            List<BillItemReportModel> lst = new List<BillItemReportModel>();
            var data = entities.tblBillDetails.Where(a => a.BillId == BillId).ToList();
            foreach (var item in data)
            {
                BillItemReportModel model = new BillItemReportModel();
                model.Amount = item.Amount;
                model.FullQty = item.FullQty.Value;
                model.ItemName = item.tblItem.Name;
                model.Vat = item.Vat.Value;
                model.VatAmount = item.VatAmount.Value;
                lst.Add(model);
            }
            return lst;
        }



    }
}