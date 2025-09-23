using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class UL : FluentNHibernate.Data.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public DateTime? ДатаОГРН { get; set; }
        public string? ОГРН { get; set; }
        public string? ИНН { get; set; }
        public string? КПП { get; set; }
        public string? СпрОПФ { get; set; }
        public string? КодОПФ { get; set; }
        public string? ПолнНаимОПФ { get; set; }
        public string? ИдДок { get; set; }
        public List<Document> document { get; set; }
        public List<EGRULOKVED>? ЕГРЮЛ_ОКВЭД { get; set; }
        public List<EGRULSvAccountingNO>? ЕГРЮЛ_СвУчетНО { get; set; }
        public List<EGRULSvAddressUL>? ЕГРЮЛ_СвАдресЮЛ { get; set; }
        public List<EGRULSvDerjRegistryAO>? ЕГРЮЛ_СвДержРеестрАО { get; set; }
        public List<EGRULSvedDoljnFL>? ЕГРЮЛ_СведДолжнФЛ { get; set; }
        public List<EGRULSvFounder>? ЕГРЮЛ_СвУчредит { get; set; }
        public List<EGRULSvLicense>? ЕГРЮЛ_СвЛицензия { get; set; }
        public List<EGRULSvNaimUL>? ЕГРЮЛ_СвНаимЮЛ { get; set; }
        public List<EGRULSvObrUL>? ЕГРЮЛ_СвОбрЮЛ { get; set; }
        public List<EGRULSvPodrazd>? ЕГРЮЛ_СвПодразд { get; set; }
        public List<EGRULSvPredsh>? ЕГРЮЛ_СвПредш { get; set;  }
        public List<EGRULSvPreem>? ЕГРЮЛ_СвПреем { get; set; }
        public List<EGRULSvPrekrUL>? ЕГРЮЛ_СвПрекрЮЛ { get; set; }
        public List<EGRULSvRegFSS>? ЕГРЮЛ_СвРегФСС { get; set; }
        public List<EGRULSvRegOrg>? ЕГРЮЛ_СвРегОрг { get; set; }
        public List<EGRULSvRegPF>? ЕГРЮЛ_СвРегПФ { get; set; }
        public List<EGRULSvReorg>? ЕГРЮЛ_СвРеорг { get; set; }
        public List<EGRULSvShareOOO>? ЕГРЮЛ_СвДоляООО { get; set; }
        public List<EGRULSvStatus>? ЕГРЮЛ_СвСтатус { get; set; }
        public List<EGRULSvUL>? ЕГРЮЛ_СвЮЛ { get; set; }
        public List<EGRULSvUstKap>? ЕГРЮЛ_СвУстКап { get; set; }
        public List<EGRULSvZapEGRUL>? ЕГРЮЛ_СвЗапЕГРЮЛ { get; set; }
    }
}
