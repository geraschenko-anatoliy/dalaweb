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

namespace DalaWeb.WebUI.Infrastructure
{
    public sealed class StatisticsCalculator
    {
        private IUnitOfWork unitOfWork;
        private IRepository<Payment> paymentRepository;
        private IRepository<Abonent> abonentRepository;
        private IRepository<AbonentCredit> abonentCreditRepository;
        private IRepository<AbonentService> abonentServiceRepository;
        private IRepository<Counter> counterRepository;
        private IRepository<CounterValues> counterValuesRepository;
        private IRepository<Service> serviceRepository;
        public StatisticsCalculator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            paymentRepository = unitOfWork.PaymentRepository;
            abonentRepository = unitOfWork.AbonentRepository;
            abonentCreditRepository = unitOfWork.AbonentCreditRepository;
            abonentServiceRepository = unitOfWork.AbonentServiceRepository;
            counterRepository = unitOfWork.CounterRepository;
            counterValuesRepository = unitOfWork.CounterValuesRepository;
            serviceRepository = unitOfWork.ServiceRepository;
        }



    }
}