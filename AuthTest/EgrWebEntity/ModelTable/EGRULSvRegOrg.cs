using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvRegOrg : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? КодНО { get; set; }
        public string? НаимНО { get; set; }
        public string? АдрРО { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
