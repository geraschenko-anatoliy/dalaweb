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

namespace DalaWeb.WebUI.Controllers
{
    public class StampController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Stamp> stampRepository;
        private IRepository<Counter> counterRepository;        

        public StampController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            stampRepository = unitOfWork.StampRepository;
            counterRepository = unitOfWork.CounterRepository;
        } 

        public ActionResult Index()
        {
            return View(stampRepository.Get().Include(c => c.Counter).Where(x => x.isOff == false));
        }

        public ActionResult Archive()
        {
            return View(stampRepository.Get().Include(c => c.Counter).Where(x => x.isOff == true));
        }


        //
        // GET: /Stamp/Details/5

        public ActionResult Details(int id = 0)
        {
            Stamp stamp = stampRepository.GetById(id);
            if (stamp == null)
            {
                return HttpNotFound();
            }
            return View(stamp);
        }

        //
        // GET: /Stamp/Create

        public ActionResult Create(int counterId)
        {
            ViewBag.Counters = new SelectList(counterRepository.Get(), "CounterId", "Name", counterId);
            return View();
        }

        //
        // POST: /Stamp/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Stamp stamp)
        {
            int abonentId = counterRepository.GetById(stamp.CounterId).AbonentId;
            if (ModelState.IsValid)
            {
                stampRepository.Insert(stamp);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = abonentId});
            }

            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", stamp.CounterId);
            return View(stamp);
        }

        //
        // GET: /Stamp/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Stamp stamp = stampRepository.GetById(id);

            if (stamp.isOff)
                return RedirectToAction("Archive");

            ViewBag.Counters = new SelectList(counterRepository.Get(), "CounterId", "Name", stamp.CounterId);

            if (stamp == null)
            {
                return HttpNotFound();
            }
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", stamp.CounterId);
            return View(stamp);
        }

        //
        // POST: /Stamp/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stamp stamp)
        {
            if (ModelState.IsValid)
            {
                stampRepository.Update(stamp);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", stamp.CounterId);
            return View(stamp);
        }

        //
        // GET: /Stamp/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Stamp stamp = stampRepository.GetById(id);

            if (stamp == null)
            {
                return HttpNotFound();
            }
            return View(stamp);
        }

        //
        // POST: /Stamp/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Stamp stamp)
        {
            stampRepository.Update(stamp);
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