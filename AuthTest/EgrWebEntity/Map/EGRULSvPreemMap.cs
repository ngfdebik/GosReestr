using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvPreemMap : Base.Map<EGRULSvPreem>
    {
        public EGRULSvPreemMap() : base("public", "ЕГРЮЛ_СвПреем")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.НаимЮЛПолн).Column("НаимЮЛПолн");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
