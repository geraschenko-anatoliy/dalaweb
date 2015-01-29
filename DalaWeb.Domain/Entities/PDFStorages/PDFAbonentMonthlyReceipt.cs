using DalaWeb.Domain.Entities.Abonents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.PDFStorages
{
    public class PDFAbonentMonthlyReceipt
    {
        [Key]
        public int PDFId { get; set; }
              
        [Required]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        [Required]
        public byte[] Value { get; set; }
        
        [Required]
        [Column(TypeName = "Date")]
        public DateTime TimeStamp { get; set; }

        [Required]
        public int AbonentId { get; set; }
        
        [ForeignKey("AbonentId")]
        public virtual Abonent Abonent { get; set; }
    }
}
