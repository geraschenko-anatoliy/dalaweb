using DalaWeb.Domain.Entities.Abonents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForStatistic
{
    public class MalignantDebtor
    {
        public Abonent Abonent { get; set; }

        public double ServicePaymentForPreviousPeriod { get; set; }
        public double UserPaymentForPreviousPeriod { get; set; }

        public MalignantDebtor(Abonent abonent, double servicePayment, double userPayment)
        {
            ServicePaymentForPreviousPeriod = servicePayment;
            UserPaymentForPreviousPeriod = userPayment;
            Abonent = abonent; 
        }

    }
}