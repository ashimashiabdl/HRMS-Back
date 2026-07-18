using System;

namespace HRMS.API.Infrastructure.Upload
{
	/// <summary>
	/// When applied to an action, overrides global allowed extensions for that endpoint only.
	/// Use for endpoints that must accept specific file types (e.g. Excel-only upload).
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class AllowUploadExtensionsAttribute : Attribute
	{
		public string[] Extensions { get; }

		public AllowUploadExtensionsAttribute(params string[] extensions)
		{
			Extensions = extensions ?? Array.Empty<string>();
			for (int i = 0; i < Extensions.Length; i++)
			{
				var e = Extensions[i];
				if (!e.StartsWith(".", StringComparison.Ordinal))
					Extensions[i] = "." + e;
				Extensions[i] = Extensions[i].ToLowerInvariant();
			}
		}
	}
}
