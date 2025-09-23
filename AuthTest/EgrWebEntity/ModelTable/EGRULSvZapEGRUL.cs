using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvZapEGRUL : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? ИдЗап { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗап { get; set; }
        public string? КодСПВЗ { get; set; }
        public string? НаимВидЗап { get; set; }
        public string? КодНО { get; set; }
        public string? НаимНО { get; set; }
        public string? СведПредДок { get; set; }
        public string? НаимДок { get; set; }
        public string? НомДок { get; set; }
        public string? ДатаДок { get; set; }
        public string? Серия { get; set; }
        public string? Номер { get; set; }
        public DateTime? ДатаВыдСвид { get; set; }
        public string? СвСтатусЗап { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
