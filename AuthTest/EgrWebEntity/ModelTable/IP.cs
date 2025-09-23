using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgrWebEntity.ModelTable
{
    public class IP : FluentNHibernate.Data.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string? ОГРНИП { get; set; }
        public DateTime? ДатаОГРНИП { get; set; }
        public string? ИНН { get; set; }
        public string? КодВидИП { get; set; }
        public string? НаимВидИП { get; set; }
        public string? ИдДок { get; set; }
        public virtual ICollection<Document>? document { get; set; }
        public virtual ICollection<EGRIPOKVED>? ЕГРИП_ОКВЭД { get; set; }
        public virtual ICollection<EGRIPSvAccountingNO>? ЕГРИП_СвУчетНО { get; set; }
        public virtual ICollection<EGRIPSvAdrMJ>? ЕГРИП_СвАдрМЖ { get; set; }
        public virtual ICollection<EGRIPSVFL>? ЕГРИП_СвФЛ { get; set; }
        public virtual ICollection<EGRIPSvGegOrg>? ЕГРИП_СвРегОрг { get; set; }
        public virtual ICollection<EGRIPSvGrajd> ЕГРИП_СвГражд { get; set; }
        public virtual ICollection<EGRIPSvIP>? ЕГРИП_СвИП { get; set; }
        public virtual ICollection<EGRIPSvLicense> ЕГРИП_СвЛицензия { get; set; }
        public virtual ICollection<EGRIPSvPrekras_> ЕГРИП_СвПрекращ { get; set; }
        public virtual ICollection<EGRIPSvRegFSS> ЕГРИП_СвРегФСС { get; set; }
        public virtual ICollection<EGRIPSvRegIP> ЕГРИП_СвРегИП { get; set; }
        public virtual ICollection<EGRIPSvRegPF> ЕГРИП_СвРегПФ { get; set; }

    }
}
