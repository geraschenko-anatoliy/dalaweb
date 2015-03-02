using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Services;
using DalaWeb.Domain.Entities.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForStatistic
{
    public class PaymentQueueViewModel
    {
        public int AbonentServiceOrCreditId { get; set; }
        public bool isAbonentService { get; set; }
        [Display(Name="Название сервиса/кредита")]
        public string AbonentServiceName { get; set; }
        [Display(Name = "# в очереди")]
        public int NumberInQueue { get; set; }
        [Display(Name = "Требуемый %")]
        public double PercentageInCalculation { get; set; }

        public PaymentQueueViewModel(AbonentService aService, int numberInQueue = 0, double percentageInCalculation = 1.0 )
        {
            AbonentServiceOrCreditId = aService.AbonentServiceId;
            isAbonentService = true;
            AbonentServiceName = aService.Service.Name;
            this.NumberInQueue = numberInQueue;
            this.PercentageInCalculation = percentageInCalculation;
        }

        public PaymentQueueViewModel(AbonentCredit aCredit, int numberInQueue = 0, double percentageInCalculation = 1.0)
        {
            AbonentServiceOrCreditId = aCredit.AbonentCreditId;
            isAbonentService = false;
            AbonentServiceName = aCredit.Credit.Name;
            this.NumberInQueue = numberInQueue;
            this.PercentageInCalculation = percentageInCalculation;
        }

        public PaymentQueueViewModel ()
        { }

        public static List<PaymentQueueViewModel> PaymentQueueViewModelListGenerator(PaymentQueue paymentQueue, IRepository<AbonentService> abonentServiceRepository, IRepository<AbonentCredit> abonentCreditRepository)
        {
            List<PaymentQueueViewModel> paymentQueueViewModelList = new List<PaymentQueueViewModel>();
            var pq = paymentQueue.Queue.Split(';');
            var pp = paymentQueue.Percentage.Split(';');

            for (int i = 0; i < pq.Length - 1; i++)
            {
                int temp_pq = int.Parse(pq[i].Substring(1, 1));
                int temp_pp = int.Parse(pp[i]);
                if (pq[i][0] == 's')
                {
                    paymentQueueViewModelList.Add(new PaymentQueueViewModel(abonentServiceRepository.GetById(temp_pq), i + 1, temp_pp));
                }
                else
                {
                    paymentQueueViewModelList.Add(new PaymentQueueViewModel(abonentCreditRepository.GetById(temp_pq), i + 1, temp_pp));
                }
            }

            return paymentQueueViewModelList;
        }

    }
}