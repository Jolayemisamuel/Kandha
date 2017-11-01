using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using WebMatrix.WebData;
using System.Text;
using System.Xml.Linq;
using System.Web.Security;
namespace NibsMVC.Repository
{
    public class HomeDeliveryRepository
    {
        NIBSEntities _entities = new NIBSEntities();
        public List<HomeDeliveryModel> GetAllCategoryList()
        {
            int oulte = getOutletId();
            var category = (from q in _entities.tblCategories where (from p in _entities.tblMenuOutlets where p.OutletId == oulte select p.CategoryId).Contains(q.CategoryId) select q).ToList();
            List<HomeDeliveryModel> List = new List<HomeDeliveryModel>();
            foreach (var item in category)
            {
                HomeDeliveryModel model = new HomeDeliveryModel();
                model.CategoryId = item.CategoryId;
                model.CategoryName = item.Name;
                model.Color = item.Color;
                model.TextColor = item.TextColor;
                List.Add(model);

            }
            return List;
        }
        public string GetAllItem(string Id)
        {
            var id = Convert.ToInt32(Id);
            int oulte = getOutletId();
            var result = (from p in _entities.tblItems
                          join q in _entities.tblMenuOutlets
                              on p.ItemId equals q.ItemId
                          where p.Active == true &&
                          q.OutletId == oulte &&
                          q.CategoryId == id
                          select new
                          {
                              ItemId = q.ItemId,
                              Name = q.tblItem.Name,
                              Color = p.tblCategory.Color,
                              TextColor = p.tblCategory.TextColor
                          }).ToList();
            StringBuilder sb = new StringBuilder();
            foreach (var item in result)
            {
                //sb.Append("<input type='button' class='btn btn-blue margin-right-5' name='itemName' value='" + item.Name + "' id='" + item.ItemId + "'style='background:" + item.Color + ";color:" + item.TextColor + "'>");
                sb.Append("<input type='button' class='btn btn-blue margin-right-5' name='itemName' value='" + item.Name + "' id='" + item.ItemId + "'style='background:" + item.Color + ";color:" + item.TextColor + "'>");
            }
            return sb.ToString();
        }
        
        public string GetXmlData(string Id, string Path, string Qty, string Type, string KotFilePath, string TokenNo)
        {
            int oulte = getOutletId();
            var itemid = Convert.ToInt32(Id);
            var result = _entities.tblBasePriceItems.Where(o => o.ItemId == itemid).FirstOrDefault();
            XDocument xd = XDocument.Load(Path);
            var items = from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString() && item.Element("ItemName").Value == result.tblItem.Name
                        select item;
            if (Type == "Half")
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {
                        int Half = Convert.ToInt32(Qty) + Convert.ToInt32(itemElement.Element("HalfQty").Value);
                        decimal TotalAmount = Convert.ToDecimal(itemElement.Element("TotalAmount").Value) + Convert.ToDecimal(result.HalfPrice);
                        var vatamtchrg = (TotalAmount * Convert.ToDecimal(Convert.ToDecimal(result.Vat)) / 100);
                        itemElement.SetElementValue("UserId", oulte);
                        itemElement.SetElementValue("ItemId", result.ItemId);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("HalfQty", Half);
                        itemElement.SetElementValue("HalfPrice", result.HalfPrice);
                        itemElement.SetElementValue("FullPrice", result.FullPrice);
                        itemElement.SetElementValue("Vat", result.Vat);
                        itemElement.SetElementValue("TotalAmount", TotalAmount);
                        itemElement.SetElementValue("VatAmt", vatamtchrg);
                        itemElement.SetElementValue("OfferQty", "0");
                        xd.Save(Path);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                         new XElement("UserId", oulte),
                          new XElement("ItemId", result.ItemId),
                         new XElement("ItemName", result.tblItem.Name),
                         new XElement("FullQty", "0"),
                         new XElement("HalfQty", Qty),
                          new XElement("HalfPrice", result.HalfPrice),
                           new XElement("FullPrice", result.FullPrice),
                            new XElement("Vat", result.Vat),
                            new XElement("TotalAmount", result.HalfPrice*(Convert.ToDecimal(Qty))),
                              new XElement("VatAmt", Convert.ToDecimal(result.HalfPrice) * Convert.ToDecimal(result.Vat) / 100),
                              new XElement("OfferQty", "0"));
                    xd.Element("Item").Add(newElement);
                    xd.Save(Path);
                }
                UpdateKotXmlData(Path, KotFilePath, TokenNo, result.tblItem.Name, "0", Qty);
            }
            else
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {

                        int Full = Convert.ToInt32(Qty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                        decimal TotalAmount = Convert.ToDecimal(itemElement.Element("TotalAmount").Value) + Convert.ToDecimal(result.FullPrice);
                        decimal newvatamount = ((TotalAmount * Convert.ToDecimal(result.Vat)) / 100);
                        var vatamtchrg = (TotalAmount * (Convert.ToDecimal(itemElement.Element("Vat").Value)) / 100);
                        itemElement.SetElementValue("UserId", oulte);
                        itemElement.SetElementValue("ItemId", result.ItemId);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", Full);
                        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                        itemElement.SetElementValue("HalfPrice", result.HalfPrice);
                        itemElement.SetElementValue("FullPrice", result.FullPrice);
                        itemElement.SetElementValue("Vat", result.Vat);
                        itemElement.SetElementValue("TotalAmount", TotalAmount);
                        itemElement.SetElementValue("VatAmt", vatamtchrg);
                        itemElement.SetElementValue("OfferQty", "0");
                        xd.Save(Path);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                         new XElement("UserId", oulte),
                          new XElement("ItemId", result.ItemId),
                         new XElement("ItemName", result.tblItem.Name),
                         new XElement("FullQty", Qty),
                         new XElement("HalfQty", "0"),
                          new XElement("HalfPrice", result.HalfPrice),
                           new XElement("FullPrice", result.FullPrice),
                            new XElement("Vat", result.Vat),
                             new XElement("TotalAmount", result.FullPrice * (Convert.ToDecimal(Qty))),
                              new XElement("VatAmt", Convert.ToDecimal(result.FullPrice) * Convert.ToDecimal(result.Vat) / 100),
                              new XElement("OfferQty", "0"));

                    xd.Element("Item").Add(newElement);
                    xd.Save(Path);
                }
                UpdateKotXmlData(Path, KotFilePath, TokenNo, result.tblItem.Name, Qty, "0");
                if (Type == "Full")
                {
                    TakeAwayhappHourOfferRepository hh = new TakeAwayhappHourOfferRepository();
                    bool Happy = hh.CallHappyHoursDaysOffer(Id, TokenNo, Path, KotFilePath,result.tblItem.Name);
                    if (Happy == true)
                    {
                        CallTakeAwayOfferRepository offer = new CallTakeAwayOfferRepository();
                        bool Data = offer.CallOffer(Id, Path, TokenNo, Qty,KotFilePath,result.tblItem.Name);
                        if (Data == true)
                        {
                            bool Combo = offer.CallComboOffer(Id, TokenNo, Path, KotFilePath);
                            if (Combo == true)
                            {
                                offer.CallAmountBasisOffer(Id, TokenNo, Path, KotFilePath, Qty);
                            }
                        }
                    }
                }
            }
            return FillXmlData(Path, KotFilePath, TokenNo);
        }
        public string FillXmlData(string Path, string KotFilePath, string TokenNo)
        {
            XDocument xd = XDocument.Load(Path);
            decimal TotalVatAmount = 0;
            decimal TotalAmount = 0;
            int oulte = getOutletId();
            var result = from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item;

            StringBuilder sb = new StringBuilder();
            int counter = 1;
            if (result.Count() > 0)
            {
                sb.Append("<table class='table'><thead><tr><th>S.No</th><th>ItemName</th><th>Full Qty</th><th>Half Qty</th><th>Full Price</th><th>Half Price</th><th>Amount</th><th>Vat</th><th>OfferQty</th></tr></thead>");
                sb.Append("<tbody>");
                foreach (var item in result)
                {

                    sb.Append("<tr>");
                    sb.Append("<td>" + counter + "</td>");
                    sb.Append("<td>" + item.Element("ItemName").Value + "</td>");
                    sb.Append("<td>" + item.Element("FullQty").Value + "</td>");
                    sb.Append("<td>" + item.Element("HalfQty").Value + "</td>");
                    sb.Append("<td>" + item.Element("FullPrice").Value + "</td>");
                    sb.Append("<td>" + item.Element("HalfPrice").Value + "</td>");
                    sb.Append("<td>" + item.Element("TotalAmount").Value + "</td>");
                    sb.Append("<td>" + item.Element("Vat").Value + "</td>");
                    sb.Append("<td>" + item.Element("OfferQty").Value + "</td>");
                    sb.Append("<td><a href='#' id=" + item.Element("ItemId").Value + " class='deleterow'><span class='fa fa-trash-o'><span></a></td>");
                    sb.Append("</tr>");
                    counter++;
                    TotalVatAmount += Convert.ToDecimal(item.Element("VatAmt").Value);
                    TotalAmount += Convert.ToDecimal(item.Element("TotalAmount").Value);
                }
                sb.Append("</tbody></table>");
            }
            XDocument xdKot = XDocument.Load(KotFilePath);
            StringBuilder kotsb = new StringBuilder();
            int kotcounter = 1;
            var kotresult = from k in xdKot.Descendants("Items")
                            where k.Element("UserId").Value == oulte.ToString()
                            select k;
            if (kotresult.Count() > 0)
            {
                kotsb.Append("<table class='table table-bordered'><thead><tr><th>S.No</th><th>TokenNo</th><th>Item Name</th><th>full Qty</th><th>Half Qty</th></tr></thead>");
                kotsb.Append("<tbody>");
                foreach (var item in kotresult)
                {
                    kotsb.Append("<tr>");
                    kotsb.Append("<td>" + kotcounter + "</td><td>" + item.Element("TokenNo").Value + "</td><td>" + item.Element("ItemName").Value + "</td><td>" + item.Element("FullQty").Value + "</td><td>" + item.Element("HalfQty").Value + "</td>");
                    kotsb.Append("</tr>");
                    kotcounter++;
                }
                kotsb.Append("</tbody></table>");
            }
            else
            {
                kotsb.Append("No Pending Kot Found");

            }
            return sb.ToString() + "^" + TotalAmount + "^" + TotalVatAmount + "^" + kotsb.ToString();
        }
        public string DeleteNode(string Id, string Path,string KotFilePath,string TokenNo)
        {
            XDocument xd = XDocument.Load(Path);
            int oulte = getOutletId();
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString() && item.Element("ItemId").Value == Id
                         select item);
            items.Remove();
            xd.Save(Path);
            int ItemId = Convert.ToInt32(Id);
            var Name = _entities.tblItems.Where(o => o.ItemId == ItemId).Select(o => o.Name).FirstOrDefault();
            XDocument Kot = XDocument.Load(KotFilePath);
            var KotItems = (from item in Kot.Descendants("Items")
                            where item.Element("UserId").Value == oulte.ToString() && item.Element("TokenNo").Value == TokenNo && item.Element("ItemName").Value == Name
                            select item);
            KotItems.Remove();
            Kot.Save(KotFilePath);
            return FillXmlData(Path,KotFilePath,TokenNo);
        }
        public string UpdateKotXmlData(string Path, string KotPath, string TokenNo, string ItemName, string FullQty, string HalfQty)
        {
            int oulte = getOutletId();
            XDocument xd = XDocument.Load(KotPath);
            var items = from item in xd.Descendants("Items")
                        where item.Element("TokenNo").Value == TokenNo.ToString() && item.Element("ItemName").Value == ItemName && item.Element("UserId").Value == oulte.ToString()
                        select item;
            if (HalfQty == "0")
            {
                if (items.Count() > 0)
                {
                    foreach (XElement itemElement in items)
                    {
                        int Full = Convert.ToInt32(FullQty) + Convert.ToInt32(itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                        itemElement.SetElementValue("TokenNo", itemElement.Element("TokenNo").Value);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", Full);
                        itemElement.SetElementValue("HalfQty", itemElement.Element("HalfQty").Value);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                        new XElement("UserId", oulte),
                         new XElement("TokenNo", TokenNo),
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
                        itemElement.SetElementValue("UserId", itemElement.Element("UserId").Value);
                        itemElement.SetElementValue("TokenNo", itemElement.Element("TokenNo").Value);
                        itemElement.SetElementValue("ItemName", itemElement.Element("ItemName").Value);
                        itemElement.SetElementValue("FullQty", itemElement.Element("FullQty").Value);
                        itemElement.SetElementValue("HalfQty", half);
                    }
                }
                else
                {
                    var newElement = new XElement("Items",
                        new XElement("UserId", oulte),
                         new XElement("TokenNo", TokenNo),
                         new XElement("ItemName", ItemName),
                         new XElement("FullQty", FullQty),
                         new XElement("HalfQty", HalfQty));
                    xd.Element("Item").Add(newElement);
                }

            }


            xd.Save(KotPath);
            return FillXmlData(Path, KotPath, TokenNo);
        }
        public string ClearKot(string Path, string KotPath, string TokenNo)
        {
            int oulte = getOutletId();
            XDocument xd = XDocument.Load(KotPath);
            var items = from item in xd.Descendants("Items")
                        where item.Element("UserId").Value == oulte.ToString()
                        select item;
            items.Remove();
            xd.Save(KotPath);
            return FillXmlData(Path, KotPath,TokenNo);
        }
        public string DispatchOrder(OrderDispatchHomeModel model, string Path)
        {
            try
            {
                int oulte = getOutletId();
                tblBillMaster tb = new tblBillMaster();
                tb.BillDate = DateTime.Now.Date;
                tb.BillingType = "H";
                tb.CustomerName = model.CustomerName;
                tb.DiscountAmount = model.DiscountAmount;
                tb.NetAmount = model.NetAmount;
                tb.ServicChargesAmount = model.ServiceCharge;
                tb.TotalAmount = model.TotalAmount;
                tb.VatAmount = model.VatAmount;
                tb.Address = model.Address;
                tb.OutletId = oulte;
                tb.TokenNo = model.TokenNo;
                _entities.tblBillMasters.Add(tb);
                _entities.SaveChanges();
                var Id = _entities.tblBillMasters.Where(o => o.OutletId == oulte && o.BillingType == "H").Select(x => x.BillId).Max();
                XDocument xd = XDocument.Load(Path);
               
                var result = from item in xd.Descendants("Items")
                            where item.Element("UserId").Value == oulte.ToString()
                            select item;
                foreach (var item in result)
                {
                    tblBillDetail bill = new tblBillDetail();
                    bill.Amount = Convert.ToDecimal(item.Element("TotalAmount").Value);
                    bill.FullQty = Convert.ToInt32(item.Element("FullQty").Value);
                    bill.HalfQty = Convert.ToInt32(item.Element("HalfQty").Value);
                    bill.ItemId = Convert.ToInt32(item.Element("ItemId").Value);
                    bill.BillId = Id;
                    _entities.tblBillDetails.Add(bill);
                    _entities.SaveChanges();
                }
                var items = from item in xd.Descendants("Items")
                            where item.Element("UserId").Value == oulte.ToString()
                            select item;
                items.Remove();
                xd.Save(Path);
                return PrintData(Id.ToString(),model.TokenNo.ToString());
            }
            catch
            {
                return "0";
            }
        }
        public string PrintData(string Id,string TokenNo)
        {
            var id = Convert.ToInt32(Id);
            var result = _entities.tblBillMasters.Where(o => o.BillId == id).FirstOrDefault();
            var SecondaryResult = _entities.tblBillDetails.Where(x => x.BillId == id).ToList();
            int counter = 1;
            int oulte = getOutletId();
            var address = (from p in _entities.tblOutlets where p.OutletId == oulte select new { Address = p.Address, ContactA = p.ContactA }).SingleOrDefault();
            StringBuilder sb = new StringBuilder();
            var VatDetail = (from p in _entities.tblBillMasters
                             where p.OutletId == oulte
                             && p.TokenNo == result.TokenNo
                             group p by p.VatAmount into g
                             select new
                             {
                                 Vat = g.Key,
                                 amtCharges = g.Sum(a => ((a.TotalAmount * a.VatAmount) / 100))
                             }).ToList();
            sb.Append("<div style='width:300px;height:auto;'>");
            sb.Append("<div class='logo' style='border-bottom:1px dashed'>");
            sb.Append("<b style='margin-left:90px;font-size:20px;'>Nibs Cafe</b></br>");
            sb.Append("<strong style='margin-left:50px; font-size:15px;'>A Unit of KGC Enterprises</strong></br>");
            sb.Append("<div>");
            sb.Append("<b style='margin-left:50px; font-weight:100'>" + address.Address + "</b><br />");
            sb.Append("<b style='margin-left:90px;'>Sales Invoice</b></div></div>");
            sb.Append("<div style='width: 300px; float:left; height: 35px; border-bottom: 1px dashed'>");
            sb.Append("<div style='width:200px;height:35px;float:left; padding-top:9px;'>Name:<b>" + result.CustomerName + "</b></div>");
            sb.Append("<div style='width:100px;height:35px;float:left;padding-top:9px;'>Token No:<b>" + result.TokenNo + "</b></div></div>");
            sb.Append("<div style='width:350px;height:auto; float:left;'>");
            sb.Append("<table style='width:350px;'>");
            sb.Append(" <tr><th style='text-align:left'>Sr</th><th  style='text-align:left'>Item</th><th  style='text-align:left'>F</th><th  style='text-align:left'>H</th><th  style='text-align:left'>Amt</th></tr><tbody>");
            foreach (var item in SecondaryResult)
            {
                int Itemid = item.ItemId.Value;
                var Name = _entities.tblItems.Where(o => o.ItemId == Itemid).Select(x => x.Name).SingleOrDefault();
                var amount = item.Amount;
                decimal amt = Convert.ToDecimal(amount);
                sb.Append("<tr><td>" + counter + "</td> <td>" + Name + "</td><td>" + item.FullQty + "</td><td>" + item.HalfQty + "</td><td>" + Math.Round(amt, 2) + "</td></tr>");
            }
            sb.Append("</tbody></table></div>");
            sb.Append("<div style='width: 300px; border-top: 1px dashed; float:left;'>");
            sb.Append("<div style='width:226px;float:left; height:auto; margin-top:20px;line-height:20px;'>");
            foreach (var item in VatDetail)
            {
                sb.Append("<b>Vat Amount(" + Math.Round(item.Vat,2) + "%)</b><br />");
            }
            sb.Append("<b>Service Amount</b><br /><b>Total Amount</b><br /> <b>Discount Amount</b><br /><b>Net Amount</b><br /></div>");
            sb.Append("<div style='width: 70px; margin-top: 20px; line-height: 20px; float: left; height: auto;'>");
            foreach (var item in VatDetail)
            {
                sb.Append(Math.Round(item.amtCharges, 2) + "<br/>");
            }
            sb.Append(result.ServicChargesAmount + "<br />" + result.TotalAmount + "<br /><b>" + result.DiscountAmount + "</b><br /><b>" + result.NetAmount + "</b>");
            sb.Append("</div></div><div style='width:300px;text-align:center; height:20px; margin-top:15px;border-top:1px dashed; float:left'>" + DateTime.Now + "</div></div>");
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
            return oulte;
        }
    }
}