using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvRegOrgMap : Base.Map<EGRULSvRegOrg>
    {
        public EGRULSvRegOrgMap() : base("public", "ЕГРЮЛ_СвРегОрг")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.КодНО).Column("КодНО");
            Map(x => x.НаимНО).Column("НаимНО");
            Map(x => x.АдрРО).Column("АдрРО");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
