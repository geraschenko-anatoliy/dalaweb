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

namespace DalaWeb.WebUI.Controllers
{
    public class ServiceController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Service> serviceRepository;
        private IRepository<ServiceCompany> serviceCompanyRepository;
       

        public ServiceController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            serviceRepository = unitOfWork.ServiceRepository;
            serviceCompanyRepository = unitOfWork.ServiceCompanyRepository;
        }
        //
        // GET: /Service/

        public ActionResult Index()
        {
            var services = serviceRepository.Get().Include(s => s.ServiceCompany).Where(s => s.Archival ==false);
            return View(services.ToList());
        }

        public ActionResult Archive()
        {
            var services = serviceRepository.Get().Include(s => s.ServiceCompany).Where(x => x.Archival == true);
            return View(services.ToList());
        }
        //
        // GET: /Service/Details/5

        public ActionResult Details(int id = 0)
        {
            Service service = serviceRepository.GetById(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        //
        // GET: /Service/Create

        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(serviceCompanyRepository.Get(), "CompanyId", "Name");
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Text = "По абоненту", Value = "1", Selected = false });
            items.Add(new SelectListItem() { Text = "По проживающим", Value = "2", Selected = false });
            items.Add(new SelectListItem() { Text = "По счетчику", Value = "3", Selected = false });
            ViewBag.Type = new SelectList(items, "Value", "Text");
            return View();
        }

        //
        // POST: /Service/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                serviceRepository.Insert(service);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(serviceCompanyRepository.Get(), "CompanyId", "Name", service.CompanyId);
            
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "По абоненту", Value = "1", Selected = false });
            items.Add(new SelectListItem() { Text = "По проживающим", Value = "2", Selected = false });
            items.Add(new SelectListItem() { Text = "По счетчику", Value = "3", Selected = false });

            ViewBag.Type = new SelectList(items, "Type", "Тип услуги", service.Type);
            return View(service);
        }

        //
        // GET: /Service/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Service service = serviceRepository.GetById(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(serviceCompanyRepository.Get(), "CompanyId", "Name", service.CompanyId);
            return View(service);
        }

        //
        // POST: /Service/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                serviceRepository.Update(service);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(serviceCompanyRepository.Get(), "CompanyId", "Name", service.CompanyId);
            return View(service);
        }

        //
        // GET: /Service/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Service service = serviceRepository.GetById(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        //
        // POST: /Service/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Service service)
        {
            if (ModelState.IsValid)
            {
                service.Archival = true;
                serviceRepository.Update(service);
                //unitOfWork.Save();

                var abonentServices = unitOfWork.AbonentServiceRepository.Get().Where(x => x.Service.ServiceId == service.ServiceId).Where(x=> x.isOff == false);
                foreach (var abonentService in abonentServices)
                {
                    abonentService.isOff = true;
                    unitOfWork.AbonentServiceRepository.Update(abonentService);
                }

                unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(service);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}