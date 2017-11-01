using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Repository;
using NibsMVC.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
namespace NibsMVC.Repository
{
    public class cls_connection
    {
        SqlConnection con = new SqlConnection();
        SqlDataAdapter ad = null;
        SqlCommand cmd = null;
        int result = 0;

        public cls_connection(string ordertype,string Paymenttype,int BillNo,DateTime fromdate,DateTime dateto)
        {
            try
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "exec BillReportGenerate @OrderType, @PaymentType, @datefrom, @dateto, @BillNo";
                cmd.Parameters.AddWithValue("@OrderType", ordertype);
                cmd.Parameters.AddWithValue("@PaymentType", Paymenttype);
                cmd.Parameters.AddWithValue("@datefrom", fromdate);
                cmd.Parameters.AddWithValue("@dateto", dateto);
                cmd.Parameters.AddWithValue("@BillNo", BillNo);
                cmd.Connection = con;
              

                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
                
            }
            catch (Exception ex)
            {

            }

            // TODO: Add constructor logic here

        }
        #region Data Function
        public DataSet FillDataSet(SqlParameter[] PM, string SpName, DataSet ds, string[] TableName)
        {
           // SqlHelper.FillDataset(SqlHelper.conString, CommandType.StoredProcedure, SpName, ds, TableName, PM);
            
            return ds;
        }
        #endregion
    }
}