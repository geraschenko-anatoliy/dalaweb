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
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Abonents;

namespace DalaWeb.WebUI.Controllers
{
    public class AbonentServiceController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<AbonentService> abonentServiceRepository;
        private IRepository<Service> serviceRepository;
        private IRepository<Abonent> abonentRepository;
        
        public AbonentServiceController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            serviceRepository = unitOfWork.ServiceRepository;
            abonentRepository = unitOfWork.AbonentRepository;
        }
        public ActionResult Index()
        {
            return View(abonentServiceRepository.Get()
                .Where(x => x.isOff == false)
                .ToList());
        }
        public ActionResult Archive()
        {
            return View(abonentServiceRepository.Get()
                .Where(x => x.isOff == true)
                .OrderBy(x => x.FinishDate)
                .ToList());
        }
        public ActionResult Details(int abonentServiceId)
        {
            var abonentService = abonentServiceRepository.GetById(abonentServiceId);
            if (abonentService == null)
            {
                return HttpNotFound();
            }
            return View(abonentService);
        }
        public ActionResult Create(int abonentId)
        {
            Abonent abonent = abonentRepository.Get().Where(x => x.AbonentId == abonentId).FirstOrDefault();
            ViewBag.AbonentName = abonent.Name;
            ViewBag.AbonentId = abonentId;
            ViewBag.ServiceCompanyId = new SelectList(unitOfWork.ServiceCompanyRepository.Get(), "ServiceCompanyId", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AbonentService abonentService)
        {
            abonentService.Service = serviceRepository.GetById(abonentService.Service.ServiceId);
            abonentService.Abonent = abonentRepository.GetById(abonentService.Abonent.AbonentId);
            abonentService.FinishDate = DateTime.MinValue;

            abonentServiceRepository.Insert(abonentService);
            unitOfWork.Save();
            return RedirectToAction("Edit", "Abonent", new { id = abonentService.Abonent.AbonentId });
        }
        public ActionResult TemporaryDisableForAbonent(int abonentId)
        {
            ViewBag.AbonentName = abonentRepository.GetById(abonentId).Name;
            ViewBag.AbonentId = abonentId;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TemporaryDisableForAbonent(int abonentId, DateTime finishDate, DateTime startDate)
        {
            Abonent abonent = abonentRepository.GetById(abonentId);
            var oldAbonentServices = abonentServiceRepository.Get()
                .Where(x => x.Abonent.AbonentId == abonentId)
                .Where(x => x.isOff == false);

            foreach(AbonentService abonentService in oldAbonentServices)
            {
                AbonentService futureAbonentService = new AbonentService()
                {
                    Abonent = abonentService.Abonent,
                    FinishDate = DateTime.MinValue,
                    //isOff = false,
                    Service = abonentService.Service,
                    StartDate = startDate
                };

                //abonentService.isOff = true;
                abonentService.FinishDate = finishDate;

                abonentServiceRepository.Insert(futureAbonentService);
                abonentServiceRepository.Update(abonentService);
            }

            unitOfWork.Save();
            return RedirectToAction("Edit","Abonent", new { id = abonentId });
        }
        public ActionResult Edit(int abonentServiceId = 0)
        {
            AbonentService abonentService = abonentServiceRepository.GetById(abonentServiceId);

            if (abonentService == null)
            {
                return HttpNotFound();
            }

            return View(abonentService);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AbonentService abonentService)
        {
            if (ModelState.IsValid)
            {
                abonentServiceRepository.Update(abonentService);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(abonentService);
        }
        public ActionResult Remove(int abonentId, int serviceId, DateTime startDate)
        {
            var abonentService = abonentServiceRepository.Get().Where(x => x.Abonent.AbonentId == abonentId)
                                                    .Where(x => x.Service.ServiceId == serviceId)
                                                    .Where(x => x.StartDate == startDate)
                                                    .FirstOrDefault();


            abonentService.FinishDate = DateTime.MinValue;

            if (abonentService == null)
            {
                return HttpNotFound();
            }
            return View(abonentService);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(AbonentService abonentService)
        {
            //abonentService.isOff = true;
            if (ModelState.IsValid)
            {
                abonentServiceRepository.Update(abonentService);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(abonentService);
        }
        public JsonResult GetServices(int companyId, int abonentId)
        {
            var currentAbonentServices = abonentServiceRepository.Get().Where(x => x.Abonent.AbonentId == abonentId).Where(x => x.isOff == false);
            List<SelectListItem> services = new List<SelectListItem>();

            foreach (var item in serviceRepository.Get().Where(x => x.ServiceCompany.ServiceCompanyId == companyId).Where(x => x.isOff == false))
            {
                if (!currentAbonentServices.Where(x=>x.Service.ServiceId == item.ServiceId).Any())
                    services.Add(new SelectListItem { Text = item.Name, Value = item.ServiceId.ToString() });
            }
            return Json(new SelectList(services, "Value", "Text"));
        }
        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}