using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvAccountingNOMap : Base.Map<EGRULSvAccountingNO>
    {
        public EGRULSvAccountingNOMap() : base("public", "ЕГРЮЛ_СвУчетНО")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.КПП).Column("КПП");
            Map(x => x.ДатаПостУч).Column("ДатаПостУч");
            Map(x => x.КодНО).Column("КодНО");
            Map(x => x.НаимНО).Column("НаимНО");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
