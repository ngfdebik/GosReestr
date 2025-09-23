using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class ULMap : Base.Map<UL>
    {
        public ULMap() : base("public", "ЮрЛицо")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ДатаОГРН).Column("ДатаОГРН");
            Map(x => x.ОГРН).Column("ОГРН");
            Map(x => x.ИНН).Column("ИНН");
            Map(x => x.КПП).Column("КПП");
            Map(x => x.СпрОПФ).Column("СпрОПФ");
            Map(x => x.КодОПФ).Column("КодОПФ");
            Map(x => x.ПолнНаимОПФ).Column("ПолнНаимОПФ");
            Map(x => x.ИдДок).Column("ИдДок");
            HasMany(x => x.document).KeyColumn("idЮЛ").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_ОКВЭД).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвАдресЮЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвДержРеестрАО).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвДоляООО).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СведДолжнФЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвЗапЕГРЮЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвПреем).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвЛицензия).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвНаимЮЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвОбрЮЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвПодразд).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвПредш).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвПрекрЮЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвРегОрг).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвРегПФ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвРегФСС).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвРеорг).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвСтатус).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвУстКап).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвУчетНО).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвУчредит).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРЮЛ_СвЮЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
        }
    }
}
