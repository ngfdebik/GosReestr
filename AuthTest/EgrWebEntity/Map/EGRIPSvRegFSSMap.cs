using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIPSvRegFSSMap : Base.Map<EGRIPSvRegFSS>
    {
        public EGRIPSvRegFSSMap() : base("public", "ЕГРИП_СвРегФСС")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.РегНомФСС).Column("РегНомФСС");
            Map(x => x.ДатаРег).Column("ДатаРег");
            Map(x => x.КодФСС).Column("НКодФСС");
            Map(x => x.НаимФСС).Column("НаимФСС");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
