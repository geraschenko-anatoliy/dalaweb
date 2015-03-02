using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Counters;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Payments;
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Statistics;
using DalaWeb.WebUI.ViewModels.ForStatistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.Infrastructure
{
    public sealed class StatisticsCalculator
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Payment> paymentRepository;
        private IRepository<Abonent> abonentRepository;
        private IRepository<AbonentCredit> abonentCreditRepository;
        private IRepository<AbonentService> abonentServiceRepository;
        private IRepository<PaymentQueue> paymentQueueRepository;
        private IRepository<Credit> creditRepository;
        private IRepository<Counter> counterRepository;
        private IRepository<Service> serviceRepository;
        DateTime lastDayOfServiceCreditPayments;
        DateTime lastDayOfAbonentPayments;

        public StatisticsCalculator(IUnitOfWork unitOfWork, DateTime lastDayOfServiceCreditPayments, DateTime lastDayOfAbonentPayments)
        {
            this.unitOfWork = unitOfWork;
            this.lastDayOfServiceCreditPayments = lastDayOfServiceCreditPayments;
            this.lastDayOfAbonentPayments = lastDayOfAbonentPayments;
            paymentRepository = unitOfWork.PaymentRepository;
            abonentRepository = unitOfWork.AbonentRepository;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            counterRepository = unitOfWork.CounterRepository;
            serviceRepository = unitOfWork.ServiceRepository;
            creditRepository = unitOfWork.CreditRepository;
            paymentQueueRepository = unitOfWork.PaymentQueueRepository;
        }

        public List<MonthlyPayment> Calculate()
        {
            var serviceIds = abonentServiceRepository.Get().Where(x=>x.isOff != true).Select(x=>x.Service.ServiceId).Distinct();
            Dictionary<int, double> servicesPaids = new Dictionary<int, double>();
            Dictionary<int, double> servicesPayments = new Dictionary<int, double>(); 

            foreach (var serviceId in serviceIds)
            {
                servicesPaids.Add(serviceId, 0);
                servicesPayments.Add(serviceId, 0);
            }

            var creditsIds = abonentCreditRepository.Get().Where(x => x.FullyPaid != true).Select(x => x.Credit.CreditId).Distinct();
            Dictionary<int, double> creditsPaids = new Dictionary<int, double>();
            Dictionary<int, double> creditsPayments = new Dictionary<int, double>();

            foreach (var creditId in creditsIds)
            {
                creditsPaids.Add(creditId, 0);
                creditsPayments.Add(creditId, 0);
            }

            foreach(var abonent in abonentRepository.Get().Where(x=>x.isDeleted != true))
            {
                CalculateAbonent(abonent, servicesPaids, servicesPayments, creditsPaids, creditsPayments);
            }

            List<MonthlyPayment> monthlyPaymentList = new List<MonthlyPayment>();
            foreach (var item in servicesPaids)
            {
                MonthlyPayment mPayment = new MonthlyPayment(serviceRepository.GetById(item.Key).Name, item.Value, servicesPayments[item.Key]);
                monthlyPaymentList.Add(mPayment);    
            }

            foreach (var item in creditsPaids)
            {
                MonthlyPayment mPayment = new MonthlyPayment(creditRepository.GetById(item.Key).Name, item.Value, creditsPayments[item.Key]);
                monthlyPaymentList.Add(mPayment);
            }

            return monthlyPaymentList;
        }

        public void CalculateAbonent(Abonent abonent, Dictionary<int, double> servicesPaids, Dictionary<int, double> servicesPayments, Dictionary<int, double> creditsPaids, Dictionary<int, double> creditsPayments)
        {
            DateTime firstDayInCurrentMonth = new DateTime(lastDayOfServiceCreditPayments.Year, lastDayOfServiceCreditPayments.Month, 1);
            double abonentBalanceForFirstDayInMonth = abonent.Payments.Where(x => x.Date < firstDayInCurrentMonth).Sum(x => x.Sum);
            double abonentPaymentsInCurrentMonth = abonent.Payments.Where(x => x.Date > firstDayInCurrentMonth).Where(x => x.Sum > 0).Sum(x => x.Sum);
            
            double abonentClearPaymentInCurrentMonth = abonentBalanceForFirstDayInMonth + abonentPaymentsInCurrentMonth;

            PaymentQueue pq = paymentQueueRepository.Get().Where(x => x.AbonentId == abonent.AbonentId).Last();
 
            List<PaymentQueueViewModel> paymentQueueViewModelList = PaymentQueueViewModel.PaymentQueueViewModelListGenerator(pq, abonentServiceRepository, abonentCreditRepository);

            var currentMonthPayments = paymentRepository.Get()
                                .Where(x => x.Abonent.AbonentId == abonent.AbonentId)
                                .Where(x => x.Date >= firstDayInCurrentMonth)
                                .Where(x => x.Date <= lastDayOfServiceCreditPayments)
                                .Where(x =>x.Sum < 0);
            
            foreach(var paymentQueue in paymentQueueViewModelList)
            {
                if (paymentQueue.isAbonentService)
                {
                    var valueToPay = (-1) * currentMonthPayments
                        .Where(x => x.AbonentServiceId == paymentQueue.AbonentServiceOrCreditId)
                        .Sum(x => x.Sum);

                    servicesPayments[abonentServiceRepository.GetById(paymentQueue.AbonentServiceOrCreditId).ServiceId] += valueToPay;

                    if (abonentClearPaymentInCurrentMonth > valueToPay)
                    {
                        abonentClearPaymentInCurrentMonth -= valueToPay;
                        servicesPaids[abonentServiceRepository.GetById(paymentQueue.AbonentServiceOrCreditId).ServiceId] += valueToPay;
                    }
                    else
                    {
                        servicesPaids[abonentServiceRepository.GetById(paymentQueue.AbonentServiceOrCreditId).ServiceId] += abonentClearPaymentInCurrentMonth;
                        abonentClearPaymentInCurrentMonth = 0;
                    }

                }
                else
                {
                    var valueToPay = (-1) * currentMonthPayments
                        .Where(x => x.AbonentCreditId == paymentQueue.AbonentServiceOrCreditId)
                        .Sum(x => x.Sum);

                    creditsPayments[abonentCreditRepository.GetById(paymentQueue.AbonentServiceOrCreditId).CreditId] += valueToPay;

                    if (abonentClearPaymentInCurrentMonth > valueToPay)
                    {
                        abonentClearPaymentInCurrentMonth -= valueToPay;
                        creditsPaids[abonentCreditRepository.GetById(paymentQueue.AbonentServiceOrCreditId).CreditId] += valueToPay;
                    }
                    else
                    {
                        creditsPaids[abonentCreditRepository.GetById(paymentQueue.AbonentServiceOrCreditId).CreditId] += abonentClearPaymentInCurrentMonth;
                        abonentClearPaymentInCurrentMonth = 0;
                    }
                }
            }
            return;
        }
    }
}