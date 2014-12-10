using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Concrete;
using DalaWeb.Domain.Entities;
using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Addresses;
using DalaWeb.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForPayment
{
    public struct MonthPayment
    {
        public int AbonentId { get; set; }
        public string AbonentNumber {get; set; }
        public string AbonentName { get; set; }    
        public int Day {get; set;}
        public string Year {get; set;}
        public string Month {get; set;}
        public double Sum { get; set; }
        public string Comment { get; set; }
    }
    public class IndexViewModel
    {
        IQueryable<Payment> payments;
        public IQueryable <MonthPayment> monthPayments;

        void SetMonthPayments()
        { 
            List<MonthPayment> monthPaymentsList = new List<MonthPayment>();
            foreach (var year in payments.Select(x=>x.Date.Year).Distinct())
            {
                foreach (var month in payments.Where(x => x.Date.Year == year).Select(x => x.Date.Month).Distinct())
                {
                    foreach (var abonentId in payments.Where(x => x.Date.Year == year)
                                                                   .Where(x => x.Date.Month == month)
                                                                   .Where(x=>x.Type == "Списание")
                                                                   .Select(x => x.AbonentId)
                                                                   .Distinct())
                    { 
                        double summ = 0;
                        Payment tempPayment = new Payment();
                        foreach (var payment in payments.Where(x => x.Date.Year == year)
                                                                   .Where(x => x.Date.Month == month)
                                                                   .Where(x => x.Type == "Списание")
                                                                   .Where(x => x.AbonentId == abonentId))
                        {
                            summ += payment.Sum;
                            tempPayment = payment;
                        }
                        MonthPayment item = new MonthPayment()
                        {
                            AbonentId  = tempPayment.AbonentId,
                            AbonentName = tempPayment.Abonent.Name,
                            AbonentNumber = tempPayment.Abonent.AbonentNumber,
                            Day = tempPayment.Date.Day,
                            Month = tempPayment.Date.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru-RU")),
                            Year = tempPayment.Date.Year.ToString(),
                            Sum = summ,
                            Comment = "Квитанция за " + tempPayment.Date.ToString("MMMM", CultureInfo.CreateSpecificCulture("ru-RU"))
                        };
                        monthPaymentsList.Add(item);
                    }
                }
            }
            monthPayments = monthPaymentsList.AsQueryable<MonthPayment>();
        }

        public IndexViewModel(IQueryable<Payment> payments)
        {
            this.payments = payments;
            SetMonthPayments();
        }
    }
}