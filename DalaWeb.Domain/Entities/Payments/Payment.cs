using DalaWeb.Domain.Entities.Abonents;
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
        public int AbonentId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Дата")]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Сумма платежа")]
        public double Sum { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Тип платежа")]
        public string Type { get; set; }
        
        [Required(ErrorMessage = "*")]
        [Display(Name = "Баланс")]
        public double Balance { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }

        public virtual Abonent Abonent { get; set; }
    }
}
