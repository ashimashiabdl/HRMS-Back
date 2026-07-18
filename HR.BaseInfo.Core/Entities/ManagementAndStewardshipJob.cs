using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.BaseInfo.Core.Entities;
[Table("Management_And_StewardshipJob", Schema = "bas")]
public class ManagementAndStewardshipJob : BaseEntity
{
    [MaxLength(5)]
    public string? Code { get; set; }
}
