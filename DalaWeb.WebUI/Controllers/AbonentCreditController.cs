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
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Abonents;

namespace DalaWeb.WebUI.Controllers
{
    [Authorize]
    public class AbonentCreditController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<AbonentCredit> abonentCreditRepository;
        private IRepository<Credit> creditRepository;
        private IRepository<Abonent> abonentRepository;

        public AbonentCreditController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
            creditRepository = unitOfWork.CreditRepository;
            abonentRepository = unitOfWork.AbonentRepository;
        }

        public ActionResult Index()
        {
            var abonentCredits = abonentCreditRepository.Get();
            return View(abonentCredits.ToList());
        }

        public ActionResult Details(int abonentCreditId)
        {
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(abonentCreditId);
            if (abonentCredit == null)
            {
                return HttpNotFound();
            }
            return View(abonentCredit);
        }

        public ActionResult Create(int abonentId)
        {
            ViewBag.AbonentId = abonentId;
            ViewBag.CreditId = new SelectList(creditRepository.Get(), "CreditId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AbonentCredit abonentCredit)
        {
            abonentCredit.PaymentForMonth = (abonentCredit.Price - abonentCredit.PrePayment) / abonentCredit.Term;
            abonentCredit.PaidForTheEntirePeriod = abonentCredit.PrePayment + abonentCredit.PaidMonths * abonentCredit.PaymentForMonth;
            abonentCredit.FinishDate = abonentCredit.StartDate.AddMonths(abonentCredit.Term);
            
            if (ModelState.IsValid)
            {
                abonentCreditRepository.Insert(abonentCredit);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = abonentCredit.AbonentId });
            }

            ViewBag.AbonentId = abonentCredit.AbonentId;
            ViewBag.CreditId = new SelectList(creditRepository.Get(), "CreditId", "Name");
            return View(abonentCredit);
        }

        public ActionResult Edit(int abonentCreditId)
        {
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(abonentCreditId);
            if (abonentCredit == null)
            {
                return HttpNotFound();
            }
            return View(abonentCredit);
        }

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

        public ActionResult Delete(int abonentCreditId)
        {
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(abonentCreditId);
            if (abonentCredit == null)
            {
                return HttpNotFound();
            }
            return View(abonentCredit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int abonentCreditId)
        {
            AbonentCredit abonentCredit = abonentCreditRepository.GetById(abonentCreditId);
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