using DalaWeb.Domain.Entities.Counters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Services
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        [ForeignKey("ServiceCompany")]
        public int CompanyId { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Сервис")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        [Display(Name = "Цена")]
        public double Price { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Тип сервиса")]
        public int Type { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Статус")]
        public bool isOff { get; set; }

        public virtual ServiceCompany ServiceCompany { get; set; }
        public virtual ICollection<AbonentService> AbonentServices { get; set; }
        public virtual ICollection<Counter> Counters { get; set; }

        public override string ToString()
        {
            switch (Type)
            {
                case (1):
                    return "По абоненту";
                case (2):
                    return "По проживающим";
                case (3):
                    return "По счетчику";
                default:
                    return "Значение не выбрано";
            }
        }
    }
}
