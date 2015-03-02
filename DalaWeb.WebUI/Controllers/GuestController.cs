using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities.PDFStorages;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Abstract;
using System.IO;

namespace DalaWeb.WebUI.Controllers
{
    public class GuestController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<PDFAbonentMonthlyReceipt> pdfAbonentMonthlyReceiptRepository;

        public GuestController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            pdfAbonentMonthlyReceiptRepository = unitOfWork.PDFAbonentMonthlyReceiptRepository;
        }


        public FileStreamResult GetPDF(int PDFId)
        {
            Domain.Entities.PDFStorages.PDFAbonentMonthlyReceipt doc = pdfAbonentMonthlyReceiptRepository.GetById(PDFId);
            if (doc == null)
                return null;
            if (!string.Equals(doc.Abonent.INN, (string)Session["INN"]) || doc == null)
                return null;            
            MemoryStream workStream = new MemoryStream(doc.Value);
            workStream.Position = 0;

            return new FileStreamResult(workStream, "application/pdf");
        }


        // GET: /Guest/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PDFReceiptsForAbonentList(string INN)
        {
            Session["INN"] = INN;
            var pdfReceipts = pdfAbonentMonthlyReceiptRepository.Get().Where(x => x.Abonent.INN == INN);
            return View(pdfReceipts);
        }



        //public ActionResult Edit(int id = 0)
        //{
        //    PDFAbonentMonthlyReceipt pdfabonentmonthlyreceipt = db.PDFDocuments.Find(id);
        //    if (pdfabonentmonthlyreceipt == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AbonentId = new SelectList(db.Abonents, "AbonentId", "AbonentNumber", pdfabonentmonthlyreceipt.AbonentId);
        //    return View(pdfabonentmonthlyreceipt);
        //}

        //
        // POST: /Guest/Edit/5

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(PDFAbonentMonthlyReceipt pdfabonentmonthlyreceipt)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(pdfabonentmonthlyreceipt).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.AbonentId = new SelectList(db.Abonents, "AbonentId", "AbonentNumber", pdfabonentmonthlyreceipt.AbonentId);
        //    return View(pdfabonentmonthlyreceipt);
        //}

        //
        // GET: /Guest/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    PDFAbonentMonthlyReceipt pdfabonentmonthlyreceipt = db.PDFDocuments.Find(id);
        //    if (pdfabonentmonthlyreceipt == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(pdfabonentmonthlyreceipt);
        //}

        //
        // POST: /Guest/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    PDFAbonentMonthlyReceipt pdfabonentmonthlyreceipt = db.PDFDocuments.Find(id);
        //    db.PDFDocuments.Remove(pdfabonentmonthlyreceipt);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}