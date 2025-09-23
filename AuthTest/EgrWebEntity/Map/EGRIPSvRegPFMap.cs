using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIPSvRegPFMap : Base.Map<EGRIPSvRegPF>
    {
        public EGRIPSvRegPFMap() : base("public", "ЕГРИП_СвРегПФ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.РегНомПФ).Column("РегНомПФ");
            Map(x => x.ДатаРег).Column("ДатаРег");
            Map(x => x.КодПФ).Column("КодПФ");
            Map(x => x.КодПФ).Column("КодПФ");
            Map(x => x.НаимПФ).Column("НаимПФ");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
