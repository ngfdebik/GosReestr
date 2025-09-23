using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRIPSvRegIP : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? ОГРНИП { get; set; }
        public DateTime? ДатаОГРНИП { get; set; }
        public string? РегНом { get; set; }
        public DateTime? ДатаРег { get; set; }
        public string? НаимРО { get; set; }
        public string? ОГРН { get; set; }
        public string? ИНН { get; set; }
        public string? НаимЮЛПолн { get; set; }
        public string? ГРНИП { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public IP? ИП { get; set; }
    }
}
