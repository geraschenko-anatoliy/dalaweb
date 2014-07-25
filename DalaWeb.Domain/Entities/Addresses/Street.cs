using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Addresses
{
    public class Street
    {
        [Key]
        public int StreetId { get; set; }

        [ForeignKey("City")]
        public int CityId { get; set; }

        [Required]
        [Display(Name = "Улица")]
        public string Name { get; set; }

        public virtual City City { get; set; }
    }
}
