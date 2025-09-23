using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvRegPFMap : Base.Map<EGRULSvRegPF>
    {
        public EGRULSvRegPFMap() : base("public", "ЕГРЮЛ_СвРегПФ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.РегНомПФ).Column("РегНомПФ");
            Map(x => x.ДатаРег).Column("ДатаРег");
            Map(x => x.КодПФ).Column("НаимПФ");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
