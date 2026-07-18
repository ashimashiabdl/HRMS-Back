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
[Table("Absence_Type", Schema = "bas")]
public class AbsenceType : BaseEntity
{
}
