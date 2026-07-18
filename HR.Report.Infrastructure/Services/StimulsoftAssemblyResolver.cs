using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HR.Report.Infrastructure.Services;

/// <summary>
/// Loads Stimulsoft and Windows Desktop assemblies at runtime when report compilation needs them.
/// </summary>
public static class StimulsoftAssemblyResolver
{
    private static bool _registered;
    private static readonly ConcurrentDictionary<string, Assembly?> ResolvedAssemblies = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, byte> Resolving = new(StringComparer.OrdinalIgnoreCase);

    public static void EnsureRegistered()
    {
        if (_registered)
            return;

        AppDomain.CurrentDomain.AssemblyResolve += ResolveStimulsoftDependencies;
        _registered = true;
    }

    private static Assembly? ResolveStimulsoftDependencies(object? sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);
        var simpleName = assemblyName.Name;
        if (string.IsNullOrEmpty(simpleName))
            return null;

        if (!IsStimulsoftDependency(simpleName))
            return null;

        if (ResolvedAssemblies.TryGetValue(simpleName, out var cached))
            return cached;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (string.Equals(assembly.GetName().Name, simpleName, StringComparison.OrdinalIgnoreCase))
            {
                ResolvedAssemblies[simpleName] = assembly;
                return assembly;
            }
        }

        // Never call Assembly.Load inside AssemblyResolve; it re-enters this handler and causes StackOverflowException.
        if (!Resolving.TryAdd(simpleName, 0))
            return null;

        try
        {
            foreach (var path in GetCandidatePaths(simpleName))
            {
                if (!File.Exists(path))
                    continue;

                try
                {
                    var loaded = Assembly.LoadFrom(path);
                    ResolvedAssemblies[simpleName] = loaded;
                    return loaded;
                }
                catch
                {
                    // Continue searching other candidate paths.
                }
            }

            ResolvedAssemblies[simpleName] = null;
            return null;
        }
        finally
        {
            Resolving.TryRemove(simpleName, out _);
        }
    }

    private static bool IsStimulsoftDependency(string assemblyName) =>
        assemblyName.StartsWith("Stimulsoft.", StringComparison.OrdinalIgnoreCase)
        || assemblyName.StartsWith("System.Windows.Forms", StringComparison.OrdinalIgnoreCase)
        || assemblyName.StartsWith("Accessibility", StringComparison.OrdinalIgnoreCase)
        || assemblyName.StartsWith("Microsoft.CodeAnalysis", StringComparison.OrdinalIgnoreCase);

    private static IEnumerable<string> GetCandidatePaths(string assemblyName)
    {
        var fileName = assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
            ? assemblyName
            : $"{assemblyName}.dll";

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in GetProbeDirectories())
        {
            var path = Path.Combine(dir, fileName);
            if (seen.Add(path))
                yield return path;
        }

        foreach (var path in GetWindowsDesktopPaths(fileName))
        {
            if (seen.Add(path))
                yield return path;
        }
    }

    private static IEnumerable<string> GetProbeDirectories()
    {
        var dirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        void Add(string? dir)
        {
            if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                dirs.Add(dir);
        }

        Add(AppDomain.CurrentDomain.BaseDirectory);
        Add(AppContext.BaseDirectory);

        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly?.Location is { Length: > 0 } entryLocation)
            Add(Path.GetDirectoryName(entryLocation));

        // Directory of already-loaded Stimulsoft.Report / Stimulsoft.Base
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = assembly.GetName().Name;
            if (name is null || !name.StartsWith("Stimulsoft.", StringComparison.OrdinalIgnoreCase))
                continue;

            try
            {
                if (!string.IsNullOrEmpty(assembly.Location))
                    Add(Path.GetDirectoryName(assembly.Location));
            }
            catch
            {
                // dynamic assemblies have no location
            }
        }

        // Known local vendor folder
        Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory ?? string.Empty, "Assets", "StimulDLL"));
        Add(@"C:\HRMS\Assets\StimulDLL");

        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
        Add(runtimeDir);

        return dirs;
    }

    private static IEnumerable<string> GetWindowsDesktopPaths(string fileName)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            yield break;

        var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (string.IsNullOrEmpty(runtimeDir))
            yield break;

        var sharedRoot = Path.GetFullPath(Path.Combine(runtimeDir, "..", ".."));
        if (!Directory.Exists(sharedRoot))
            yield break;

        foreach (var desktopDir in Directory
                     .EnumerateDirectories(sharedRoot, "Microsoft.WindowsDesktop.App*")
                     .OrderByDescending(path => path, StringComparer.OrdinalIgnoreCase))
        {
            yield return Path.Combine(desktopDir, fileName);
        }
    }
}
