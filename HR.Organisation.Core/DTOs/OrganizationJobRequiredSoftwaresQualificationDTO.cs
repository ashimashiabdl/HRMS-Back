using HR.SharedKernel.Data;
namespace HR.Organisation.Core.DTOs
{
    public class OrganizationJobRequiredSoftwaresQualificationDTO : BaseDTO
    {
        public long? OrganizationJobId { get; set; }
        public string? OrganizationJob { get; set; }
        public long SoftwareTypeId { get; set; }
        public string? SoftwareType { get; set; }
        public long MasteryLevelTypeId { get; set; }
        public string? MasteryLevelType { get; set; }
        public long SoftwareId { get; set; }
        public string? Software { get; set; }
    }
}
