using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvedDoljnFL : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? СведДолжнФЛ { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? Фамилия { get; set; }
        public string? Имя { get; set; }
        public string? Отчество { get; set; }
        public string? ИННФЛ { get; set; }
        public string? ВидДолжн { get; set; }
        public string? НаимВидДолжн { get; set; }
        public string? НаимДолжн { get; set; }
        public string? ПризнНедДанДолжнФЛ { get; set; }
        public string? ТекстНедДанДолжнФЛ { get; set; }
        public DateTime? ДатаНачДискв { get; set; }
        public DateTime? ДатаОкончДискв { get; set; }
        public DateTime? ДатаРеш { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
