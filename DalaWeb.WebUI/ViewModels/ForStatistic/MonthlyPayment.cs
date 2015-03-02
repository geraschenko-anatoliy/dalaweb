using DalaWeb.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForStatistic
{
    public class MonthlyPayment
    {
        [Display(Name="Услуга")]
        public string Name { get; set; }
        [Display(Name="Начисленная сумма")]        
        public Double PaymentSum {get; set;}
        [Display(Name = "Оплаченная сумма")]
        public Double PaidSum { get; set; }

        public MonthlyPayment(string name, double paidSum, double paymentSum)
        {
            Name = name;
            PaymentSum = paymentSum;
            PaidSum = paidSum;
        }
    }
}