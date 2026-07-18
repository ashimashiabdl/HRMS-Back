using Stimulsoft.Base;

namespace HR.Report.Infrastructure.Services;

/// <summary>
/// Loads report fonts from project Assets/Fonts so server OS font installation is not required.
/// </summary>
internal static class StiFontLoader
{
    private static readonly object SyncRoot = new();
    private static readonly HashSet<string> RegisteredFontFiles = new(StringComparer.OrdinalIgnoreCase);

    private static readonly string[] FontSearchPatterns = ["*.ttf", "*.otf", "*.TTF", "*.OTF"];

    public static string ResolveFontsDirectory(string? fontsDirectory = null)
    {
        if (TryResolveDirectoryWithFonts(fontsDirectory, out var resolved))
            return resolved;

        if (!string.IsNullOrWhiteSpace(fontsDirectory))
        {
            var nestedFonts = Path.Combine(fontsDirectory, "Assets", "Fonts");
            if (TryResolveDirectoryWithFonts(nestedFonts, out resolved))
                return resolved;
        }

        var projectFonts = ResolveProjectFontsDirectory();
        if (TryResolveDirectoryWithFonts(projectFonts, out resolved))
            return resolved;

        throw new DirectoryNotFoundException(
            "مسیر فونت‌های پروژه یافت نشد. پوشه Assets\\Fonts باید در خروجی برنامه موجود باشد.");
    }

    public static int EnsureProjectFontsLoaded(string fontsDirectory)
    {
        if (string.IsNullOrWhiteSpace(fontsDirectory))
            throw new ArgumentException("مسیر فونت‌های پروژه مشخص نشده است.", nameof(fontsDirectory));

        if (!Directory.Exists(fontsDirectory))
            throw new DirectoryNotFoundException($"پوشه فونت یافت نشد: {fontsDirectory}");

        lock (SyncRoot)
        {
            var fontFiles = CollectFontFiles(fontsDirectory);
            if (fontFiles.Count == 0)
            {
                throw new InvalidOperationException(
                    $"هیچ فایل فونت (.ttf / .otf) در مسیر '{fontsDirectory}' یافت نشد.");
            }

            var loadedCount = 0;
            var errors = new List<string>();

            foreach (var fontFile in fontFiles)
            {
                if (!RegisteredFontFiles.Add(fontFile))
                {
                    loadedCount++;
                    continue;
                }

                try
                {
                    StiFontCollection.AddFontFile(fontFile);
                    loadedCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{Path.GetFileName(fontFile)}: {ex.Message}");
                }
            }

            if (loadedCount == 0)
            {
                var details = errors.Count > 0
                    ? string.Join(" | ", errors)
                    : "دلیل مشخصی ثبت نشد";
                throw new InvalidOperationException($"بارگذاری فونت‌های پروژه ناموفق بود. {details}");
            }

            return loadedCount;
        }
    }

    public static string? ResolveProjectFontsDirectory()
    {
        var baseDirs = new[]
        {
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory(),
        };

        foreach (var baseDir in baseDirs.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(baseDir))
                continue;

            var directCandidate = Path.Combine(baseDir, "Assets", "Fonts");
            if (TryResolveDirectoryWithFonts(directCandidate, out _))
                return directCandidate;

            var probe = baseDir;
            for (var depth = 0; depth <= 6 && !string.IsNullOrEmpty(probe); depth++)
            {
                var candidate = Path.Combine(probe, "Assets", "Fonts");
                if (TryResolveDirectoryWithFonts(candidate, out _))
                    return candidate;

                probe = Path.GetDirectoryName(probe);
            }
        }

        return null;
    }

    private static bool TryResolveDirectoryWithFonts(string? directory, out string resolved)
    {
        if (!string.IsNullOrWhiteSpace(directory) &&
            Directory.Exists(directory) &&
            CollectFontFiles(directory).Count > 0)
        {
            resolved = directory;
            return true;
        }

        resolved = string.Empty;
        return false;
    }

    private static List<string> CollectFontFiles(string fontsDirectory)
    {
        var fontFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var pattern in FontSearchPatterns)
        {
            foreach (var file in Directory.GetFiles(fontsDirectory, pattern))
            {
                fontFiles.Add(file);
            }
        }

        return fontFiles.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
    }
}
