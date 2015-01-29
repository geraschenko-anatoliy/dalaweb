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
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.WebUI.ViewModels.ForAbonent;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Services;

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
            return View(new IndexViewModel(streetRepository.Get(), cityRepository.Get(), abonentRepository.Get().Where(x=>x.isDeleted == false)));
        }
        public ActionResult DeletedIndex()
        {
            return View(new DeletedViewModel(streetRepository, cityRepository, abonentRepository));
        }
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
        public ActionResult Create()
        {
            return View();
        }
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
        public ActionResult Edit(int id = 0)
        {
            Abonent abonent = abonentRepository.Get()
                .Where(x => x.AbonentId == id)
                .Single()
                ;

            Session["AbonenId"] = abonent.AbonentId;

            if (abonent == null)
            {
                return HttpNotFound();
            }

            if (abonent.isDeleted)
                return RedirectToAction("DeletedIndex");

            return View(abonent);
        }
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
                ViewBag.City = cityRepository.GetById(abonent.Address.City.CityId).Name;
                ViewBag.Street = streetRepository.GetById(abonent.Address.Street.StreetId).Name;
            }
            if (abonent == null)
            {
                return HttpNotFound();
            }
            return View(abonent);
        }
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