using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities
{
    [Table("Organisation_Chart", Schema = "Org")]
    public class OrganisationChart : HR.SharedKernel.Data.BaseEntity
    {
        public OrganisationChart()
        {
            title = string.Empty;
            IPAddress = string.Empty;
            CreatedBy = string.Empty;
            LastModifiedBy = string.Empty;
            IsDeleted = false;
        }

        [ForeignKey("ParentOrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long? ParentOrganisationChartId { get; set; }
        public virtual OrganisationChart? ParentOrganisationChart { get; set; }
        [ForeignKey("OrganizationType")]
        public long? OrganizationTypeId { get; set; }
        public virtual OrganizationType? OrganizationType { get; set; }

        [ForeignKey("Place")]
        public long? PlaceId { get; set; }
        public virtual Places? Place { get; set; }

        public bool IsPayLocation { get; set; } = false;
        public bool? IsCostCenter { get; set; } = false;

        public long? OrgTypeId { get; set; }

        [StringLength(128)]
        public string? LetterCode { get; set; } = string.Empty;
        [StringLength(128)]
        public string? Code { get; set; } = string.Empty;
        [StringLength(128)]
        public string? SystemCode { get; set; } = string.Empty;
        [StringLength(128)]
        public string? Rank { get; set; } = string.Empty;

        /// <summary>شناسه یکتا</summary>
        [StringLength(128)]
        public string? UniqueIdentifier { get; set; } = string.Empty;

        /// <summary>علامت اختصاری</summary>
        [StringLength(128)]
        public string? AbbreviationMark { get; set; } = string.Empty;

        /// <summary>شرح</summary>
        [StringLength(500)]
        public string? Description { get; set; } = string.Empty;

        /// <summary>نام اختصار</summary>
        [StringLength(128)]
        public string? ShortName { get; set; } = string.Empty;

        public int? Order { get; set; } = 0;
        public bool? IsRegister { get; set; } = false;
        public bool? IsIndependentOrg { get; set; } = false;
        public bool? IsOrg { get; set; } = false;

        public bool IsRoot { get; set; } = false;

        public bool IsApproved { get; set; } = false;
        /// <summary>
        /// From base table value table -- > 40275
        /// </summary>
        public long? TaxNodeStatusId { get; set; }
    }
}
