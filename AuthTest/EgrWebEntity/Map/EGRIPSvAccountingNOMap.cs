using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIPSvAccountingNOMap : Base.Map<EGRIPSvAccountingNO>
    {
        public EGRIPSvAccountingNOMap() : base("public", "ЕГРИП_СвУчетНО")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ИННФЛ).Column("ИННФЛ");
            Map(x => x.ДатаПостУч).Column("ДатаПостУч");
            Map(x => x.КодНО).Column("КодНО");
            Map(x => x.НаимНО).Column("НаимНО");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
