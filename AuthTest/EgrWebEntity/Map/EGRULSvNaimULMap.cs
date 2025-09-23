using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvNaimULMap : Base.Map<EGRULSvNaimUL>
    {
        public EGRULSvNaimULMap() : base("public", "ЕГРЮЛ_СвНаимЮЛ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.НаимЮЛПолн).Column("НаимЮЛПолн");
            Map(x => x.НаимЮЛСокр).Column("НаимЮЛСокр");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
