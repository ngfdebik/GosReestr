using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRIPSvLicense : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? НомЛиц { get; set; }
        public DateTime? ДатаЛиц { get; set; }
        public DateTime? ДатаНачЛиц { get; set; }
        public string? НаимЛицВидДеят { get; set; }
        public string? ЛицОргВыдЛиц { get; set; }
        public string? ГРНИП { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public DateTime? ДатаОкончЛиц { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public IP ИП { get; set; }
    }
}
