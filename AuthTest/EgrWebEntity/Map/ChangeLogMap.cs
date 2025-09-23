using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class ChangeLogMap : Base.Map<ChangeLog>
    {
        public ChangeLogMap() : base("public", "ИсторияИзменений")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.Таблица).Column("Таблица");
            Map(x => x.Столбец).Column("Столбец");
            Map(x => x.ЗначениеДо).Column("ЗначениеДо");
            Map(x => x.ЗначениеПосле).Column("ЗначениеПосле");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.ДатаИзменения).Column("ДатаИзменения");
        }
    }
}
