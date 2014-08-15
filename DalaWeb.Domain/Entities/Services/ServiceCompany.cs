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

  

}
