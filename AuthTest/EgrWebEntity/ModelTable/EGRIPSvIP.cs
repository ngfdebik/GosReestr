using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRIPSvIP : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public DateTime? ДатаВып { get; set; }
        public string? ОГРН { get; set; }
        public DateTime? ДатаОГРН { get; set; }
        public string? ИНН { get; set; }
        public string? КодВидИП { get; set; }
        public string? НаимВидИП { get; set; }
        public string? НаимСокр { get; set; }
        public string? ОКВЭДОсн { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public IP ИП { get; set; }
    }
}
