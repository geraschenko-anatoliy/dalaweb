using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Counters
{
    public class Counter
    {
        [Key]
        public int CounterId { get; set; }
        [ForeignKey("Service")]
        [Display(Name = "Сервис")]
        public int ServiceId { get; set; }
        [ForeignKey("Abonent")]
        [Display(Name = "Абонент")]
        public int AbonentId { get; set; }

        [Required]
        [Display(Name = "Дата подключения")]
        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата отключения")]
        [Column(TypeName = "Date")]
        public DateTime FinishDate { get; set; }

        [Required]
        [Display(Name = "Номер счетчика")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Значение счетчика")]
        [Range(1, double.MaxValue, ErrorMessage = "Please enter a positive price")]
        public double InitialValue { get; set; }
        [DefaultValue(false)]
        public bool isOff { get; set; }
        public ICollection<Stamp> Stamps { get; set; }
        public ICollection<CounterValues> CounterValues { get; set; }

        public virtual Abonent Abonent {get; set;}
        public virtual Service Service { get; set; }
    }
}
