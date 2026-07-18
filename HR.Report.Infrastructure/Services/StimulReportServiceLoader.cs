using System.Reflection;
using System.IO;
using System.Linq;

namespace HR.Report.Infrastructure.Services;

/// <summary>
/// Helper class for dynamically loading StimulReportService from ConvertToolBox without direct reference
/// </summary>
public static class StimulReportServiceLoader
{
    private static Assembly? _convertToolBoxAssembly;
    private static Type? _stimulReportServiceType;
    private static object? _serviceInstance;
    private static bool _assemblyResolveHandlerAdded = false;

    static StimulReportServiceLoader()
    {
        // Add AssemblyResolve handler to help load dependencies
        if (!_assemblyResolveHandlerAdded)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            _assemblyResolveHandlerAdded = true;
        }
    }

    private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);
        var name = assemblyName.Name;
        
        if (string.IsNullOrEmpty(name))
            return null;

        // Try to load from already loaded assemblies first
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name == name)
            {
                return assembly;
            }
        }

        // Try to find the assembly in the same directory as ConvertToolBox
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var entryAssemblyDir = Assembly.GetEntryAssembly()?.Location != null 
            ? Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) 
            : null;

        var possiblePaths = new List<string>();

        // Add paths from current directory
        if (!string.IsNullOrEmpty(currentDir))
        {
            possiblePaths.Add(Path.Combine(currentDir, $"{name}.dll"));
            possiblePaths.Add(Path.Combine(currentDir, $"{name}.exe"));
        }

        // Add paths from entry assembly directory
        if (!string.IsNullOrEmpty(entryAssemblyDir))
        {
            possiblePaths.Add(Path.Combine(entryAssemblyDir, $"{name}.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, $"{name}.exe"));
        }

        // For System.Windows.Forms and other framework assemblies, try runtime directory
        if (name.StartsWith("System.") || name.StartsWith("Microsoft."))
        {
            var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (!string.IsNullOrEmpty(runtimeDir))
            {
                possiblePaths.Add(Path.Combine(runtimeDir, $"{name}.dll"));
            }
        }

        // Try to find in the same directory as ConvertToolBox
        if (_convertToolBoxAssembly != null)
        {
            try
            {
                var convertToolBoxLocation = _convertToolBoxAssembly.Location;
                if (!string.IsNullOrEmpty(convertToolBoxLocation))
                {
                    var convertToolBoxDir = Path.GetDirectoryName(convertToolBoxLocation);
                    if (!string.IsNullOrEmpty(convertToolBoxDir))
                    {
                        possiblePaths.Add(Path.Combine(convertToolBoxDir, $"{name}.dll"));
                        possiblePaths.Add(Path.Combine(convertToolBoxDir, $"{name}.exe"));
                    }
                }
            }
            catch
            {
                // Ignore
            }
        }

        // Try all possible paths
        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                try
                {
                    return Assembly.LoadFrom(path);
                }
                catch
                {
                    // Continue searching
                }
            }
        }

        // For System.Windows.Forms and other framework assemblies, try to load from runtime
        if (name == "System.Windows.Forms" || name.StartsWith("System.Windows.Forms"))
        {
            try
            {
                // Try to load from runtime - System.Windows.Forms is part of the shared framework
                // In .NET Core/.NET 5+, it should be available in the runtime directory
                var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
                if (!string.IsNullOrEmpty(runtimeDir))
                {
                    var winFormsPath = Path.Combine(runtimeDir, "System.Windows.Forms.dll");
                    if (File.Exists(winFormsPath))
                    {
                        return Assembly.LoadFrom(winFormsPath);
                    }
                }

                // Try loading by full name
                return Assembly.Load(assemblyName);
            }
            catch
            {
                // Ignore and continue to next method
            }
        }

        // For .NET framework assemblies, try to load by name (will use GAC or runtime)
        try
        {
            return Assembly.Load(assemblyName);
        }
        catch
        {
            // Ignore
        }

        return null;
    }

    /// <summary>
    /// Loads the ConvertToolBox assembly and creates an instance of StimulReportService
    /// </summary>
    /// <param name="dllPath">Path to ConvertToolBox.dll. If null, tries to find it automatically</param>
    /// <returns>Instance of StimulReportService</returns>
    public static object LoadStimulReportService(string? dllPath = null)
    {
        try
        {
            // If already loaded, return cached instance
            if (_serviceInstance != null && _stimulReportServiceType != null)
            {
                return _serviceInstance;
            }

            // Determine DLL path
            if (string.IsNullOrEmpty(dllPath))
            {
                dllPath = FindConvertToolBoxDll();
            }

            if (string.IsNullOrEmpty(dllPath) || !File.Exists(dllPath))
            {
                var searchedPaths = GetSearchedPaths();
                throw new FileNotFoundException(
                    $"ConvertToolBox.dll not found. " +
                    $"Searched in: {string.Join("; ", searchedPaths)}. " +
                    $"Current Directory: {AppDomain.CurrentDomain.BaseDirectory}. " +
                    $"Entry Assembly: {Assembly.GetEntryAssembly()?.Location ?? "N/A"}");
            }

            // Load assembly
            _convertToolBoxAssembly = Assembly.LoadFrom(dllPath);

            // Verify assembly is loaded correctly
            if (_convertToolBoxAssembly == null)
            {
                throw new InvalidOperationException($"Failed to load assembly from: {dllPath}");
            }

            // Get StimulReportService type - try different approaches
            _stimulReportServiceType = _convertToolBoxAssembly.GetType("ConvertToolBox.StimulReportService", throwOnError: false);

            // If not found, try to find it by name only
            if (_stimulReportServiceType == null)
            {
                _stimulReportServiceType = _convertToolBoxAssembly.GetTypes()
                    .FirstOrDefault(t => t.Name == "StimulReportService" && t.IsClass && !t.IsAbstract);
            }

            // If still not found, list all available types for debugging
            if (_stimulReportServiceType == null)
            {
                try
                {
                    var allTypes = _convertToolBoxAssembly.GetTypes();
                    var availableTypes = allTypes
                        .Where(t => t.IsClass && !t.IsAbstract)
                        .Select(t => t.FullName ?? t.Name)
                        .ToList();

                    var allNamespaces = allTypes
                        .Where(t => !string.IsNullOrEmpty(t.Namespace))
                        .Select(t => t.Namespace!)
                        .Distinct()
                        .ToList();

                    throw new TypeLoadException(
                        $"StimulReportService type not found in ConvertToolBox assembly. " +
                        $"Assembly Location: {dllPath}. " +
                        $"Assembly FullName: {_convertToolBoxAssembly.FullName}. " +
                        $"Available namespaces: {string.Join(", ", allNamespaces)}. " +
                        $"Available types: {string.Join(", ", availableTypes)}");
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var loaderExceptions = ex.LoaderExceptions
                        .Where(e => e != null)
                        .Select(e => e!.Message)
                        .ToList();

                    throw new TypeLoadException(
                        $"Failed to load types from ConvertToolBox assembly. " +
                        $"Assembly Location: {dllPath}. " +
                        $"Loader Exceptions: {string.Join("; ", loaderExceptions)}", ex);
                }
            }

            // Create instance
            _serviceInstance = Activator.CreateInstance(_stimulReportServiceType);

            if (_serviceInstance == null)
            {
                throw new InvalidOperationException("Failed to create instance of StimulReportService");
            }

            return _serviceInstance;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load StimulReportService: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Calls ExportMrtToPdf method using reflection
    /// </summary>
    public static void ExportMrtToPdf(string mrtPath, string pdfPath, string? dllPath = null)
    {
        var service = LoadStimulReportService(dllPath);

        // Get ExportMrtToPdf method
        var method = _stimulReportServiceType!.GetMethod("ExportMrtToPdf", 
            BindingFlags.Public | BindingFlags.Instance);

        if (method == null)
        {
            throw new MissingMethodException("ExportMrtToPdf method not found in StimulReportService");
        }

        // Invoke method
        method.Invoke(service, new object[] { mrtPath, pdfPath });
    }

    /// <summary>
    /// Tries to find ConvertToolBox.dll in common locations
    /// </summary>
    private static string? FindConvertToolBoxDll()
    {
        // Try current directory
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var entryAssemblyLocation = Assembly.GetEntryAssembly()?.Location;
        var entryAssemblyDir = entryAssemblyLocation != null ? Path.GetDirectoryName(entryAssemblyLocation) : null;
        
        var possiblePaths = new List<string>();

        // مسیرهای مربوط به BaseDirectory
        if (!string.IsNullOrEmpty(currentDir))
        {
            possiblePaths.Add(Path.Combine(currentDir, "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Debug", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Release", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Debug", "net8.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Release", "net8.0", "ConvertToolBox.dll"));
        }

        // مسیرهای مربوط به EntryAssembly
        if (!string.IsNullOrEmpty(entryAssemblyDir))
        {
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Debug", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Release", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Debug", "net8.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Release", "net8.0", "ConvertToolBox.dll"));
        }

        // مسیرهای نسبی از پروژه ConvertToolBox
        if (!string.IsNullOrEmpty(currentDir))
        {
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "ConvertToolBox", "bin", "Debug", "net9.0-windows7.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "ConvertToolBox", "bin", "Release", "net9.0-windows7.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "..", "ConvertToolBox", "bin", "Debug", "net9.0-windows7.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "..", "ConvertToolBox", "bin", "Release", "net9.0-windows7.0", "ConvertToolBox.dll"));
        }

        // جستجو در تمام Assembly های لود شده
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic) continue;
            
            try
            {
                var assemblyLocation = assembly.Location;
                if (string.IsNullOrEmpty(assemblyLocation)) continue;

                var assemblyDir = Path.GetDirectoryName(assemblyLocation);
                if (string.IsNullOrEmpty(assemblyDir)) continue;

                var dllPath = Path.Combine(assemblyDir, "ConvertToolBox.dll");
                if (File.Exists(dllPath))
                {
                    return dllPath;
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        // بررسی مسیرهای ممکن
        foreach (var path in possiblePaths)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all paths that were searched for the DLL (for error messages)
    /// </summary>
    private static List<string> GetSearchedPaths()
    {
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        var entryAssemblyLocation = Assembly.GetEntryAssembly()?.Location;
        var entryAssemblyDir = entryAssemblyLocation != null ? Path.GetDirectoryName(entryAssemblyLocation) : null;
        
        var possiblePaths = new List<string>();

        if (!string.IsNullOrEmpty(currentDir))
        {
            possiblePaths.Add(Path.Combine(currentDir, "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Debug", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Release", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Debug", "net8.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "bin", "Release", "net8.0", "ConvertToolBox.dll"));
        }

        if (!string.IsNullOrEmpty(entryAssemblyDir))
        {
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Debug", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Release", "net9.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Debug", "net8.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(entryAssemblyDir, "bin", "Release", "net8.0", "ConvertToolBox.dll"));
        }

        if (!string.IsNullOrEmpty(currentDir))
        {
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "ConvertToolBox", "bin", "Debug", "net9.0-windows7.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "ConvertToolBox", "bin", "Release", "net9.0-windows7.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "..", "ConvertToolBox", "bin", "Debug", "net9.0-windows7.0", "ConvertToolBox.dll"));
            possiblePaths.Add(Path.Combine(currentDir, "..", "..", "..", "..", "ConvertToolBox", "bin", "Release", "net9.0-windows7.0", "ConvertToolBox.dll"));
        }

        return possiblePaths.Select(p => 
        {
            try { return Path.GetFullPath(p); }
            catch { return p; }
        }).ToList();
    }
}

