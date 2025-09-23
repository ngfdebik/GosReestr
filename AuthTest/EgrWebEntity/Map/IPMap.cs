using EgrWebEntity.ModelTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.Map
{
    public class IPMap : Base.Map<IP>
    {
        public IPMap() : base("public", "ИП")
        {
            Map(x => x.Id).Column("Id");
            Map(x => x.ОГРНИП).Column("ОГРНИП");
            Map(x => x.ДатаОГРНИП).Column("ДатаОГРНИП");
            Map(x => x.ИНН).Column("ИННФЛ");
            Map(x => x.КодВидИП).Column("КодВидИП");
            Map(x => x.НаимВидИП).Column("НаимВидИП");
            Map(x => x.ИдДок).Column("ИдДок");
            HasMany(x => x.document).KeyColumn("idИП").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_ОКВЭД).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвАдрМЖ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвГражд).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвИП).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвЛицензия).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвПрекращ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвРегИП).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвРегОрг).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвРегПФ).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвРегФСС).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвУчетНО).KeyColumn("idЛицо").Inverse().Cascade.None();
            HasMany(x => x.ЕГРИП_СвФЛ).KeyColumn("idЛицо").Inverse().Cascade.None();
        }
    }
}
