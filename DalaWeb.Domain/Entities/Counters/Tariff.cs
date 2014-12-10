using DalaWeb.Domain.Entities.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Counters
{
    public class Tariff
    {
        [Key]
        public int TarifId { get; set; }
        [Required]
        [Display(Name = "Сервис")]
        public int ServiceId { get; set; }
        [Required]
        [Display(Name = "С какого значения действует?")]
        public int LimitValue { get; set; }
        [Required]
        [Display(Name = "Тариф")]
        [Range(0, double.MaxValue, ErrorMessage = "Введите положительное значение")]
        public double OverPrice { get; set; }

        public virtual Service Service { get; set; }
    }
}
