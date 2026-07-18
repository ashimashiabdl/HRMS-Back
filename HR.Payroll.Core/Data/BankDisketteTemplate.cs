using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Bank_Diskette_Template", Schema = "Payroll")]
    public class BankDisketteTemplate : BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Bank")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long BankId { get; set; }
        public virtual Bank? Bank { get; set; }
        [StringLength(1024)]
        public string? FileName { get; set; }
        [StringLength(64)]
        public string? FileExtension { get; set; }
        [StringLength(8096)]
        public string? FileHeader { get; set; }
        [StringLength(8096)]
        public string? FileEnd { get; set; }
        public bool HasLineStartCharacter { get; set; }
        [StringLength(256)]
        public string? LineStartCharacter { get; set; }
        public bool HasLineEndCharacter { get; set; }
        [StringLength(256)]
        public string? LineEndCharacter { get; set; }
        public bool HasLineDelimiterCharacter { get; set; }
        [StringLength(256)]
        public string? LineDelimiterCharacter { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
