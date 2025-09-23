using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvStatusMap : Base.Map<EGRULSvStatus>
    {
        public EGRULSvStatusMap() : base("public", "ЕГРЮЛ_СвСтатус")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвСтатус).Column("СвСтатус");
            Map(x => x.КодСтатусЮЛ).Column("КодСтатусЮЛ");
            Map(x => x.НаимСтатусЮЛ).Column("НаимСтатусЮЛ");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ДатаРеш).Column("ДатаРеш");
            Map(x => x.НомерРеш).Column("НомерРеш");
            Map(x => x.ДатаПубликации).Column("ДатаПубликации");
            Map(x => x.НомерЖурнала).Column("НомерЖурнала");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
