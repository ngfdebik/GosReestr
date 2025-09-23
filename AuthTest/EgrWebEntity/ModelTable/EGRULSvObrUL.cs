using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvObrUL : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? ОГРН { get; set; }
        public DateTime? ДатаОГРН { get; set; }
        public string? КодСпОбрЮЛ { get; set; }
        public string? НаимСпОбрЮЛ { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? РегНом { get; set; }
        public DateTime? ДатаРег { get; set; }
        public string? НаимРО { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
