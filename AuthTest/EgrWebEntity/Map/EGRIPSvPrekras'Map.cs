using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIPSvPrekras_Map : Base.Map<EGRIPSvPrekras_>
    {
        public EGRIPSvPrekras_Map() : base("public", "ЕГРИП_СвПрекращ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвПрекращ).Column("СвПрекращ");
            Map(x => x.КодСтатус).Column("КодСтатус");
            Map(x => x.НаимСтатус).Column("НаимСтатус");
            Map(x => x.ДатаПрекращ).Column("ДатаПрекращ");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
