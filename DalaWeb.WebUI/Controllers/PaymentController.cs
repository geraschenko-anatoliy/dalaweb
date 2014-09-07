    using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Counters;
using System.Globalization;
using DalaWeb.WebUI.ViewModels.ForPayment;

namespace DalaWeb.WebUI.Controllers
{
    public class PaymentController : Controller
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Payment> paymentRepository;
        private IRepository<Abonent> abonentRepository;
        private IRepository<AbonentCredit> abonentCreditRepository;
        private IRepository<AbonentService> abonentServiceRepository;
        private IRepository<Counter> counterRepository;
        private IRepository<CounterValues> counterValuesRepository;
        
        public PaymentController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            paymentRepository = unitOfWork.PaymentRepository;
            abonentRepository = unitOfWork.AbonentRepository;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            counterRepository = unitOfWork.CounterRepository;
            counterValuesRepository = unitOfWork.CounterValuesRepository;
        }

        public ActionResult Index()
        {
            ViewBag.AbonentNumber = AddAllToList(abonentRepository.Get().Select(x => x.AbonentNumber));
            ViewBag.Year = AddAllToList(paymentRepository.Get().Select(x => x.Date.Year.ToString()).Distinct());
            ViewBag.Month = AddMonthToList(paymentRepository.Get().Select(x => x.Date).Distinct());
            ViewBag.PaymentType = AddAllToList(paymentRepository.Get().Select(x => x.Type).Distinct());

            var payments = paymentRepository.Get().Include(p => p.Abonent);

            return View(payments);
        }

        public ActionResult Month()
        {
            ViewBag.AbonentNumber = AddAllToList(abonentRepository.Get().Select(x => x.AbonentNumber));
            ViewBag.Year = AddAllToList(paymentRepository.Get().Select(x => x.Date.Year.ToString()).Distinct());
            ViewBag.Month = AddMonthToList(paymentRepository.Get().Select(x => x.Date).Distinct());
            ViewBag.PaymentType = AddAllToList(paymentRepository.Get().Select(x => x.Type).Distinct());
            return View(new IndexViewModel(paymentRepository.Get()));
        }

        SelectList AddMonthToList(IQueryable<DateTime> dateTimes)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            SelectListItem all = new SelectListItem()
            {
                Text = "Все",
                Value = "0"
            };

            result.Add(all);

            foreach (var item in dateTimes.ToList())
            {
                SelectListItem sItem = new SelectListItem()
                {
                    Text = item.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru-RU")),
                    Value = item.Month.ToString()
                };
                if (!result.Where(x=>x.Text == sItem.Text).Any())
                    result.Add(sItem);
            }
            return new SelectList(result, "Value", "Text");
        }

        SelectList AddAllToList(IQueryable<string> IQueryable)
        {
            List<string> selectListItems = new List<string>();
            selectListItems.Add("Все");
            foreach (var item in IQueryable.ToList())
            {
                selectListItems.Add(item);
            }
            return new SelectList (selectListItems);
        }

        public ActionResult ForAbonent(int abonentId, int year, string month)
        {
            var monthPayments =  paymentRepository.Get().Where(x => x.AbonentId == abonentId)
                                                        .Where(x => x.Date.ToString("MMMM", System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU")) == month)
                                                        .Where(x => x.Date.Year == year);
            return View(monthPayments);
        }

        public PartialViewResult All(string AbonentNumber, string Year, int? Month, string PaymentType)
        {
            IQueryable<Payment> payments = paymentRepository.Get().Include(x=>x.Abonent);

            if (!string.IsNullOrEmpty(AbonentNumber) && AbonentNumber != "Все")
                payments = payments.Where(x=>x.Abonent.AbonentNumber == AbonentNumber);
            if (!string.IsNullOrEmpty(Year) && Year != "Все")
                payments = payments.Where(x => x.Date.Year == TryToParseInt(Year));
            if (Month.HasValue && Month != 0)
                payments = payments.Where(x => x.Date.Month == Month.Value);
            if (!string.IsNullOrEmpty(PaymentType) && PaymentType!= "Все")
                payments = payments.Where(x => x.Type == PaymentType);


            return PartialView(payments);
        }

        public PartialViewResult Filter(string AbonentNumber, string Year, int? Month, string PaymentType)
        {
            IQueryable<Payment> payments = paymentRepository.Get().Include(x => x.Abonent);

            if (!string.IsNullOrEmpty(AbonentNumber) && AbonentNumber != "Все")
                payments = payments.Where(x => x.Abonent.AbonentNumber == AbonentNumber);
            if (!string.IsNullOrEmpty(Year) && Year != "Все")
                payments = payments.Where(x => x.Date.Year == TryToParseInt(Year));
            if (Month.HasValue && Month != 0)
                payments = payments.Where(x => x.Date.Month == Month.Value);
            if (!string.IsNullOrEmpty(PaymentType) && PaymentType != "Все")
                payments = payments.Where(x => x.Type == PaymentType);

            return PartialView(new IndexViewModel(payments));
        }


        private static int TryToParseInt(string value)
        {
            int number;
            bool result = Int32.TryParse(value, out number);
            if (result)
            {
                return number;
            }
            else
            {
                if (value == null)
                    return 0;
                else
                    return -1;
            }
        }

        public ActionResult Details(int id = 0)
        {
            Payment payment = paymentRepository.GetById(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }


        public ActionResult Create()
        {
            ViewBag.PaymentTypes = new SelectList(paymentRepository.Get().Select(x => x.Type).Distinct());
            return View();
        }

   
        private void CheckPaymentAbility(Abonent abonent, DateTime date)
        {
            if (!paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Any())
            {
                Payment initialPayment = new Payment()
                {
                    AbonentId = abonent.AbonentId,
                    Balance = 0,
                    Date = date,
                    Sum = 0,
                    Comment = "Стартовое значение для каждого пользователя",
                    Type = "Пополнение"
                };
                paymentRepository.Insert(initialPayment);
                unitOfWork.Save();
            }
        }

        private bool CheckOpenMonthAbility(DateTime date)
        {
            if (paymentRepository.Get().Where(x => x.Date.Month == date.Month)
                                       .Where(x => x.Type == "Платеж")
                                       .Any())
                return true;
            return false;
        }

        public void CalculateCreditPayment(Abonent abonent, AbonentCredit aCredit, DateTime date)
        {
            CheckPaymentAbility(abonent, date);
            Payment creditPayment = new Payment()
                    {
                        AbonentId = abonent.AbonentId,
                        Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Last().Balance - aCredit.PaymentForMonth,
                        Date = date,
                        Sum = aCredit.PaymentForMonth,
                        Comment = "Платеж по кредиту " + aCredit.Credit.Name + " за " + (aCredit.PaidMonths + 1).ToString() + " месяц",
                        Type = "Платеж"
                    };

            aCredit.PaidMonths += 1;
            aCredit.PaidForTheEntirePeriod += aCredit.PaymentForMonth;
            if (aCredit.PaidMonths == aCredit.Term)
                aCredit.FullyPaid = true;

            abonentCreditRepository.Update(aCredit);
            paymentRepository.Insert(creditPayment);
            unitOfWork.Save();
        }
        public void CalculateFirstTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date)
        {
            CheckPaymentAbility(abonent, date);

            Payment servicePayment = new Payment()
            {
                AbonentId = abonent.AbonentId,
                Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Last().Balance - aService.Service.Price,
                Date = date, 
                Sum = aService.Service.Price,
                Comment = "Платеж по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц",
                Type = "Платеж"
            };
            paymentRepository.Insert(servicePayment);
            unitOfWork.Save();
        }
        public void CalculateSecondTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date)
        {
            CheckPaymentAbility(abonent, date);
            Payment servicePayment = new Payment()
            {
                AbonentId = abonent.AbonentId,
                Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Last().Balance - aService.Service.Price * abonent.NumberOfInhabitants,
                Date = date,
                Sum = aService.Service.Price * abonent.NumberOfInhabitants,
                Comment = "Платеж по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц",
                Type = "Платеж"
            };
            paymentRepository.Insert(servicePayment);
            unitOfWork.Save();
        }
        public void CalculateThirdTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date, List<string> warnings)
        {
            CheckPaymentAbility(abonent, date);

            IQueryable<Counter> counters = counterRepository.Get().Where(x => x.AbonentId == abonent.AbonentId)
                                                     .Where(x => x.ServiceId == aService.ServiceId)
                                                     .Where(x => x.Service.isOff == false)
                                                     .Include(x=>x.CounterValues);
            if (counters.Count() > 1)
                warnings.Add("У абонента" + abonent.AbonentNumber + " " + abonent.Name + " обнаружено " + counters.Count() + " счетчиков по услуге " + aService.Service.Name);

            Counter aCounter = counters.Last();
            aCounter.CounterValues = counterValuesRepository.Get().Where(x => x.CounterId == aCounter.CounterId).ToList();
            if (aCounter.CounterValues.Last().Date.Month != date.Month-1)
                warnings.Add("У абонента" + abonent.AbonentNumber + " " + abonent.Name + " не обнаружено данных по счетчику" + aCounter.Name + " по услуге " + aService.Service.Name);
            
            double valueForPayment;
            
            if (aCounter.CounterValues.Count<2)
            {
                valueForPayment = aCounter.CounterValues.ToList()[aCounter.CounterValues.Count - 1].Value - aCounter.InitialValue;
            }
            else
            {
                valueForPayment = aCounter.CounterValues.ToList()[aCounter.CounterValues.Count - 1].Value - aCounter.CounterValues.ToList()[aCounter.CounterValues.Count - 2].Value; 
            }

            Payment servicePayment = new Payment()
            {
                AbonentId = abonent.AbonentId,
                Balance = paymentRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Last().Balance - aService.Service.Price * valueForPayment,
                Date = date,
                Sum = aService.Service.Price * valueForPayment,
                Comment = "Платеж по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц",
                Type = "Платеж"
            };
            paymentRepository.Insert(servicePayment);
            unitOfWork.Save();
        }
           
        public ActionResult OpenMonth()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OpenMonth(DateTime date)
        {
            if (CheckOpenMonthAbility(date))
            {
                ModelState.AddModelError("date", "Этот месяц уже открыт");
                return View();
            }
            List<string> warnings = new List<string>();
            foreach (Abonent abonent in abonentRepository.Get().Where(x => x.isDeleted != true))
            { 
                foreach(AbonentCredit aCredit in abonentCreditRepository.Get().Where(x=>x.AbonentId == abonent.AbonentId)
                                                                              .Where(x=>x.FullyPaid != true)
                                                                              .Include(x=>x.Credit))
                {
                    CalculateCreditPayment(abonent, aCredit, date);
                }

                foreach (AbonentService aService in abonentServiceRepository.Get().Where(x => x.AbonentId == abonent.AbonentId)
                                                                                  .Where(x => x.isOff == false)
                                                                                  .Include(x=>x.Service))
                {
                    switch (aService.Service.Type)
                    {
                        case 1:
                            {
                                CalculateFirstTypeServicePayment(abonent, aService, date);
                                break;
                            }
                        case 2:
                            {
                                CalculateSecondTypeServicePayment(abonent, aService, date);
                                break;
                            }
                        case 3:
                            {
                                CalculateThirdTypeServicePayment(abonent, aService, date, warnings);
                                break;
                            }
                    }

                }
                unitOfWork.Save();                                                                              
            }
            return RedirectToAction("Index");
        }

        //
        // POST: /Payment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Payment payment, string AbonentNumber)
        {
            if (!abonentRepository.Get().Where(x => x.AbonentNumber == AbonentNumber).Any())
                ModelState.AddModelError("AbonentNumber", "Абонент не найден");
            else
            {
                payment.AbonentId = abonentRepository.Get().Where(x => x.AbonentNumber == AbonentNumber)
                                                           .Where(x => x.isDeleted == false)
                                                           .Select(x => x.AbonentId)
                                                           .LastOrDefault();
                double balance  = paymentRepository.Get().Where(x=>x.AbonentId == payment.AbonentId)
                                                      .Select(x=>x.Balance)
                                                      .LastOrDefault();
                if (payment.Type == "Платеж")
                    balance -= payment.Sum;
                else
                    balance += payment.Sum;
                payment.Balance = balance;
            }
            if (ModelState.IsValid)
            {
                paymentRepository.Insert(payment);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            ViewBag.PaymentTypes = new SelectList(paymentRepository.Get().Select(x => x.Type).Distinct());
            return View(payment);
        }

        public PartialViewResult Recalculation(int? abonentId)
        {
            double balance = 0;
            foreach (var payment in paymentRepository.Get().Where(x=>x.AbonentId == abonentId))
            {
                if (payment.Type == "Платеж")
                {
                    payment.Balance = balance - payment.Sum;
                    balance = payment.Balance;
                }
                else
                {
                    payment.Balance = balance + payment.Sum;
                    balance = payment.Balance;
                }        
            }
            unitOfWork.Save();
            return PartialView(balance);
        }

        public ActionResult Numerous()
        {
            var abonents = abonentRepository.Get().Where(x => x.isDeleted == false)
                                                  .Include(x => x.Payments)
                                                  .ToList();

            ViewBag.Abonents = abonents;
            List<NumerousViewModel> abonentModels = new List<NumerousViewModel>();

            foreach (var item in abonents)
            { 
                abonentModels.Add(new NumerousViewModel(item.AbonentNumber, item.Name, item.AbonentId, item.Payments.Any()? item.Payments.LastOrDefault().Balance : 0));
            }

            return View(abonentModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Numerous(List<NumerousViewModel> Numerous, DateTime date)
        {
            foreach (var item in Numerous)
            {
                if (item.Sum != 0)
                {
                    Payment payment = new Payment()
                    {
                        AbonentId = item.AbonentId,
                        Balance = item.Balance += item.Sum,
                        Comment = "Пополнение счета за " + date.ToString("D", CultureInfo.CreateSpecificCulture("ru-RU")),
                        Type = "Пополнение",
                        Date = date,
                        Sum = item.Sum
                    };
                    paymentRepository.Insert(payment);
                }
            }
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id = 0)
        {
            Payment payment = paymentRepository.GetById(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        ////
        //// POST: /Payment/Delete/5
        public JsonResult AutoCompleteAbonentNumber(string term)
        {
            var result = (from r in abonentRepository.Get()
                          where r.AbonentNumber.ToLower().Contains(term.ToLower())
                          select new { r.AbonentNumber }).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            paymentRepository.Delete(id);
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