using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Counters;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Entities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DalaWeb.WebUI.Infrastructure
{      
    public struct CounterProperties
        {
            public double CurrentValue { get; set; }
            public double PreviousValue { get; set; }
            public double CounterValueForPayment { get; set; }
            public double SummForPayment { get; set; }
        }
    public sealed class MonthCalculator
    {
        private IUnitOfWork unitOfWork;
        private List<string> warnings;
        private IRepository<Payment> paymentRepository;
        private IRepository<Abonent> abonentRepository;
        private IRepository<AbonentCredit> abonentCreditRepository;
        private IRepository<AbonentService> abonentServiceRepository;
        private IRepository<Counter> counterRepository;
        private IRepository<CounterValues> counterValuesRepository;

        public MonthCalculator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            warnings = new List<string>();
            paymentRepository = unitOfWork.PaymentRepository;
            abonentRepository = unitOfWork.AbonentRepository;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            counterRepository = unitOfWork.CounterRepository;
            counterValuesRepository = unitOfWork.CounterValuesRepository;
        }
        public static List<string> isPossibleToOpenMonths(IRepository<Payment> paymentRepository, IRepository<Abonent> abonentRepository)
        {
            List<string> result = new List<string>();

            if (abonentRepository.Get()
                .Where(x => x.isDeleted == false)
                .Where(x => (string.IsNullOrWhiteSpace(x.AbonentNumber)))
                .Any())
            {
                result.Add("Существуют абоненты без номера. Открытие месяца невозможно");
                return result;
            }

            var abonentsThatHasPayment = paymentRepository.Get()
                                            .Select(x => x.Abonent.AbonentNumber)
                                            .Distinct();
            var allAbonents = abonentRepository.Get().Where(x=>x.isDeleted == false);
            var abonentsWithOutPayment = from abonents in allAbonents
                                         where (!abonentsThatHasPayment.Contains(abonents.AbonentNumber))
                                         select abonents.AbonentNumber;

            if ((!String.IsNullOrEmpty(abonentsWithOutPayment.FirstOrDefault()) && abonentsWithOutPayment.Any()))
            {
                string error = "Данные пользователи ";
                foreach (var abonent in abonentsWithOutPayment)
                {
                    error += abonent + ", ";
                }
                error = error.Insert(error.Length - 2, " не имеют платежей в базе. Для открытия месяца у каждого пользователя должен быть хотя бы один платеж");
                result.Add(error);
            }

            return result;
        }
        public List<string> CalculateAllAbonents(DateTime date)
        {
            foreach (var abonent in abonentRepository.Get().Where(x => x.isDeleted == false))
            {
                CalculateAbonent(abonent, date);
            }

            return warnings;
        }
        public void CalculateAbonent(Abonent abonent, DateTime date)
        {
            foreach (AbonentCredit credit in abonent.AbonentCredits.Where(x => x.FullyPaid != true))
            {
                CalculateCreditPayment(abonent, credit, date);
            }

            foreach (AbonentService service in abonent.AbonentServices.Where(x => x.isOff == false || (x.isOff == true && x.FinishDate > date))
                                                                      .Where(x => x.StartDate <= date))
            {
                switch (service.Service.Type)
                {
                    case 1:
                        {
                            CalculateFirstTypeServicePayment(abonent, service, date);
                            break;
                        }
                    case 2:
                        {
                            CalculateSecondTypeServicePayment(abonent, service, date);
                            break;
                        }
                    case 3:
                        {
                            CalculateThirdTypeServicePayment(abonent, service, date);
                            break;
                        }
                }
            }

            unitOfWork.Save();
        }
        private void CalculateCreditPayment(Abonent abonent, AbonentCredit aCredit, DateTime date)
        {
            Payment creditPayment = new Payment()
                    {
                        AbonentId = abonent.AbonentId,
                        Date = date,
                        AbonentCredit = aCredit,
                        Sum = 0 - aCredit.PaymentForMonth,
                        Comment = "Списание по кредиту " + aCredit.Credit.Name + " за " + (aCredit.PaidMonths + 1).ToString() + " месяц"
                    };

            aCredit.PaidMonths += 1;
            aCredit.PaidForTheEntirePeriod += aCredit.PaymentForMonth > 0 ? aCredit.PaymentForMonth : 0;
            if (aCredit.PaidMonths == aCredit.Term)
                aCredit.FullyPaid = true;

            abonentCreditRepository.Update(aCredit);
            paymentRepository.Insert(creditPayment);
            unitOfWork.Save();
        }
        private void CalculateFirstTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date)
        {
            Payment servicePayment = new Payment()
            {
                AbonentId = abonent.AbonentId,
                AbonentServiceId = aService.AbonentServiceId,
                Date = date,
                Sum = 0 - aService.Service.ServicePrice.Where(x => x.StartDate < date).OrderBy(x => x.StartDate).Last().Price,
                Comment = "Списание по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц"
            };
            paymentRepository.Insert(servicePayment);
            unitOfWork.Save();
        }
        private void CalculateSecondTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date)
        {
            Payment servicePayment = new Payment()
            {
                AbonentId = abonent.AbonentId,
                AbonentServiceId = aService.AbonentServiceId,
                Date = date,
                Sum = 0 - aService.Service.ServicePrice.Where(x => x.StartDate < date).OrderBy(x => x.StartDate).Last().Price * abonent.NumberOfInhabitants,
                Comment = "Списание по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц"
            };
            paymentRepository.Insert(servicePayment);
            unitOfWork.Save();
        }


        private void CalculateThirdTypeServicePayment(Abonent abonent, AbonentService aService, DateTime date)
        {
            var counters = counterRepository.Get()
                .Where(x => x.Abonent.AbonentId == abonent.AbonentId)
                .Where(x => x.Service.ServiceId == aService.Service.ServiceId)
                .Where(x => ((x.isOff != true && x.StartDate <= date) || (x.isOff == true && x.FinishDate > date)));

            if (counters.Count() == 0 || counters.Count() > 1)
            {
                warnings.Add("У абонента" + abonent.AbonentNumber + " " + abonent.Name + " обнаружено " + counters.Count() + " активных счетчиков по услуге " + aService.Service.Name);
                return;
            }

            Counter aCounter = counters.Single();

            CounterProperties counterProperties = new CounterProperties();

            if (aCounter.CounterValues.Count < 2)
            {
                warnings.Add("У абонента " + abonent.AbonentNumber + " " + abonent.Name + " не обнаружено данных по счетчику " + aCounter.Name + " по услуге " + aService.Service.Name);
                return;
            }
            else
            {
                counterProperties.CurrentValue = aCounter.CounterValues.Where(x => x.Date <= date).OrderBy(x => x.Date).LastOrDefault().Value;
                counterProperties.PreviousValue = aCounter.CounterValues.ToList()[aCounter.CounterValues.Count - 2].Value;
            }

            counterProperties.CounterValueForPayment = counterProperties.CurrentValue - counterProperties.PreviousValue;
            counterProperties.SummForPayment = 0;

            int minimalLimit = 0;
            if (aService.Service.Tariffs.Any())
                minimalLimit = aService.Service.Tariffs.Min(x => x.LimitValue);

            if (aService.Service.Tariffs.Any() && (minimalLimit < counterProperties.CounterValueForPayment))
            {
                var tariffs = aService.Service.Tariffs.OrderByDescending(x => x.LimitValue);
                foreach (var tarif in tariffs)
                {
                    if (counterProperties.CounterValueForPayment > tarif.LimitValue)
                    {
                        counterProperties.SummForPayment += (counterProperties.CounterValueForPayment - tarif.LimitValue) * tarif.OverPrice;
                        counterProperties.CounterValueForPayment = tarif.LimitValue;
                    }
                }
                counterProperties.SummForPayment += tariffs.Last().LimitValue * aService.Service.ServicePrice.Where(x => x.StartDate < date).OrderBy(x => x.StartDate).Last().Price;
            }
            else
            {
                counterProperties.SummForPayment = counterProperties.CounterValueForPayment * aService.Service.ServicePrice.Where(x => x.StartDate < date).OrderBy(x => x.StartDate).Last().Price;
            }

            Payment servicePayment = new Payment()
                {
                    AbonentId = abonent.AbonentId,
                    AbonentServiceId = aService.AbonentServiceId,
                    Date = date,
                    Sum = 0 - counterProperties.SummForPayment,
                    Comment = "Списание по сервису " + aService.Service.Name + " за " + date.Month.ToString() + " месяц"
                };

            paymentRepository.Insert(servicePayment);
            unitOfWork.Save();
            return;
        }

        public List<string> ReCalculateAbonent(Abonent abonent, DateTime date)
        {
            DeleteMonthPayments(abonent, date);
            CalculateAbonent(abonent, date);
            return warnings;
        }

        private void DeleteMonthPayments(Abonent abonent, DateTime date)
        {
            var monthPayments = paymentRepository.Get()
                                    .Where(x => x.Abonent.AbonentId == abonent.AbonentId)
                                    .Where(x => x.Date == date)
                                    .Where(x => (x.AbonentCredit != null || x.AbonentService != null))
                                    .Where(x => x.Sum <= 0);

            var creditPayments = monthPayments.Where(x => x.AbonentCredit != null).Select(x=>x.AbonentCredit);

            if (creditPayments.Any())
            {
                foreach (var credit in creditPayments)
                {
                    AbonentCredit aCredit = credit; 
                    aCredit.PaidForTheEntirePeriod -= aCredit.PaymentForMonth;
                    aCredit.PaidMonths -= 1;
                    if (aCredit.FullyPaid)
                        aCredit.FullyPaid = false;
                    abonentCreditRepository.Update(aCredit);
                }
            }

            foreach (var payment in monthPayments)
            {
                paymentRepository.Delete(payment);
            }

            unitOfWork.Save();

        }
    }
}