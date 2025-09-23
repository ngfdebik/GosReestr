using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRIPSvAdrMJ : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? СвАдрМЖ { get; set; }
        public string? КодРегион { get; set; }
        public string? ТипРегион { get; set; }
        public string? НаимРегион { get; set; }
        public string? ТипГород { get; set; }
        public string? НаимГород { get; set; }
        public string? ГРНИП { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? ТипРайон { get; set; }
        public string? НаимРайон { get; set; }
        public string? ТипНаселПункт { get; set; }
        public string? НаимНаселПункт { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public IP ИП { get; set; }
    }
}
