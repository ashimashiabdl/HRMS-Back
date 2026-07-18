using System.Reflection;
using Stimulsoft.Base;
using Stimulsoft.Report;

namespace HR.Report.Infrastructure.Services;

/// <summary>
/// Single process-wide Stimulsoft bootstrap and exclusive gate.
/// Stimulsoft engine/font caches are not thread-safe; concurrent Order/Fiche/Settlement/Dynamic
/// print corrupts internal state and surfaces as NullReferenceException on Render.
/// </summary>
public static class StimulsoftBootstrap
{
    private const string StimulsoftLicenseKey =
        "6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHkO46nMQvol4ASeg91in+mGJLnn2KMIpg3eSXQSgaFOm15+0l" +
        "hekKip+wRGMwXsKpHAkTvorOFqnpF9rchcYoxHXtjNDLiDHZGTIWq6D/2q4k/eiJm9fV6FdaJIUbWGS3whFWRLPHWC" +
        "BsWnalqTdZlP9knjaWclfjmUKf2Ksc5btMD6pmR7ZHQfHXfdgYK7tLR1rqtxYxBzOPq3LIBvd3spkQhKb07LTZQoyQ" +
        "3vmRSMALmJSS6ovIS59XPS+oSm8wgvuRFqE1im111GROa7Ww3tNJTA45lkbXX+SocdwXvEZyaaq61Uc1dBg+4uFRxv" +
        "yRWvX5WDmJz1X0VLIbHpcIjdEDJUvVAN7Z+FW5xKsV5ySPs8aegsY9ndn4DmoZ1kWvzUaz+E1mxMbOd3tyaNnmVhPZ" +
        "eIBILmKJGN0BwnnI5fu6JHMM/9QR2tMO1Z4pIwae4P92gKBrt0MqhvnU1Q6kIaPPuG2XBIvAWykVeH2a9EP6064e11" +
        "PFCBX4gEpJ3XFD0peE5+ddZh+h495qUc1H2B";

    private static readonly object Gate = new();
    private static bool _engineReady;

    public static void EnsureInitialized(string? fontsDirectory = null)
    {
        lock (Gate)
        {
            InitializeEngine_NoLock(fontsDirectory);
        }
    }

    /// <summary>
    /// Runs the full Load → Compile → Render → Export pipeline exclusively across all print modules.
    /// </summary>
    public static T ExecuteExclusive<T>(Func<T> action, string? fontsDirectory = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        lock (Gate)
        {
            InitializeEngine_NoLock(fontsDirectory);
            return action();
        }
    }

    private static void InitializeEngine_NoLock(string? fontsDirectory)
    {
        StimulsoftAssemblyResolver.EnsureRegistered();

        if (_engineReady)
        {
            EnsureFonts_NoLock(fontsDirectory);
            return;
        }

      //  StiLicense.Key = StimulsoftLicenseKey;

        // Match ConvertToolBox: Compilation mode + Compile()/Render(). Do not force Interpretation —
        // Interpretation with these Designer templates yields NullReferenceException inside Render.
        StiOptions.Engine.ForceInterpretationMode = false;
        StiOptions.Export.Pdf.AllowFontsCache = false;

        // Ensure Compile can resolve Stimulsoft.Data (referenced by name validation during ScriptUpdate).
        EnsureAssemblyLoaded("Stimulsoft.Data");
        EnsureAssemblyLoaded("Stimulsoft.Report.Helper");

        EnsureFonts_NoLock(fontsDirectory);
        _engineReady = true;
    }

    private static void EnsureAssemblyLoaded(string assemblyName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (string.Equals(assembly.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase))
                return;
        }

        foreach (var dir in new[]
                 {
                     AppDomain.CurrentDomain.BaseDirectory,
                     AppContext.BaseDirectory,
                     Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "Assets", "StimulDLL"),
                     @"C:\HRMS\Assets\StimulDLL",
                 })
        {
            if (string.IsNullOrWhiteSpace(dir))
                continue;

            var path = Path.Combine(dir, assemblyName + ".dll");
            if (!File.Exists(path))
                continue;

            try
            {
                Assembly.LoadFrom(path);
                return;
            }
            catch
            {
                // try next location
            }
        }
    }

    private static void EnsureFonts_NoLock(string? fontsDirectory)
    {
        try
        {
            var resolved = StiFontLoader.ResolveFontsDirectory(fontsDirectory);
            StiFontLoader.EnsureProjectFontsLoaded(resolved);
        }
        catch (DirectoryNotFoundException)
        {
            if (!string.IsNullOrWhiteSpace(fontsDirectory))
                throw;
        }
    }
}
