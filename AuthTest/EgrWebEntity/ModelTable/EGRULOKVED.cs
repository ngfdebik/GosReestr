using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULOKVED : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? КодОквэд { get; set; }
        public bool? ОснКод { get; set; }
        public string? Наименование { get; set; }
        public string? Версия { get; set; }
        public string? ГРНИП { get; set; }
        public DateTime? ДатаГРНИП { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
