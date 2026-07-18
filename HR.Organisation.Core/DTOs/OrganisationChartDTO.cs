using HR.Organisation.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Organisation.Core.DTOs
{
    public class OrganisationChartDTO : BaseDTO
    {
      
        public long? ParentOrganisationChartId { get; set; }
        public string? ParentOrganisationChart { get; set; }
        public long? OrganizationTypeId { get; set; }
        public long? tempFileId { get; set; }
        public string? OrganizationType { get; set; }
        public long? PlaceId { get; set; }
        public string? Place { get; set; }
        public bool IsPayLocation { get; set; }
        public bool? IsCostCenter { get; set; }
        public long? OrgTypeId { get; set; }
        public string? OrgType { get; set; }
        [StringLength(128)]
        public string? LetterCode { get; set; }
        [StringLength(128)]
        public string? Code { get; set; }
        [StringLength(128)]
        public string? SystemCode { get; set; }
        [StringLength(128)]
        public string? Rank { get; set; }
        [StringLength(128)]
        public string? UniqueIdentifier { get; set; }
        [StringLength(128)]
        public string? AbbreviationMark { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(128)]
        public string? ShortName { get; set; }
        public int? Order { get; set; }
        public bool? IsRegister { get; set; }
        public bool? IsIndependentOrg { get; set; }
        public bool? IsOrg { get; set; }
        public bool IsApproved { get; set; }
        public long? TaxNodeStatusId { get; set; }
    }
}
