using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgrWebEntity.ModelTable;

namespace EgrWebEntity.Map
{
    public class EGRULSvPredshMap : Base.Map<EGRULSvPredsh>
    {
        public EGRULSvPredshMap() : base("public", "ЕГРЮЛ_СвПредш")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.НаимЮЛПолн).Column("НаимЮЛПОлн");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
