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
        [Key]
        public int AbonentServiceId { get; set; }

        [DefaultValue(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        [Display(Name = "Услуга отключена?")]
        public bool? isOff 
        {
            get 
            {
                if (FinishDate != DateTime.MinValue)
                    return DateTime.Now > FinishDate ? true : false;
                else
                    return false;
            }  
            private set {} 
        }
        [Required]
        public int AbonentId { get; set; }

        [ForeignKey("AbonentId")]
        public virtual Abonent Abonent { get; set; }
        [Required]
        public int ServiceId { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }

        
        [Required]
        [Column(TypeName="Date")]
        [Display(Name = "Дата подключения")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата отключения")]
        [Column(TypeName="Date")]
        public DateTime FinishDate { get; set; }
    }
}
