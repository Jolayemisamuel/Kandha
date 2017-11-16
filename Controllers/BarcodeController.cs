using iTextSharp.text;
using iTextSharp.text.pdf;
using NibsMVC.EDMX;
using NibsMVC.Models;
using NibsMVC.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NibsMVC.Controllers
{
    public class BarcodeController : Controller
    {
        KitchenItemRepository obj = new KitchenItemRepository();
        NIBSEntities db = new NIBSEntities();

        // GET: Barcode
        public ActionResult Index()
        {
            return View(obj.ShowAllBarcodeGenList());
        }
        public ActionResult Create(int Id = 0)
        {
            IEnumerable<SelectListItem> Categorylist = (from q in db.RawCategories where q.Active == true select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawCategoryID.ToString() });

            ViewBag.Categorylists = new SelectList(Categorylist, "Value", "Text");

            IEnumerable<SelectListItem> itemlist = (from q in db.tbl_RawMaterials  select q).AsEnumerable().Select(q => new SelectListItem() { Text = q.Name, Value = q.RawMaterialId .ToString() });

            ViewBag.itemlist = new SelectList(itemlist, "Value", "Text");

            return View(); //obj.EditRawMaterial(Id)
        }
        [HttpPost]
        public ActionResult Create(BarcodeGenerateModel model)
        {
            var Data = obj.SaveBarcodeGen(model);
            TempData["Error"] = Data;
            GenerateReport(model);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Id = 0)
        {
            var data = obj.DeleteBarcodeGen(Id);
            TempData["Error"] = data;
            return RedirectToAction("Index");
        }

        public JsonResult getitem(int id)
        {
             
            List<SelectListItem> list = new List<SelectListItem>();
            var li = (from p in db.tbl_RawMaterials where p.rawcategoryId == id select p);

            foreach (var item in li)
            {

                list.Add(new SelectListItem { Text = Convert.ToString(item.Name ), Value = Convert.ToString(item.RawMaterialId) });

            }
            return Json(new SelectList(list, "Value", "Text", JsonRequestBehavior.AllowGet));


        }
        protected void GenerateReport(BarcodeGenerateModel model)
        {
            var img = (from p in db.tbl_RawMaterials  where p.RawMaterialId == model.RawMaterialsId  select p).SingleOrDefault();

            string imgPath = Server.MapPath("~/barcodes/" + img.barcode  + ".png");

            Font Font15 = FontFactory.GetFont("Verdana", 12, Font.BOLDITALIC);
            string str_pdffilename = "barcode.pdf";
            string str_pdfpath = Server.MapPath("~/barcodes/") + str_pdffilename; 
            
            Document doc = new Document(PageSize.A4, 50, 50, 55, 25);
            PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
            doc.Open();

            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(4);
            table.WidthPercentage = 100;
            float[] intwidth = new float[4] { 1, 1, 1, 1 };
            table.SetWidths(intwidth);
            table.DefaultCell.BorderColor = new BaseColor(0, 0, 0, 0);
            table.DefaultCell.BorderWidth = 1;
            table.SpacingAfter = 1;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cellPdf1 = new PdfPCell();

            cellPdf1 = new PdfPCell(new Phrase(img.Name, Font15));
            cellPdf1.Border = 0;
            cellPdf1.Colspan = 4;
            cellPdf1.BorderWidthBottom = 1F;
            cellPdf1.BorderWidthLeft = 0;
            cellPdf1.BorderWidthRight = 0;
            cellPdf1.BorderWidthTop = 0;
            cellPdf1.SetLeading(.5F, .5F);
            cellPdf1.BorderWidthBottom = 0;
            cellPdf1.HorizontalAlignment = Element.ALIGN_MIDDLE;
            cellPdf1.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.AddCell(cellPdf1);

            for (int i = 0; i < model.NoOfBarcode; i++)
            {
                
                Image png = Image.GetInstance(imgPath);
                PdfPCell cellPdf = new PdfPCell((Image.GetInstance(png)));
                png.ScaleToFit(1F, 2F);
                png.SpacingBefore = 0F;
                png.Alignment = Element.ALIGN_MIDDLE;
                cellPdf.Border = 0;
                cellPdf.Colspan = 0;
                cellPdf.Rowspan = 1;
                cellPdf.BorderWidthTop = 0;
                cellPdf.SetLeading(0F, 0F);
                cellPdf.BorderWidthBottom = 0;
                cellPdf.HorizontalAlignment = Element.ALIGN_MIDDLE;
                cellPdf.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cellPdf);
            }
            
         

            doc.Add(table);

            writer.CloseStream = false;
            doc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + img.Name  + " - Barcode.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(doc);
            Response.End();
            string script = string.Format(@"showDialogfile('{0}')", str_pdffilename);
            //ScriptManager.RegisterClientScriptBlock(this, typeof(Page), UniqueID, script, true);
        }

    }
}