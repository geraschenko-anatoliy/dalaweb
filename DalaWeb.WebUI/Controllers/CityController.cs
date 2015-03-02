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

namespace DalaWeb.WebUI.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<City> cityRepository;

        public CityController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            cityRepository = unitOfWork.CityRepository;
        }

        public ActionResult Index()
        {
            return View(cityRepository.Get());
        }


        public ActionResult Details(int id = 0)
        {
            City city = cityRepository.GetById(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(City city)
        {
            if (ModelState.IsValid)
            {
                cityRepository.Insert(city);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(city);
        }

        public ActionResult Edit(int id = 0)
        {
            City city = cityRepository.GetById(id);

            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(City city)
        {
            if (ModelState.IsValid)
            {
                cityRepository.Update(city);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(city);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}