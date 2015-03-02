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
    [Authorize]
    public class ServiceCompanyController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<ServiceCompany> serviceCompanyRepository;

        public ServiceCompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            serviceCompanyRepository = unitOfWork.ServiceCompanyRepository;
        }

        public ActionResult Index()
        {
            return View(serviceCompanyRepository.Get());
        }

        public ActionResult Details(int id = 0)
        {
            ServiceCompany serviceCompany = serviceCompanyRepository.GetById(id);
            if (serviceCompany == null)
            {
                return HttpNotFound();
            }
            return View(serviceCompany);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServiceCompany serviceCompany)
        {
            if (ModelState.IsValid)
            {
                serviceCompanyRepository.Insert(serviceCompany);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(serviceCompany);
        }

        public ActionResult Edit(int id = 0)
        {
            ServiceCompany servicecompany = serviceCompanyRepository.GetById(id);
            if (servicecompany == null)
            {
                return HttpNotFound();
            }
            return View(servicecompany);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceCompany serviceCompany)
        {
            if (ModelState.IsValid)
            {
                serviceCompanyRepository.Update(serviceCompany);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(serviceCompany);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}