using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.Settings
{
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [Required]
        [Display(Name = "Подпись к счету")]
        public string SignatureText { get; set; }
    }
}
