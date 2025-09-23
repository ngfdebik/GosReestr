using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvRegFSSMap : Base.Map<EGRULSvRegFSS>
    {
        public EGRULSvRegFSSMap() : base("public", "ЕГРЮЛ_СвРегФСС")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.РегНомФСС).Column("РегНомФСС");
            Map(x => x.ДатаРег).Column("ДатаРег");
            Map(x => x.КодФСС).Column("КодФСС");
            Map(x => x.НаимФСС).Column("НаимФСС");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
