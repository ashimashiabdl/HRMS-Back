using System.Collections.Generic;

namespace HR.SharedKernel.Upload;

/// <summary>
/// تنظیمات اعتبارسنجی آپلود فایل. ملاک واحد پسوندهای مجاز در کل بک‌اند از appsettings است.
/// </summary>
public class FileUploadValidationOptions
{
    public long MaxFileSizeBytes { get; set; } = 20 * 1024 * 1024;

    /// <summary>
    /// پسوندهای مجاز؛ فقط از appsettings.json بخش FileUploadValidation:AllowedExtensions بارگذاری می‌شود.
    /// </summary>
    public List<string> AllowedExtensions { get; set; } = new List<string>();

    public List<string> DisallowedSignatures { get; set; } = new List<string>
    {
        "4D5A",       // MZ - Windows PE/EXE/DLL
        "7F454C46",   // ELF - Linux binaries
        "CAFEBABE"    // Java class
    };

    public bool BlockArchives { get; set; } = false;
    public bool AllowPdf { get; set; } = true;
}
