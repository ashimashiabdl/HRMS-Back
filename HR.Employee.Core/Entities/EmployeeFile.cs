using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Employee.Core.Entities
{
    [Table("Employee_File", Schema = "emp")]
    public class EmployeeFile : SharedKernel.Data.BaseEntity, IOrganisationChartId, IignoreDateRangeValidation
    {
            public EmployeeFile()
    {
        IPAddress = string.Empty;
        CreatedBy = string.Empty;
        LastModifiedBy = string.Empty;
        IsDeleted = false;
    }
public const long OtherFileGroupId = 21740;
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public long FileGroupId { get; set; }
        public virtual BaseTableValue FileGroup { get; set; }
        [StringLength(200)]
        public string? OtherFileGroupName { get; set; } = string.Empty;
        [ForeignKey("File")]
        public long? FileId { get; set; }
        public virtual File? File { get; set; }
        [StringLength(500)]
        public string? Name { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; } = string.Empty;
        public bool IsImage { get; set; } = false;
        [NotMapped]
        private new string title { get; set; } = string.Empty;
    }
}
