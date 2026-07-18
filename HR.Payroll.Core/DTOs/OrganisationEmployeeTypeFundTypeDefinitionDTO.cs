using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HR.Payroll.Core.DTOs;

public class OrganisationEmployeeTypeFundTypeDefinitionDTO : BaseDTO
{
    public long OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long EmployeeTypeId { get; set; }
    public string? EmployeeType { get; set; }
    /// <summary>
    /// درصد کارمند
    /// </summary>
    public int EmployeePercent { get; set; }
    public long EmployeeWageItemId { get; set; }
    public string? EmployeeWageItem { get; set; }
    public long EmployerWageItemId { get; set; }
    public string? EmployerWageItem { get; set; }
    public long? EmployeeFormulaId { get; set; }
    public string? EmployeeFormula { get; set; }
    public long? EmployerFormulaId { get; set; }
    public string? EmployerFormula { get; set; }
    public long FundTypeId { get; set; }
    public string? FundType { get; set; }
}

