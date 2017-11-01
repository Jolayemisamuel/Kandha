using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NibsMVC.Models;
using NibsMVC.EDMX;
using System.Data.Entity;
using System.Web.Security;
using WebMatrix.WebData;
using System.Web.Mvc;
namespace NibsMVC.Repository
{
    public class OutletVendorRepository
    {
        NIBSEntities entities = new NIBSEntities();
        CheckStockItemRepository stock = new CheckStockItemRepository();

        #region Vendor Account
        public OutletVendor getOutLetVendor(int Id)
        {
            OutletVendor model = entities.OutletVendors.Find(Id);
            if (model == null)
            {
                model = new OutletVendor();
                model.OutletId = getOutletId();
            }
            return model;
        }
        public void saveVendor(OutletVendor model)
        {
            if (model.OutletVenderId > 0)
            {
                entities.Entry(model).State = EntityState.Modified;
                entities.SaveChanges();
            }
            else
            {
                entities.Entry(model).State = EntityState.Added;
                entities.SaveChanges();
            }
        }
        public List<OutletVendor> getAllVendor()
        {
            int OutletId = getOutletId();
            return entities.OutletVendors.Where(a => a.OutletId == OutletId).ToList();
        }
        public void DeleteVendor(int Id)
        {
            OutletVendor model = entities.OutletVendors.Find(Id);
            if (model != null)
            {
                
                    if (model.IsActive)
                    {
                        model.IsActive = false;
                    }
                    else
                    {
                        model.IsActive = true;
                    }
                entities.Entry(model).State = EntityState.Modified;
                entities.SaveChanges();
            }

        }
        #endregion

        #region Vendor Items
        public OutletVendorModel getVendorsForPriceUpdate()
        {
            OutletVendorModel model = new OutletVendorModel();
            model.getAllCategory = getAllCategory();
            model.getAllVendor = getAllVendorList();

            return model;
        }
        public List<SelectListItem> getAllVendorList()
        {
            var lst = new List<SelectListItem>();
            int OutletId = getOutletId();
            foreach (var item in entities.OutletVendors.Where(a => a.OutletId == OutletId&&a.IsActive==true).ToList())
            {
                lst.Add(new SelectListItem { Value = item.OutletVenderId.ToString(), Text = item.VenderName });
            }
            return lst;
        }

        public List<SelectListItem> getAllCategory()
        {
            var lst = new List<SelectListItem>();
            int OutletId = getOutletId();
            var categories = entities.tblMenuOutlets.Where(a => a.OutletId == OutletId).Select(a => a.CategoryId).ToList();
            var data = entities.tblCategories.Where(a => categories.Contains(a.CategoryId)).ToList();
            foreach (var item in data)
            {
                lst.Add(new SelectListItem { Value = item.CategoryId.ToString(), Text = item.Name });
            }
            return lst;
        }
        public bool checkBillingIsRunning(int VendorId)
        {
            bool status=false;
            status = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId).Any();
            return status;
        }
        public List<OutletVendorItemModel> getAllItems(int CategoryId, int VendorId)
        {
            var lst = new List<OutletVendorItemModel>();
            int OutletId = getOutletId();
            var data = entities.tblMenuOutlets.Where(a => a.OutletId == OutletId && a.CategoryId == CategoryId).ToList();

            foreach (var item in data)
            {
                OutletVendorItemModel model = new OutletVendorItemModel();
                var price = entities.VendorPrices.Where(a => a.MenuOutletId == item.MenuOutletId&& a.VendorId==VendorId).Select(a => a.Price).FirstOrDefault();
                if (price != 0)
                {
                    model.Baseprice = price;
                }
                else
                {
                    model.Baseprice = item.FullPrice;
                }
                model.ActualPrice = item.FullPrice;
                model.ItemName = item.tblItem.Name;
                model.MenuOutletId = item.MenuOutletId;
                lst.Add(model);
            }
            return lst;
        }
        public void UpdatePrice(OutletVendorModel model)
        {
            int OutletId = getOutletId();
            for (int i = 0; i < model.MenuOutletId.Length; i++)
            {
                VendorPrice tbdelete = new VendorPrice();
                int menuId = model.MenuOutletId[i];
                tbdelete = entities.VendorPrices.Where(a => a.MenuOutletId == menuId && a.VendorId==model.VendorId).FirstOrDefault();
                if (tbdelete != null)
                {
                    entities.Entry(tbdelete).State = EntityState.Deleted;
                    entities.SaveChanges();
                }
                VendorPrice tb = new VendorPrice();
                tb.MenuOutletId = model.MenuOutletId[i];
                tb.Price = model.Price[i];
                tb.VendorId = model.VendorId;
                entities.VendorPrices.Add(tb);
                entities.SaveChanges();
            }
        }
        #endregion
        #region
        public AddVendorPayment getVenderPayment(int VendorId)
        {
            AddVendorPayment model = new AddVendorPayment();
            model.getAllVendors = getAllVendorList();
            if (VendorId > 0)
            {
                int OutletId = getOutletId();
                var vendorAmount = entities.SelesVendorAmountDetails.Where(a => a.VendorId == VendorId).ToList();
                if (vendorAmount.Count>0)
                {

                    var vendor = entities.SelesVendorAmounts.Where(a => a.VendorId == VendorId).FirstOrDefault();
                    decimal DepositAmount = 0;
                    if (vendor!=null)
                    {
                        DepositAmount = vendor.Amount;
                    }
                    decimal TotalAmount = vendorAmount.Sum(a=>a.Amout);
                    if (DepositAmount > TotalAmount)
                    {
                        model.Ispending = true;
                    }
                    else
                    {
                        model.Ispending = false;
                    }
                    model.StatusAmount = TotalAmount - DepositAmount;
                   
                }
                model.VendorId = VendorId; 
            }
            
            return model;
        }
        public List<VendorPaymentModel> getAllVendorAmount()
        {
            int OutletId = getOutletId();
            List<VendorPaymentModel> lst = new List<VendorPaymentModel>();
            var vendor = entities.SelesVendorAmounts.Where(a => a.OutletId == OutletId).ToList();
            foreach (var item in vendor)
            {
                VendorPaymentModel model = new VendorPaymentModel();
                model.DepositAmount = item.OutletVendor.SelesVendorAmountDetails.Sum(a => a.Amout);
                //item.OutletVendor.SelesVendorAmountDetails.Where(a=>a.VendorId==item.VendorId).ToList();
                model.getAllAmount = entities.SelesVendorAmountDetails.Where(a => a.VendorId == item.VendorId).ToList();
                model.PurchaseAmount = item.Amount;
                model.StatusBalance = model.DepositAmount - model.PurchaseAmount;
                model.VedorId = item.OutletVendor.OutletVenderId;
                model.VendorName = item.OutletVendor.VenderName;
                lst.Add(model);
            }
            return lst;
        }
        public void UpdateBalance(int VendorId,decimal Amount)
        {
            int OutletId = getOutletId();
            UpdateBillingDetailBalance(OutletId, VendorId, Amount);
        }
        #endregion
        #region Billing Region

        public CreateVendorBillingModel getCteateBilling(int VendorId)
        {
            CreateVendorBillingModel model = new CreateVendorBillingModel();
            model.getAllVendors = getAllVendorList();
            model.getAllCategory = getAllCategoryForBilling();
            model.VendorId = VendorId;
            if (VendorId>0)
            {
                int OutletId = getOutletId();
                var vendorAmount = entities.SelesVendorAmounts.Where(a => a.OutletId == OutletId && a.VendorId == VendorId).FirstOrDefault();
                if (vendorAmount!=null)
                {
                    decimal DepositAmount = entities.SelesVendorAmountDetails.Where(a => a.VendorId == VendorId).Sum(a => a.Amout);
                    decimal TotalAmount = vendorAmount.Amount;
                    if (TotalAmount > DepositAmount)
                    {
                        model.Ispending = true;
                    }
                    else
                    {
                        model.Ispending = false;
                    }
                    model.AmountBalance = DepositAmount - TotalAmount;
                }
                
            }
            
            return model;
        }

        public List<tblCategory> getAllCategoryForBilling()
        {
            List<tblCategory> lst = new List<tblCategory>();
            int OutletId = getOutletId();
            var categories = entities.tblMenuOutlets.Where(a => a.OutletId == OutletId).Select(a => a.CategoryId).ToList();
            var data = entities.tblCategories.Where(a => categories.Contains(a.CategoryId)).ToList();
            foreach (var item in data)
            {
                tblCategory model = new tblCategory();
                model.CategoryId = item.CategoryId;
                model.Name = item.Name;
                model.Color = item.Color;
                model.TextColor = item.TextColor;
                lst.Add(model);
            }
            return lst;
        }
        public List<GetBillingSubItemModel> getAllItemForBilling(int Id, int VendorId)
        {
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            int OutletId = getOutletId();
            var result = entities.tblMenuOutlets.Where(o => o.OutletId == OutletId && o.CategoryId == Id && o.tblItem.Active == true).ToList();
            foreach (var item in result)
            {
                bool status=item.VendorPrices.Where(a=>a.MenuOutletId==item.MenuOutletId&&a.VendorId==VendorId).Any();
                if (status)
                {
                    GetBillingSubItemModel model = new GetBillingSubItemModel();
                    model.Color = item.tblCategory.Color;
                    model.ItemId = item.ItemId;
                    model.Name = item.tblItem.Name;
                    model.TextColor = item.tblCategory.TextColor;
                    model.Outstock = stock.CheckOutStockItem(item.ItemId);
                    model.VendorId = VendorId;
                    lst.Add(model);
                }
               
                
            }
            return lst;
        }
        public VendorBillingModel getBill(int ItemId, int Qty, int VendorId)
        {
            UpdateBillingTable(ItemId, Qty, VendorId);
            VendorBillingModel model = new VendorBillingModel();
            model.getAllItems = getBillingModel(ItemId, Qty, VendorId);
            model.Totalamount = model.getAllItems.Sum(a => a.TotalPrice);
            var tax = entities.tbl_ServiceTax.FirstOrDefault();
            decimal ServiceTax = 0;
            if (tax != null)
            {
                ServiceTax = (tax.ServiceCharge / 100) * model.Totalamount;
            }
            model.ServicTaxAmount = ServiceTax;
            var Charge = entities.tblServiceCharges.FirstOrDefault();
            decimal ServiceCharge = 0;
            if (Charge != null)
            {
                ServiceCharge = (Charge.Charges / 100) * model.Totalamount;
            }

            model.ServiceChargeAmount = ServiceCharge;
            model.getPaymentMethd = getPaymentMethod();
            decimal VatAmount = 0;
            foreach (var item in model.getAllItems)
            {
                decimal vat = (item.Vat / 100) * item.TotalPrice;
                VatAmount = VatAmount + vat;
            }
            model.VatAmount = VatAmount;
            model.IsPrinted = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId && a.IsPrinted == true).Any();
            model.NetAmount = model.Totalamount + model.ServicTaxAmount + model.ServiceChargeAmount + model.VatAmount;
            model.DepositAmount = model.NetAmount;
            return model;
        }
        //

        public List<SelectListItem> getPaymentMethod()
        {
            var lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Value = "Cash", Text = "Cash" });
            lst.Add(new SelectListItem { Value = "Cheque", Text = "Cheque" });
            lst.Add(new SelectListItem { Value = "Card", Text = "Card" });
            lst.Add(new SelectListItem { Value = "Cash&Card", Text = "Cash&Card" });
            lst.Add(new SelectListItem { Value = "Due", Text = "Due" });
            return lst;
        }
        public List<VendorBillingItemModel> getBillingModel(int ItemId, int Qty, int VendorId)
        {
            List<VendorBillingItemModel> model = new List<VendorBillingItemModel>();
            int OutletId = getOutletId();

            var data = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId).Select(a => a).ToList();
            foreach (var item in data)
            {
                VendorBillingItemModel list = new VendorBillingItemModel();
                var vendor = entities.VendorPrices.Find(item.VendorPriceId);
                if (vendor != null)
                {
                    list.Id = item.ID;
                    list.VendorId = item.VendorId;
                    list.ActualPrice = vendor.tblMenuOutlet.FullPrice;
                    list.BasePrice = vendor.Price;
                    list.ItemName = vendor.tblMenuOutlet.tblItem.Name;
                    list.Qty = item.Qty;
                    list.TotalPrice = list.Qty * list.BasePrice;
                    list.Vat = item.Vat;
                    list.Iskot = item.IskotCleared;
                    list.QtyAfterClearKot = item.QtyAfterClearKot;
                    list.KotQty = list.Qty - list.QtyAfterClearKot;
                }

                model.Add(list);
            }
            return model;
        }
        public void UpdateBillingTable(int ItemId, int Qty, int VendorId)
        {
            int OutletId = getOutletId();
            Temp_VendorBilling tb = new Temp_VendorBilling();
            bool status = entities.Temp_VendorBilling.Where(a => a.OutletId == OutletId&& a.VendorId==VendorId && a.ItemId == ItemId).Any();
            if (status)
            {
                tb = entities.Temp_VendorBilling.Where(a => a.ItemId == ItemId && a.OutletId == OutletId&& a.VendorId==VendorId).FirstOrDefault();
                tb.Qty = tb.Qty + Qty;
                tb.IskotCleared = false;
                entities.SaveChanges();
            }
            else
            {
                var vendor = entities.VendorPrices.Where(a => a.VendorId == VendorId && a.tblMenuOutlet.ItemId == ItemId).FirstOrDefault();
                tb.ItemId = ItemId;
                tb.OutletId = OutletId;
                tb.VendorId = VendorId;
                tb.VendorPriceId = vendor.VendorPriceId;
                tb.Qty = Qty;
                tb.QtyAfterClearKot = 0;
                tb.IskotCleared = false;
                tb.IsPrinted = false;
                tb.Isdispatched = false;
                tb.Vat = vendor.tblMenuOutlet.tblBasePriceItem.Vat;
                entities.Temp_VendorBilling.Add(tb);
                entities.SaveChanges();
            }
        }
        public VendorBillingModel getBillondelete(int VendorId)
        {
            VendorBillingModel model = new VendorBillingModel();
            model.getAllItems = getBillingModelOndelete(VendorId);
            model.getPaymentMethd = getPaymentMethod();
            model.Totalamount = model.getAllItems.Sum(a => a.TotalPrice);
            model.IsPrinted = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId && a.IsPrinted == true).Any();
            var tax = entities.tbl_ServiceTax.FirstOrDefault();
            decimal ServiceTax = 0;
            if (tax != null)
            {
                ServiceTax = (tax.ServiceCharge / 100) * model.Totalamount;
            }
            model.ServicTaxAmount = ServiceTax;
            var Charge = entities.tblServiceCharges.FirstOrDefault();
            decimal ServiceCharge = 0;
            if (Charge != null)
            {
                ServiceCharge = (Charge.Charges / 100) * model.Totalamount;
            }

            model.ServiceChargeAmount = ServiceCharge;
            model.getPaymentMethd = getPaymentMethod();
            decimal VatAmount = 0;
            foreach (var item in model.getAllItems)
            {
                decimal vat = (item.Vat / 100) * item.TotalPrice;
                VatAmount = VatAmount + vat;
            }
            model.VatAmount = VatAmount;
            model.NetAmount = model.Totalamount + model.ServicTaxAmount + model.ServiceChargeAmount + model.VatAmount;
            return model;
        }
        public List<VendorBillingItemModel> getBillingModelOndelete(int VendorId)
        {
            List<VendorBillingItemModel> model = new List<VendorBillingItemModel>();
            int OutletId = getOutletId();

            var data = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId).Select(a => a).ToList();
            foreach (var item in data)
            {
                VendorBillingItemModel list = new VendorBillingItemModel();
                var vendor = entities.VendorPrices.Find(item.VendorPriceId);
                if (vendor != null)
                {
                    list.Id = item.ID;
                    list.ActualPrice = vendor.tblMenuOutlet.FullPrice;
                    list.BasePrice = vendor.Price;
                    list.ItemName = vendor.tblMenuOutlet.tblItem.Name;
                    list.Qty = item.Qty;
                    list.VendorId = item.VendorId;
                    list.TotalPrice = list.Qty * list.BasePrice;
                    list.Vat = item.Vat;
                    list.Iskot = item.IskotCleared;
                    list.QtyAfterClearKot = item.QtyAfterClearKot;
                    list.KotQty = list.Qty - list.QtyAfterClearKot;
                }

                model.Add(list);
            }
            return model;
        }
        public void DeleteItem(int ID)
        {
            Temp_VendorBilling tb = entities.Temp_VendorBilling.Find(ID);
            
            entities.Entry(tb).State = EntityState.Deleted;
            entities.SaveChanges();
        }
        public void PrintBill(int VendorId)
        {
            entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId).ToList().ForEach(a => a.IsPrinted = true);
            entities.SaveChanges();
        }
        public bool clearKot(int VendorId)
        {
            bool status = false;
            try
            {
                var tb = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId).ToList();
                foreach (var item in tb)
                {
                    item.IskotCleared = true;
                    item.QtyAfterClearKot = item.Qty;

                }
                entities.SaveChanges();
                status = true;
            }
            catch
            {

            }
            return status;
        }
        public VendorPrintBillModel getprintBill(int VendorId)
        {
            VendorPrintBillModel model = new VendorPrintBillModel();
            var vendor = entities.OutletVendors.Find(VendorId);
            if (vendor != null)
            {
                model.Address = string.Empty;
                model.VendorAddress = vendor.VendorAddress;
                model.VendorContactNo = vendor.VenderContact;
                model.VendorName = vendor.VenderName;
                model.Address = vendor.tblOutlet.Address;
                model.ContactA = vendor.tblOutlet.ContactA;
                model.ServiceTaxNo = vendor.tblOutlet.ServiceTaxNo;
                model.TinNo = vendor.tblOutlet.TinNo;

            }
            model.getAllItem = getallprintItem(VendorId);
            model.getAllVat = getAllVat(VendorId);
            model.Isprinted = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId && a.IsPrinted == true).Any();
            model.TotalAmount = model.getAllItem.Sum(a => a.BasicPrice);
            var tax = entities.tbl_ServiceTax.FirstOrDefault();
            decimal ServiceTax = 0;
            if (tax != null)
            {
                ServiceTax = (tax.ServiceCharge / 100) * model.TotalAmount;
            }
            model.ServiceTax = ServiceTax;
            var Charge = entities.tblServiceCharges.FirstOrDefault();
            decimal ServiceCharge = 0;
            if (Charge != null)
            {
                ServiceCharge = (Charge.Charges / 100) * model.TotalAmount;
            }
            model.VatAmount = model.getAllVat.Sum(a => a.amtCharges);
            model.ServicesCharge = ServiceCharge;
            model.NetAmount = model.TotalAmount + model.ServiceTax + model.ServicesCharge + model.VatAmount;
            return model;
        }
        public List<PrintItemModel> getallprintItem(int VendorId)
        {
            var lst = new List<PrintItemModel>();
            var data = (from p in entities.Temp_VendorBilling where p.VendorId == VendorId select p).ToList();
            foreach (var item in data)
            {
                PrintItemModel model = new PrintItemModel();
                model.BasicPrice = item.VendorPrice.Price;
                // model.Amount = item.VendorPrice.tblMenuOutlet.tblBasePriceItem.FullPrice;
                model.FullQty = item.Qty.ToString();
                model.ItemName = item.tblItem.Name;
                lst.Add(model);

            }
            return lst;
        }
        public List<PrintVatModel> getAllVat(int VendorId)
        {
            var lst = new List<PrintVatModel>();
            var VatDetail = (from p in entities.Temp_VendorBilling where p.VendorId == VendorId select p).ToList();

            foreach (var item in VatDetail)
            {
                PrintVatModel model = new PrintVatModel();
                model.Vat = item.Vat;
                decimal price = (item.VendorPrice.Price) * item.Qty;
                decimal vatAmount = (item.Vat / 100) * price;
                model.amtCharges = vatAmount;
                lst.Add(model);
            }
            return lst;
        }
        public VendorBillingModel getBillonPrint(int VendorId)
        {
            VendorBillingModel model = new VendorBillingModel();
            model.getAllItems = getBillingModelOndelete(VendorId);
            model.getPaymentMethd = getPaymentMethod();
            model.Totalamount = model.getAllItems.Sum(a => a.TotalPrice);
            model.IsPrinted = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId && a.IsPrinted == true).Any();
            var tax = entities.tbl_ServiceTax.FirstOrDefault();
            decimal ServiceTax = 0;
            if (tax != null)
            {
                ServiceTax = (tax.ServiceCharge / 100) * model.Totalamount;
            }
            model.ServicTaxAmount = ServiceTax;
            var Charge = entities.tblServiceCharges.FirstOrDefault();
            decimal ServiceCharge = 0;
            if (Charge != null)
            {
                ServiceCharge = (Charge.Charges / 100) * model.Totalamount;
            }

            model.ServiceChargeAmount = ServiceCharge;
            model.getPaymentMethd = getPaymentMethod();
            decimal VatAmount = 0;
            foreach (var item in model.getAllItems)
            {
                decimal vat = (item.Vat / 100) * item.TotalPrice;
                VatAmount = VatAmount + vat;
            }
            model.VatAmount = VatAmount;
            model.NetAmount = model.Totalamount + model.ServicTaxAmount + model.ServiceChargeAmount + model.VatAmount;
            return model;
        }

        public void DispatchOrder(VendorBillingModel model)
        {
            VendorBillingMaster tb = new VendorBillingMaster();
            int OutletId = getOutletId();
            if (model.ChequeDate != null)
            {
                tb.ChequeDate = Convert.ToDateTime(model.ChequeDate);
            }
            else
            {
                tb.ChequeDate = null;
            }
            tb.ChequeNo = model.ChequeNo;
            tb.NetAmount = model.NetAmount;
            tb.OutletId = OutletId;
            tb.PaymentMethod = model.PaymentMethod;
            tb.remainingAmount = model.remainingAmount;
            tb.ServiceChargeAmount = model.ServiceChargeAmount;
            tb.ServicTaxAmount = model.ServicTaxAmount;
            tb.TotalAmount = model.Totalamount;
            tb.VatAmount = model.VatAmount;
            tb.VendorId = model.VendorId;
            if (model.PaymentMethod=="Cash"||model.PaymentMethod=="Card"||model.PaymentMethod=="Cash&Card")
            {
                tb.DepositAmount = model.NetAmount;
                UpdateMainBalance(OutletId, model.VendorId, model.NetAmount,model.NetAmount);
            }
            else
            {
                if (model.DepositAmount == 0)
                {
                    tb.DepositAmount = model.NetAmount;
                }
                else
                {
                    tb.DepositAmount = model.DepositAmount;
                }
                
                UpdateMainBalance(OutletId, model.VendorId, model.NetAmount,model.DepositAmount);
            }
            
            tb.BillDate = DateTime.Now;
            entities.VendorBillingMasters.Add(tb);
            entities.SaveChanges();
            saveBillingItem(model.VendorId, tb.OrderId);
            deletefromTemp(model.VendorId);
            
        }
        public void UpdateBillingDetailBalance(int OutletId, int VendorId, decimal Amount)
        {

            SelesVendorAmountDetail tb = new SelesVendorAmountDetail();
            tb.Amout = Amount;
            tb.DepositDate = DateTime.Now;
            tb.VendorId = VendorId;
            entities.SelesVendorAmountDetails.Add(tb);
            entities.SaveChanges();
            var vendor = entities.SelesVendorAmounts.Where(a => a.VendorId == VendorId).FirstOrDefault();
            if (vendor==null)
            {
                UpdateMainBalance(OutletId, VendorId, 0, 0);
            }
        }
        public void UpdateMainBalance(int OutletId,int VendorId,decimal Amount,decimal depositAmout)
        {
            SelesVendorAmount tb = entities.SelesVendorAmounts.Where(a => a.VendorId == VendorId && a.OutletId == OutletId).FirstOrDefault();
            if (tb!=null)
            {
                tb.Amount = tb.Amount + Amount;
                entities.SaveChanges();
                UpdateBillingDetailBalance(OutletId, VendorId, depositAmout);
            }
            else
            {
                tb = new SelesVendorAmount();
                tb.Amount = Amount;
                tb.OutletId = OutletId;
                tb.VendorId = VendorId;
                entities.SelesVendorAmounts.Add(tb);
                entities.SaveChanges();
                if (Amount>0)
                {
                    UpdateBillingDetailBalance(OutletId, VendorId, depositAmout);
                }
                
            }
           
        }
        public void deletefromTemp(int VendorId)
        {
            List<Temp_VendorBilling> tb = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId).ToList();
            foreach (var item in tb)
            {
                entities.Entry(item).State = EntityState.Deleted;
                entities.SaveChanges();
            }
        }
        public void saveBillingItem(int VendorId, int OrderId)
        {
            var items = entities.Temp_VendorBilling.Where(a => a.VendorId == VendorId && a.Isdispatched == false).ToList();
            foreach (var item in items)
            {
                VendorBillingItem tb = new VendorBillingItem();
                tb.Amount = item.VendorPrice.Price * item.Qty;
                tb.ItemId = item.ItemId;
                tb.OrderId = OrderId;
                tb.Quantity = item.Qty;
                tb.Vat = item.Vat;
                entities.VendorBillingItems.Add(tb);
                entities.SaveChanges();
            }
        }
        public BillReportModel getBillReport(int VendorId = 0)
        {
            BillReportModel model = new BillReportModel();
            List<VendorBillingMaster> bill = new List<VendorBillingMaster>();
            if (VendorId > 0)
            {
                bill = entities.VendorBillingMasters.Where(a => a.VendorId == VendorId).ToList();
                model.TotalDipositAmount = bill.Sum(a => a.DepositAmount.Value);
                model.TotalRemainingAmount = bill.Sum(a => a.remainingAmount.Value);
            }
            model.getAllBill = bill;
            model.getAllVendor = getAllVendorList();
            return model;
        }
        #endregion
        #region Print Old Bill
        public VendorBillingMaster getOldBill(int ID)
        {
            VendorBillingMaster tb = entities.VendorBillingMasters.Find(ID);
            return tb;
        }
        public OutletVendor getOutletVwendor(int VendorId)
        {
            OutletVendor vendor = entities.OutletVendors.Find(VendorId);
            return vendor;
        }
        #endregion

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
                    oulte = Convert.ToInt32((from n in entities.tblOperators where n.UserId == WebSecurity.CurrentUserId select n.OutletId).FirstOrDefault());
                }

            }
            return oulte;
        }
    }
}