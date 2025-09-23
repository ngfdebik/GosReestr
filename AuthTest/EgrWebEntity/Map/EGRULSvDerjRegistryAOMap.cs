using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvDerjRegistryAOMap : Base.Map<EGRULSvDerjRegistryAO>
    {
        public EGRULSvDerjRegistryAOMap() : base("public", "ЕГРЮЛ_СвДержРеестрАО")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвДержРеестрАО).Column("СвДержРеестрАО");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.НаимЮЛПолн).Column("КодАдрКладр");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
