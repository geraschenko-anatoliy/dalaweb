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
    public class StreetController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<City> cityRepository;
        private IRepository<Street> streetRepository;
        
        public StreetController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            cityRepository = unitOfWork.CityRepository;
            streetRepository = unitOfWork.StreetRepository;
        }
        //
        // GET: /Street/

        public ActionResult Index()
        {
            var streets = streetRepository.Get().Include(s => s.City);
            return View(streets.ToList());
        }

        //
        // GET: /Street/Details/5

        public ActionResult Details(int id = 0)
        {
            Street street = streetRepository.GetById(id);
            if (street == null)
            {
                return HttpNotFound();
            }
            return View(street);
        }

        //
        // GET: /Street/Create

        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(cityRepository.Get(), "CityId", "Name");
            return View();
        }

        //
        // POST: /Street/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Street street)
        {
            if (ModelState.IsValid)
            {
                streetRepository.Insert(street);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(cityRepository.Get(), "CityId", "Name", street.CityId);
            return View(street);
        }

        //
        // GET: /Street/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Street street = streetRepository.GetById(id);
            if (street == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(cityRepository.Get(), "CityId", "Name", street.CityId);
            return View(street);
        }

        //
        // POST: /Street/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Street street)
        {
            if (ModelState.IsValid)
            {
                streetRepository.Update(street);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(cityRepository.Get(), "CityId", "Name", street.CityId);
            return View(street);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Save();
            base.Dispose(disposing);
        }
    }
}