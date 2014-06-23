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
    public class AddressController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Address> addressRepository;
        private IRepository<City> cityRepository;
        private IRepository<Street> streetRepository;
        private IRepository<Abonent> abonentRepository;


        public AddressController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            addressRepository = unitOfWork.AddressRepository;
            cityRepository = unitOfWork.CityRepository;
            streetRepository = unitOfWork.StreetRepository;
            abonentRepository = unitOfWork.AbonentRepository;
        }

        //
        // GET: /Address/

        public ActionResult Index()
        {
            var addresses = addressRepository.Get();
            return View(addresses.ToList());
        }

        //
        // GET: /Address/Details/5

        public ActionResult Details(int id = 0)
        {
            Address address = addressRepository.GetById(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        //
        // GET: /Address/Create

        public ActionResult Create(int abonentId)
        {      
            //ViewBag.AbonentID = new SelectList(abonentRepository.Get(), "AbonentID", "Name");

            ViewBag.AbonentId = abonentId;
            ViewBag.CityId = new SelectList(cityRepository.Get(), "CityId", "Name");
            ViewBag.StreetId = new SelectList(streetRepository.Get(), "StreetId", "Name");

            return View();
        }

        //
        // POST: /Address/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Address address)
        {
            if (ModelState.IsValid)
            {
                addressRepository.Insert(address);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.AbonentID = new SelectList(abonentRepository.Get() , "AbonentID", "Name", address.AbonentID);
            return View(address);
        }

        //
        // GET: /Address/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Address address = addressRepository.GetById(id);

            if (address == null)
            {
                return HttpNotFound();
            }
            ViewBag.AbonentID = new SelectList(abonentRepository.Get(), "AbonentID", "Name", address.AbonentID);
            return View(address);
        }

        //
        // POST: /Address/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Address address)
        {
            if (ModelState.IsValid)
            {
                addressRepository.Update(address);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.AbonentID = new SelectList(abonentRepository.Get(), "AbonentID", "Name", address.AbonentID);
            return View(address);
        }

        //
        // GET: /Address/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Address address = addressRepository.GetById(id);

            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        //
        // POST: /Address/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            addressRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public JsonResult GetStreets(int cityId)
        {
            List<SelectListItem> streets = new List<SelectListItem>();

            foreach (var item in streetRepository.Get().Where(x => x.CityId == cityId))
            {
                streets.Add(new SelectListItem { Text = item.Name, Value = item.StreetId.ToString() });
            }
            return Json(new SelectList(streets, "Value", "Text"));
        }



        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}