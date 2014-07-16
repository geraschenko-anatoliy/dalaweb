using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities
{
    public class Abonent
    {
        [Key]
        public int AbonentId { get; set; }
        [Display(Name="Номер абонента")]
        public string AbonentNumber { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name="Абонент")]
        public string Name { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Телефон")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "ИНН")]
        public string INN { get; set; }
        [Required(ErrorMessage = "*")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a positive price")]
        [Display(Name = "Число жителей")]
        public int NumberOfInhabitants { get; set; }
        public virtual ICollection<AbonentCredit> AbonentCredits { get; set; }
        public virtual ICollection<AbonentService> AbonentServices { get; set; }
        public virtual Address Address { get; set; }
    }
}
