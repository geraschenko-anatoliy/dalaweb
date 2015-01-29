using DalaWeb.Domain.Entities.Abonents;
using DalaWeb.Domain.Entities.Credits;
using DalaWeb.Domain.Entities.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Payments
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Дата")]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Сумма платежа")]
        public double Sum { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }

        [Required]
        public int AbonentId { get; set; }
        [ForeignKey("AbonentId")]
        public virtual Abonent Abonent { get; set; }

        public int? AbonentCreditId { get; set; } 
        [ForeignKey("AbonentCreditId")]
        public virtual AbonentCredit AbonentCredit { get; set; }

        public int? AbonentServiceId { get; set; }
        [ForeignKey("AbonentServiceId")]
        public virtual AbonentService AbonentService { get; set; }
    }
}
