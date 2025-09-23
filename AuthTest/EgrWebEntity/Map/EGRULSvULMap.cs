using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvULMap : Base.Map<EGRULSvUL>
    {
        public EGRULSvULMap() : base("public", "ЕГРЮЛ_СвЮЛ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ДатаВып).Column("ДатаВып");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ДатаОГРН).Column("ДатаОГРН");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.КПП).Column("КПП");
            Map(x => x.СпрОПФ).Column("СпрОПФ");
            Map(x => x.КодОПФ).Column("КодОПФ");
            Map(x => x.ПолнНаимОПФ).Column("ПолнНаимОПФ");
            Map(x => x.НаимСокр).Column("НаимСокр");
            Map(x => x.ОКВЭДОсн).Column("ОКВЭДОсн");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
