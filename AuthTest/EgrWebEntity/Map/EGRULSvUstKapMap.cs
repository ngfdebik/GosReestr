using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvUstKapMap : Base.Map<EGRULSvUstKap>
    {
        public EGRULSvUstKapMap() : base("public", "ЕГРЮЛ_СвУстКап")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.НаимВидКап).Column("НаимВидКап");
            Map(x => x.СумКап).Column("СумКап");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
