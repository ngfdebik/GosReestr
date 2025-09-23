using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvPodrazdMap : Base.Map<EGRULSvPodrazd>
    {
        public EGRULSvPodrazdMap() : base("public", "ЕГРЮЛ_СвПодразд")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвПодразд).Column("СвПодразд");
            Map(x => x.СвФилиал).Column("СвФилиал");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.Индекс).Column("Индекс");
            Map(x => x.КодРегион).Column("КодРегион");
            Map(x => x.КодАдрКладр).Column("КодАдрКладр");
            Map(x => x.Дом).Column("Дом");
            Map(x => x.ТипРегион).Column("ТипРегион");
            Map(x => x.НаимРегион).Column("НаимРегион");
            Map(x => x.ТипГород).Column("ТипГород");
            Map(x => x.НаимГород).Column("НаимГород");
            Map(x => x.НаимПолн).Column("НаимПолн");
            Map(x => x.ТипУлица).Column("ТипУлица");
            Map(x => x.НаимУлица).Column("НаимУлица");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
