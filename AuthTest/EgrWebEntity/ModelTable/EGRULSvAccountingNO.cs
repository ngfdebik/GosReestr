using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvAccountingNO : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? ИНН { get; set; }
        public string? КПП { get; set; }
        public DateTime? ДатаПостУч { get; set; }
        public string? КодНО { get; set; }
        public string? НаимНО { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
