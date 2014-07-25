using DalaWeb.Domain.Entities.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Services
{
    public class ServiceCompany
    {
        [Key]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Название компании")]
        public string Name { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }

    //public class Service
    //{
    //    [Key]
    //    public int ServiceId { get; set; }
    //    [ForeignKey("ServiceCompany")]
    //    public int CompanyId { get; set; }
    //    [Required(ErrorMessage = "*")]
    //    [Display(Name = "Сервис")]
    //    public string Name { get; set; }
    //    [Required(ErrorMessage = "*")]
    //    [Range(0, int.MaxValue, ErrorMessage = "Please enter a positive price")]
    //    [Display(Name = "Цена")]
    //    public decimal Price { get; set; }
    //    [Required(ErrorMessage = "*")]
    //    [Range(0, int.MaxValue, ErrorMessage = "Please enter a positive price")]
    //    [Display(Name = "Тип сервиса")]
    //    public int Type { get; set; }
    //    public virtual ServiceCompany ServiceCompany { get; set; }
    //    public virtual ICollection<AbonentService> AbonentServices { get; set; }

    //    public override string ToString()
    //    {
    //        switch (Type)
    //        {
    //            case (1):
    //                return "По абоненту";
    //            case (2):
    //                return "По проживающим";
    //            case (3):
    //                return "По счетчику";
    //            default:
    //                return "Значение не выбрано";
    //        }
    //    }
    //}

    //public class AbonentService
    //{
    //    [Key, Column(Order = 0)]
    //    public int AbonentId { get; set; }
    //    [Key, Column(Order = 1)]
    //    public int ServiceId { get; set; }

    //    [Required]
    //    [DefaultValue(false)]
    //    [Display(Name = "Услуга приостановлена?")]
    //    public bool isSuspended { get; set; }
    //    [Required]
    //    [DefaultValue(false)]
    //    [Display(Name = "Услуга отключена?")]
    //    public bool isOff { get; set; }

    //    public virtual Abonent Abonent { get; set; }
    //    public virtual Service Service { get; set; }

    //    [Required]
    //    [Key, Column(Order = 2, TypeName = "Date")]
    //    [Display(Name = "Дата подключения")]
    //    public DateTime StartDate { get; set; }

    //    [Display(Name = "Дата отключения")]
    //    [Column(TypeName = "Date")]
    //    public DateTime FinishDate { get; set; }
    //}

}
