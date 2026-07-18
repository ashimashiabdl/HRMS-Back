using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HRMS.API.Infrastructure.Upload
{
	public class FileUploadValidationFilter : IAsyncActionFilter
	{
		private readonly ILogger<FileUploadValidationFilter> _logger;
		private readonly FileUploadValidationOptions _options;

		public FileUploadValidationFilter(
			ILogger<FileUploadValidationFilter> logger,
			IOptions<FileUploadValidationOptions> options)
		{
			_logger = logger;
			_options = options.Value;
		}

	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var request = context.HttpContext.Request;
		if (!IsMultipart(request))
		{
			await next();
			return;
		}

		// Check if the action has SkipFileSignatureValidationAttribute
		var skipSignatureValidation = context.ActionDescriptor.EndpointMetadata
			.Any(m => m is SkipFileSignatureValidationAttribute);

		// Per-action allowed extensions override (e.g. Excel-only / DBF-only upload)
		var allowExtAttr = context.ActionDescriptor.EndpointMetadata
			.OfType<AllowUploadExtensionsAttribute>()
			.FirstOrDefault();

		// Fallback: some hosting/filter pipelines may not surface method attributes in EndpointMetadata
		if (allowExtAttr == null
			&& context.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor controllerAction)
		{
			allowExtAttr = controllerAction.MethodInfo
				.GetCustomAttributes(typeof(AllowUploadExtensionsAttribute), inherit: true)
				.OfType<AllowUploadExtensionsAttribute>()
				.FirstOrDefault();
		}

		var allowedExtensions = allowExtAttr?.Extensions?.Length > 0
			? allowExtAttr.Extensions.ToList()
			: (_options.AllowedExtensions ?? new List<string>());

		// Safety net for DBF viewer if attribute/config lag behind a deploy
		if (string.Equals(context.HttpContext.Request.Path.Value, "/api/InsuranceDiskette/UploadAndReadDbf", StringComparison.OrdinalIgnoreCase)
			|| (context.HttpContext.Request.Path.Value?.EndsWith("/UploadAndReadDbf", StringComparison.OrdinalIgnoreCase) ?? false))
		{
			if (!allowedExtensions.Contains(".dbf", StringComparer.OrdinalIgnoreCase))
			{
				allowedExtensions = allowedExtensions.Append(".dbf").Distinct(StringComparer.OrdinalIgnoreCase).ToList();
			}
		}

		try
		{
			if (!request.HasFormContentType)
			{
				context.Result = new BadRequestObjectResult(OperationResult.Failed("نوع درخواست نامعتبر است"));
				return;
			}

			var form = await request.ReadFormAsync();
			foreach (var file in form.Files)
			{
			// Basic file name hardening
			var originalFileName = file.FileName ?? string.Empty;
				if (string.IsNullOrWhiteSpace(originalFileName)
					|| originalFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
					|| originalFileName.Contains('/')
					|| originalFileName.Contains('\\')
					|| originalFileName.Contains("..", StringComparison.Ordinal))
				{
					var ip = GetClientIP(context.HttpContext);
					_logger.LogWarning("SECURITY FAILURE - Invalid file name from IP: {IP}, FileName: {FileName}", ip, originalFileName);
					context.Result = new BadRequestObjectResult(OperationResult.Failed("نام فایل نامعتبر است"));
					return;
				}

				if (file.Length <= 0)
				{
					// SECURITY FAILURE LOG: Empty file upload attempt
					var ip = GetClientIP(context.HttpContext);
					_logger.LogWarning("SECURITY FAILURE - Empty file upload attempt from IP: {IP}, FileName: {FileName}", ip, file.FileName);
					
					context.Result = new BadRequestObjectResult(OperationResult.Failed("فایل خالی است"));
					return;
				}

				if (file.Length > _options.MaxFileSizeBytes)
				{
					// SECURITY FAILURE LOG: File size limit exceeded
					var ip = GetClientIP(context.HttpContext);
					_logger.LogWarning("SECURITY FAILURE - File size limit exceeded from IP: {IP}, FileName: {FileName}, Size: {FileSize}, Limit: {SizeLimit}", 
						ip, file.FileName, file.Length, _options.MaxFileSizeBytes);
					
					context.Result = new BadRequestObjectResult(OperationResult.Failed($"حجم فایل بیش از حد مجاز است ({_options.MaxFileSizeBytes} bytes)"));
					return;
				}

				// Fail-closed: if no allowed extensions are configured (e.g. appsettings missing
				// FileUploadValidation:AllowedExtensions), reject every upload instead of letting
				// everything through. The allowed list is read ONLY from the JSON configuration.
				if (allowedExtensions?.Any() != true)
				{
					var ip = GetClientIP(context.HttpContext);
					_logger.LogError("SECURITY FAILURE - No allowed file extensions configured; rejecting upload from IP: {IP}, FileName: {FileName}",
						ip, file.FileName);

					context.Result = new BadRequestObjectResult(OperationResult.Failed("نوع فایل مجاز نیست"));
					return;
				}

				var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
				if (!allowedExtensions.Contains(ext))
				{
					var ip = GetClientIP(context.HttpContext);
					var endpoint = context.ActionDescriptor.DisplayName;
					_logger.LogWarning(
						"SECURITY FAILURE - Disallowed file extension from IP: {IP}, FileName: {FileName}, Extension: {Extension}, Endpoint: {Endpoint}, AllowedExtensions: {AllowedExtensions}",
						ip, file.FileName, ext, endpoint, string.Join(", ", allowedExtensions));

					context.Result = new BadRequestObjectResult(OperationResult.Failed("نوع فایل مجاز نیست"));
					return;
				}

			// Skip signature validation if attribute is present
			if (!skipSignatureValidation)
			{
				// Read first up to 16 bytes to check signatures
				var headerBytes = new byte[Math.Min(16, (int)Math.Min(file.Length, 16))];
				using (var stream = file.OpenReadStream())
				{
					var bytesRead = await stream.ReadAsync(headerBytes.AsMemory(0, headerBytes.Length));
					if (bytesRead < 2)
					{
						var ip = GetClientIP(context.HttpContext);
						_logger.LogWarning("SECURITY FAILURE - File too small to validate signature from IP: {IP}, FileName: {FileName}", 
							ip, file.FileName);
						context.Result = new BadRequestObjectResult(OperationResult.Failed("فایل برای اعتبارسنجی بسیار کوچک است"));
						return;
					}
				}
				var headerHex = BitConverter.ToString(headerBytes).Replace("-", string.Empty).ToUpperInvariant();

				// Disallow obvious binary formats like EXE/ELF/Java Class; PDFs can be allowed
				if (_options.DisallowedSignatures != null && _options.DisallowedSignatures.Any(sig => headerHex.StartsWith(sig)))
				{
					if (!(headerHex.StartsWith("25504446") && _options.AllowPdf))
					{
						// SECURITY FAILURE LOG: Malicious file signature detected
						var ip = GetClientIP(context.HttpContext);
						var detectedSig = _options.DisallowedSignatures.FirstOrDefault(sig => headerHex.StartsWith(sig));
						_logger.LogError("SECURITY FAILURE - Malicious file signature detected from IP: {IP}, FileName: {FileName}, FileSignature: {Signature}, DetectedAs: {DetectedSig}", 
							ip, file.FileName, headerHex, detectedSig);
						
						context.Result = new BadRequestObjectResult(OperationResult.Failed("نوع فایل غیرمجاز/مشکوک شناسایی شد"));
						return;
					}
				}

				// Positive signature validation for common allowed types
				if (!SignatureMatchesAllowedType(ext, headerHex))
				{
					var ip = GetClientIP(context.HttpContext);
					_logger.LogWarning("SECURITY FAILURE - Signature mismatch for allowed type from IP: {IP}, FileName: {FileName}, Ext: {Ext}, Signature: {Signature}, FirstBytes: {FirstBytes}", 
						ip, file.FileName, ext, headerHex, 
						headerBytes.Length > 0 ? $"{headerBytes[0]:X2}{headerBytes[1]:X2}" : "N/A");
					context.Result = new BadRequestObjectResult(OperationResult.Failed("محتوای فایل با پسوند انتخاب‌شده سازگار نیست"));
					return;
				}

				if (_options.BlockArchives && (headerHex.StartsWith("504B0304") || headerHex.StartsWith("52617221")))
				{
					// Allow Office Open XML formats (which are ZIP containers) if extension is allowed
					var isOfficeOpenXml = ext == ".docx" || ext == ".xlsx" || ext == ".pptx";
					// Allow ZIP and RAR files if their extensions are explicitly in AllowedExtensions
					var isAllowedArchive = ext == ".zip" || ext == ".rar";
					if (!isOfficeOpenXml && !isAllowedArchive)
					{
						// SECURITY FAILURE LOG: Archive file blocked
						var ip = GetClientIP(context.HttpContext);
						var archiveType = headerHex.StartsWith("504B0304") ? "ZIP" : "RAR";
						_logger.LogWarning("SECURITY FAILURE - Archive file blocked from IP: {IP}, FileName: {FileName}, ArchiveType: {ArchiveType}, FileSignature: {Signature}", 
							ip, file.FileName, archiveType, headerHex);
						
						context.Result = new BadRequestObjectResult(OperationResult.Failed("آپلود فایل‌های آرشیو مجاز نیست"));
						return;
					}
				}
			}
				}
			}
		catch (Exception ex)
		{
			// SECURITY FAILURE LOG: File upload validation exception
			var ip = GetClientIP(context.HttpContext);
			_logger.LogError(ex, "SECURITY FAILURE - File upload validation exception from IP: {IP}, Error: {Error}", ip, ex.Message);
			
			context.Result = new BadRequestObjectResult(OperationResult.Failed("اعتبارسنجی فایل با خطا مواجه شد"));
			return;
		}

			await next();
		}

	private static bool IsMultipart(HttpRequest request)
	{
		return request.ContentType != null && request.ContentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase);
	}

	private static string GetClientIP(HttpContext httpContext)
	{
		return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
	}

	private static bool SignatureMatchesAllowedType(string ext, string headerHex)
	{
		// Normalize header string length checks to avoid IndexOutOfRange
		if (string.IsNullOrEmpty(headerHex)) return false;

		switch (ext)
		{
			case ".pdf":
				return headerHex.StartsWith("25504446"); // %PDF
			case ".jpg":
			case ".jpeg":
				// JPEG files start with FFD8 (SOI - Start of Image marker)
				// Common JPEG formats:
				// - Standard JPEG: FFD8FF
				// - JFIF: FFD8FFE0
				// - EXIF: FFD8FFE1
				// - JPEG 2000: FFD8FF (but different structure)
				// We check if it starts with FFD8 which is the standard JPEG signature
				// Also handle case-insensitive comparison
				if (headerHex.Length >= 4)
				{
					var firstFour = headerHex.Substring(0, 4).ToUpperInvariant();
					return firstFour == "FFD8";
				}
				return false;
			case ".png":
				return headerHex.StartsWith("89504E47"); // PNG
			case ".webp":
				// WebP: RIFF (52 49 46 46) then at offset 8: WEBP (57 45 42 50)
				return headerHex.Length >= 24 && headerHex.StartsWith("52494646") && headerHex.Substring(16, 8) == "57454250";
			case ".gif":
				return headerHex.StartsWith("47494638"); // GIF
			case ".bmp":
				return headerHex.StartsWith("424D"); // BM
			case ".svg":
				// SVG: <?xml (3C3F786D6C) or <svg (3C737667)
				return headerHex.StartsWith("3C3F786D6C") || (headerHex.Length >= 8 && headerHex.StartsWith("3C737667"));
			case ".docx":
			case ".xlsx":
			case ".pptx":
				return headerHex.StartsWith("504B0304"); // ZIP container (OOXML)
			case ".doc":
			case ".xls":
				return headerHex.StartsWith("D0CF11E0"); // OLE Compound File
			case ".txt":
				// Plain text may start with various bytes; allow unless it's a known binary disallowed by earlier checks
				return true;
			case ".dbf":
				// DBF variants start with 0x03, 0x83, 0x8B, 0x8E, 0xF5; allow loosely
				return true;
			case ".rar":
				// RAR files: RAR v1.50+ starts with "526172211A0700" or "526172211A0701"
				// Older RAR v1.50- starts with "526172211A07"
				return headerHex.StartsWith("52617221"); // RAR!
			case ".zip":
				// ZIP files start with "504B0304" (PK..) or "504B0506" (empty archive)
				// Office Open XML files (.docx, .xlsx, .pptx) also use ZIP format, but are handled separately
				return headerHex.StartsWith("504B0304") || headerHex.StartsWith("504B0506");
			default:
				return true; // For other allowed types (if any), rely on extension whitelist and disallowed signature checks
		}
	}
}
}


