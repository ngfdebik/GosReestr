using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{

    public class EGRULSvZapEGRULMap : Base.Map<EGRULSvZapEGRUL>
    {
        public EGRULSvZapEGRULMap() : base("public", "ЕГРЮЛ_СвЗапЕГРЮЛ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ИдЗап).Column("ИдЗап");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗап).Column("ДатаЗап");
            Map(x => x.КодСПВЗ).Column("КодСПВЗ");
            Map(x => x.НаимВидЗап).Column("НаимВидЗап");
            Map(x => x.КодНО).Column("КодНО");
            Map(x => x.НаимНО).Column("НаимНО");
            Map(x => x.СведПредДок).Column("СведПредДок");
            Map(x => x.НаимДок).Column("НаимДок");
            Map(x => x.НомДок).Column("НомДок");
            Map(x => x.ДатаДок).Column("ДатаДок");
            Map(x => x.Серия).Column("Серия");
            Map(x => x.Номер).Column("Номер");
            Map(x => x.ДатаВыдСвид).Column("ДатаВыдСвид");
            Map(x => x.СвСтатусЗап).Column("СвСтатусЗап");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
