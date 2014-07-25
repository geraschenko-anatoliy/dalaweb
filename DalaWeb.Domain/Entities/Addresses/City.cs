using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Addresses
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        [Required]
        [Display(Name = "Город")]
        public string Name { get; set; }

        public ICollection<Street> Streets { get; set; }
    }
}
