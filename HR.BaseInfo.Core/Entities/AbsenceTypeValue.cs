using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities;

/// <summary>
/// نوع عدم حضور
/// </summary>
[Table("Absence_Type_Value", Schema = "bas")]
public class AbsenceTypeValue : BaseEntity
{
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [ForeignKey("AbsenceType")]
    public long AbsenceTypeId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual AbsenceType? AbsenceType { get; set; }
}
