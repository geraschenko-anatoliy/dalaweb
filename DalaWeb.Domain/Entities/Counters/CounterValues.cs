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
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CounterValuesId { get; set; }
        [Key, Column(Order = 1)]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
        [ForeignKey("Counter")]
        public int CounterId { get; set; }
        [Required]
        [Display(Name = "Значение")]
        public double Value { get; set; }

        public virtual Counter Counter { get; set; }
    }
}
