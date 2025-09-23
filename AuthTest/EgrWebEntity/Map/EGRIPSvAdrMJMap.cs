using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgrWebEntity.ModelTable;

namespace EgrWebEntity.Map
{
    public class EGRIPSvAdrMJMap : Base.Map<EGRIPSvAdrMJ>
    {
        public EGRIPSvAdrMJMap() : base("public", "ЕГРИП_СвАдрМЖ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвАдрМЖ).Column("СвАдрМЖ");
            Map(x => x.ТипРегион).Column("ТипРегион");
            Map(x => x.НаимРегион).Column("НаимРегион");
            Map(x => x.ТипГород).Column("ТипГород");
            Map(x => x.НаимГород).Column("НаимГород");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ТипРегион).Column("ТипРегион");
            Map(x => x.НаимРайон).Column("НаимРайон");
            Map(x => x.ТипНаселПункт).Column("ТипНаселПункт");
            Map(x => x.НаимНаселПункт).Column("НаимНаселПункт");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
