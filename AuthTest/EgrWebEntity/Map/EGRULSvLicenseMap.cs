using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvLicenseMap : Base.Map<EGRULSvLicense>
    {
        public EGRULSvLicenseMap() : base("public", "ЕГРЮЛ_СвЛицензия")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.НомЛиц).Column("НомЛиц");
            Map(x => x.ДатаЛиц).Column("ДатаЛиц");
            Map(x => x.ДатаНачЛиц).Column("ДатаНачЛиц");
            Map(x => x.НаимЛицВидДеят).Column("НаимЛицВидДеят");
            Map(x => x.ЛицОргВыдЛиц).Column("ЛицОргВыдЛиц");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.ДатаОкончЛиц).Column("ДатаОкончЛиц");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
