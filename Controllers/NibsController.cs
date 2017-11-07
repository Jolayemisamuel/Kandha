using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NibsMVC.EDMX;
using NibsMVC.Models;
using System.Text;
using NibsMVC.Repository;
using System.Xml.Linq;
using System.Web.Security;
using WebMatrix.WebData;
using System.Xml;
namespace NibsMVC.Controllers
{
    public class NibsController : Controller
    {
        NIBSEntities db = new NIBSEntities();
        StringBuilder sb = new StringBuilder();
        XMLTablesRepository xml = new XMLTablesRepository();
        NIbsBillingRepository nibsrepo = new NIbsBillingRepository();
        AdminSearchRepository search = new AdminSearchRepository();
        CheckStockItemRepository outstock = new CheckStockItemRepository();
        public ActionResult Index()
        {
            NibsBillingModel nibs = new NibsBillingModel();
            List<BillTableModelList> ItemLIst = new List<BillTableModelList>();

            int oulte = nibsrepo.getOutletId();
            var address = (from p in db.tblOutlets where p.OutletId == oulte select p.Address).SingleOrDefault();
            ViewBag.outletaddress = address;
            var category = (from q in db.tblCategories where q.Active == true && (from p in db.tblMenuOutlets where p.OutletId == oulte select p.CategoryId).Contains(q.CategoryId) select q).ToList();
            // var ItemList = db.tblMenuOutlets.Where(o => o.OutletId == WebSecurity.CurrentUserId).ToList();
            foreach (var item in category)
            {
                BillTableModelList model = new BillTableModelList();
                model.CategoryId = item.CategoryId;
                model.CategoryName = item.Name;
                model.Color = item.Color;
                model.TextColor = item.TextColor;
                ItemLIst.Add(model);

            }
            nibs.getAllItem = ItemLIst;
            var result = db.tblTableMasters.Where(o => o.OutletId == oulte).ToList();

            List<BillTableModel> List = new List<BillTableModel>();
            foreach (var item in result)
            {
                BillTableModel mo = new BillTableModel();
                var filepath = Server.MapPath("/xmltables/table" + item.TableNo + ".xml");
                if (System.IO.File.Exists(filepath))
                {
                    XDocument xd = XDocument.Load(filepath);
                    var items = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                                 select p).ToList();
                    if (items.Count > 0)
                    {
                        bool printed = db.tblBillMasters.Where(a => a.OutletId == oulte && a.TableNo == item.TableNo && a.Isprinted == true).Any();
                        if (printed)
                        {
                            mo.Current = "printed";
                        }
                        else
                        {
                            mo.Current = "current";
                        }

                    }
                }
                mo.TableNo = item.TableNo.ToString();
                mo.AcType = item.AcType.ToString();
                List.Add(mo);
            }
            nibs.getAllTables = List;
            var serviccharg = (from p in db.tbl_ServiceTax select p.ServiceCharge).FirstOrDefault();
            if (serviccharg != 0)
            {
                nibs.ServiceCharge = serviccharg;
            }
            else
            {
                //nibs.ServiceCharge = Convert.ToDecimal(4.940);
                nibs.ServiceCharge = Convert.ToDecimal(5.6);

            }
            nibs.getAllAutoCompleteItem = getAllAutocompleteItems(oulte);

            return View(nibs);
        }
        public List<SelectListItem> getAllAutocompleteItems(int oulte)
        {
            var autocompleteItems = db.tblMenuOutlets.Where(a => a.OutletId == oulte).ToList();
            var lst = new List<SelectListItem>();
            foreach (var item in autocompleteItems)
            {
                lst.Add(new SelectListItem { Value = item.ItemId.ToString(), Text = item.tblItem.Name });
            }
            return lst;
        }
        public PartialViewResult _GetAllItemPartial(string Id)
        {
            List<GetBillingSubItemModel> lst = new List<GetBillingSubItemModel>();
            lst = xml.GetAllItems(Id);
            return PartialView("_GetAllItemPartial", lst);
        }
        public bool checkOutStock(int ItemId, int Qty, string TableNo)
        {
            var Path = Server.MapPath("/xmltables/table" + TableNo + ".xml");

            bool status = outstock.CheckOutStockItemWithQty(ItemId, Qty, TableNo, Path);
            return status;
        }
        public PartialViewResult _CreatePartial(int Id)
        {
            Session["RunningTable"] = Id;
            GetBillingModel model = new GetBillingModel();
            var Path = Server.MapPath("/xmltables/table" + Id + ".xml");
            var KotFilePath = Server.MapPath("/xmlkot/Kot.xml");
            if (System.IO.File.Exists(Path))
            {
                model = xml.GetBillingItem(Id.ToString(), Path, KotFilePath);
            }
            else
            {
                int oulte = nibsrepo.getOutletId();
                XmlTextWriter writer = new XmlTextWriter(Server.MapPath("/xmltables/table" + Id + ".xml"), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Item");
                xml.createNode(oulte.ToString(), Id.ToString(), "0", " ", "0", "0", "0", "0", "0", "0", "0", "0", writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                // MessageBox.Show("XML File created ! ");
                model = xml.GetBillingItem(Id.ToString(), Path, KotFilePath);
            }

            return PartialView("_CreatePartial", model);
        }
        [HttpPost]
        public ActionResult UpdateBillingXml(GetBillItemModel m)
        {
            //GetBillItemModel m = new GetBillItemModel();
            //var data = Id.Split('^');
            //m.ItemId = Convert.ToInt32(data[0]);
            //m.Qty = Convert.ToInt32(data[2]);
            //m.RunningTable = Convert.ToInt32(data[1]);
            //m.Type = data[3];
            var filepath = Server.MapPath("~/xmltables/table" + m.RunningTable + ".xml");
            var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool update = nibsrepo.CheckBillItem(m, filepath, KotFilePath);
            if (update)
            {
                model = xml.GetBillingItem(m.RunningTable.ToString(), filepath, KotFilePath);
            }


            return PartialView("_CreatePartial", model);
        }
        public ActionResult DeleteItem(string ItemName, int Id = 0, int TableNo = 0)
        {
            var Path = Server.MapPath("~/xmltables/table" + TableNo + ".xml");
            var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool Update = xml.DeleteNode(Id.ToString(), TableNo.ToString(), Path, KotFilePath, ItemName);
            if (Update)
            {
                model = xml.GetBillingItem(TableNo.ToString(), Path, KotFilePath);
            }


            return PartialView("_CreatePartial", model);
        }
        public ActionResult ClearKotItem(int Id = 0)
        {
            var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
            var Path = Server.MapPath("~/xmltables/table" + Id + ".xml");
            GetBillingModel model = new GetBillingModel();
            bool Update = xml.ClearKot(Path, KotFilePath, Id.ToString());
            if (Update)
            {
                model = xml.GetBillingItem(Id.ToString(), Path, KotFilePath);
            }

            return PartialView("_CreatePartial", model);
        }
        public ActionResult PrintOrder(GetBillingModel model)
        {
            var Path = Server.MapPath("~/xmltables/table" + model.TableNo + ".xml");
            PrintBillModel m = new PrintBillModel();
            if (model.BillId == 0)
            {

                var ReturnXml = Server.MapPath("~/xmlkot/ReturnXML.xml");
                var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
                nibsrepo.SaveReturnItem(ReturnXml);
                int BillId = xml.DispatchOrder(model, Path);
                model.BillId = BillId;

            }
            m = nibsrepo.GetBill(Path, model);
            m.BillType = model.OrderType;
            return PartialView("PrintOrder", m);

        }
        public ActionResult PrintOrderAgain()
        {
            return PartialView("PrintOrder");
        }
        //public ActionResult PrintKot()
        //{
        //    var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
        //    return PartialView();
        //}
        [HttpPost]
        public ActionResult DispatchOrder(GetBillingModel model)
        {
            var Path = Server.MapPath("~/xmltables/table" + model.TableNo + ".xml");
            var ReturnXml = Server.MapPath("~/xmlkot/ReturnXML.xml");
            var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
            nibsrepo.SaveReturnItem(ReturnXml);
            xml.updateBillOnDispatch(model, Path);
            //bool status = xml.DispatchOrder(model, Path);
            var tableNo = model.TableNo;
            ModelState.Clear();
            model = new GetBillingModel();
            model = xml.GetBillingItem(tableNo.ToString(), Path, KotFilePath);
            return PartialView("_CreatePartial", model);
        }
        [HttpPost]
        public PartialViewResult DispatchOpenFood(BillOpenFoodModel model)
        {
            int oulte = xml.getOutletId();
            OpenFood food = new OpenFood();
            food.Date = DateTime.Now.Date;
            food.ItemName = model.OpenFoodName;
            food.OutletId = oulte;
            food.Price = model.OpenFoodPrice;
            food.Quantity = model.OpenFoodQuantity;
            food.Amount = model.OpenFoodPrice * model.OpenFoodQuantity;
            food.Vat = model.OpenFoodVat;
            db.OpenFoods.Add(food);
            db.SaveChanges();
            var Path = Server.MapPath("/xmltables/table" + model.TableNo + ".xml");
            var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
            GetBillingModel m = new GetBillingModel();
            nibsrepo.SaveXmlOpenFood(model, Path, KotFilePath);
            m = xml.GetBillingItem(model.TableNo.ToString(), Path, KotFilePath);
            return PartialView("_CreatePartial", m);
        }
        [HttpPost]
        public ActionResult CancelOrder(int TableNo = 0)
        {
            int oulte = xml.getOutletId();
            GetBillingModel model = new GetBillingModel();
            var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
            var Path = Server.MapPath("~/xmltables/table" + TableNo + ".xml");
            XDocument xd = XDocument.Load(Path);
            var items = (from item in xd.Descendants("Items")
                         where item.Element("UserId").Value == oulte.ToString()
                         select item);
            items.Remove();
            xd.Save(Path);
            bool Update = xml.ClearKot(Path, KotFilePath, TableNo.ToString());
            if (Update)
            {
                model = xml.GetBillingItem(TableNo.ToString(), Path, KotFilePath);
            }

            return PartialView("_CreatePartial", model);
        }

        public PartialViewResult _MargeTable()
        {
            int oulte = xml.getOutletId();
            List<BillTableModel> List = new List<BillTableModel>();
            var result = db.tblTableMasters.Where(o => o.OutletId == oulte).ToList();
            foreach (var item in result)
            {
                BillTableModel mo = new BillTableModel();
                var filepath = Server.MapPath("~/xmltables/table" + item.TableNo + ".xml");
                if (System.IO.File.Exists(filepath))
                {
                    XDocument xd = XDocument.Load(filepath);
                    var items = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                                 select p).ToList();
                    if (items.Count == 0)
                    {
                        mo.TableNo = item.TableNo.ToString();
                        mo.AcType = item.AcType.ToString();
                        List.Add(mo);
                    }


                }

            }

            return PartialView("_MargeTable", List);
        }

        public PartialViewResult _shiftTable()
        {
            int oulte = xml.getOutletId();
            shiftBillModel model = new shiftBillModel();
            List<BillTableModel> book = new List<BillTableModel>();
            List<BillTableModel> free = new List<BillTableModel>();
            var result = db.tblTableMasters.Where(o => o.OutletId == oulte).ToList();
            foreach (var item in result)
            {
                BillTableModel mo = new BillTableModel();
                var filepath = Server.MapPath("~/xmltables/table" + item.TableNo + ".xml");
                if (System.IO.File.Exists(filepath))
                {
                    XDocument xd = XDocument.Load(filepath);
                    var items = (from p in xd.Descendants("Items")
                                 where p.Element("UserId").Value == oulte.ToString() && p.Element("TableNo").Value == item.TableNo.ToString()
                                 select p).ToList();
                    if (items.Count == 0)
                    {
                        mo.TableNo = item.TableNo.ToString();
                        mo.AcType = item.AcType.ToString();
                        free.Add(mo);
                    }
                    else
                    {
                        mo.TableNo = item.TableNo.ToString();
                        mo.AcType = item.AcType.ToString();
                        book.Add(mo);
                    }


                }

            }
            model.getAllBooktable = book;
            model.getAllfreetable = free;
            return PartialView("_shiftTable", model);
        }
        public bool ShiftTable(int Shiftfrom = 0, int ShiftTo = 0)
        {
            int oulte = nibsrepo.getOutletId();
            var Path = Server.MapPath("~/xmltables/table" + ShiftTo + ".xml");
            if (!System.IO.File.Exists(Path))
            {

                XmlTextWriter writer = new XmlTextWriter(Server.MapPath("~/xmltables/table" + ShiftTo + ".xml"), System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.WriteStartElement("Item");
                xml.createNode(oulte.ToString(), ShiftTo.ToString(), "0", " ", "0", "0", "0", "0", "0", "0", "0", "0", writer);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
            }
            var shiftfrompath = Server.MapPath("~/xmltables/table" + Shiftfrom + ".xml");
            var shiftTopath = Server.MapPath("~/xmltables/table" + ShiftTo + ".xml");
            bool status = nibsrepo.ShiftTable(shiftfrompath, shiftTopath, oulte, Shiftfrom.ToString(), ShiftTo.ToString());
            return status;
        }
        public PartialViewResult _splitTable(int TableNo, decimal NetAmount)
        {
            return PartialView("_splitTable");
        }

        public PartialViewResult _Return(int TableNo = 0, int ItemId = 0)
        {
            var Path = Server.MapPath("~/xmltables/table" + TableNo + ".xml");
            var ReturnXml = Server.MapPath("~/xmlkot/ReturnXML.xml");
            var KotFilePath = Server.MapPath("~/xmlkot/Kot.xml");
            GetBillingModel model = new GetBillingModel();
            bool status = nibsrepo._TransferToReturnXML(Path, ReturnXml, TableNo, ItemId);
            if (status)
            {
                model = xml.GetBillingItem(TableNo.ToString(), Path, KotFilePath);
            }
            return PartialView("_CreatePartial", model);
        }

    }
}
