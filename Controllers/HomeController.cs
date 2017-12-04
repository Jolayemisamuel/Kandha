using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.Repository;
using NibsMVC.Models;
using System.Web.Security;
using NibsMVC.Filters;
using WebMatrix.WebData;
using NibsMVC.EDMX;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace NibsMVC.Controllers
{

    public class HomeController : Controller
    {
        AdminHomeRepository repo = new AdminHomeRepository();
        NIBSEntities entities = new NIBSEntities();
        KOTEntities kot_entities = new KOTEntities();
        OutletHomeRepository outrepo = new OutletHomeRepository();
        [Authorize(Roles = "admin,Outlet,Operator")]
        public ActionResult Index()
        {
           


                DateTime dt = Convert.ToDateTime(DateTime.Today.ToString("dd-MMM-yyyy"));
            var billsKOT = (from p in kot_entities.vwBillMsts where p.BillDate >= dt select p).ToList();
            var billsCount = (from p in entities.tblBillMasters where p.BillDate>=dt select p).ToList().Count;
            if (billsKOT.Count > billsCount)
            {
                foreach (var item in billsKOT)
                {
                    var itemcheck = (from p in entities.tblBillMasters where p.TokenNo == item.BillNo && p.BillingType == item.Description && p.BillDate == item.BillDate  select p).FirstOrDefault();

                    
                    if (itemcheck == null)
                    {
                        tblBillMaster tb = new tblBillMaster();
                        tb.BillDate = item.BillDate;
                        tb.TotalAmount = item.Total;
                        tb.VatAmount = Convert.ToDecimal (item.taxamount);
                        tb.ServicChargesAmount = item.serviceChrge;
                        tb.DiscountAmount = item.Discount;
                        tb.NetAmount = item.NettValue;
                        tb.TableNo = item.tableNo==null?"": item.tableNo;
                        tb.CustomerName = "";
                        tb.OutletId = item.outlet;
                        tb.BillingType = item.Description;
                        tb.TokenNo = Convert .ToInt32 ( item.BillNo);
                        tb.Address = item.address;
                        tb.PaymentType = item.paymenttype;
                        tb.ChequeNo = item.chqno;
                        tb.ChequeDate = Convert.ToDateTime ("01-Jan-1900");// item.chqdate
                        tb.Discount = item.disPer;
                        tb.ContactNo = item.contactNo;
                        tb.PackingCharges = item.packingChags;
                        tb.ServiceTax = item.Servicetax;
                        tb.Isprinted = Convert.ToBoolean (item.isprnt);
                        tb.NetAmountWithoutDiscount = item.befDisc;
                        entities.tblBillMasters.Add(tb);
                        entities.SaveChanges();

                        var billsDetKOT = (from p in kot_entities.vwBillDets where p.BillTrans_UID.Equals(item.UID)  select p).ToList();
                        foreach (var itemDet in billsDetKOT)
                        {
                            tblBillDetail tbDet = new tblBillDetail();
                            int billid = (from p in entities.tblBillMasters select p.BillId).Max();
                            var itemid = (from p in entities.tblItems where p.Name == itemDet .ItemName  && p.ItemCode  == itemDet.ItemCode  select p.ItemId ).SingleOrDefault();
                            tbDet.BillId = billid;
                            tbDet.ItemId = itemid;
                            tbDet.FullQty = (Int32)itemDet.Qty;
                            tbDet.HalfQty = 0;
                            tbDet.Amount = itemDet.Value;

                            if (item.Description == "Ac Hall")
                                tbDet.VatAmount = itemDet.Value + (itemDet.Value * (decimal)0.05);
                            else
                                tbDet.VatAmount = itemDet.Value + (itemDet.Value * (decimal)0.05);

                            if (item.Description == "Ac Hall")
                                tbDet.Vat = Convert.ToDecimal("5");
                            else
                                tbDet.Vat = Convert.ToDecimal("5");

                            entities.tblBillDetails.Add(tbDet);
                            entities.SaveChanges();


                            var RawList = (from p in entities.tbl_KitchenRawIndent where p.ItemId==tbDet.ItemId select p).ToList();
                            if (RawList != null)
                            {
                                if(RawList.Count >0)
                                {
                                    foreach (var li in RawList)
                                    {
                                        decimal qty = Convert.ToDecimal (li.Quantity /li.Portion);
                                        string pUnit = li.tbl_RawMaterials.units;
                                        string cUnit = li.Unit;
                                        // Convert cUnit (Consumption Unit)  from pUnit (Purchase Unit) 
                                        if (pUnit!=cUnit)
                                        {
                                            qty=unitConvert(qty, cUnit, pUnit);
                                        }
                                        qty = Math.Round(qty, 2, MidpointRounding.AwayFromZero);
                                        if (qty == 0) qty = (decimal)0.01;
                                        string qry = "  select Id,MaterialId,Rate,Date,Qty,IssQty,ConsumptionQty=isnull(ConsumptionQty,0),table1='os' from tblOpStckRate where IssQty > isnull(ConsumptionQty,0)   and MaterialId = " + li.RawMaterialId  ;  
                                        qry = qry + " union all ";
                                        qry = qry + " select Id,MaterialId,Rate,Date,Qty,IssQty,ConsumptionQty=isnull(ConsumptionQty,0),table1='gs' from tblGRNStock where IssQty > isnull(ConsumptionQty,0)   and MaterialId = " + li.RawMaterialId + " order by date ";
                                        string webconnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                                        SqlConnection con = new SqlConnection(webconnection);
                                        SqlCommand cmd = new SqlCommand(qry, con);
                                        DataTable dt1 = new DataTable();
                                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                                        sda.Fill(dt1);
                                        qry = "";
                                        foreach (DataRow dr in dt1.Rows)
                                        {
                                           

                                            if (qty > 0)
                                            {
                                                if (dr["table1"].ToString() == "os")
                                                {
                                                    if (qty <= Convert.ToDecimal(dr["IssQty"]) - Convert.ToDecimal(dr["ConsumptionQty"]))
                                                    {
                                                        qry = qry + " update tblOpStckRate  set ConsumptionQty = isnull(ConsumptionQty,0) + " + qty+ " where MaterialId = " + li.RawMaterialId + " and id=" + dr["id"];
                                                        qry = qry + " insert into  tblConsumption values( " + tbDet.BillDetailsId + ", '" + li.RawMaterialId + "', " + qty+ "," + dr["Rate"].ToString() + " ) ";
                                                        qty-= qty;



                                                    }
                                                    else
                                                    {
                                                        qry = qry + " update tblOpStckRate  set ConsumptionQty = isnull(ConsumptionQty,0) + " + (Convert.ToDecimal(dr["IssQty"]) - Convert.ToDecimal(dr["ConsumptionQty"])) + " where MaterialId = " + li.RawMaterialId + " and id=" + dr["id"];
                                                        qry = qry + " insert into  tblConsumption values( " + tbDet.BillDetailsId + ", '" + li.RawMaterialId + "', " + Convert.ToDecimal(dr["ConsumptionQty"]) + "," + dr["Rate"].ToString() + " ) ";
                                                        qty-= Convert.ToDecimal(dr["ConsumptionQty"]);
                                                    }

                                                }
                                                else
                                                {
                                                    if (qty<= Convert.ToDecimal(dr["IssQty"]) - Convert.ToDecimal(dr["ConsumptionQty"]))
                                                    {
                                                        qry = qry + " update tblGRNStock  set ConsumptionQty = isnull(ConsumptionQty,0) + " + qty+ " where MaterialId = " + li.RawMaterialId + " and id=" + dr["id"];
                                                        qry = qry + " insert into  tblConsumption values( " + tbDet.BillDetailsId + ", '" + li.RawMaterialId + "', " + qty+ "," + dr["Rate"].ToString() + " ) ";
                                                        qty-= qty;
                                                    }
                                                    else
                                                    {
                                                        qry = qry + " update tblGRNStock  set ConsumptionQty = isnull(ConsumptionQty,0) + " + (Convert.ToDecimal(dr["IssQty"]) - Convert.ToDecimal(dr["ConsumptionQty"])) + " where MaterialId = " + li.RawMaterialId + " and id=" + dr["id"];
                                                        qry = qry + " insert into  tblConsumption values( " + tbDet.BillDetailsId + ", '" + li.RawMaterialId + "', " + Convert.ToDecimal(dr["ConsumptionQty"]) + "," + dr["Rate"].ToString() + " ) ";
                                                        qty-= Convert.ToDecimal(dr["ConsumptionQty"]);
                                                    }



                                                }
                                            }
                                        }
                                        con = new SqlConnection(webconnection);
                                        con.Open();
                                        cmd = new SqlCommand(qry, con);
                                        cmd.ExecuteNonQuery();
                                        con.Close();

                                    }

                                }
                            }



                        }

                    }

                }
            }

           


            string[] roles = Roles.GetRolesForUser();
            foreach (var role in roles)
            {
                if (role == "admin")
                {
                    return View(repo.getAdminHomeReports());
                }
                else if (role == "Outlet")
                {
                    ViewBag.RunningTable = getRunningTable();
                    return View(outrepo.getOutletHomeReports());
                }
                else
                {
                    return View(outrepo.getAllBillData());
                }
            }
            return View();

        }

        public decimal unitConvert(decimal val, string frUnit, string toUnit)
        {
            if((frUnit=="Gms"|| frUnit == "ML") && (toUnit == "Kgs"|| toUnit == "Ltr"))
            {
                return (val / 1000);
            }
            else if ((frUnit == "Kgs" || frUnit == "Ltr") && (toUnit == "Gms" || toUnit == "ML"))
            {
                return (val * 1000);
            }
            else
            {
                return val;
            }
            
        }
        public List<GetRunningTable> getRunningTable()
        {
            List<GetRunningTable> lst = new List<GetRunningTable>();
            foreach (var item in entities.tblTableMasters.Where(a => a.OutletId == WebSecurity.CurrentUserId).ToList())
            {
                //var filepath = Server.MapPath("/xmltables/table" + TableNo + ".xml");
                GetRunningTable model = new GetRunningTable();

                var path = Server.MapPath("/xmltables/table" + item.TableNo + ".xml");
                if (System.IO.File.Exists(path))
                {
                    XDocument xd = XDocument.Load(path);
                    var items = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == WebSecurity.CurrentUserId.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                                 select p).ToList();
                    if (items.Count > 0)
                    {
                        model.TableNo = item.TableNo;
                        lst.Add(model);
                    }
                }


            }
            return lst;
        }
        [NonAction]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        [NonAction]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ChangeInventory()
        {

            return View(outrepo.getinventory());
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeInventory(ChangeInventoryModel model)
        {
            outrepo.UpdateInventory(model);
            return RedirectToAction("Index", "Home");
        }
    }
}
