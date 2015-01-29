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
        [Range(0, double.MaxValue, ErrorMessage = "Значение должно быть больше нуля")]
        public double InitialValue { get; set; }
        [DefaultValue(false)]
        public bool isOff { get; set; }


        [Required]
        public int AbonentId { get; set; }
        [ForeignKey("AbonentId")]
        public virtual Abonent Abonent {get; set;}

        [Required]
        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }


        public virtual ICollection<Stamp> Stamps { get; set; }
        public virtual ICollection<CounterValues> CounterValues { get; set; }
    }
}
