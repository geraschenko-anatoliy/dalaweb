using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities.Statistics;
using DalaWeb.Domain.Concrete;
using System.Web.Services.Description;
using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.WebUI.ViewModels.ForStatistic;
using System.Text;
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Credits;

namespace DalaWeb.WebUI.Controllers
{
    [Authorize]
    public class PaymentQueueController : Controller
    {  
        private IUnitOfWork unitOfWork;
        private IRepository<PaymentQueue> paymentQueueRepository;
        private IRepository<Abonent> abonentRepository;
        private IRepository<AbonentService> abonentServiceRepository ;
        private IRepository<AbonentCredit> abonentCreditRepository;

        public PaymentQueueController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            paymentQueueRepository = unitOfWork.PaymentQueueRepository;
            abonentRepository = unitOfWork.AbonentRepository;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
        }

        public ActionResult Index()
        {            
            return View(abonentRepository.Get().Where(x => x.isDeleted != true));
        }

        //
        // GET: /PaymentQueue/Details/5

        public ActionResult Details(int id = 0)
        {
            PaymentQueue paymentqueue = paymentQueueRepository.GetById(id);
            if (paymentqueue == null)
            {
                return HttpNotFound();
            }
            return View(paymentqueue);
        }

        //
        // GET: /PaymentQueue/Create

        public ActionResult Create(int abonentId)
        {
            var activeAbonentServices = abonentRepository.GetById(abonentId).AbonentServices.Where(x=>x.isOff != true);
            var activeAbonentCredits =  abonentRepository.GetById(abonentId).AbonentCredits.Where(x=>x.FullyPaid != true);
            List<PaymentQueueViewModel> paymentQueueViewModelList = new List<PaymentQueueViewModel>();
            foreach (var activeAbonentService in activeAbonentServices)
            {
                paymentQueueViewModelList.Add(new PaymentQueueViewModel(activeAbonentService));
            }
            foreach (var activeAbonentCredit in activeAbonentCredits)
            {
                paymentQueueViewModelList.Add(new PaymentQueueViewModel(activeAbonentCredit));
            }

            ViewBag.abonentId = abonentId;
            ViewBag.abonentName = abonentRepository.GetById(abonentId).Name;
            return View(paymentQueueViewModelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<PaymentQueueViewModel> paymentQueueList, int abonentId, DateTime date)
        {
            paymentQueueList = paymentQueueList.OrderBy(x => x.NumberInQueue).ToList();

            PaymentQueue pq = new PaymentQueue()
            {
                AbonentId = abonentId,
                Date = date,
                Percentage = GetQueuePercetnageOrderString(paymentQueueList),
                Queue = GetQueueServiceOrderString(paymentQueueList)
            };

            paymentQueueRepository.Insert(pq);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        private string GetQueueServiceOrderString(List<PaymentQueueViewModel> paymentQueueList)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var pQL in paymentQueueList)
            {
                if (pQL.isAbonentService)
                {
                    sb.Append("s"+pQL.AbonentServiceOrCreditId + ";");
                }
                else
                {
                    sb.Append("c" + pQL.AbonentServiceOrCreditId + ";");
                }
            }
            return sb.ToString();
        }

        private string GetQueuePercetnageOrderString(List<PaymentQueueViewModel> paymentQueueList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pQL in paymentQueueList)
            {
                sb.Append(pQL.PercentageInCalculation + ";");
            }

            return sb.ToString();
        }

        //
        // GET: /PaymentQueue/Edit/5

        public ActionResult Edit(int paymentQueueId)
        {
            PaymentQueue paymentQueue = paymentQueueRepository.GetById(paymentQueueId);

            if (paymentQueue == null)
            {
                return HttpNotFound();
            }

            List<PaymentQueueViewModel> paymentQueueViewModelList = PaymentQueueViewModel.PaymentQueueViewModelListGenerator(paymentQueue, abonentServiceRepository, abonentCreditRepository);

            ViewBag.abonentId = paymentQueue.AbonentId;
            ViewBag.abonentName = paymentQueue.Abonent.Name;
            ViewBag.paymentQueueId = paymentQueue.PaymentQueueId;

            return View(paymentQueueViewModelList);
        }

        //private List<PaymentQueueViewModel> PaymentQueueViewModelListGenerator(PaymentQueue paymentQueue)
        //{
        //    List<PaymentQueueViewModel> paymentQueueViewModelList = new List<PaymentQueueViewModel>();
        //    var pq = paymentQueue.Queue.Split(';');
        //    var pp = paymentQueue.Percentage.Split(';');

        //    for (int i = 0; i < pq.Length - 1; i++)
        //    {
        //        int temp_pq = int.Parse(pq[i].Substring(1, 1));
        //        int temp_pp = int.Parse(pp[i]);
        //        if (pq[i][0] == 's')
        //        {
        //            paymentQueueViewModelList.Add(new PaymentQueueViewModel(abonentServiceRepository.GetById(temp_pq), i + 1, temp_pp));
        //        }
        //        else
        //        {
        //            paymentQueueViewModelList.Add(new PaymentQueueViewModel(abonentCreditRepository.GetById(temp_pq), i + 1, temp_pp));
        //        }
        //    }

        //    return paymentQueueViewModelList;
        //}

        //
        // POST: /PaymentQueue/Edit/5


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<PaymentQueueViewModel> paymentQueueList, int abonentId, int paymentQueueId, DateTime date)
        {
            paymentQueueList = paymentQueueList.OrderBy(x => x.NumberInQueue).ToList();

            PaymentQueue pq = new PaymentQueue()
            {
                PaymentQueueId = paymentQueueId,
                AbonentId = abonentId,
                Date = date,
                Percentage = GetQueuePercetnageOrderString(paymentQueueList),
                Queue = GetQueueServiceOrderString(paymentQueueList)
            };

            paymentQueueRepository.Update(pq);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(PaymentQueue paymentqueue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        paymentQueueRepository.Update(paymentqueue);
        //        unitOfWork.Save();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.AbonentId = new SelectList(abonentRepository.Get(), "AbonentId", "AbonentNumber", paymentqueue.AbonentId);
        //    return View(paymentqueue);
        //}

        //
        // GET: /PaymentQueue/Delete/5

        public ActionResult Delete(int id = 0)
        {
            PaymentQueue paymentqueue = paymentQueueRepository.GetById(id);
            if (paymentqueue == null)
            {
                return HttpNotFound();
            }
            return View(paymentqueue);
        }

        //
        // POST: /PaymentQueue/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            paymentQueueRepository.Delete(id);
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