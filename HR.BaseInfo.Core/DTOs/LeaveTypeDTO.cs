using HR.SharedKernel.Data;

namespace HR.BaseInfo.Core.DTOs
{
    public class LeaveTypeDTO : BaseDTO
    {
        public int? Duration { get; set; }
        public bool IsPaid { get; set; }
        public string? PaymentReference { get; set; }
        public string? LegalArticle { get; set; }
        public string? Description { get; set; }
    }
}


