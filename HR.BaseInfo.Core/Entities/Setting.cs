using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Setting", Schema = "bas")]
public class Setting : BaseEntity
{
    [StringLength(128)]
    public string? Value { get; set; }
}
