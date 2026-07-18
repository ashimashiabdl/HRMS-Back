using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("User_File_Upload", Schema = "bas")]
public class UserFileUpload : BaseEntity
{
    [ForeignKey(nameof(File))]
    public long FileId { get; set; }
    
    public virtual File? File { get; set; }

    [IsEffectiveInGenericSearch(IsEffective = true)]
    [StringLength(2000)]
    [Required]
    public string Description { get; set; } = string.Empty;

    [StringLength(500)]
    public string? OrganizationName { get; set; }

    [ForeignKey("UploadedByUser")]
    public long? UploadedByUserId { get; set; }
    
    // Navigation property - Note: AspNetUsers is in Identity schema, so this is a soft reference
    // The actual relationship should be configured in DbContext if needed
}

