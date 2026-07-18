using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data;

[Table("Personnel_Payment", Schema = "Payroll")]
public class PersonnelPayment : BaseEntity
{
    [ForeignKey("PaymentType")]
    public long PaymentTypeId { get; set; }
    public virtual PaymentType? PaymentType { get; set; }
    [ForeignKey("BankBranch")]
    public long? BankBranchId { get; set; }
    public virtual BankBranch? BankBranch { get; set; }
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

    [ForeignKey("Fiche")]
    public long FicheId { get; set; }
    public virtual Fiche? Fiche { get; set; }

    [ForeignKey("PersonnelFicheItem")]
    public long PersonnelFicheItemId { get; set; }
    public virtual PersonnelFicheItem? PersonnelFicheItem { get; set; }
    /// <summary>
    /// „»·€
    /// </summary>
    public long Amount { get; set; }
    /// <summary>
    ///  «—ÌŒ Å—œ«Œ 
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }
    [StringLength(256)]
    public string? Description { get; set; }

}
