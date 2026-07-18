using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;

[Table("Personnel_Loan", Schema = "Payroll")]
public class PersonnelLoan : BaseEntity,IOrganisationChartId , IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
    [ForeignKey("OrganisationChart")]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("LoanType")]
    public long LoanTypeId { get; set; }
    public virtual LoanType? LoanType { get; set; }
    [ForeignKey("BankBranch")]
    public long? BankBranchId { get; set; }
    public virtual BankBranch? BankBranch { get; set; }
    [ForeignKey("StartDeductPaymentPeriod")]
    public long StartDeductPaymentPeriodId { get; set; }
    public virtual PaymentPeriod StartDeductPaymentPeriod { get; set; }
    //public int? InstallmentCount { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }
    //public long RemainAmount { get; set; }
    public bool IsActive { get; set; }
    [StringLength(128)]
    public string? LoanPaymentDocNo { get; set; }
    [StringLength(128)]
    public string? LoanPaymentDocDesc { get; set; }
    public long? AllAmount { get; set; }
    public long? InstallmentAmount { get; set; }
    [StringLength(50)]
    public string? Code { get; set; }
    //[StringLength(50)]
    //public string? ReciverId { get; set; }
    //public bool? ReciverTypeId { get; set; }
    [StringLength(128)]
    public string? AccountNumber { get; set; }
    [StringLength(128)]
    public string? ReciverDesc { get; set; }
    public bool? AutoReceive { get; set; }
    [StringLength(128)]
    public string? ShebaNo { get; set; }
    /// <summary>
    /// خرده باقیمانده وام در اولین قسط کم شود
    /// </summary>
    public bool RemainingCrumbsAtFirst { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
