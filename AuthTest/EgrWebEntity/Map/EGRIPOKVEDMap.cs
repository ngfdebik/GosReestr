using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgrWebEntity.ModelTable;

namespace EgrWebEntity.Map
{
    public class EGRIPOKVEDMap : Base.Map<EGRIPOKVED>
    {
        public EGRIPOKVEDMap() : base("public", "ЕГРИП_ОКВЭД")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.КодОквэд).Column("КодОКВЭД");
            Map(x => x.ОснКод).Column("ОснКод");
            Map(x => x.Наименование).Column("Наименование");
            Map(x => x.Версия).Column("Версия");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаГРНИП).Column("ДатаГРНИП");
            Map(x => x.idЛицо).Column("idЛицо");
           // References(x => x.ИП).Column("Id").Cascade.None();
        }
    }
}
