using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIPSvGrajdMap : Base.Map<EGRIPSvGrajd>
    {
        public EGRIPSvGrajdMap() : base("public", "ЕГРИП_СвГражд")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ВидГражд).Column("ВидГражд");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ОКСМ).Column("ОКСМ");
            Map(x => x.НаимСтран).Column("НаимСтран");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
