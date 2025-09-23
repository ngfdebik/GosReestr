using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvStatus : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? СвСтатус { get; set; }
        public string? КодСтатусЮЛ { get; set; }
        public string? НаимСтатусЮЛ { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public DateTime? ДатаРеш { get; set; }
        public string? НомерРеш { get; set; }
        public DateTime? ДатаПубликации { get; set; }
        public string? НомерЖурнала { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }

    }
}
