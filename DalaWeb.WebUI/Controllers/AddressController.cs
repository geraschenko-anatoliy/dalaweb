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
            ViewBag.Cities = cityRepository.Get();
            ViewBag.Streets = streetRepository.Get();
            var addresses = addressRepository.Get();

            return View(new AdressIndexViewModel(streetRepository, addressRepository, cityRepository));
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
                GenerateAbonentNumber(address);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = address.AbonentId });
            }

            ViewBag.AbonentId = new SelectList(abonentRepository.Get() , "AbonentId", "Name", address.AbonentId);
            return View(address);
        }

        //
        // GET: /Address/Edit/5

        public ActionResult Edit(int abonentId )
        {
            Abonent abonent = abonentRepository.GetById(abonentId);
            Address address = addressRepository.GetById(abonentId);

            if (address == null)
            {
                return HttpNotFound();
            }

            SelectList CityIds = new SelectList(cityRepository.Get(), "CityId", "Name", abonent.Address.CityId);
            //var selectedCity = CityIds.Where(x => x.Value == abonent.Address.CityId.ToString()).First().Value;

            ////selectedCity.Selected = true;
            //CityIds.Where(x => x.Value == abonent.Address.CityId.ToString()).First().Selected = true;
            ViewBag.Cities = CityIds;
            //ViewBag.CityIds = cityRepository.Get();

            SelectList StreetIds = new SelectList(streetRepository.Get(), "StreetId", "Name", abonent.Address.StreetId);
            ////var selectedStreet = StreetIds.Where(x => x.Value == abonent.Address.StreetId.ToString()).First().Selected = true;
            ////selectedStreet.Selected = true;
            //StreetIds.Where(x => x.Value == abonent.Address.StreetId.ToString()).First().Selected = true;
            ViewBag.Streets = StreetIds;

            //ViewBag.StreetId = streetRepository.Get();
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
                GenerateAbonentNumber(address);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = address.AbonentId });
            }
            ViewBag.AbonentId = new SelectList(abonentRepository.Get(), "AbonentId", "Name", address.AbonentId);
            return View(address);
        }

        public void GenerateAbonentNumber(Address address)
        {
            Abonent abonent = abonentRepository.GetById(address.AbonentId);
            if (string.IsNullOrEmpty(abonent.AbonentNumber))
            {
                string abonentNumber = address.CityId.ToString("00") + address.StreetId.ToString("00");
                int number = abonentRepository.Get().Where(x=> x.AbonentNumber != null).Where(x => x.AbonentNumber.IndexOf(abonentNumber) == 0).Count();
                number++;
                abonentNumber += number.ToString("000");
                abonent.AbonentNumber = abonentNumber;
                abonentRepository.Update(abonent);
            }
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