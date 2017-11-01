using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Text;
using System.Xml.Linq;
using NibsMVC.Repository;
namespace NibsMVC.Repository
{
    public class OpenFoodRepository
    {
        public StringBuilder AddOpenFoodToXml(string FilePath, string KotPath, string TableNo, string Name, string Price, string Quantity, string Vat)
         {
            XMLTablesRepository xml=new XMLTablesRepository();
            XDocument xd = XDocument.Load(FilePath);
            StringBuilder sb = new StringBuilder();
            int OutletId = xml.getOutletId();
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == OutletId.ToString()
                         && item.Element("TableNo").Value == TableNo
                         && item.Element("ItemName").Value == Name.ToString()
                         select item).ToList();
            if (items.Count>0)
            {
                foreach (XElement itemElement in items)
                        {
                           decimal Prices = Convert.ToInt32(Price) * Convert.ToInt32(Quantity);
                            var totalamount = Convert.ToDecimal(Prices) + Convert.ToDecimal(itemElement.Element("Amount").Value);
                            var vatamtchrg = (totalamount * Convert.ToDecimal(Convert.ToDecimal(Vat)) / 100);
                            int Full = Convert.ToInt32(itemElement.Element("FullQty").Value) + Convert.ToInt32(Quantity);
                            itemElement.SetElementValue("FullQty", Full);
                            itemElement.SetElementValue("Amount", totalamount);
                            itemElement.SetElementValue("VatAmountCharges", vatamtchrg);
                            
                        }
                xd.Save(FilePath);
            }
            else
            {
                decimal Prices = Convert.ToDecimal(Price) * Convert.ToDecimal(Quantity);
                var vatamtchrg = (Convert.ToDecimal(Prices) * Convert.ToDecimal(Convert.ToDecimal(Vat)) / 100);
                var newElement = new XElement("Items",
                     new XElement("UserId", OutletId.ToString()),
                     new XElement("TableNo", TableNo),
                     new XElement("ItemId", "0"),
                     new XElement("ItemName", Name),
                     new XElement("FullQty", Quantity),
                     new XElement("HalfQty", "0"),
                     new XElement("Fullprice", Price),
                     new XElement("HalfPrice", "0"),
                     new XElement("Amount", Prices),
                     new XElement("VatAmt", Vat),
                     new XElement("VatAmountCharges", vatamtchrg),
                     new XElement("OfferQty", "0"));
                xd.Element("Item").Add(newElement);
                xd.Save(FilePath);
            }
            XDocument kot = XDocument.Load(KotPath);
            var data = (from item in kot.Descendants("Items")
                        where item.Element("UserId").Value == OutletId.ToString()
                        && item.Element("TableNo").Value == TableNo.ToString()
                        && item.Element("ItemName").Value == Name
                        select item).ToList();
            if (data.Count>0)
            {
                foreach (XElement itemElement in items)
                    {
                        int Full = Convert.ToInt32(Quantity) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("FullQty", Full);
                    }
            }
            else
            {
                var newElement = new XElement("Items",
                         new XElement("UserId", OutletId.ToString()),
                         new XElement("TableNo", TableNo),
                         new XElement("ItemName", Name),
                         new XElement("FullQty", Quantity),
                         new XElement("HalfQty", "0"));
                kot.Element("Item").Add(newElement);
            }
            kot.Save(KotPath);
            return sb;
        }
    }
}