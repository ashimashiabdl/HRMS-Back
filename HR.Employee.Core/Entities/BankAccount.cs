using HR.BaseInfo.Core.Entities;
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

namespace HR.Employee.Core.Entities;

[Table("Bank_Account", Schema = "emp")]
public class BankAccount : HR.SharedKernel.Data.BaseEntity , IEmployeeHistoryOrganisationChartId , IignoreDateRangeValidation
{
        public BankAccount()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
    public long? OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
    [StringLength(100)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? AccountNumber { get; set; } = string.Empty;
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTableValue? AccountType { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public int? Priority { get; set; } = 0;
    public int? FromPrice { get; set; } = 0;
    public int? ToPrice { get; set; } = 0;
    public bool Status { get; set; } = false;
    [StringLength(512)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? Description { get; set; } = string.Empty;
    public long? BankId { get; set; }
    public int? BankBranchId { get; set; } = 0;
    public int? OldId { get; set; } = 0;

    [StringLength(50)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? BonCardNumber { get; set; } = string.Empty;
    [StringLength(50)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? CardNumber { get; set; } = string.Empty;
    [StringLength(50)]
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public string? ShabaNumber { get; set; } = string.Empty;
    public long? AccountTypeId { get; set; }

    [NotMapped]
    private new string title { get; set; } = string.Empty;

}
