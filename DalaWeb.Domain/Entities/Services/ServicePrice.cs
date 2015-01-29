using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Services
{
    public class ServicePrice
    {
        [Key]
        public int ServicePriceId { get; set; }

        [Required(ErrorMessage = "*")]
        [Range(0, double.MaxValue, ErrorMessage = "Введите положительное значение")]
        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Required]
        [Column(TypeName = "Date")]
        [Display(Name = "Дата подключения")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата отключения")]
        [Column(TypeName = "Date")]
        public DateTime FinishDate { get; set; }
        
        [Required]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }

    }
}
