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
using DalaWeb.WebUI.ViewModels;
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Abonents;

namespace DalaWeb.WebUI.Controllers
{
    public class AbonentController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Abonent> abonentRepository;
        private IRepository<City> cityRepository;
        private IRepository<Street> streetRepository;

        public AbonentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            abonentRepository = unitOfWork.AbonentRepository;
            cityRepository = unitOfWork.CityRepository;
            streetRepository = unitOfWork.StreetRepository;
        }

        public ActionResult Index()
        {
            return View(new AbonentIndexViewModel(streetRepository, cityRepository, abonentRepository));
        }

        public ActionResult DeletedIndex()
        {
            return View(new DeletedAbonentIndexViewModel(streetRepository, cityRepository, abonentRepository));
        }

        //
        // GET: /Abonent/Details/5

        public ActionResult Details(int id = 0)
        {
            Abonent abonent = abonentRepository.GetById(id);

            if (abonent.isDeleted)
                return RedirectToAction("DeletedIndex");

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

            foreach (var counter in abonent.Counters)
            {
                counter.Stamps = unitOfWork.StampRepository.Get().Where(x => x.CounterId == counter.CounterId).ToList();
            }

            if (abonent == null)
            {
                return HttpNotFound();
            }

            if (abonent.isDeleted)
                return RedirectToAction("DeletedIndex");

            if (abonent.Address != null)
            {
                ViewBag.City = cityRepository.GetById(abonent.Address.CityId).Name;
                ViewBag.Street = streetRepository.GetById(abonent.Address.StreetId).Name;
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

        public ActionResult DeletedDetails(int id = 0)
        {
            Abonent abonent = abonentRepository.GetById(id);
            if (abonent.Address != null)
            {
                ViewBag.City = cityRepository.GetById(abonent.Address.CityId).Name;
                ViewBag.Street = streetRepository.GetById(abonent.Address.StreetId).Name;
            }
            if (abonent == null)
            {
                return HttpNotFound();
            }
            return View(abonent);
        }

        //
        // GET: /Abonent/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Abonent abonent = abonentRepository.GetById(id);

            if (abonent.isDeleted)
                return RedirectToAction("DeletedIndex");

            if (abonent == null)
            {
                return HttpNotFound();
            }
            return View(abonent);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Abonent abonent)
        {
            if (ModelState.IsValid)
            {
                abonent.isDeleted = true;
                abonentRepository.Update(abonent);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(abonent);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}