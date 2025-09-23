using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvPrekrULMap : Base.Map<EGRULSvPrekrUL>
    {
        public EGRULSvPrekrULMap() : base("public", "ЕГРЮЛ_СвПрекрЮЛ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ДатаПрекрЮЛ).Column("ДатаПрекрЮЛ");
            Map(x => x.КодСпПрекрЮЛ).Column("КодСпПрекрЮЛ");
            Map(x => x.НаимСпПрекрЮЛ).Column("НаимСпПрекрЮЛ");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
