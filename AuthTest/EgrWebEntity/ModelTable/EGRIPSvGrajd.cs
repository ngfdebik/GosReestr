using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRIPSvGrajd : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? ВидГражд { get; set; }
        public string? ГРНИП { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? ОКСМ { get; set; }
        public string? НаимСтран { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public IP ИП { get; set; }
    }
}
