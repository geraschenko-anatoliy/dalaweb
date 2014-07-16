using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities
{
    public class AbonentCredit
    {
        [Key, Column(Order = 0)]
        public int AbonentId { get; set; }
        [Key, Column(Order = 1)]
        public int CreditId { get; set; }

        public virtual Abonent Abonent { get; set; }
        public virtual Credit Credit { get; set; }

        [Required]
        [Display(Name = "Дата открытия")]
        [Column(TypeName = "Date")]

        public DateTime DateWhereCreated { get; set; }

        [Required(ErrorMessage = "*")]
        [DefaultValue(0)]
        [Display(Name = "Предоплата")]
        public decimal PrePayment { get; set; }

        [Required(ErrorMessage = "*")]
        [DefaultValue(0)]
        [Display(Name = "Общая сумма проплат")]
        public decimal PaidForTheEntirePeriod { get; set; }

        [Required(ErrorMessage = "*")]
        [DefaultValue(0)]
        [Display(Name = "Оплачено месяцев")]
        public int PaidMonths { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Необходимо ввести положительное значение")]
        [Display(Name = "Срок")]
        public int Term { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Необходимо ввести положительное значение")]
        [Display(Name = "Общая сумма кредита")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Сумма в месяц")]
        public decimal PaymentForMonth { get; set; }
        [DefaultValue(false)]
        public bool FullyPaid { get; set; }
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
    }
}
