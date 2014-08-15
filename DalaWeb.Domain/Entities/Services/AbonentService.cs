using DalaWeb.Domain.Entities.Abonents;
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
    public class AbonentService
    {
        [Key, Column(Order = 0)]
        public int AbonentId { get; set; }
        [Key, Column(Order = 1)]
        public int ServiceId { get; set; }

        [Required]
        [DefaultValue(false)]
        [Display(Name = "Услуга отключена?")]
        public bool isOff { get; set; }

        public virtual Abonent Abonent { get; set; }
        public virtual Service Service { get; set; }

        
        [Required]
        [Key, Column(Order = 2)]
        [Display(Name = "Дата подключения")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата отключения")]
        public DateTime FinishDate { get; set; }
    }
}
