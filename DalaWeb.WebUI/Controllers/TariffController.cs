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
using DalaWeb.Domain.Entities.Abonents;

namespace DalaWeb.WebUI.Controllers
{
    public class TariffController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Tariff> tariffRepository;
        private IRepository<Abonent> abonentRepository;
        
        public TariffController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            tariffRepository = unitOfWork.TariffRepository;
            abonentRepository = unitOfWork.AbonentRepository;
        }

        public ActionResult Index()
        {
            return View(tariffRepository.Get().Include(x=>x.Service));
        }

        public ActionResult Details(int id = 0)
        {
            Tariff tariff = tariffRepository.GetById(id);
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        public ActionResult Create()
        {
            ViewBag.ServiceId = new SelectList(unitOfWork.ServiceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                tariffRepository.Insert(tariff);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceId = new SelectList(unitOfWork.ServiceRepository.Get().Where(x => x.Type == 3), "ServiceId", "Name");
            return View(tariff);
        }

        public ActionResult Edit(int id = 0)
        {
            Tariff tariff = tariffRepository.GetById(id);
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                tariffRepository.Update(tariff);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(tariff);
        }

        public ActionResult Delete(int id = 0)
        {
            Tariff tariff = tariffRepository.GetById(id);
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tariffRepository.Delete(id);
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