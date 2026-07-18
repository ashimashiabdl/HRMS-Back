using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Function_Excel_Definition", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Function_Excel_Definition_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("ExcelDefinitionTypeId", Name = "IX_Function_Excel_Definition_ExcelDefinitionTypeId")]
[Microsoft.EntityFrameworkCore.Index("LeaveTypeId", Name = "IX_Function_Excel_Definition_LeaveTypeId")]
[Microsoft.EntityFrameworkCore.Index("MappedExcelColumnId", Name = "IX_Function_Excel_Definition_MappedExcelColumnId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Function_Excel_Definition_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelFunctionColumnId", Name = "IX_Function_Excel_Definition_PersonnelFunctionColumnId")]
public partial class FunctionExcelDefinition
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long MappedExcelColumnId { get; set; }

    public long? PersonnelFunctionColumnId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public bool IsMandatory { get; set; }

    public long ExcelDefinitionTypeId { get; set; }

    public bool IsFirstOrSecondSection { get; set; }

    public bool IsHourMinute { get; set; }

    public bool NeedMinuteNormalization { get; set; }

    public long EmployeeTypeId { get; set; }

    public bool IsLeave { get; set; }

    public long? LeaveTypeId { get; set; }

    public bool IsDaily { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("FunctionExcelDefinitions")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("ExcelDefinitionTypeId")]
    [InverseProperty("FunctionExcelDefinitions")]
    public virtual ExcelDefinitionType ExcelDefinitionType { get; set; } = null!;

    [ForeignKey("LeaveTypeId")]
    [InverseProperty("FunctionExcelDefinitions")]
    public virtual LeaveType? LeaveType { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("FunctionExcelDefinitions")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
