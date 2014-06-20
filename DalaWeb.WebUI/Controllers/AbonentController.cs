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
    public class AbonentController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Abonent> abonentRepository;
        //private IRepository<Credit> creditRepository;

        public AbonentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            abonentRepository = unitOfWork.AbonentRepository;
            //creditRepository = unitOfWork.CreditRepository;
        }

        public ActionResult Index()
        {
            return View(abonentRepository.Get());
        }

        //
        // GET: /Abonent/Details/5

        public ActionResult Details(int id = 0)
        {
            Abonent abonent = abonentRepository.GetById(id);
            if (abonent == null)
            {
                return HttpNotFound();
            }
            return View(abonent);
        }

        //
        // GET: /Abonent/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Abonent/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Abonent abonent)
        {
            if (ModelState.IsValid)
            {
                abonentRepository.Insert(abonent);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(abonent);
        }

        //
        // GET: /Abonent/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Abonent abonent = abonentRepository.GetById(id); 

            if (abonent == null)
            {
                return HttpNotFound();
            }
            return View(abonent);
        }

        //
        // POST: /Abonent/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Abonent abonent)
        {
            if (ModelState.IsValid)
            {
                abonentRepository.Update(abonent);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(abonent);
        }

        //
        // GET: /Abonent/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Abonent abonent = abonentRepository.GetById(id);

            if (abonent == null)
            {
                return HttpNotFound();
            }
            return View(abonent);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            abonentRepository.Delete(id);
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