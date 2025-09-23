using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class EGRULSvFounderMap : Base.Map<EGRULSvFounder>
    {
        public EGRULSvFounderMap() : base("public", "ЕГРЮЛ_СвУчредит")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.СвУчредит).Column("СвУчредит");
            Map(x => x.УчрФЛ).Column("УчрФЛ");
            Map(x => x.ГРН).Column("ГРН");
            Map(x => x.ДатаЗаписи).Column("ДатаЗаписи");
            Map(x => x.Фамилия).Column("Фамилия");
            Map(x => x.Имя).Column("Имя");
            Map(x => x.Отчество).Column("Отчество");
            Map(x => x.ИННФЛ).Column("ИННФЛ");
            Map(x => x.НоминСтоим).Column("НоминСтоим");
            Map(x => x.РазмерДоли).Column("РазмерДоли");
            Map(x => x.Процент).Column("Процент");
            Map(x => x.УчрЮЛРос).Column("УчрЮЛРос");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.НаимЮЛПолн).Column("НаимЮЛПолн");
            Map(x => x.УчрРФСубМО).Column("УчрРФСубМО");
            Map(x => x.КодУчрРФСубМО).Column("КодУчрРФСубМО");
            Map(x => x.НаимМО).Column("НаимМО");
            Map(x => x.КодРегион).Column("КодРегион");
            Map(x => x.НаимРегион).Column("НаимРегион");
            Map(x => x.СвОргОсущПр).Column("СвОргОсущПр");
            Map(x => x.ДробДесят).Column("ДробДесят");
            Map(x => x.ПризнНедДанУчр).Column("ПризнНедДанУчр");
            Map(x => x.ТекстНедДанУчр).Column("ТекстНедДанУчр");
            Map(x => x.Числит).Column("Числит");
            Map(x => x.Знаменат).Column("Знаменат");
            Map(x => x.idЛицо).Column("idЛицо");
            References(x => x.ЮрЛицо).Cascade.None();
        }
    }
}
