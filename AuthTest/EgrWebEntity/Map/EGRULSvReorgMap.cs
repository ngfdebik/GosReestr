using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgrWebEntity.ModelTable;

namespace EgrWebEntity.Map
{
    public class EGRULSvReorgMap : Base.Map<EGRULSvReorg>
    {
        public EGRULSvReorgMap() : base("public", "ЕГРЮЛ_СвРеорг")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвРеорг).Column("СвРеорг");
            Map(x => x.КодСтатусЮЛ).Column("КодСтатусЮЛ");
            Map(x => x.НаимСтатусЮЛ).Column("НаимСтатусЮЛ");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.НаимЮЛПолн).Column("НаимЮЛПолн");
            Map(x => x.СостЮЛпосле).Column("СостЮЛпосле");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
