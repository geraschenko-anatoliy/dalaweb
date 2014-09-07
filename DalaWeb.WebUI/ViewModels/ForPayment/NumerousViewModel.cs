using DalaWeb.Domain.Abstract;
using DalaWeb.Domain.Entities.Abonents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForPayment
{
    public class NumerousViewModel
    {
        public string AbonentNumber { get; set; }
        public string AbonentName { get; set; }
        public int AbonentId { get; set; }
        public double Sum { get; set; }
        public double Balance { get; set; }

        public NumerousViewModel(string abonentNumber, string abonentName, int abonentId, double balance, int sum = 0)
        {
            this.AbonentName = abonentName;
            this.AbonentNumber = abonentNumber;
            this.AbonentId = abonentId;
            this.Balance = balance;
            this.Sum = sum;
        }

        public NumerousViewModel()
        { }
    }
}