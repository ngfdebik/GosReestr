using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvReorg : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? СвРеорг { get; set; }
        public string? КодСтатусЮЛ { get; set; }
        public string? НаимСтатусЮЛ { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? ОГРН { get; set; }
        public string? ИНН { get; set; }
        public string? НаимЮЛПолн { get; set; }
        public string? СостЮЛпосле { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
