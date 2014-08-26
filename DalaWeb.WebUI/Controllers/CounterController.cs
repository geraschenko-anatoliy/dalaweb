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


        //
        // GET: /Counter/

        public ActionResult Index()
        {
            var counters = counterRepository.Get().Where(x => x.isOff == false).Include(x => x.CounterValues).Include(x => x.Stamps);
            return View(counters);
        }
        public ActionResult Archive()
        {
            return View(counterRepository.Get().Where(x => x.isOff == true).Include(x => x.CounterValues));
        }

        //
        // GET: /Counter/Details/5

        public ActionResult Details(int id = 0)
        {
            Counter counter = counterRepository.GetById(id);
            counter.CounterValues = counterValuesRepository.Get().Where(x => x.CounterId == counter.CounterId).ToList();
            if (counter == null)
            {
                return HttpNotFound();
            }            
            
            var stamps =  stampRepository.Get().Where(x => x.CounterId == counter.CounterId) ;
            if (stamps != null)
                counter.Stamps = stamps.ToList();

            return View(counter);
        }

        //
        // GET: /Counter/Create

        public ActionResult Create(int abonentId, int serviceId, DateTime StartDate)
        {
            ViewBag.Services = new SelectList(serviceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name", serviceId);
            ViewBag.AbonentName = abonentRepository.GetById(abonentId).Name;
            ViewBag.AbonentId = abonentId;
            return View();
        }

        //
        // POST: /Counter/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Counter counter)
        {                
            if (counterRepository.Get().Where( x=> x.Name == counter.Name).Where(x=>x.ServiceId == counter.ServiceId).Where(x=> x.isOff == false).Any())
                ModelState.AddModelError("Name", "Добавляемый счетчик существует и активно используется");
            if (ModelState.IsValid)
            {
                counterRepository.Insert(counter);
                unitOfWork.Save();
                CounterValues countervalue = new CounterValues
                {
                    CounterId = counter.CounterId,
                    CounterValuesId = 0,
                    Date = counter.StartDate,
                    Value = counter.InitialValue
                };
                counterValuesRepository.Insert(countervalue);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Services = new SelectList(serviceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name", counter.ServiceId);
            ViewBag.AbonentName = abonentRepository.GetById(counter.AbonentId).Name;
            ViewBag.AbonentId = counter.AbonentId;

            return View(counter);
        }

        //
        // GET: /Counter/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Counter counter = counterRepository.GetById(id);

            if (counter.isOff)
                return RedirectToAction("Archive");

            counter.Stamps = stampRepository.Get().Where(x => x.CounterId == counter.CounterId).ToList();
            ViewBag.Services = new SelectList(serviceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name", counter.ServiceId);
            ViewBag.Abonents = new SelectList(abonentRepository.Get(), "AbonentId", "Name");

            if (counter == null)
            {
                return HttpNotFound();
            }

            return View(counter);
        }

        //
        // POST: /Counter/Edit/5

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

        //
        // GET: /Counter/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Counter counter = counterRepository.GetById(id);
            if (counter == null)
            {
                return HttpNotFound();
            }
            return View(counter);
        }

        //
        // POST: /Counter/Delete/5

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