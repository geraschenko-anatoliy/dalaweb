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
using DalaWeb.WebUI.ViewModels.ForAddress;
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Abonents;

namespace DalaWeb.WebUI.Controllers
{
    [Authorize]
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

        public ActionResult Index()
        {
            ViewBag.Cities = cityRepository.Get();
            ViewBag.Streets = streetRepository.Get();
            var addresses = addressRepository.Get();

            return View(new AdressIndexViewModel(streetRepository, addressRepository, cityRepository));
        }

        //public ActionResult Details(int id = 0)
        //{
        //    Address address = addressRepository.GetById(id);
        //    if (address == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(address);
        //}

        public ActionResult Create(int abonentId)
        {      
            Session["AbonentId"] = abonentId;
            ViewBag.AbonentName = abonentRepository.GetById(abonentId).Name;
            ViewBag.CityId = new SelectList(cityRepository.Get(), "CityId", "Name");
            ViewBag.StreetId = new SelectList(streetRepository.Get(), "StreetId", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(String House, int CityId, int StreetId)
        {
            Address address = new Address()
            {
                AbonentId = (int)Session["AbonentId"],
                Abonent = abonentRepository.GetById((int)Session["AbonentId"]),
                CityId = CityId,
                StreetId = StreetId,
                House = House
            };

            if (ModelState.IsValid)
            {
                addressRepository.Insert(address);
                GenerateAbonentNumber(address);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = address.Abonent.AbonentId });
            }

            ViewBag.CityId = new SelectList(cityRepository.Get(), "CityId", "Name");
            ViewBag.StreetId = new SelectList(streetRepository.Get(), "StreetId", "Name");
            return View(address);
        }

        public ActionResult Edit(int abonentId )
        {
            Abonent abonent = abonentRepository.GetById(abonentId);
            Address address = addressRepository.GetById(abonentId);

            if (address == null)
            {
                return HttpNotFound();
            }

            ViewBag.Cities = new SelectList(cityRepository.Get(), "CityId", "Name", abonent.Address.City.CityId);
            ViewBag.Streets= new SelectList(streetRepository.Get().Where(x => x.City.CityId == abonent.Address.City.CityId), "StreetId", "Name", abonent.Address.Street.StreetId);

            return View(address);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Address address)
        {
            address.Abonent = abonentRepository.GetById(address.AbonentId);
            GenerateAbonentNumber(address);
            addressRepository.Update(address);
            unitOfWork.Save();
            return RedirectToAction("Edit", "Abonent", new { id = address.AbonentId });
        }
        public ActionResult Delete(int id = 0)
        {
            Address address = addressRepository.GetById(id);

            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }
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

            foreach (var item in streetRepository.Get().Where(x => x.City.CityId == cityId))
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
        
        private void GenerateAbonentNumber(Address address)
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
    }
}