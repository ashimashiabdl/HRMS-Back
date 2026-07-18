using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities;

[Table("Tamin_Insurance_JobList", Schema = "bas")]
public class TaminInsuranceJobList : HR.SharedKernel.Data.BaseEntity
{
    [MaxLength(6)]
    public string? Code { get; set; }
}
