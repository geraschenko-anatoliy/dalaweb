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

namespace DalaWeb.WebUI.Controllers
{
    [Authorize]
    public class CreditController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Credit> creditRepository;

        public CreditController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            creditRepository = unitOfWork.CreditRepository;
        }

        public ActionResult Index()
        {
            return View(creditRepository.Get());
        }

        public ActionResult Details(int id = 0)
        {
            Credit credit = creditRepository.GetById(id);
            
            if (credit == null)
            {
                return HttpNotFound();
            }
            return View(credit);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Credit credit)
        {
            if (ModelState.IsValid)
            {
                creditRepository.Insert(credit);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(credit);
        }

        public ActionResult Edit(int id = 0)
        {
            Credit credit = creditRepository.GetById(id);

            if (credit == null)
            {
                return HttpNotFound();
            }
            return View(credit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Credit credit)
        {
            if (ModelState.IsValid)
            {
                creditRepository.Update(credit);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(credit);
        }

        public ActionResult Delete(int id = 0)
        {
            Credit credit = creditRepository.GetById(id);
            if (credit == null)
            {
                return HttpNotFound();
            }
            return View(credit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            creditRepository.Delete(id);
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