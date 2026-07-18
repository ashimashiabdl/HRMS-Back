using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using HR.SharedKernel.Data;
using HR.SharedKernel.Attribute;

namespace HR.BaseInfo.Core.Entities;

[Table("Base_Table_Value", Schema = "bas")]
public class BaseTableValue : BaseEntity
{
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    [ForeignKey("BaseTable")]
    public long BaseTableId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual BaseTable? BaseTable { get; set; }
    public int Order { get; set; }
    [StringLength(256)]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public string? Value { get; set; }
    public bool Visible { get; set; }
    [StringLength(256)]
    public string? Description { get; set; }

}
