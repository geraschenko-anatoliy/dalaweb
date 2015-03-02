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
using DalaWeb.Domain.Entities.Payments;

namespace DalaWeb.WebUI.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Service> serviceRepository;
        private IRepository<ServiceCompany> serviceCompanyRepository;
        private IRepository<AbonentService> abonentServiceRepository;
        private IRepository<Payment> paymentRepository;

        public ServiceController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            serviceRepository = unitOfWork.ServiceRepository;
            serviceCompanyRepository = unitOfWork.ServiceCompanyRepository;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            paymentRepository = unitOfWork.PaymentRepository;
        }
        
        public ActionResult Index()
        {
            var services = serviceRepository.Get()
                .Where(s => s.isOff == false);
            return View(services.ToList());
        }

        public ActionResult Archive()
        {
            var services = serviceRepository.Get().Where(x => x.isOff == true);
            return View(services.ToList());
        }

        public ActionResult Details(int id = 0)
        {
            Service service = serviceRepository.GetById(id);

            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        public ActionResult Create()
        {
            ViewBag.ServiceCompanyId = new SelectList(serviceCompanyRepository.Get(), "ServiceCompanyId", "Name");
            ViewBag.Type = getServiceTypes(null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Service service, double Price)
        {
            if (ModelState.IsValid)
            {
                serviceRepository.Insert(service);
                unitOfWork.Save();
                ServicePrice sp = new ServicePrice()
                {
                    StartDate = DateTime.MinValue,
                    ServiceId = service.ServiceId,
                    Price = Price
                };
                unitOfWork.ServicePriceRepository.Insert(sp);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.СompanyId = new SelectList(serviceCompanyRepository.Get(), "ServiceCompanyId", "Name", service.ServiceCompanyId);
            ViewBag.Type = getServiceTypes(service);
            return View(service);
        }

        public ActionResult Edit(int id = 0)
        {
            Service service = serviceRepository.Get().Where(x => x.ServiceId == id).Single();
 
            if (service.isOff)
                return RedirectToAction("Archive");
            if (service == null)
            {
                return HttpNotFound();
            }
            ViewBag.сompanyId = new SelectList(serviceCompanyRepository.Get(), "ServiceCompanyId", "Name", service.ServiceCompanyId);
            ViewBag.price = service.ServicePrice.OrderBy(x => x.StartDate).Last().Price.ToString();
            ViewBag.Type = getServiceTypes(service);

            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Service service, double Price, DateTime FinishDate)
        {
            if (service.Type == 0)
                ModelState.AddModelError("Type", "Выберите тип услуги");

            if (ModelState.IsValid)
            {
                var oldPrice = unitOfWork.ServicePriceRepository.Get().Where(x => x.ServiceId == service.ServiceId).Last();
                if (oldPrice.Price != Price)
                {
                    ServicePrice servicePrice = new ServicePrice()
                    {
                        Price = Price,
                        StartDate = FinishDate.AddDays(1),
                        Service = service
                    };
                    oldPrice.FinishDate = FinishDate;
                    unitOfWork.ServicePriceRepository.Update(oldPrice);
                    unitOfWork.ServicePriceRepository.Insert(servicePrice);
                }
                else
                {
                    serviceRepository.Update(service);
                }

                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            
            ViewBag.сompanyId = new SelectList(serviceCompanyRepository.Get(), "ServiceCompanyId", "Name", service.ServiceCompanyId);
            ViewBag.price = service.ServicePrice.OrderBy(x => x.StartDate).Last().Price.ToString();
            ViewBag.Type = getServiceTypes(service);
            return View(service);
        }
        public ActionResult Delete(int id = 0)
        {
            Service service = serviceRepository.GetById(id);
            if (service.isOff)
                return RedirectToAction("Archive");
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Service service, DateTime FinishDate)
        {
            if (ModelState.IsValid)
            {
                service.isOff = true;
                serviceRepository.Update(service);

                var abonentServices = abonentServiceRepository.Get().Where(x => x.Service.ServiceId == service.ServiceId).Where(x=> x.isOff == false);
                foreach (var abonentService in abonentServices)
                {
                    abonentService.FinishDate = FinishDate;
                    unitOfWork.AbonentServiceRepository.Update(abonentService);
                }

                unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(service);
        }

        private SelectList getServiceTypes(Service service)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "По абоненту", Value = "1", Selected = false });
            items.Add(new SelectListItem() { Text = "По проживающим", Value = "2", Selected = false });
            items.Add(new SelectListItem() { Text = "По счетчику", Value = "3", Selected = false });
            SelectList result = (service == null) ? new SelectList(items, "Value", "Text") : new SelectList(items, "Value", "Text", service.Type);
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}