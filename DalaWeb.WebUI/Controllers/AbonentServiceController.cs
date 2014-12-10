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
        
        public AbonentServiceController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            serviceRepository = unitOfWork.ServiceRepository;
        }

        //
        // GET: /AbonentService/

        public ActionResult Index()
        {
            return View(abonentServiceRepository.Get()
                .Include(x => x.Service)
                .Include(x=>x.Abonent)
                .Where(x=> x.isOff == false)
                .ToList());
        }
        public ActionResult Archive()
        {
            return View(abonentServiceRepository.Get()
                .Include(x => x.Service)
                .Include(x => x.Abonent)
                .Where(x => x.isOff == true)
                .OrderBy(x => x.FinishDate)
                .ToList());
        }

        //
        // GET: /AbonentService/Details/5

        public ActionResult Details(int abonentId, int serviceId, DateTime startDate)
        {
            var abonentService = abonentServiceRepository.Get().Where(x => x.AbonentId == abonentId)
                                                                .Where(x => x.ServiceId == serviceId)
                                                                .Where(x => x.StartDate == startDate)
                                                                .FirstOrDefault();
            if (abonentService == null)
            {
                return HttpNotFound();
            }
            return View(abonentService);
        }

        //
        // GET: /AbonentService/Create

        public ActionResult Create(int abonentId)
        {
            Abonent abonent = unitOfWork.AbonentRepository.Get().Where(x => x.AbonentId == abonentId).Include(x=>x.AbonentServices).FirstOrDefault();
            ViewBag.AbonentName = abonent.Name;
            ViewBag.AbonentId = abonentId;
            ViewBag.FinishDate = "01/01/1000";
            ViewBag.CompanyId = new SelectList(unitOfWork.ServiceCompanyRepository.Get(), "CompanyId", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AbonentService abonentService)
        {
            abonentService.FinishDate = new DateTime(1000, 1, 1);

            if (ModelState.IsValid)
            {
                abonentServiceRepository.Insert(abonentService);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = abonentService.AbonentId });
            }

            ViewBag.CompanyId = new SelectList(unitOfWork.ServiceCompanyRepository.Get(), "CompanyId", "Name");
            return View(abonentService);
        }

        public ActionResult TemporaryDisableForAbonent(int abonentId)
        {
            ViewBag.AbonentName = unitOfWork.AbonentRepository.GetById(abonentId).Name;
            ViewBag.AbonentId = abonentId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TemporaryDisableForAbonent(int abonentId, DateTime finishDate, DateTime startDate)
        {
            var abonentServices = abonentServiceRepository.Get()
                .Where(x => x.AbonentId == abonentId)
                .Where(x => x.isOff == false);

            foreach(AbonentService abonentService in abonentServices)
            {
                AbonentService futureAbonentService = new AbonentService()
                {
                    Abonent = abonentService.Abonent,
                    AbonentId = abonentService.AbonentId,
                    FinishDate = DateTime.MinValue,
                    isOff = false,
                    Service = abonentService.Service,
                    ServiceId = abonentService.ServiceId,
                    StartDate = startDate
                };

                abonentService.isOff = true;
                abonentService.FinishDate = finishDate;

                abonentServiceRepository.Insert(futureAbonentService);
                abonentServiceRepository.Update(abonentService);
            }

            unitOfWork.Save();
            return RedirectToAction("Edit","Abonent", new { id = abonentId });
        }

        //
        // GET: /AbonentService/Edit/5

        public ActionResult Edit(int abonentId, int serviceId, DateTime startDate)
        {
            var abonentService = abonentServiceRepository.Get().Where(x => x.AbonentId == abonentId)
                                                                .Where(x => x.ServiceId == serviceId)
                                                                .Where(x => x.StartDate == startDate)
                                                                .FirstOrDefault();
            ViewBag.AbonentId = abonentId;
            ViewBag.ServiceId = serviceId;

            if (abonentService == null)
            {
                return HttpNotFound();
            }

            //Add default abonentService to return View

            return View(abonentService);
        }

  

        //
        // POST: /AbonentService/Edit/5

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
            ViewBag.AbonentId = abonentService.AbonentId;
            ViewBag.ServiceId = abonentService.ServiceId;
            return View(abonentService);
        }


        //
        // GET: /AbonentService/Delete/5

        public ActionResult Remove(int abonentId, int serviceId, DateTime startDate)
        {
            var abonentService = abonentServiceRepository.Get().Where(x => x.AbonentId == abonentId)
                                                    .Where(x => x.ServiceId == serviceId)
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
            abonentService.isOff = true;
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
            var currentAbonentServices = abonentServiceRepository.Get().Where(x => x.AbonentId == abonentId).Where(x => x.isOff == false);
            List<SelectListItem> services = new List<SelectListItem>();

            foreach (var item in unitOfWork.ServiceRepository.Get().Where(x => x.CompanyId == companyId).Where(x => x.isOff == false))
            {
                if (!currentAbonentServices.Where(x=>x.ServiceId == item.ServiceId).Any())
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