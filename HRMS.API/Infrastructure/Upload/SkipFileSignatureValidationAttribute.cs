using System;

namespace HRMS.API.Infrastructure.Upload
{
	/// <summary>
	/// When applied to an action, skips file signature validation in FileUploadValidationFilter.
	/// Use this for endpoints that need to accept files with non-standard signatures or when
	/// signature validation is too strict for legitimate files.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SkipFileSignatureValidationAttribute : Attribute
	{
	}
}

