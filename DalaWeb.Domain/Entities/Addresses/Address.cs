using DalaWeb.Domain.Entities.Abonents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Addresses
{
    public class Address
    {
        [Key]
        [ForeignKey("Abonent")]
        public int AbonentId { get; set; }
        public int CityId { get; set; }
        public int StreetId { get; set; }
        [Required]
        [Display(Name = "Дом")]
        public string House { get; set; }
        public virtual Abonent Abonent { get; set; }
    }
}
