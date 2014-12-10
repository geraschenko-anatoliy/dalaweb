using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalaWeb.Domain.Entities.PDFStorages
{
    public class PDFDocument
    {
        [Key]
        public int PDFid { get; set; }
        [Required]
        public int AbonentId { get; set; }
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public byte[] Value { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; } 
    }
}
