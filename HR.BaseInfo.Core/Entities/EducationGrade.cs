using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities;

/// <summary>
/// مقطع تحصیلی
/// </summary>
[Table("Education_Grade", Schema = "bas")]
public class EducationGrade : BaseEntity
{
    /// <summary>
    /// کد متناظر در جدول پایه مالیاتی
    /// </summary>
    [StringLength(1)]
    public string? TaxCode { get; set; }
    [StringLength(256)]
    public string? Description { get; set; }
    public int Order { get; set; }
    public int OldID { get; set; }
}
