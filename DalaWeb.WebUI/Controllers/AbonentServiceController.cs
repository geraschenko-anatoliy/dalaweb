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
            return View(abonentServiceRepository.Get().Include(x => x.Service).Include(x=>x.Abonent).ToList());
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
            ViewBag.AbonentId = abonentId;
            ViewBag.FinishDate = DateTime.MinValue;
            ViewBag.ServiceId = new SelectList(serviceRepository.Get(), "ServiceId", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AbonentService abonentService)
        {
            abonentService.FinishDate = DateTime.MinValue;
            abonentService.StartDate = new DateTime(abonentService.StartDate.Year,
                                                    abonentService.StartDate.Month,
                                                    abonentService.StartDate.Day);
            if (ModelState.IsValid)
            {
                abonentServiceRepository.Insert(abonentService);
                unitOfWork.Save();
                return RedirectToAction("Edit", "Abonent", new { id = abonentService.AbonentId });
            }

            ViewBag.ServiceId = new SelectList(serviceRepository.Get(), "SerivceId", "Сервис");
            return View(abonentService);
        }

        //
        // GET: /AbonentService/Edit/5

        public ActionResult Edit(int abonentId, int serviceId, DateTime startDate)
        {
            var abonentService = abonentServiceRepository.Get().Where(x => x.AbonentId == abonentId)
                                                                .Where(x => x.ServiceId == serviceId)
                                                                .Where(x => x.StartDate == startDate)
                                                                .FirstOrDefault();
            //int[] ids = new int[] { abonentId, serviceId };

            //AbonentService abonentService = abonentServiceRepository.GetById(ids);

            var abonentServices = abonentServiceRepository.Get().Where(x => x.AbonentId == abonentId)
                                                                .Where(x => x.ServiceId == serviceId)
                                                                .ToList();
            if (abonentServices == null)
            {
                return HttpNotFound();
            }

            ViewBag.ServiceId = serviceRepository.Get();

            return View(abonentServices);
        }

        public ActionResult EditSelectedServices(int abonentId)
        {
            //PopulateAssignedAbonentServices(abonentId);

            List<int> selectedServicesIdsList = abonentServiceRepository.Get().Where(x => x.AbonentId == abonentId).Select(x => x.ServiceId).ToList();
            List<SelectListItem> selectedServicesList = new List<SelectListItem>();

            foreach (var item in serviceRepository.Get())
            {
                if (selectedServicesIdsList.Contains(item.ServiceId))
                    selectedServicesList.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.ServiceId.ToString(),
                        Selected = true
                    });
                else
                {
                    selectedServicesList.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.ServiceId.ToString(),
                        Selected = false
                    });
                }
            }

            ViewBag.ServicesList = selectedServicesList;

            return View(unitOfWork.AbonentRepository.GetById(abonentId));
        }

        //private void PopulateAssignedAbonentServices(int abonentId)
        //{
        //    List<SelectListItem> allProducts = new List<SelectListItem>();
        //    List<SelectListItem> selectedProducts = new List<SelectListItem>();

        //    foreach (var item in productRepository.Get())
        //    {
        //        allProducts.Add(new SelectListItem
        //        {
        //            Text = item.Name,
        //            Value = item.ProductId.ToString(),
        //            Selected = false
        //        });
        //    }

        //    foreach (var item in category.Products.Select(c => c.ProductId))
        //    {
        //        Product pr = productRepository.GetById(item);

        //        selectedProducts.Add(new SelectListItem
        //        {
        //            Text = pr.Name,
        //            Value = pr.ProductId.ToString(),
        //            Selected = false
        //        });
        //    }

        //    ViewBag.AllProducts = allProducts;
        //    ViewBag.SelectedProducts = selectedProducts;
        //}

        //
        // POST: /AbonentService/Edit/5

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit()
        //{
        //    //abonent = 
        //    //SetServices(abonent, ServiceIds);

        //    AbonentService abonentService = new AbonentService();

        //    if (ModelState.IsValid)
        //    {
        //        abonentServiceRepository.Update(abonentService);
        //        unitOfWork.Save();
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.ServiceId = new SelectList(serviceRepository.Get(), "ServiceId", "Name");
        //    return View(abonentService);
        //}
            
        
        //public ActionResult Edit(AbonentService abonentService)
        //{
        //    //abonent = 
        //    //SetServices(abonent, ServiceIds);
            
        //    if (ModelState.IsValid)
        //    {
        //        abonentServiceRepository.Update(abonentService);
        //        unitOfWork.Save();
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.ServiceId = new SelectList(serviceRepository.Get(), "ServiceId", "Name");
        //    return View(abonentService);
        //}


        //
        // GET: /AbonentService/Delete/5

        public ActionResult Delete(int abonentId, int serviceId, DateTime startDate)
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

        
         //POST: /AbonentService/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(AbonentService abonentService)
        {
            abonentServiceRepository.Delete(abonentService);
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