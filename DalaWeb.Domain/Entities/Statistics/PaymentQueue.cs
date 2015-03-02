using DalaWeb.Domain.Entities.Abonents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Statistics
{
    public class PaymentQueue
    {
        [Key]
        public int PaymentQueueId { get; set; }

        [Required]
        [Display(Name = "Очередь")]
        public string Queue { get; set; }

        [Required]
        [Display(Name = "Требуемый процент")]
        public string Percentage { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Дата")]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        public int AbonentId { get; set; }
        [ForeignKey("AbonentId")]
        public virtual Abonent Abonent { get; set; }
    }
}
