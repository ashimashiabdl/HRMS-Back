using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;

namespace HR.BaseInfo.Core.DTOs;

public class UserFileUploadDTO : BaseDTO
{
    public long FileId { get; set; }

    [StringLength(2000)]
    [Required]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    public string? OrganizationName { get; set; }

    public long? UploadedByUserId { get; set; }
    
    public string? UploadedByUserName { get; set; }
    
    public string? FileName { get; set; }
    
    public long? FileSize { get; set; }
}

