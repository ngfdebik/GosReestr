using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvShareOOOMap : Base.Map<EGRULSvShareOOO>
    {
        public EGRULSvShareOOOMap() : base("public", "ЕГРЮЛ_СвДоляООО")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.НоминСтоим).Column("НоминСтоим");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
