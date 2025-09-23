using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULOKVEDMap : Base.Map<EGRULOKVED>
    {
        public EGRULOKVEDMap() : base("public", "ЕГРЮЛ_ОКВЭД")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.КодОквэд).Column("КодОквэд");
            Map(x => x.ОснКод).Column("ОснКод");
            Map(x => x.Наименование).Column("Наименование");
            Map(x => x.Версия).Column("Версия");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаГРНИП).Column("ДатаГРНИП");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
