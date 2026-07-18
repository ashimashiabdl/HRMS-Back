using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;

[Table("Employee_Deduction_Payment", Schema = "Payroll")]
public class EmployeeDeductionPayment : BaseEntity
{
    [ForeignKey("Fiche")]
    public long FicheId { get; set; }
    public virtual Fiche? Fiche { get; set; }
    [ForeignKey("EmployeeDeduction")]
    public long EmployeeDeductionId { get; set; }
    public virtual EmployeeDeduction? EmployeeDeduction { get; set; }
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
