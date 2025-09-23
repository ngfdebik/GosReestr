using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRIPSVFL : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? Пол { get; set; }
        public string? Фамилия { get; set; }
        public string? Имя { get; set; }
        public string? Отчество { get; set; }
        public string? ГРНИП { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public IP ИП { get; set; }

    }
}
