using DalaWeb.Domain.Entities.Abonents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Credits
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
        [Display(Name = "Дата открытия"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Column(TypeName = "Date")]

        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Дата закрытия"), DisplayFormat(DataFormatString="{0:dd/MM/yyyy}", ApplyFormatInEditMode=true )]
        [Column(TypeName = "Date")]
        public DateTime FinishDate { get; set; }

        [Required(ErrorMessage = "*")]
        [DefaultValue(0)]
        [Range(0, double.MaxValue, ErrorMessage = "Введите положительное значение")]
        [Display(Name = "Предоплата")]
        public double PrePayment { get; set; }

        [Required(ErrorMessage = "*")]
        [DefaultValue(0)]
        [Range(0, double.MaxValue, ErrorMessage = "Введите положительное значение")]
        [Display(Name = "Общая сумма проплат")]
        public double PaidForTheEntirePeriod { get; set; }

        [Required(ErrorMessage = "*")]
        [DefaultValue(0)]
        [Range(0, 100, ErrorMessage = "Введите положительное значение")]
        [Display(Name = "Оплачено месяцев")]
        public int PaidMonths { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Введите положительное значение")]
        [Display(Name = "Срок")]
        public int Term { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Введите положительное значение")]
        [Display(Name = "Общая сумма кредита")]
        public double Price { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Сумма в месяц")]
        public double PaymentForMonth { get; set; }
        [DefaultValue(false)]
        [Display(Name = "Полностью оплачен")]
        public bool FullyPaid { get; set; }
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
    }
}
