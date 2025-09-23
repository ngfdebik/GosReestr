using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvObrULMap : Base.Map<EGRULSvObrUL>
    {
        public EGRULSvObrULMap() : base("public", "ЕГРЮЛ_СвОбрЮЛ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ДатаОГРН).Column("ДатаОГРН");
            Map(x => x.КодСпОбрЮЛ).Column("КодСпОбрЮЛ");
            Map(x => x.НаимСпОбрЮЛ).Column("НаимСпОбрЮЛ");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.РегНом).Column("РегНом");
            Map(x => x.ДатаРег).Column("ДатаРег");
            Map(x => x.НаимРО).Column("НаимРО");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
