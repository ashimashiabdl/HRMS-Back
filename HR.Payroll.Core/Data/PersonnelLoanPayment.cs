using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Personnel_Loan_Payment", Schema = "Payroll")]
    public class PersonnelLoanPayment : BaseEntity
    {
        [ForeignKey("Fiche")]
        public long FicheId { get; set; }
        public virtual Fiche? Fiche { get; set; }
        [ForeignKey("PersonnelLoan")]
        public long PersonnelLoanId { get; set; }
        public virtual PersonnelLoan? PersonnelLoan { get; set; }
        public bool IsPaid { get; set; }
        public long PaymentAmount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PaymentDate { get; set; }
        [ForeignKey("PaymentType")]
        public long PaymentTypeId { get; set; }
        public virtual PaymentType? PaymentType { get; set; }
        [NotMapped]
        private new string title { get; set; }
      
    }
}