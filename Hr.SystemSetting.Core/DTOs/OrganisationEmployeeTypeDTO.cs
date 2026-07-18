using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs;

public class OrganisationEmployeeTypeDTO : BaseDTO
{
    public long EmployeeTypeId { get; set; }
    public long EmployeeTypeGroupId { get; set; }
    public string? EmployeeTypeTitle { get; set; }
    public string? EmployeeTypeGroupTitle { get; set; }
    /// <summary>
    /// جدول 7 نوع استخدام مالیات
    /// </summary>
    public long? TaxBaseTable7Id { get; set; }

    /// <summary>
    /// کد تفضیل نوع استخدام
    /// </summary>
    public string? EmployeeTypeFinancialCode { get; set; }
}
