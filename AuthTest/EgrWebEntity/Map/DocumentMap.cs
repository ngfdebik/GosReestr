using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class DocumentMap : Base.Map<Document>
    {
        public DocumentMap() : base("public", "Документ")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ИдДок).Column("ИдДок");
            Map(x => x.ДатаЗагрузки).Column("ДатаЗагрузки");
            Map(x => x.idЮЛ).Column("idЮЛ");
            Map(x => x.idИП).Column("idИП");
            References(x => x.ИП).Cascade.None();
            References(x => x.ЮрЛицо).Cascade.None();          
        }
    }
}
