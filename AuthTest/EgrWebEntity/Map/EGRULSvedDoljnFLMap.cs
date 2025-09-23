using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvedDoljnFLMap : Base.Map<EGRULSvedDoljnFL>
    {
        public EGRULSvedDoljnFLMap() : base("public", "ЕГРЮЛ_СведДолжнФЛ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СведДолжнФЛ).Column("СведДолжнФЛ");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.Фамилия).Column("Фамилия");
            Map(x => x.Имя).Column("Имя");
            Map(x => x.Отчество).Column("Отчество");
            Map(x => x.ИННФЛ).Column("ИННФЛ");
            Map(x => x.ВидДолжн).Column("ВидДолжн");
            Map(x => x.НаимВидДолжн).Column("НаимВидДолжн");
            Map(x => x.НаимДолжн).Column("НаимДолжн");
            Map(x => x.ПризнНедДанДолжнФЛ).Column("ПризнНедДанДолжнФЛ");
            Map(x => x.ТекстНедДанДолжнФЛ).Column("ТекстНедДанДолжнФЛ");
            Map(x => x.ДатаНачДискв).Column("ДатаНачДискв");
            Map(x => x.ДатаОкончДискв).Column("ДатаОкончДискв");
            Map(x => x.ДатаРеш).Column("ДатаРеш");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
