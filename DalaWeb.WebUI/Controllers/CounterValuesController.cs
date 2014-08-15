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
    public class CounterValuesController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<CounterValues> counterValuesRepository;
        private IRepository<Counter> counterRepository;


        public CounterValuesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            counterValuesRepository = unitOfWork.CounterValuesRepository;
            counterRepository = unitOfWork.CounterRepository;
        }     
        
        public ActionResult Index()
        {
            return View(counterValuesRepository.Get().Include(c => c.Counter));
        }

        public ActionResult Details(int id = 0)
        {
            CounterValues countervalues = counterValuesRepository.GetById(id);

            if (countervalues == null)
            {
                return HttpNotFound();
            }
            return View(countervalues);
        }


        public ActionResult Create()
        {
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CounterValues countervalues)
        {
            if (ModelState.IsValid)
            {
                counterValuesRepository.Insert(countervalues);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", countervalues.CounterId);
            return View(countervalues);
        }

        public ActionResult Edit(int id = 0)
        {
            CounterValues countervalues = counterValuesRepository.GetById(id);
            if (countervalues == null)
            {
                return HttpNotFound();
            }
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", countervalues.CounterId);
            return View(countervalues);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CounterValues countervalues)
        {
            if (ModelState.IsValid)
            {
                counterValuesRepository.Update(countervalues);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", countervalues.CounterId);
            return View(countervalues);
        }


        public ActionResult Delete(int id = 0)
        {
            CounterValues countervalues = counterValuesRepository.GetById(id);
            if (countervalues == null)
            {
                return HttpNotFound();
            }
            return View(countervalues);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            counterValuesRepository.Delete(id);
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