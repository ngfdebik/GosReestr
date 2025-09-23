using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class ChangeLog : FluentNHibernate.Data.Entity
    {
        public int? Id { get; set; }
        public string? Таблица { get; set; }
        public string? Столбец { get; set; }
        public string? ЗначениеДо { get; set; }
        public string? ЗначениеПосле { get; set; }
        public string? ИНН { get; set; }
        public DateTime? ДатаИзменения { get; set; }
    }
}
