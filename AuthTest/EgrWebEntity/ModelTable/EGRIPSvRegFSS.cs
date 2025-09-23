using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRIPSvRegFSS : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? РегНомФСС { get; set; }
        public DateTime? ДатаРег { get; set; }
        public string? КодФСС { get; set; }
        public string? НаимФСС { get; set; }
        public string? ГРНИП { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public IP ИП { get; set; }
    }
}
