using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Counters
{
    public class CounterValues
    {
        [Key]
        public int CounterValuesId { get; set; }

        [Display(Name = "Дата")]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Значение")]
        public double Value { get; set; }

        [Required]
        public int CounterId { get; set; }
        [ForeignKey("CounterId")]

        public virtual Counter Counter { get; set; }
    }
}
