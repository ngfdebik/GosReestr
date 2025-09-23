using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvAddressULMap : Base.Map<EGRULSvAddressUL>
    {
        public EGRULSvAddressULMap() : base("public", "ЕГРЮЛ_СвАдресЮЛ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвАдресЮЛ).Column("СвАдресЮЛ");
            Map(x => x.Индекс).Column("Индекс");
            Map(x => x.КодРегион).Column("КодРегион");
            Map(x => x.КодАдрКладр).Column("КодАдрКладр");
            Map(x => x.Дом).Column("Дом");
            Map(x => x.ТипРегион).Column("ТипРегион");
            Map(x => x.НаимРегион).Column("НаимРегион");
            Map(x => x.ТипУлица).Column("ТипУлица");
            Map(x => x.ТипГород).Column("ТипГород");
            Map(x => x.НаимГород).Column("НаимГород");
            Map(x => x.НаимУлица).Column("НаимУлица");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.Кварт).Column("Кварт");
            Map(x => x.ПризнНедАдресЮЛ).Column("ПризнНедАдресЮЛ");
            Map(x => x.ТекстНедАдресЮЛ).Column("ТекстНедАдресЮЛ");
            Map(x => x.ТипРайон).Column("ТипРайон");
            Map(x => x.НаимРайон).Column("НаимРайон");
            Map(x => x.ТипНаселПункт).Column("ТипНаселПункт");
            Map(x => x.НаимНаселПункт).Column("НаимНаселПункт");
            Map(x => x.Корпус).Column("Корпус");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
