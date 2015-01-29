using DalaWeb.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DalaWeb.WebUI.ViewModels.ForPayment
{
    public class OpenMonthSummaryViewModel 
    {
        public List<string> warnings { get; set; }
        public List<Payment> payments { get; set; }
        public OpenMonthSummaryViewModel(List<string> warnings, List<Payment> payments)
        {
            this.payments = payments;
            this.warnings = warnings;
        }

    }

    public class RecalculateMonthSummaryViewModel
    {
        public List<string> warnings { get; set; }
        public List<Payment> payments { get; set; }
        public RecalculateMonthSummaryViewModel(List<string> warnings, List<Payment> payments)
        {
            this.payments = payments;
            this.warnings = warnings;
        }

    }
}