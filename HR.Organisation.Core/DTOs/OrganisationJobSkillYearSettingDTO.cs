using HR.Organisation.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Organisation.Core.DTOs;

public class OrganisationJobSkillYearSettingDTO : BaseDTO
{
    public long? OrganisationChartId { get; set; }
    public string? OrganisationChart { get; set; }
    public long OrganizationJobId { get; set; }
    public string? OrganizationJob { get; set; }
    public long SkillLevelId { get; set; }
    public string? SkillLevel { get; set; }
    public int Year { get; set; }
    public int Value { get; set; }
  
}

