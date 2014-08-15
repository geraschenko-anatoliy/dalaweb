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
    public class Stamp
    {
        [Key]
        public int StampId { get; set; }

        [ForeignKey("Counter")]
        public int CounterId { get; set; }
        [Required]
        [Display(Name = "Пломба")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Дата установки")]
        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "Date")]
        [Display(Name = "Дата снятия")]
        public DateTime FinishDate { get; set; }

        [DefaultValue(false)]
        public bool isOff { get; set; }

        public virtual Counter Counter { get; set; }
    }
}
