using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.WebUI.Infrastructure;
using DalaWeb.WebUI.ViewModels.ForStatistic;
using MvcApplication1.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DalaWeb.WebUI.Controllers
{
    [Authorize1(Roles="Administrator") ]
    public class StatisticController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Payment> paymentRepository;

        public StatisticController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            paymentRepository = unitOfWork.PaymentRepository;
        } 

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DailyPayments()
        {
            return View();
        }
        public ActionResult ListOfDebtors()
        {
            DateTime CurrentMonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            List<int> DebtorsIds = new List<int>();
            foreach(var abonentId in paymentRepository.Get().Select(x=>x.AbonentId).Distinct())
            {
                if (paymentRepository.Get()
                    .Where(x => x.AbonentId == abonentId)
                    .Sum(x => x.Sum) < 0)
                    DebtorsIds.Add(abonentId);
            }
            
            List<MalignantDebtor> malignantDebtors = new List<MalignantDebtor>();
            
            foreach(var abonentId in DebtorsIds)
            {
                var paymentForServicesInPastMonth = paymentRepository.Get()
                                                        .Where(x => x.AbonentId == abonentId)
                                                        .Where(x => x.Sum < 0)
                                                        .Where(x => x.Date < CurrentMonthFirstDay)
                                                        .Where(x => x.Date >= CurrentMonthFirstDay.AddMonths(-1))
                                                        .Sum(x => x.Sum);
                var userPaymentForPastPeriod = paymentRepository.Get()
                                                        .Where(x => x.AbonentId == abonentId)
                                                        .Where(x => x.Sum > 0)
                                                        .Where(x => x.Date >= CurrentMonthFirstDay.AddMonths(-1))
                                                        .Sum(x => x.Sum);
                if (userPaymentForPastPeriod < (paymentForServicesInPastMonth *-1) )
                {
                    var malignantDebtor = new MalignantDebtor(unitOfWork.AbonentRepository.GetById(abonentId), paymentForServicesInPastMonth, userPaymentForPastPeriod);
                    malignantDebtors.Add(malignantDebtor);    
                }
            }

            return View(malignantDebtors);
        }

        public PartialViewResult MonthDailyPayments(DateTime dateFrom, DateTime dateTo)
        {
            ViewBag.OverallSum = paymentRepository.Get()
                .Where(x => x.Date >= dateFrom)
                .Where(x => x.Date <= dateTo)
                .Where(x => x.Sum > 0)
                .Sum(x => x.Sum)
                .ToString("F2");
            ViewBag.DateFrom = dateFrom.ToString("dd MMMM yyyy");
            ViewBag.DateTo = dateTo.ToString("dd MMMM yyyy");

            List<DailyPayment> DailyPayments = new List<DailyPayment>();
            while (dateFrom <= dateTo)
            {
                double tempSumm = paymentRepository.Get().Where(x => x.Date == dateFrom).Where(x => x.Sum > 0).Sum(x => x.Sum);
                DailyPayments.Add(new DailyPayment(dateFrom, tempSumm));
                dateFrom = dateFrom.AddDays(1);
            }

            return PartialView(DailyPayments);
        }

        public PartialViewResult MonthServicePayments(DateTime lastDayOfServiceCreditPayments, DateTime lastDayOfAbonentPayments)
        {
            StatisticsCalculator sCalc = new StatisticsCalculator(unitOfWork, lastDayOfServiceCreditPayments, lastDayOfAbonentPayments);
            List<MonthlyPayment> mPayments = sCalc.Calculate();

            return PartialView(mPayments);
        }


    }
}
