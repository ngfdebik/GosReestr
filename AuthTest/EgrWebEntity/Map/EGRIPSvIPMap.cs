using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIPSvIPMap : Base.Map<EGRIPSvIP>
    {
        public EGRIPSvIPMap() : base("public", "ЕГРИП_СвИП")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ДатаВып).Column("ДатаВып");
            Map(x => x.ОГРН).Column("ОГРНИП");
            Map(x => x.ДатаОГРН).Column("ДатаОГРН");
            Map(x => x.ИНН).Column("ИННФЛ");
            Map(x => x.КодВидИП).Column("КодВидИП");
            Map(x => x.НаимВидИП).Column("НаимВидИП");
            Map(x => x.НаимСокр).Column("НаимСокр");
            Map(x => x.ОКВЭДОсн).Column("ОКВЭДОсн");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
