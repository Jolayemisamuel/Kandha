using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using WebMatrix.WebData;
using System.Text;
using System.Xml;
using Microsoft.SqlServer;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.SqlServer.Server;
using System.IO;
namespace NibsMVC.Repository
{
    public class XMLTablesRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        CheckStockItemRepository outstock = new CheckStockItemRepository();
        public List<BillTableModel> GetListofTables()
        {
            int oulte = getOutletId();
            var result = _entities.tblTableMasters.Where(o => o.OutletId == oulte).ToList();

            List<BillTableModel> List = new List<BillTableModel>();
            foreach (var item in result)
            {
                string dirpath = Directory.GetCurrentDirectory();
                //XDocument xd = XDocument.Load(Path);
                //var current = from p in xd.Descendants("Items")
                //             where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                //             select p.Element("TableNo").Value;
                BillTableModel model = new BillTableModel();
                model.TableNo = item.TableNo.ToString();
                model.AcType = item.AcType.ToString();
                List.Add(model);
            }
            return List;
        }

        public List<GetBillingSubItemModel> GetAllItems(string Id)
        {
            var ID = Convert.ToInt32(Id);
            int oulte = getOutletId();
            var result = _entities.tblMenuOutlets.Where(o => o.OutletId == oulte && o.CategoryId == ID && o.tblItem.Active == true).ToList();
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            // query for when item is false in tbitems
            var dd = (from p in _entities.tblItems
                      join q in _entities.tblMenuOutlets
                          on p.ItemId equals q.ItemId
                      where p.Active == true &&
                      q.OutletId == oulte &&
                      q.CategoryId == ID
                      select new
                      {
                          ItemId = q.ItemId,
                          Name = q.tblItem.Name,
                          Color = p.tblCategory.Color,
                          TextColor = p.tblCategory.TextColor
                      }).ToList();
            foreach (var item in result)
            {
                GetBillingSubItemModel model = new GetBillingSubItemModel();
                model.Color = item.tblItem.tblCategory.Color;
                model.ItemId = item.ItemId;
                model.Name = item.tblItem.Name;
                model.TextColor = item.tblItem.tblCategory.TextColor;
                model.Outstock = outstock.CheckOutStockItem(item.ItemId);
                lst.Add(model);
            }

            return lst;
        }

        public void createNode(string UserId, string TableNo, string ItemId, string ItemName, string FullQty, string HalfQty, string Fullprice, string HalfPrice, string Amount, string VatAmt, string Vatamtchrg, string OfferQty, XmlTextWriter writer)
        {
            writer.WriteStartElement("Items");
            writer.WriteStartElement("UserId");
            writer.WriteString(UserId);
            writer.WriteEndElement();
            writer.WriteStartElement("TableNo");
            writer.WriteString(TableNo);
            writer.WriteEndElement();
            writer.WriteStartElement("ItemId");
            writer.WriteString(ItemId);
            writer.WriteEndElement();
            writer.WriteStartElement("ItemName");
            writer.WriteString(ItemName);
            writer.WriteEndElement();
            writer.WriteStartElement("FullQty");
            writer.WriteString(FullQty);
            writer.WriteEndElement();

            writer.WriteStartElement("HalfQty");
            writer.WriteString(HalfQty);
            writer.WriteEndElement();
            writer.WriteStartElement("Fullprice");
            writer.WriteString(Fullprice);
            writer.WriteEndElement();
            writer.WriteStartElement("HalfPrice");
            writer.WriteString(HalfPrice);
            writer.WriteEndElement();
            writer.WriteStartElement("Amount");
            writer.WriteString(Amount);
            writer.WriteEndElement();
            writer.WriteStartElement("VatAmt");
            writer.WriteString(VatAmt);
            writer.WriteEndElement();
            writer.WriteStartElement("VatAmountCharges");
            writer.WriteString(Vatamtchrg);
            writer.WriteEndElement();
            writer.WriteStartElement("OfferQty");
            writer.WriteString(OfferQty);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
        [ValidateAntiForgeryToken]
        public GetBillingModel GetBillingItem(string TableNo, string Path, string KotPath)
        {
            GetBillingModel model = new GetBillingModel();
            List<GetBillingItemModel> itemslst = new List<GetBillingItemModel>();
            List<GetKotItemModel> kotList = new List<GetKotItemModel>();
            XDocument xd = XDocument.Load(Path);
            int oulte = getOutletId();
            var result = (from item in xd.Descendants("Items")
                          where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo && item.Element("ItemName").Value != string.Empty
                          select item).ToList();
            foreach (var item in result)
            {
                GetBillingItemModel m = new GetBillingItemModel();
                m.ItemName = item.Element("ItemName").Value;
                m.FullQty = Convert.ToInt32(item.Element("FullQty").Value);
                m.FullPrice = Math.Round(Convert.ToDecimal(item.Element("Fullprice").Value), 2);
                m.Amount = Math.Round(Convert.ToDecimal(item.Element("Amount").Value), 2);
                m.Vat = Convert.ToDecimal(item.Element("VatAmt").Value);
                m.OfferQty = Convert.ToInt32(item.Element("OfferQty").Value);
                m.ItemId = Convert.ToInt32(item.Element("ItemId").Value);

                itemslst.Add(m);
            }
            model._getbillingItems = itemslst;
            XDocument xdKot = XDocument.Load(KotPath);
            var kotresult = (from k in xdKot.Descendants("Items")
                             where k.Element("UserId").Value == oulte.ToString()
                             select k).ToList();
            foreach (var item in kotresult)
            {
                GetKotItemModel m = new GetKotItemModel();
                m.TNo = Convert.ToInt32(item.Element("TableNo").Value);
                m.ItemName = item.Element("ItemName").Value;
                m.FullQty = Convert.ToInt32(item.Element("FullQty").Value);
                m.HalfQty = Convert.ToInt32(item.Element("HalfQty").Value);
                kotList.Add(m);
            }
            var ServiceTax = (from p in _entities.tbl_ServiceTax select p.ServiceCharge).FirstOrDefault();
            if (ServiceTax == 0)
            {
                //serviccharg = Convert.ToDecimal(4.940);
                ServiceTax = Convert.ToDecimal(5.6);
            }

            model._getKotitems = kotList;
            model.getPaymentType = getPaymentMode();
            model.TableNo = TableNo;
            int tableno = Convert.ToInt32(model.TableNo);
            var billId = _entities.tblBillMasters.Where(a => a.TableNo == tableno && a.OutletId == oulte && a.Isprinted == true).FirstOrDefault();
            if (billId != null)
            {
                model.BillId = billId.BillId;
                model.IsPrinted = billId.Isprinted.Value;
                model.Discount = Math.Truncate(billId.Discount.Value * 100) / 100;
                model.DiscountAmount = Math.Truncate(billId.DiscountAmount * 100) / 100;
                model.VatAmount = Math.Truncate(billId.VatAmount * 100) / 100;
                model.TotalAmount = Math.Truncate(billId.TotalAmount * 100) / 100;
                model.ServicesCharge = Math.Truncate(billId.ServicChargesAmount * 100) / 100;
                model.ServiceTax = Math.Truncate(billId.ServiceTax.Value * 100) / 100;
                model.NetAmount = Math.Truncate(billId.NetAmount * 100) / 100;
                model.NetAmountWithoutDiscount = Math.Truncate(billId.NetAmountWithoutDiscount.Value * 100) / 100;
                model.OrderType = billId.BillingType;
                model.CustomerAddress = billId.Address;
                model.PackingCharges = Convert.ToInt32(billId.PackingCharges.Value);
                model.CustomerName = billId.CustomerName;
                model.ContactNo = billId.ContactNo;
            }
            else
            {
                model.VatAmount = Math.Truncate(result.Sum(a => Convert.ToDecimal(a.Element("VatAmountCharges").Value)) * 100) / 100;
                model.TotalAmount = Math.Truncate(result.Sum(a => Convert.ToDecimal(a.Element("Amount").Value)) * 100) / 100;
                var ServicesCharge = _entities.tblServiceCharges.FirstOrDefault();
                decimal serviceChargeAmount = 0;
                if (ServicesCharge != null)
                {
                    serviceChargeAmount = (model.TotalAmount * ServicesCharge.Charges) / 100;
                }
                decimal ServiceTaxAmount = (model.TotalAmount * ServiceTax) / 100;
                model.ServiceTax = Math.Truncate(ServiceTaxAmount * 100) / 100;
                model.ServicesCharge = Math.Truncate(serviceChargeAmount * 100) / 100;
                model.NetAmount = Math.Round(model.TotalAmount + model.VatAmount + model.ServiceTax + serviceChargeAmount, 2);
                model.NetAmountWithoutDiscount = Math.Round(model.TotalAmount + model.VatAmount + model.ServiceTax + serviceChargeAmount, 2);
            }
            // model.getAllAutoCompleteItem = getAllAutocompleteItems(oulte);
            return model;

        }
        public List<SelectListItem> getAllAutocompleteItems(int oulte)
        {
            var autocompleteItems = _entities.tblMenuOutlets.Where(a => a.OutletId == oulte).ToList();
            var lst = new List<SelectListItem>();
            foreach (var item in autocompleteItems)
            {
                lst.Add(new SelectListItem { Value = item.ItemId.ToString(), Text = item.tblItem.Name });
            }
            return lst;
        }
        public void updateBillOnDispatch(GetBillingModel model, string path)
        {
            XDocument xd = XDocument.Load(path);
            tblBillMaster tb = new tblBillMaster();
            if (model.BillId > 0)
            {
                tb = _entities.tblBillMasters.Find(model.BillId);
                if (tb != null)
                {
                    tb.Address = model.CustomerAddress;
                    tb.ChequeDate = model.CheckDate;
                    tb.ChequeNo = model.ChequeNo;
                    tb.ContactNo = model.ContactNo;
                    tb.CustomerName = model.CustomerName;
                    tb.Isprinted = false;
                    tb.PaymentType = model.PaymentType;
                    _entities.SaveChanges();
                }
            }
            int oulte = getOutletId();
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item);

            items.Remove();
            xd.Save(path);

        }
        public List<SelectListItem> getPaymentMode()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Cash", Value = "Cash" });
            list.Add(new SelectListItem { Text = "Cheque", Value = "Cheque" });
            list.Add(new SelectListItem { Text = "Card", Value = "Card" });
            list.Add(new SelectListItem { Text = "Cash&Card", Value = "Cash&Card" });
            return list;
        }
        public string FillFromXmlData(string TableNo, string Path, string KotPath)
        {
            XDocument xd = XDocument.Load(Path);
            int oulte = getOutletId();
            var result = from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo
                         select item;


            StringBuilder sb = new StringBuilder();
            int counter = 1;
            decimal TotalVatAmount = 0;
            decimal TotalAmount = 0;
            sb.Append("<table class='table'><thead><tr><th>S.No</th><th>Item Name</th><th>Full Qty</th><th>Full Price</th><th>Amount</th><th>Vat</th><th>OfferQty</th></tr></thead>");
            sb.Append("<tbody>");
            foreach (var item in result)
            {

                sb.Append("<tr>");
                sb.Append("<td>" + counter + "</td><td>" + item.Element("ItemName").Value + "</td><td>" + item.Element("FullQty").Value + "</td><td>" + item.Element("Fullprice").Value + "</td><td>" + item.Element("Amount").Value + "</td><td>" + item.Element("VatAmt").Value + "</td>");
                sb.Append("<td>" + item.Element("OfferQty").Value + " </td>");
                sb.Append("<td><a href='#' id=" + item.Element("ItemId").Value + " class='deleterow'><span class='fa fa-trash-o'><span></a></td>");
                sb.Append("</tr>");
                counter++;
                TotalVatAmount += Convert.ToDecimal(item.Element("VatAmountCharges").Value);
                TotalAmount += Convert.ToDecimal(item.Element("Amount").Value);
            }
            sb.Append("</tbody></table>");
            //==== code for kot
            XDocument xdKot = XDocument.Load(KotPath);
            StringBuilder kotsb = new StringBuilder();
            int kotcounter = 1;
            var kotresult = from k in xdKot.Descendants("Items")
                            where k.Element("UserId").Value == oulte.ToString()
                            select k;
            if (kotresult.Count() > 0)
            {
                kotsb.Append("<table class='table table-bordered'><thead><tr><th>No</th><th>TNo</th><th>Item Name</th><th>F</th><th>H</th></tr></thead>");
                kotsb.Append("<tbody>");
                foreach (var item in kotresult)
                {
                    kotsb.Append("<tr>");
                    kotsb.Append("<td>" + kotcounter + "</td><td>" + item.Element("TableNo").Value + "</td><td>" + item.Element("ItemName").Value + "</td><td>" + item.Element("FullQty").Value + "</td><td>" + item.Element("HalfQty").Value + "</td>");
                    kotsb.Append("</tr>");
                    kotcounter++;
                }
                kotsb.Append("</tbody></table>");
            }
            else
            {
                kotsb.Append("No Pending Kot Found");

            }
            return sb.ToString() + "^" + TotalVatAmount + "^" + TotalAmount + "^" + kotsb.ToString();
        }
        public string UpdateKotXmlData(string Path, string KotPath, string TableNo, string ItemName, string FullQty, string HalfQty)
        {
            int oulte = getOutletId();
            XDocument xd = XDocument.Load(KotPath);
            var items = from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo.ToString() && item.Element("ItemName").Value == ItemName
                        select item;

            if (HalfQty == "0")
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {
                        int Full = Convert.ToInt32(FullQty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("UserId", oulte.ToString());
                        itemElement.SetElementValue("TableNo", itemElement.Element("TableNo").Value);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", Full);
                        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                         new XElement("UserId", oulte.ToString()),
                         new XElement("TableNo", TableNo),
                         new XElement("ItemName", ItemName),
                         new XElement("FullQty", FullQty),
                         new XElement("HalfQty", HalfQty));
                    xd.Element("Item").Add(newElement);
                }
            }
            else
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {
                        int half = Convert.ToInt32(HalfQty) + Convert.ToInt32(itemElement.Element("HalfQty").Value);
                        itemElement.SetElementValue("UserId", oulte.ToString());
                        itemElement.SetElementValue("TableNo", itemElement.Element("TableNo").Value);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("HalfQty", half);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                         new XElement("UserId", oulte.ToString()),
                         new XElement("TableNo", TableNo),
                         new XElement("ItemName", ItemName),
                         new XElement("FullQty", FullQty),
                         new XElement("HalfQty", HalfQty));
                    xd.Element("Item").Add(newElement);
                }

            }


            xd.Save(KotPath);
            return "";
        }
        public bool DeleteNode(string Id, string TableNo, string Path, string KotFilePath, string ItemName)
        {

            int oulte = getOutletId();
            AdminCallOfferRepository offer = new AdminCallOfferRepository();
            offer.RemoveOffer(Id, Path, TableNo);
            XDocument xd = XDocument.Load(Path);
            if (Convert.ToInt32(Id) > 0)
            {
                var items = (from item in xd.Descendants("Items")
                             where item.Element("UserId").Value == oulte.ToString() && item.Element("ItemId").Value == Id && item.Element("TableNo").Value == TableNo
                             select item);
                items.Remove();
                xd.Save(Path);
                int ItemId = Convert.ToInt32(Id);
                var Name = _entities.tblItems.Where(o => o.ItemId == ItemId).Select(o => o.Name).FirstOrDefault();
                XDocument Kot = XDocument.Load(KotFilePath);
                var KotItems = (from item in Kot.Descendants("Items")
                                where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo && item.Element("ItemName").Value == Name
                                select item);
                KotItems.Remove();
                Kot.Save(KotFilePath);
            }
            else
            {
                var items = (from item in xd.Descendants("Items")
                             where item.Element("UserId").Value == oulte.ToString() && item.Element("ItemName").Value == ItemName && item.Element("TableNo").Value == TableNo
                             select item);
                items.Remove();
                xd.Save(Path);

                XDocument Kot = XDocument.Load(KotFilePath);
                var KotItems = (from item in Kot.Descendants("Items")
                                where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == TableNo && item.Element("ItemName").Value == ItemName
                                select item);
                KotItems.Remove();
                Kot.Save(KotFilePath);
            }
            return true;
            //return FillFromXmlData(TableNo, Path, KotFilePath);
        }
        public bool ClearKot(string Path, string KotPath, string Id)
        {

            int oulte = getOutletId();
            XDocument xd = XDocument.Load(KotPath);
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item);
            items.Remove();
            xd.Save(KotPath);
            return true;
            //return FillFromXmlData(Id, Path, KotPath);
        }
        public int DispatchOrder(GetBillingModel model, string Path)
        {

            try
            {
                int oulte = getOutletId();
                XDocument xd = XDocument.Load(Path);
                var result = (from item in xd.Descendants("Items")
                              where item.Element("UserId").Value == oulte.ToString()
                              select item).ToList();
                decimal totalVat = 0;
                foreach (var item in result)
                {
                    decimal VatAmt = 0;
                    if (model.Discount > 0)
                    {
                        decimal dis = Convert.ToDecimal(item.Element("VatAmountCharges").Value);
                        decimal vat = (model.Discount * dis) / 100;
                        VatAmt = dis - vat;
                    }
                    else
                    {
                        VatAmt = Convert.ToDecimal(item.Element("VatAmountCharges").Value);
                    }
                    totalVat = totalVat + VatAmt;
                }
                tblBillMaster tb = new tblBillMaster();
                tb.BillDate = DateTime.Now;
                tb.BillingType = model.OrderType;
                tb.CustomerName = model.CustomerName;
                tb.Address = model.CustomerAddress;
                tb.PackingCharges = model.PackingCharges;
                tb.ContactNo = model.ContactNo;
                tb.DiscountAmount = model.DiscountAmount;
                tb.NetAmount = model.NetAmount;
                tb.ServicChargesAmount = model.ServicesCharge;
                tb.TotalAmount = model.TotalAmount;
                tb.Isprinted = true;
                //tb.VatAmount = model.VatAmount;
                tb.VatAmount = totalVat;
                tb.ServiceTax = model.ServiceTax;
                tb.TableNo = Convert.ToInt32(model.TableNo);
                tb.PaymentType = model.PaymentType;
                tb.ChequeNo = model.ChequeNo;
                tb.ChequeDate = model.CheckDate;
                tb.OutletId = oulte;
                tb.Discount = model.Discount;
                tb.NetAmountWithoutDiscount = model.NetAmountWithoutDiscount;
                tb.ContactNo = model.ContactNo;
                _entities.tblBillMasters.Add(tb);
                _entities.SaveChanges();


                foreach (var item in result)
                {
                    int ItemId = Convert.ToInt32(item.Element("ItemId").Value);
                    if (ItemId == 0)
                    {
                        //OpenFood food = new OpenFood();
                        //food.Date = DateTime.Now.Date;
                        //food.ItemName = item.Element("ItemName").Value;
                        //food.OutletId = oulte;
                        //food.Price = Convert.ToDecimal(item.Element("Fullprice").Value);
                        //food.Quantity = Convert.ToInt32(item.Element("FullQty").Value);
                        //food.Amount = Convert.ToDecimal(item.Element("Amount").Value);
                        //food.Vat = Convert.ToInt32(item.Element("VatAmt").Value);
                        //_entities.OpenFoods.Add(food);
                        //_entities.SaveChanges();
                    }
                    else
                    {
                        tblBillDetail bill = new tblBillDetail();
                        bill.Amount = Convert.ToDecimal(item.Element("Amount").Value);
                        bill.FullQty = Convert.ToInt32(item.Element("FullQty").Value);
                        bill.HalfQty = Convert.ToInt32(item.Element("HalfQty").Value);
                        bill.ItemId = Convert.ToInt32(item.Element("ItemId").Value);
                        bill.Vat = Convert.ToDecimal(item.Element("VatAmt").Value);
                        decimal VatAmt = 0;
                        if (model.Discount > 0)
                        {
                            decimal dis = Convert.ToDecimal(item.Element("VatAmountCharges").Value);
                            decimal vat = (model.Discount * dis) / 100;
                            VatAmt = dis - vat;
                        }
                        else
                        {
                            VatAmt = Convert.ToDecimal(item.Element("VatAmountCharges").Value);
                        }
                        bill.VatAmount = Math.Truncate(VatAmt * 100) / 100;
                        bill.BillId = tb.BillId;
                        _entities.tblBillDetails.Add(bill);
                        _entities.SaveChanges();
                        // call auto inventory start..
                        AutoInventoryRepository auto = new AutoInventoryRepository();
                        auto.AutoInventory(Convert.ToInt32(item.Element("ItemId").Value), Convert.ToInt32(item.Element("FullQty").Value), Convert.ToInt32(item.Element("HalfQty").Value), oulte);

                    }

                }

                //var items = (from item in xd.Descendants("Items")
                //             where item.Element("UserId").Value == oulte.ToString()
                //             select item);

                //items.Remove();
                //xd.Save(Path);
                CheckStockItemRepository check = new CheckStockItemRepository();
                // string chk = check.OutStockItems(Id);
                return tb.BillId;
            }
            catch (Exception ex)
            {
                //Error error = new Error();
                //error.ErrorMessage = ex.Message;
                //_entities.Errors.Add(error);
                //_entities.SaveChanges();
                return 0;
            }
        }
        public string PrintData(string Id, string SplitPath)
        {
            var id = Convert.ToInt32(Id);
            var result = _entities.tblBillMasters.Where(o => o.BillId == id).FirstOrDefault();
            var SecondaryResult = _entities.tblBillDetails.Where(x => x.BillId == id).ToList();
            int counter = 1;
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='margin-left:100px;width:350px;margin-top:100px; height:auto;'>");
            sb.Append("<div class='logo' style='border-bottom:1px dashed'>");
            sb.Append("<h2 style='margin-left:125px; padding-top:20px;'>Nibs Cafe</h2>");
            sb.Append("<div>");
            sb.Append("<b style='margin-left:85px; font-weight:100'>Near Friends Colony,Lalkothi</b><br />");
            sb.Append("<b style='margin-left:85px; font-weight:100'>Jaipur-302029</b><br />");
            sb.Append("<b style='margin-left:85px; font-weight:100'>PH:9680625173</b><br />");
            sb.Append("<b style='margin-left:125px;'>Sales Invoice</b></div></div>");
            sb.Append("<div style='width: 350px; float:left; height: 35px; border-bottom: 1px dashed'>");
            sb.Append("<div style='width:250px;height:35px;float:left; padding-top:9px;'>Name:<b>" + result.CustomerName + "</b></div>");
            sb.Append("<div style='width:100px;height:35px;float:left;padding-top:9px;'>Table No:<b>" + result.TableNo + "</b></div></div>");
            sb.Append("<div style='width:350px;height:auto; float:left;'>");
            sb.Append("<table style='width:350px;'>");
            sb.Append(" <tr><th>Sr</th><th>Item</th><th>Full</th><th>Half</th><th>Amount</th></tr><tbody>");
            foreach (var item in SecondaryResult)
            {
                var Name = _entities.tblItems.Where(o => o.ItemId.Equals(item.ItemId)).Select(x => x.Name).SingleOrDefault();
                sb.Append("<tr><td>" + counter + "</td> <td>" + Name + "</td><td>" + item.FullQty + "</td><td>" + item.HalfQty + "</td><td>" + item.Amount + "</td></tr>");
            }
            sb.Append("</tbody></table></div>");
            sb.Append("<div style='width: 350px; border-top: 1px dashed; float:left;'>");
            sb.Append("<div style='width:280px;float:left; height:100px; margin-top:20px;line-height:20px;'>");
            sb.Append("<b>Vat Amount</b><br /><b>Servic Amount</b><br /><b>Total Amount</b><br /> <b>Discount Amount</b><br /><b>Net Amount</b><br /></div>");
            sb.Append("<div style='width: 70px; margin-top: 20px; line-height: 20px; float: left; height: 100px;'>");
            sb.Append(result.VatAmount + "<br/>" + result.ServicChargesAmount + "<br />" + result.TotalAmount + "<br /><b>" + result.DiscountAmount + "</b><br /><b>" + result.NetAmount + "</b>");
            sb.Append("</div></div><div style='width:350px;text-align:center; height:20px; margin-top:15px;border-top:1px dashed; float:left'>" + result.BillDate + "</div></div>");
            //=====code for split detail======//

            int oulte = getOutletId();
            XDocument xd = XDocument.Load(SplitPath);
            var split = from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == result.TableNo.ToString()
                        select item;
            var spType = (from p in xd.Descendants("Items")
                          where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == result.TableNo.ToString()
                          select p.Element("Type").Value).FirstOrDefault();
            var spNoOfPersion = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == result.TableNo.ToString()
                                 select p.Element("NoOfPersion").Value).FirstOrDefault();
            var splitcount = split.Count();
            if (splitcount > 0)
            {
                sb.Append("<div style='width: 350px; float:left; height: 35px; border-bottom: 1px dashed;margin-left:100px;'>");
                sb.Append("<div style='width:190px;height:35px;float:left; padding-top:9px;'>Split Type:<b>" + spType + "</b></div>");
                sb.Append("<div style='width:138px;height:35px;float:left;padding-top:9px;'>No Of Person:<b>" + spNoOfPersion + "</b>");
                sb.Append("</div></div><br/>");
                sb.Append(" <div style='width:350px;height:auto; float:left;;margin-left:100px;'><table style='width:350px;'><thead><tr><th>Name</th><th>Amount</th></tr></thead><tbody>");
                //sb.Append("<table  style='width:350px;'><tbody>");
                foreach (var item in split)
                {
                    sb.Append("<tr><td style='text-align: center;'>" + item.Element("Name").Value + "</td><td style='text-align: center;'>" + item.Element("Amount").Value + "</td></tr>");
                }
                sb.Append("</tbody></table></div>");
                //sb.Append("</div></div>");
            }
            split.Remove();
            xd.Save(SplitPath);




            //<tr><td>1</td> <td>Hot Pizza</td><td>2</td><td>0</td><td>240.0000</td></tr><tr><td>1</td> <td>Pizza 1</td><td>1</td><td>0</td><td>130.0000</td></tr>
            return sb.ToString();
        }


        //============

        public string PrintOrderData(OrderDispatchModel model, string Path)
        {
            int oulte = getOutletId();
            var outletData = (from p in _entities.tblOutlets where p.OutletId == oulte select p).SingleOrDefault();
            XDocument xd = XDocument.Load(Path);
            var result = from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString() && item.Element("TableNo").Value == model.TableNo.ToString()
                         select item;
            int counter = 1;
            StringBuilder sb = new StringBuilder();
            var VatDetail = (from p in xd.Descendants("Items")
                             where p.Element("UserId").Value == oulte.ToString()
                             && p.Element("TableNo").Value == model.TableNo.ToString()
                             group p by p.Element("VatAmt").Value into g
                             select new
                             {
                                 Vat = g.Key,
                                 amtCharges = g.Sum(a => Convert.ToDecimal(a.Element("VatAmountCharges").Value))// xd.Descendants("Items").Sum(a => Convert.ToDecimal(a.Element("VatAmountCharges").Value))
                             }).ToList();
            sb.Append("<div style='width:300px;height:auto;'>");
            sb.Append("<div class='logo' style='border-bottom:1px dashed'>");
            sb.Append("<b style='margin-left:90px;font-size:23px;'>Nibs Cafe</b></br>");
            sb.Append("<strong style='margin-left:50px; font-size:17px;'>A Unit of KGC Enterprises</strong></br>");
            sb.Append("<strong style='margin-left:50px; font-size:17px;'>TIN No:" + outletData.TinNo + "</strong></br>");
            sb.Append("<strong style='margin-left:50px; font-size:17px;'>ServiceTax No:" + outletData.ServiceTaxNo + "</strong></br>");
            sb.Append("<div>");
            sb.Append("<b style='margin-left:50px; font-weight:100;font-size: 19px;'>" + outletData.Address + "</b><br />");
            sb.Append("<b style='margin-left:50px; font-weight:100;font-size: 19px;'>Jaipur-302029</b><b style='margin-left:10px; font-weight:100'>PH:" + outletData.ContactA + "</b><br />");
            //sb.Append("<b style='margin-left:50px; font-weight:100'>PH:9680625173</b><br />");
            sb.Append("<b style='margin-left:90px;font-size: 19px;'>Sales Invoice</b></div></div>");
            sb.Append("<div style='width: 300px; float:left; height: 35px; border-bottom: 1px dashed;font-size: 18px;'>");
            sb.Append("<div style='width:200px;height:35px;float:left; padding-top:9px;'>Name:<b>" + model.CustomerName + "</b></div>");
            sb.Append("<div style='width:100px;height:35px;float:left;padding-top:9px;'>Table No:<b>" + model.TableNo + "</b></div></div>");
            sb.Append("<div style='width:350px;height:auto; float:left;'>");
            sb.Append("<table style='width:350px;font-size: 19px;'>");
            sb.Append(" <tr><th style='text-align:left'>Sr</th><th  style='text-align:left'>Item</th><th  style='text-align:left'>F</th><th  style='text-align:left'>H</th><th  style='text-align:left'>Amt</th></tr><tbody>");
            foreach (var item in result)
            {
                int Itemid = Convert.ToInt32(item.Element("ItemId").Value);
                if (Itemid == 0)
                {
                    var amount = item.Element("Amount").Value;
                    decimal amt = Convert.ToDecimal(amount);
                    sb.Append("<tr><td>" + counter + "</td> <td>" + item.Element("ItemName").Value + "</td><td>" + item.Element("FullQty") + "</td><td>" + item.Element("HalfQty") + "</td><td>" + Math.Round(amt, 2) + "</td></tr>");
                }
                else
                {
                    var Name = _entities.tblItems.Where(o => o.ItemId == Itemid).Select(x => x.Name).SingleOrDefault();
                    var amount = item.Element("Amount").Value;
                    decimal amt = Convert.ToDecimal(amount);
                    sb.Append("<tr><td>" + counter + "</td> <td>" + Name + "</td><td>" + item.Element("FullQty") + "</td><td>" + item.Element("HalfQty") + "</td><td>" + Math.Round(amt, 2) + "</td></tr>");

                }
            }
            sb.Append("</tbody></table></div>");
            sb.Append("<div style='width: 300px; border-top: 1px dashed; float:left;'>");
            sb.Append("<div style='width:226px;float:left; height:auto; margin-top:20px;line-height:20px;font-size: 18px;'>");
            foreach (var item in VatDetail)
            {
                sb.Append("<b>Vat Amount(" + item.Vat + "%)</b><br />");
            }
            //sb.Append("<b>Service Tax(4.94)</b><br /><b>Total Amount</b><br /> <b>Discount Amount</b><br /><b>Net Amount</b><br /></div>");
            sb.Append("<b>Service Tax(5.6)</b><br /><b>Total Amount</b><br /> <b>Discount Amount</b><br /><b>Net Amount</b><br /></div>");
            sb.Append("<div style='width: 70px; margin-top: 20px; line-height: 20px; float: left; height: auto;font-size: 20px;'>");
            foreach (var item in VatDetail)
            {
                sb.Append(Math.Round(item.amtCharges, 2) + "<br/>");
            }
            sb.Append(model.ServiceCharge + "<br />" + model.TotalAmount + "<br /><b>" + model.DiscountAmount + "</b><br /><b>" + model.NetAmount + "</b>");
            sb.Append("</div></div><div style='width:300px;text-align:center; height:20px; margin-top:15px;border-top:1px dashed; float:left'>" + DateTime.Now + "</div></div>");
            //<tr><td>1</td> <td>Hot Pizza</td><td>2</td><td>0</td><td>240.0000</td></tr><tr><td>1</td> <td>Pizza 1</td><td>1</td><td>0</td><td>130.0000</td></tr>
            return sb.ToString();
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
                    oulte = Convert.ToInt32((from n in _entities.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return 99;
        }
    }
}
