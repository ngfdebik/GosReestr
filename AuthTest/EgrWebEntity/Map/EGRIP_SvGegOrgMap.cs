using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIP_SvGegOrgMap : Base.Map<EGRIPSvGegOrg>
    {
        public EGRIP_SvGegOrgMap() : base("public", "ЕГРИП_СвРегОрг")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.КодНО).Column("КодНО");
            Map(x => x.НаимНО).Column("НаимНО");
            Map(x => x.АдрРО).Column("АдресРО");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
