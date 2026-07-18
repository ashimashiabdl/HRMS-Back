using System;
using System.Linq;
using Stimulsoft.Base.Drawing;
using Stimulsoft.Report;
using Stimulsoft.Report.Components;

namespace HR.Report.Infrastructure.Services;

internal static class StiReportUtilities
{
    private static readonly string[] ServerBlockedAssemblies =
    [
        "System.Windows.Forms",
        "Stimulsoft.Controls",
    ];

    private static readonly string[] ServerBlockedUsings =
    [
        "using System.Windows.Forms;",
        "using Stimulsoft.Controls;",
    ];

    public static byte[] SanitizeMrtContent(byte[] content)
    {
        if (content == null || content.Length == 0)
            return content;

        // Packed/binary MRT must not be UTF-8 round-tripped — that corrupts the template and causes Render NRE.
        if (!LooksLikeTextualMrt(content))
            return content;

        var text = System.Text.Encoding.UTF8.GetString(content);
        var updated = text
            .Replace("<value>System.Windows.Forms.Dll</value>", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("<value>Stimulsoft.Controls.Dll</value>", string.Empty, StringComparison.OrdinalIgnoreCase);

        foreach (var blockedUsing in ServerBlockedUsings)
        {
            updated = updated.Replace(blockedUsing, string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return System.Text.Encoding.UTF8.GetBytes(updated);
    }

    private static bool LooksLikeTextualMrt(byte[] content)
    {
        var offset = 0;
        if (content.Length >= 3 && content[0] == 0xEF && content[1] == 0xBB && content[2] == 0xBF)
            offset = 3;

        if (offset >= content.Length)
            return false;

        var first = content[offset];
        return first is (byte)'<' or (byte)'{';
    }

    public static void SanitizeForServer(StiReport report)
    {
        if (report == null) return;
        try
        {
            // Match ConvertToolBox: only strip WinForms references (do not rewrite CalculationMode/script aggressively).
            if (report.ReferencedAssemblies != null && report.ReferencedAssemblies.Length > 0)
            {
                report.ReferencedAssemblies = report.ReferencedAssemblies
                    .Where(a => a != null &&
                                a.IndexOf("System.Windows.Forms", StringComparison.OrdinalIgnoreCase) < 0)
                    .ToArray();
            }

            if (!string.IsNullOrWhiteSpace(report.Script))
            {
                var lineEndings = report.Script.Contains("\r\n") ? "\r\n" : "\n";
                var updatedScript = string.Join(lineEndings,
                    report.Script
                        .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                        .Where(l => !l.TrimStart().StartsWith("using System.Windows.Forms;", StringComparison.OrdinalIgnoreCase))
                );
                report.Script = updatedScript;
            }
        }
        catch
        {
            // Intentionally ignore; sanitization is best-effort and must not block rendering
        }
    }

    /// <summary>
    /// Do not mutate CalculationMode after Load — that breaks Compile/Render on Designer templates.
    /// </summary>
    public static void PrepareForServerRender(StiReport report)
    {
        if (report == null)
            return;

        StimulsoftBootstrap.EnsureInitialized();
    }

    /// <summary>
    /// Compiles the report.
    /// Designer templates can arrive with NeedsCompiling=false while IsCompiled=false;
    /// calling Compile() then throws "Report already compiled". Force NeedsCompiling first.
    /// </summary>
    public static void CompileSafe(StiReport report)
    {
        if (report == null)
            return;

        StimulsoftBootstrap.EnsureInitialized();

        if (report.IsCompiled)
            return;

        // Critical: NeedsCompiling=false + IsCompiled=false is an inconsistent post-Load state.
        report.NeedsCompiling = true;
        report.Compile();
    }

    /// <summary>
    /// Compile (if needed) then Render — same sequence as ConvertToolBox.
    /// </summary>
    public static void CompileAndRender(StiReport report, bool showProgress = false)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        CompileSafe(report);
        report.Render(showProgress);
    }

    /// <summary>
    /// Renders under the process-wide Stimulsoft lock (Compile + Render).
    /// </summary>
    public static void RenderSafe(StiReport report, bool showProgress = false)
    {
        if (report == null)
            throw new ArgumentNullException(nameof(report));

        StimulsoftBootstrap.ExecuteExclusive(() =>
        {
            CompileAndRender(report, showProgress);
            return true;
        });
    }

    private static bool IsServerBlockedAssembly(string assemblyName) =>
        assemblyName.IndexOf("System.Windows.Forms", StringComparison.OrdinalIgnoreCase) >= 0 ||
        ServerBlockedAssemblies.Any(blocked =>
            assemblyName.IndexOf(blocked, StringComparison.OrdinalIgnoreCase) >= 0);

    /// <summary>
    /// شرح حکم (متن نامه) در قالب‌های MRT سازمانی نام‌های متفاوت دارد؛
    /// از Designer گاهی RTL/Justify درست اعمال نمی‌شود، بنابراین اینجا یکسان‌سازی می‌شود.
    /// </summary>
    public static void ApplyOrderLetterTextFormatting(StiReport report)
    {
        if (report?.Pages == null)
            return;

        foreach (StiPage page in report.Pages)
            ApplyOrderLetterTextFormatting(page.Components);
    }

    private static void ApplyOrderLetterTextFormatting(StiComponentsCollection components)
    {
        if (components == null)
            return;

        foreach (StiComponent component in components)
        {
            if (component is StiContainer container)
            {
                ApplyOrderLetterTextFormatting(container.Components);
                continue;
            }

            if (component is StiText text && IsOrderLetterTextComponent(text))
                ApplyRtlJustifyFormatting(text);
        }
    }

    private static bool IsOrderLetterTextComponent(StiText text)
    {
        var binding = text.Text?.Value ?? string.Empty;
        var name = text.Name ?? string.Empty;

        if (binding.Contains(".Description", StringComparison.OrdinalIgnoreCase) ||
            binding.Contains("شرح حکم", StringComparison.OrdinalIgnoreCase) ||
            binding.Contains("شرح_حکم", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return name.Equals("Text47", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("OrderDescription", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("LetterContent", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("LetterText", StringComparison.OrdinalIgnoreCase) ||
               name.Contains("OrderLetter", StringComparison.OrdinalIgnoreCase);
    }

        private static void ApplyRtlJustifyFormatting(StiText text)
    {
        text.HorAlignment = StiTextHorAlignment.Width;
        text.TextOptions ??= new StiTextOptions();
        text.TextOptions.RightToLeft = true;
        text.TextOptions.WordWrap = true;
    }
}


