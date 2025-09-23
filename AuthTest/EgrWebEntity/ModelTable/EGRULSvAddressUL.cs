using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvAddressUL : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? СвАдресЮЛ { get; set; }
        public string? Индекс { get; set; }
        public string? КодРегион { get; set; }
        public string? КодАдрКладр { get; set; }
        public string? Дом { get; set; }
        public string? ТипРегион { get; set; }
        public string? НаимРегион { get; set; }
        public string? ТипГород { get; set; }
        public string? НаимГород { get; set; }
        public string? ТипУлица { get; set; }
        public string? НаимУлица { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? Кварт { get; set; }
        public string? ПризнНедАдресЮЛ { get; set; }
        public string? ТекстНедАдресЮЛ { get; set; }
        public string? ТипРайон { get; set; }
        public string? НаимРайон { get; set; }
        public string? ТипНаселПункт { get; set; }
        public string? НаимНаселПункт { get; set; }
        public string? Корпус { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
