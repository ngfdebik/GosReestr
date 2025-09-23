using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//27

namespace EgrWebEntity.ModelTable
{
    public class EGRULSvFounder : FluentNHibernate.Data.Entity, IGenericTable
    {
        public string? Id { get; set; }
        public string? СвУчредит { get; set; }
        public string? УчрФЛ { get; set; }
        public string? ГРН { get; set; }
        public DateTime? ДатаЗаписи { get; set; }
        public string? Фамилия { get; set; }
        public string? Имя { get; set; }
        public string? Отчество { get; set; }
        public string? ИННФЛ { get; set; }
        public string? НоминСтоим { get; set; }
        public string? РазмерДоли { get; set; }
        public string? Процент { get; set; }
        public string? УчрЮЛРос { get; set; }
        public string? ОГРН { get; set; }
        public string? ИНН { get; set; }
        public string? НаимЮЛПолн { get; set; }
        public string? УчрРФСубМО { get; set; }
        public string? КодУчрРФСубМО { get; set; }
        public string? НаимМО { get; set; }
        public string? КодРегион { get; set; }
        public string? НаимРегион { get; set; }
        public string? СвОргОсущПр { get; set; }
        public string? ДробДесят { get; set; }
        public string? ПризнНедДанУчр { get; set; }
        public string? ТекстНедДанУчр { get; set; }
        public string? Числит { get; set; }
        public string? Знаменат { get; set; }
        public int? idЛицо { get; set; }
        [ForeignKey("idЛицо")]
        public UL? ЮрЛицо { get; set; }
    }
}
