using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgrWebEntity.ModelTable;

namespace EgrWebEntity.Map
{
    public class EGRIPSVFLMap : Base.Map<EGRIPSVFL>
    {
        public EGRIPSVFLMap() : base("public", "ЕГРИП_СвФЛ")
        {
            //Map(x => x.Id).Column("id");
            Map(x => x.Id).Column("Id");
            Map(x => x.Пол).Column("Пол");
            Map(x => x.Фамилия).Column("Фамилия");
            Map(x => x.Имя).Column("Имя");
            Map(x => x.Отчество).Column("Отчество");
            Map(x => x.ГРНИП).Column("ГРНИП");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ИП).Cascade.None();
        }
    }
}
