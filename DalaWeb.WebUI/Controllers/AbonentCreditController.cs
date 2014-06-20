using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Abstract;

namespace DalaWeb.WebUI.Controllers
{
    public class AbonentCreditController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<AbonentCredit> abonentCreditRepository;
        private IRepository<Credit> creditRepository;


        public AbonentCreditController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
            creditRepository = unitOfWork.CreditRepository;
        }

        //
        // GET: /AbonentCredit/

        public ActionResult Index()
        {
            var abonentCredits = abonentCreditRepository.Get().Include(a => a.Abonent).Include(a => a.Credit);
            return View(abonentCredits.ToList());
        }

        //
        // GET: /AbonentCredit/Details/5

        public ActionResult Details(int abonentId, int creditId)
        {
            int[] ids = new int[] { abonentId, creditId };
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(ids);
            if (abonentCredit == null)
            {
                return HttpNotFound();
            }
            return View(abonentCredit);
        }

        //
        // GET: /AbonentCredit/Create

        public ActionResult Create(int abonentId)
        {
            ViewBag.AbonentID = abonentId;
            ViewBag.CreditID = new SelectList(creditRepository.Get(), "CreditId", "Name");
            return View();
        }

        //
        // POST: /AbonentCredit/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AbonentCredit abonentCredit)
        {
            if (ModelState.IsValid)
            {
                abonentCreditRepository.Insert(abonentCredit);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = abonentCredit.AbonentID });
            }

            ViewBag.AbonentID = abonentCredit.AbonentID;
            ViewBag.CreditID = new SelectList(creditRepository.Get(), "CreditId", "Name");
            return View(abonentCredit);
        }

        //
        // GET: /AbonentCredit/Edit/5

        public ActionResult Edit(int abonentId, int creditId)
        {
            int[] ids = new int[] { abonentId, creditId };
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(ids);
            if (abonentCredit == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AbonentID = new SelectList(db.Abonents, "AbonentID", "Name", abonentcredit.AbonentID);
            //ViewBag.CreditID = new SelectList(db.Credits, "CreditId", "Name", abonentcredit.CreditID);
            return View(abonentCredit);
        }

        //
        // POST: /AbonentCredit/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AbonentCredit abonentCredit)
        {
            if (ModelState.IsValid)
            {
                abonentCreditRepository.Update(abonentCredit);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(abonentCredit);
        }

        //
        // GET: /AbonentCredit/Delete/5

        public ActionResult Delete(int abonentId, int creditId)
        {
            int[] ids = new int[] { abonentId, creditId };
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(ids);
            if (abonentCredit == null)
            {
                return HttpNotFound();
            }
            return View(abonentCredit);
        }

        //
        // POST: /AbonentCredit/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int abonentId, int creditId)
        {
            int[] ids = new int[] { abonentId, creditId };
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(ids);
            abonentCreditRepository.Delete(abonentCredit);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}