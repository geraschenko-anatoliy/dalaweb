using DalaWeb.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForStatistic
{
    public class DailyPayment
    {
        [Display(Name="Дата")]
        public DateTime Day { get; set; }
        [Display(Name="Сумма")]        
        public Double Summ {get; set;}

        public DailyPayment(DateTime day, double summ)
        {
            Day = day;
            Summ = summ;
        }

    }
}