using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities
{
    public class Address
    {
        [Key]
        [ForeignKey("Abonent")]
        public int AbonentID { get; set; }
        public int CityId { get; set; }
        //public virtual City City {get; set;}
        public int StreetId { get; set; }
        //public virtual Street Street { get; set; }
        public string House { get; set; }

        public virtual Abonent Abonent { get; set; }
    }

    public class City 
    {
        [Key]
        public int CityId {get; set;}
        [Required]
        public string Name { get; set; }

        public ICollection<Street> Streets { get; set; }
    }

    public class Street
    {
        [Key]
        public int StreetId { get; set; }
        
        [ForeignKey("City")]
        public int CityId { get; set; }
        
        [Required]
        public string Name { get; set; }

        public virtual City City { get; set; }
    }
}
