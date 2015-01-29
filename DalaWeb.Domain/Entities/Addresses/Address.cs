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
        public int AddressId { get; set; }

        [Required]
        [Display(Name = "Дом")]
        public string House { get; set; }

        public int AbonentId { get; set; }
        [Required]
        [ForeignKey("AbonentId")]
        public virtual Abonent Abonent { get; set; }

        [Required]
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }


        [Required]
        public int StreetId { get; set; }
        [ForeignKey("StreetId")]
        public virtual Street Street { get; set; }
    }
}
