using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Xml.Linq;
using WebMatrix.WebData;
using System.Text;
namespace NibsMVC.Repository
{
    public class ShiftTableRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public string ShiftTable(ShiftTableModel model, string ShidtPath, string MasterPath)
        {
            XDocument xd = XDocument.Load(ShidtPath);
            var name = WebSecurity.CurrentUserName;
            var outletid = (from n in _entities.tblOperators where n.Name.Equals(name) select n.OutletId).FirstOrDefault();
            int oulte = Convert.ToInt32(outletid);
            var result = from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item;

            StringBuilder sb = new StringBuilder();
            //=== code for shift xml data
            XDocument master = XDocument.Load(MasterPath);

                    foreach (XElement itemElement in result)
                    {
                        
                        itemElement.SetElementValue("UserId", oulte.ToString());
                        itemElement.SetElementValue("TableNo", model.MasterTable);
                        itemElement.SetElementValue("ItemId", itemElement.Element("ItemId").Value);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("Fullprice", itemElement.Element("Fullprice").Value);
                        itemElement.SetElementValue("HalfPrice", itemElement.Element("HalfPrice").Value);
                        itemElement.SetElementValue("VatAmt", itemElement.Element("VatAmt").Value);
                        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                        itemElement.SetElementValue("Amount", itemElement.Element("Amount").Value);
                        itemElement.SetElementValue("VatAmountCharges", itemElement.Element("VatAmountCharges").Value);
                        xd.Save(MasterPath);
                    }
                    //master.Save(MasterPath);

            int counter = 1;
            decimal TotalVatAmount = 0;
            decimal TotalAmount = 0;
            sb.Append("<table class='table'><thead><tr><th>S.No</th><th>ItemName</th><th>Full Qty</th><th>Half Qty</th><th>Full Price</th><th>Half Price</th><th>Amount</th><th>Vat</th></tr></thead>");
            sb.Append("<tbody>");
            foreach (var item in result)
            {

                sb.Append("<tr>");
                sb.Append("<td>" + counter + "</td><td>" + item.Element("ItemName").Value + "</td><td>" + item.Element("FullQty").Value + "</td><td>" + item.Element("HalfQty").Value + "</td><td>" + item.Element("Fullprice").Value + "</td><td>" + item.Element("HalfPrice").Value + "</td><td>" + item.Element("Amount").Value + "</td><td>" + item.Element("VatAmt").Value + "</td><td><a href='#' id=" + item.Element("ItemId").Value + " class='deleterow'><span class='fa fa-trash-o'><span></a></td>");
                sb.Append("</tr>");
                counter++;
                TotalVatAmount += Convert.ToDecimal(item.Element("VatAmountCharges").Value);
                TotalAmount += Convert.ToDecimal(item.Element("Amount").Value);
            }
            sb.Append("</tbody></table>");
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item);
            items.Remove();
            xd.Save(ShidtPath);
            return sb.ToString() + "^" + TotalVatAmount + "^" + TotalAmount;
        }
    }
}