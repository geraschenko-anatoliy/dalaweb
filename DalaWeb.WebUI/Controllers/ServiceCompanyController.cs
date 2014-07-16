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

namespace DalaWeb.WebUI.Controllers
{
    public class ServiceCompanyController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<ServiceCompany> serviceCompanyRepository;

        public ServiceCompanyController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            serviceCompanyRepository = unitOfWork.ServiceCompanyRepository;
        }
        //
        // GET: /ServiceCompany/

        public ActionResult Index()
        {
            return View(serviceCompanyRepository.Get());
        }

        //
        // GET: /ServiceCompany/Details/5

        public ActionResult Details(int id = 0)
        {
            ServiceCompany servicecompany = serviceCompanyRepository.GetById(id);
            if (servicecompany == null)
            {
                return HttpNotFound();
            }
            return View(servicecompany);
        }

        //
        // GET: /ServiceCompany/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ServiceCompany/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ServiceCompany servicecompany)
        {
            if (ModelState.IsValid)
            {
                serviceCompanyRepository.Insert(servicecompany);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(servicecompany);
        }

        //
        // GET: /ServiceCompany/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ServiceCompany servicecompany = serviceCompanyRepository.GetById(id);
            if (servicecompany == null)
            {
                return HttpNotFound();
            }
            return View(servicecompany);
        }

        //
        // POST: /ServiceCompany/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ServiceCompany servicecompany)
        {
            if (ModelState.IsValid)
            {
                serviceCompanyRepository.Update(servicecompany);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(servicecompany);
        }

        //
        // GET: /ServiceCompany/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ServiceCompany servicecompany = serviceCompanyRepository.GetById(id);
            if (servicecompany == null)
            {
                return HttpNotFound();
            }
            return View(servicecompany);
        }

        //
        // POST: /ServiceCompany/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            serviceCompanyRepository.Delete(id);
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