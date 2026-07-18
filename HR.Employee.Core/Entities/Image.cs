using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities;

[Table("Image", Schema = "emp")]
public class Image : HR.SharedKernel.Data.BaseEntity, IEmployeeHistoryOrganisationChartId
{
        public Image()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
[ForeignKey("OrganisationChart")]
    public long? OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("Employee")]
    [Index(IsUnique = true)]
    public long EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }
    public bool IsDefault { get; set; } = false;
    public long? ImageTypeId { get; set; }
    public virtual BaseTableValue? ImageType { get; set; }

    public byte[] ImageData { get; set; } = null!;
    [NotMapped]
    private new string title { get; set; } = string.Empty;

 
}
