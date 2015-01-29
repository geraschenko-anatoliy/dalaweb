using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities.Counters;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Abonents;

namespace DalaWeb.WebUI.Controllers
{
    public class CounterController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Counter> counterRepository;
        private IRepository<Service> serviceRepository;
        private IRepository<Abonent> abonentRepository;
        private IRepository<Stamp> stampRepository;
        private IRepository<CounterValues> counterValuesRepository;

        public CounterController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            counterRepository = unitOfWork.CounterRepository;
            serviceRepository = unitOfWork.ServiceRepository;
            abonentRepository = unitOfWork.AbonentRepository;
            stampRepository = unitOfWork.StampRepository;
            counterValuesRepository = unitOfWork.CounterValuesRepository;
        }

        public ActionResult Index()
        {
            var counters = counterRepository.Get().Where(x => x.isOff == false);
            return View(counters);
        }

        public ActionResult Archive()
        {
            return View(counterRepository.Get().Where(x => x.isOff == true));
        }

        public ActionResult Details(int id = 0)
        {
            Counter counter = counterRepository.GetById(id);

            //counter.CounterValues = counterValuesRepository.Get().Where(x => x.Counter == counter).ToList();
            if (counter == null)
            {
                return HttpNotFound();
            }            
            
            //var stamps =  stampRepository.Get().Where(x => x.Counter == counter) ;
            //if (stamps != null)
            //    counter.Stamps = stamps.ToList();

            return View(counter);
        }

        public ActionResult Create(int abonentId, int serviceId)
        {
            ViewBag.Services = new SelectList(serviceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name", serviceId);
            ViewBag.AbonentName = abonentRepository.GetById(abonentId).Name;
            ViewBag.AbonentId = abonentId;
            ViewBag.ServiceId = serviceId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Counter counter)
        {                
            if (counterRepository.Get()
                .Where( x=> x.Name == counter.Name)
                .Where(x=>x.Service == counter.Service)
                .Where(x=> x.isOff == false)
                .Any())
                ModelState.AddModelError("Name", "Добавляемый счетчик существует и активно используется");
            if (ModelState.IsValid)
            {
                counterRepository.Insert(counter);
                unitOfWork.Save();
                CounterValues countervalue = new CounterValues
                {
                    Counter = counter,
                    CounterValuesId = 0,
                    Date = counter.StartDate,
                    Value = counter.InitialValue
                };
                counterValuesRepository.Insert(countervalue);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = counter.AbonentId});
            }
            ViewBag.Services = new SelectList(serviceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name", counter.Service.ServiceId);
            ViewBag.AbonentName = abonentRepository.GetById(counter.Abonent.AbonentId).Name;
            ViewBag.AbonentId = counter.Abonent.AbonentId;

            return View(counter);
        }

        public ActionResult Edit(int id = 0)
        {
            Counter counter = counterRepository.Get().Where(x => x.CounterId == id).Single();

            if (counter.isOff)
                return RedirectToAction("Archive");

            //counter.Stamps = stampRepository.Get().Where(x => x.Counter == counter).ToList();
            ViewBag.Services = new SelectList(serviceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name", counter.ServiceId);
            ViewBag.Abonents = new SelectList(abonentRepository.Get(), "AbonentId", "Name");

            if (counter == null)
            {
                return HttpNotFound();
            }

            return View(counter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Counter counter)
        {
            if (ModelState.IsValid)
            {
                counterRepository.Update(counter);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Services = new SelectList(serviceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name", counter.ServiceId);
            ViewBag.Abonents = new SelectList(abonentRepository.Get(), "AbonentId", "Name", counter.AbonentId);
            return View(counter);
        }

        public ActionResult Delete(int id = 0)
        {
            Counter counter = counterRepository.GetById(id);
            if (counter == null)
            {
                return HttpNotFound();
            }
            return View(counter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Counter counter)
        {
            counter.isOff = true;
            counterRepository.Update(counter);
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