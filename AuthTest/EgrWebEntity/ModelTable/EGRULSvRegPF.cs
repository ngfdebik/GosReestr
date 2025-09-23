using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvRegPF : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? РегНомПФ { get; set; }
        public DateTime? ДатаРег { get; set; }
        public string? КодПФ { get; set; }
        public string? НаимПФ { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
