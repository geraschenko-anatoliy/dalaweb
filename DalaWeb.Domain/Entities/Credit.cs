using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities
{
    public class Credit
    {
        public int CreditId { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public virtual ICollection<AbonentCredit> AbonentCredits { get; set; }
    }
}
