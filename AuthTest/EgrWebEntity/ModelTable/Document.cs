using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class Document : FluentNHibernate.Data.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string? ИдДок { get; set; }
        public int? idИП { get; set; }
        public int? idЮЛ { get; set; }
        public DateTime? ДатаЗагрузки { get; set; }
        [ForeignKey("idИП")]
        public IP? ИП { get; set; }
        [ForeignKey("idЮЛ")]
        public UL? ЮрЛицо { get; set; }
    }
}
