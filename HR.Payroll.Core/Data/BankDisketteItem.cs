using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Bank_Diskette_Item", Schema = "Payroll")]
    public class BankDisketteItem : BaseEntity
    {
        [ForeignKey("BankDiskette")]
        public long BankDisketteId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BankDiskette? BankDiskette { get; set; }

        [ForeignKey("BankDisketteGroupAndFile")]
        public long? BankDisketteGroupAndFileId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual BankDisketteGroupAndFile? BankDisketteGroupAndFile { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual Employee.Core.Entities.Employee? Employee { get; set; }
        [ForeignKey("Fiche")]
        public long? FicheId { get; set; }
        public virtual Fiche? Fiche { get; set; }

        //[ForeignKey("PersonnelPayment")]
        //public long? PersonnelPaymentId { get; set; }
        //public virtual PersonnelPayment? PersonnelPayment { get; set; }

        //[ForeignKey("OrganCodes")]
        //public long? OrganCodesId { get; set; }
        //public virtual OrganisationChart? OrganCodes { get; set; }

        [ForeignKey("CostCenter")]
        public long? CostCenterId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrganisationChart? CostCenter { get; set; }

        [StringLength(128)]
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public string? AccountNo { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public long Amount { get; set; }

        [NotMapped]
        private new string title { get; set; }
    }
}
