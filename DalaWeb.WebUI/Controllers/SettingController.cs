using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities.Settings;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Abstract;

namespace DalaWeb.WebUI.Controllers
{
    public class SettingController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Setting> settingRepository;

        public SettingController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            settingRepository = unitOfWork.SettingRepository;
        } 


        public ActionResult Index()
        {
            return View(settingRepository.Get());
        }

        //
        // GET: /Setting/Details/5

        public ActionResult Details(int id = 0)
        {
            Setting setting = settingRepository.Get().Single();
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        //
        // GET: /Setting/Create

        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /Setting/Create

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Setting setting)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Settings.Add(setting);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(setting);
        //}

        //
        // GET: /Setting/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Setting setting = settingRepository.Get().Single();
            if (setting == null)
            {
                return HttpNotFound();
            }
            return View(setting);
        }

        //
        // POST: /Setting/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Setting setting)
        {
            if (ModelState.IsValid)
            {
                settingRepository.Update(setting);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(setting);
        }

        //
        // GET: /Setting/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    Setting setting = db.Settings.Find(id);
        //    if (setting == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(setting);
        //}

        ////
        //// POST: /Setting/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Setting setting = db.Settings.Find(id);
        //    db.Settings.Remove(setting);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}