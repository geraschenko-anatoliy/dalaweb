﻿using System;
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
    [Authorize]
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
            return View(stampRepository.Get().Where(x => x.isOff == false));
        }

        public ActionResult Archive()
        {
            return View(stampRepository.Get().Where(x => x.isOff == true));
        }

        public ActionResult Details(int id = 0)
        {
            Stamp stamp = stampRepository.GetById(id);
            if (stamp == null)
            {
                return HttpNotFound();
            }
            return View(stamp);
        }


        public ActionResult Create(int counterId)
        {
            ViewBag.Counters = new SelectList(counterRepository.Get(), "CounterId", "Name", counterId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Stamp stamp)
        {      
            if (ModelState.IsValid)
            {
                stampRepository.Insert(stamp);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { AbonentId = counterRepository.GetById(stamp.CounterId).AbonentId });
            }

            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", stamp.CounterId);
            return View(stamp);
        }

        public ActionResult Edit(int id = 0)
        {
            Stamp stamp = stampRepository.GetById(id);

            if (stamp.isOff)
                return RedirectToAction("Archive");

            if (stamp == null)
            {
                return HttpNotFound();
            }
            ViewBag.Counters = new SelectList(counterRepository.Get(), "CounterId", "Name", stamp.Counter.Name);
            return View(stamp);
        }

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
            ViewBag.CounterId = new SelectList(counterRepository.Get(), "CounterId", "Name", stamp.Counter.CounterId);
            return View(stamp);
        }

        public ActionResult Delete(int id = 0)
        {
            Stamp stamp = stampRepository.GetById(id);

            if (stamp == null)
            {
                return HttpNotFound();
            }
            return View(stamp);
        }

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