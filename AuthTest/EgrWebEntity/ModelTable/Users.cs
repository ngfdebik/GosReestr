using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class Users : FluentNHibernate.Data.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string? Логин { get; set; }
        public string? Пароль { get; set; }
        public string? Роль { get; set; }
        public string? ФИО { get; set; }
    }
}
