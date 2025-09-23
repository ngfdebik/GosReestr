using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRIPSvRegIPMap : Base.Map<EGRIPSvRegIP>
    {
        public EGRIPSvRegIPMap() : base("public", "ЕГРИП_СвРегИП")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ОГРНИП).Column("ОГРНИП");
            Map(x => x.ДатаОГРНИП).Column("ДатаОГРНИП");
            Map(x => x.РегНом).Column("РегНом");
            Map(x => x.ДатаРег).Column("ДатаРег");
            Map(x => x.НаимРО).Column("НаимРО");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.НаимЮЛПолн).Column("НаимЮЛПолн");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
