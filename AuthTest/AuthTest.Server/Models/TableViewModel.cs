using EgrWebEntity.ModelTable;

namespace AuthTest.Server.Models
{
    public class TableViewModel
    {
        public List<EGRIPSVFL> ЕГРИП_СвФЛ { get; set; }
        public List<EGRIPOKVED> ЕГРИП_ОКВЭД { get; set; }
        public List<EGRIPSvAdrMJ> ЕГРИП_СвАдрМЖ { get; set; }
        public List<EGRIPSvGrajd> ЕГРИП_СвГражд { get; set; }
        public List<EGRIPSvIP> ЕГРИП_СвИП { get; set; }
        public List<EGRIPSvLicense> ЕГРИП_СвЛицензия { get; set; }
        public List<EGRIPSvPrekras_> ЕГРИП_СвПрекращ { get; set; }
        public List<EGRIPSvRegIP> ЕГРИП_СвРегИП { get; set; }
        public List<EGRIPSvGegOrg> ЕГРИП_СвРегОрг { get; set; }
        public List<EGRIPSvRegPF> ЕГРИП_СвРегПФ { get; set; }
        public List<EGRIPSvRegFSS> ЕГРИП_СвРегФСС { get; set; }
        public List<EGRIPSvAccountingNO> ЕГРИП_СвУчетНО { get; set; }
        public List<EGRULOKVED> ЕГРЮЛ_ОКВЭД { get; set; }
        public List<EGRULSvAddressUL> ЕГРЮЛ_СвАдресЮЛ { get; set; }
        public List<EGRULSvDerjRegistryAO> ЕГРЮЛ_СвДержРеестрАО { get; set; }
        public List<EGRULSvShareOOO> ЕГРЮЛ_СвДоляООО { get; set; }
        public List<EGRULSvZapEGRUL> ЕГРЮЛ_СвЗапЕГРЮЛ { get; set; }
        public List<EGRULSvLicense> ЕГРЮЛ_СвЛицензия { get; set; }
        public List<EGRULSvNaimUL> ЕГРЮЛ_СвНаимЮЛ { get; set; }
        public List<EGRULSvObrUL> ЕГРЮЛ_СвОбрЮЛ { get; set; }
        public List<EGRULSvPodrazd> ЕГРЮЛ_СвПодразд { get; set; }
        public List<EGRULSvPredsh> ЕГРЮЛ_СвПредш { get; set; }
        public List<EGRULSvPreem> ЕГРЮЛ_СвПреем { get; set; }
        public List<EGRULSvPrekrUL> ЕГРЮЛ_СвПрекрЮЛ { get; set; }
        public List<EGRULSvRegOrg> ЕГРЮЛ_СвРегОрг { get; set; }
        public List<EGRULSvRegPF> ЕГРЮЛ_СвРегПФ { get; set; }
        public List<EGRULSvRegFSS> ЕГРЮЛ_СвРегФСС { get; set; }
        public List<EGRULSvReorg> ЕГРЮЛ_СвРеорг { get; set; }
        public List<EGRULSvStatus> ЕГРЮЛ_СвСтатус { get; set; }
        public List<EGRULSvAccountingNO> ЕГРЮЛ_СвУчетНО { get; set; }
        public List<EGRULSvFounder> ЕГРЮЛ_СвУчредит { get; set; }
        public List<EGRULSvUL> ЕГРЮЛ_СвЮЛ { get; set; }
        public List<EGRULSvedDoljnFL> ЕГРЮЛ_СведДолжнФЛ { get; set; }
        public List<IP> ИП { get; set; }
        public List<UL> ЮрЛицо { get; set; }
    }
}
