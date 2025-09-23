using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvPodrazd : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? СвПодразд { get; set; }
        public string? СвФилиал { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? Индекс { get; set; }
        public string? КодРегион { get; set; }
        public string? КодАдрКладр { get; set; }
        public string? Дом { get; set; }
        public string? ТипРегион { get; set; }
        public string? НаимРегион { get; set; }
        public string? ТипГород { get; set; }
        public int? idЛицо { get; set; }
        public string? НаимГород { get; set; }
        public string? НаимПолн { get; set; }
        public string? ТипУлица { get; set; }
        public string? НаимУлица { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
